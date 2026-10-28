using Cryville.Common.Compat;
using Cryville.EEW.Features;
using Pancakes.CysTerra;
using Cryville.Measure;
using System;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;
using Cryville.EEW;

namespace Pancakes.CysTerra.Features {
	public sealed class LineFeatureGenerator : IGenerator<LineEvent, Feature?> {
		static readonly Unit Kilometre = Units.Metre.WithPrefix(MetricPrefixes.Kilo);

		public Feature? Generate(LineEvent e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			var feature = new Feature {
				{ Is, Earthquake },
				{ Ongoing, !e.IsCancel && !e.IsFinal },
				{ Source, e.Agency },
				{ Subject, ReportForecast },
			};
			if (!string.IsNullOrEmpty(e.Place)) {
				feature[Name] = e.Place;
			}
			if (e.OriginUtc is DateTime origin) {
				feature[Time] = new DateTimeOffset(DateTime.SpecifyKind(origin, DateTimeKind.Utc));
			}
			if (e.Magnitude is double mag) {
				feature[Magnitude] = new Quantity(mag, Units.Dimensionless);
			}
			if (e.Latitude is double lat && e.Longitude is double lon) {
				var at = new Feature(new Point(lon, lat)) { { Is, Hypocenter } };
				if (e.DepthKm is double depth) {
					at[HypocenterDepth] = new Quantity(depth, Kilometre);
				}
				feature[At] = at;
			}
			// PancakesAPI/gq：intensity 为 MMI 罗马数字（I–XII）
			if (!string.IsNullOrEmpty(e.Intensity)) {
				feature[IntensityMMI] = e.Intensity;
			}
			return feature;
		}
	}
}
