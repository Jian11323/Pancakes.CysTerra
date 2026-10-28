using Cryville.EEW;
using Cryville.EEW.Report;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Pancakes.CysTerra {
	[Export(typeof(IBuilder<ISourceWorker>))]
	public sealed class WorkerBuilder : IBuilder<LineWsWorker> {
		const string DefaultUri = "wss://api.aloys23.link/api/v1/alert/ws/quake/gq";

		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);

		public LineWsWorker Build(ref CultureInfo? culture) {
			if (!Uri.TryCreate(DefaultUri, UriKind.Absolute, out var uri)
				|| (uri.Scheme != "ws" && uri.Scheme != "wss")) {
				throw new SourceWorkerClientException("Invalid WebSocket URI: " + DefaultUri);
			}
			return new LineWsWorker(uri);
		}
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public sealed class ReportBuilder : SimpleBuilder<LineReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
