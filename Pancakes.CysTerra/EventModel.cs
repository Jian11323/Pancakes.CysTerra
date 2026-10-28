using System;

namespace Pancakes.CysTerra {
	public sealed class LineEvent {
		public string Agency { get; init; } = "gq";
		public string UnitId { get; init; } = "";
		public int? Serial { get; init; }
		public bool IsCancel { get; init; }
		public bool IsFinal { get; init; }
		public string? Place { get; init; }
		public double? Magnitude { get; init; }
		public double? DepthKm { get; init; }
		public double? Latitude { get; init; }
		public double? Longitude { get; init; }
		public DateTime? OriginUtc { get; init; }
		public string? Intensity { get; init; }
		public string Fingerprint { get; init; } = "";
	}
}
