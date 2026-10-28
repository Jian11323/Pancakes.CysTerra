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
		/// <summary>展示用烈度原文（多为 MMI 罗马数字）。</summary>
		public string? Intensity { get; init; }
		/// <summary>MMI 数值 1–12，供 IntensityMMI / SeverityScheme 使用。</summary>
		public float? IntensityMmi { get; init; }
		public string Fingerprint { get; init; } = "";
	}
}
