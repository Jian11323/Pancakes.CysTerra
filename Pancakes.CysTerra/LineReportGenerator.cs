using Cryville.Common.Compat;
using Cryville.EEW.Report;
using System;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;
using Cryville.EEW;

namespace Pancakes.CysTerra {
	public sealed class LineReportGenerator : IContextedGenerator<LineEvent, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(LineEvent e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Source = res.GetStringRequired("SourceName"),
				Title = e.IsCancel
					? res.GetStringRequired("TitleCancel")
					: e.IsFinal
						? res.GetStringRequired("TitleFinal")
						: (e.Serial is > 1 ? res.GetStringRequired("TitleUpdate") : res.GetStringRequired("Title")),
				Location = e.Place,
				LocationSpecificity = e.Latitude != null ? 12 : 8,
				TimeZone = TimeZoneInfo.Utc,
				UtcIssueTime = DateTime.UtcNow,
			};
			if (e.OriginUtc is DateTime origin) {
				result.Time = new DateTimeOffset(DateTime.SpecifyKind(origin, DateTimeKind.Utc));
			}
			var issued = e.OriginUtc ?? DateTime.UtcNow;
			result.InvalidatedTime = new DateTimeOffset(
				DateTime.SpecifyKind(issued, DateTimeKind.Utc) + TimeSpan.FromMinutes(e.IsFinal || e.IsCancel ? 10 : 3));
			result.GroupKeys.Add(new UnitKey(e.UnitId));
			result.RevisionKey = new RevKey(e.Serial ?? 0, e.IsCancel, e.IsFinal);
			if (e.Latitude is double lat && e.Longitude is double lon && e.Magnitude is double mag
				&& e.DepthKm is double depth && e.OriginUtc is DateTime ht) {
				result.GroupKeys.Add(new HypocenterGroupKey(
					(float)lat, (float)lon, DateTime.SpecifyKind(ht, DateTimeKind.Utc), (float)mag, (float)depth));
			}
			if (e.Magnitude is double m) {
				result.Properties.Add(new(
					Magnitude, res.GetStringRequired("PropertyMagnitude"), m.ToString("0.0", culture),
					context.SeverityScheme, m) { AccuracyOrder = 90 });
			}
			if (e.DepthKm is double d) {
				result.Properties.Add(new(
					HypocenterDepth, res.GetStringRequired("PropertyDepth"),
					string.Format(culture, res.GetStringRequired("PropertyDepthValue"), d.ToString("0.0", culture)),
					context.SeverityScheme, d) { AccuracyOrder = 80 });
			}
			if (!string.IsNullOrEmpty(e.Intensity)) {
				result.Properties.Add(new(
					IntensityMMI, res.GetStringRequired("PropertyIntensity"), e.Intensity,
					context.SeverityScheme, e.Intensity) { AccuracyOrder = 85 });
			}
			if (e.Serial is int s) {
				result.Properties.Add(new(
					"Serial", res.GetStringRequired("PropertySerial"), s.ToString(culture),
					context.SeverityScheme, s) { AccuracyOrder = 40 });
			}
			return result;
		}

		sealed record UnitKey(string Id) : IReportUnitKey { }

		sealed record RevKey(int Number, bool Cancel, bool Final) : IReportRevisionKey {
			public int? Serial => Number > 0 ? Number : null;
			public bool IsCancellation => Cancel;
			public bool IsFinalRevision => Final;
			public bool IsComparableWith(IReportRevisionKey obj) => obj is RevKey;
			public int CompareTo(IReportRevisionKey? obj) {
				if (obj is not RevKey other) {
					throw new ArgumentException("bad key");
				}
				var cmp = Number.CompareTo(other.Number);
				return cmp != 0 ? cmp : Cancel.CompareTo(other.Cancel);
			}
		}
	}
}
