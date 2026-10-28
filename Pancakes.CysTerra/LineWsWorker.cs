using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Cryville.EEW;

namespace Pancakes.CysTerra {
	public sealed class LineWsWorker : WebSocketWorker, ISourceWorker {
		readonly Dictionary<string, string> _seen = new();

		public LineWsWorker(Uri uri) : base(uri) { }

		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
		public event Handler<object?>? Received;
		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;

		protected override async Task Handle(Stream stream, WebSocketMessageType messageType, CancellationToken cancellationToken) {
			if (messageType != WebSocketMessageType.Text) {
				return;
			}
			try {
				using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(true);
				var evt = LineParser.Parse(doc.RootElement);
				if (evt == null) {
					return;
				}
				if (_seen.TryGetValue(evt.UnitId, out var prev) && prev == evt.Fingerprint) {
					return;
				}
				_seen[evt.UnitId] = evt.Fingerprint;
				Received?.Invoke(this, evt);
			}
			catch (JsonException ex) {
				ErrorEmitted?.Invoke(this, ex);
			}
		}

		protected override void OnHeartbeat() => Heartbeat?.Invoke(this, Cryville.EEW.Heartbeat.Instance);
		protected override void OnError(Exception ex) => ErrorEmitted?.Invoke(this, ex);
	}
}
