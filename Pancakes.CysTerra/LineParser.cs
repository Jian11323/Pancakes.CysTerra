using System;
using System.Globalization;
using System.Text.Json;

namespace Pancakes.CysTerra {
	/// <summary>
	/// 解析 PancakesAPI /gq（官方旧 TCP ObjectStream 已停，字段映射尽量对齐 GlobalQuakeReport）。
	/// </summary>
	static class LineParser {
		public static LineEvent? Parse(JsonElement root) {
			if (root.ValueKind != JsonValueKind.Object) {
				return null;
			}

			var type = Text(root, "type");
			if (type is "heartbeat" or "pong" or "error" or "ping") {
				return null;
			}

			var action = Text(root, "action");
			JsonElement data;
			if (root.TryGetProperty("payload", out var payload) && payload.ValueKind == JsonValueKind.Object) {
				data = payload;
				if (type != null && type != "earthquake") {
					return null;
				}
			}
			else if (root.TryGetProperty("Data", out var legacy) && legacy.ValueKind == JsonValueKind.Object) {
				data = legacy;
			}
			else {
				data = root;
			}

			var status = Text(data, "status") ?? Text(root, "status");
			var cancel = string.Equals(action, "cancelled", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(action, "canceled", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(status, "cancelled", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(status, "canceled", StringComparison.OrdinalIgnoreCase)
				|| Flag(data, "isCancel", "cancelled", "canceled");
			var final = string.Equals(action, "archived", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(status, "archived", StringComparison.OrdinalIgnoreCase)
				|| Flag(data, "isFinal");

			var mag = Num(data, "magnitude");
			var lat = Num(data, "latitude");
			var lon = Num(data, "longitude");
			var depth = Num(data, "depth", "depthKm");
			var place = Text(data, "region", "place", "placeName");
			var serial = Revision(data);
			if (serial == null && !cancel && !final) {
				serial = 1;
			}
			var id = Text(data, "id", "eventId", "EventID", "uuid")
				?? string.Join("|", place ?? "", Fmt(lat), Fmt(lon), Fmt(mag));
			var origin = Time(data, "originTimeMs", "originTime", "originTimeIso", "time", "startAt", "origin");
			var lastUpdated = Time(data, "lastUpdate", "lastUpdated", "lastUpdateMs", "updatedAt", "finalUpdateMillis")
				?? Time(root, "timestampMs", "timestamp")
				?? origin;
			var intensity = Intensity(data);
			var intensityMmi = MmiIntensity.TryParse(intensity);

			if (!cancel && (mag == null || lat == null || lon == null || depth == null || origin == null)) {
				return null;
			}

			var fp = string.Join("|", id, serial, cancel, final, Fmt(mag), Fmt(depth), Fmt(lat), Fmt(lon), place, intensity,
				lastUpdated?.Ticks);
			return new LineEvent {
				Agency = "gq",
				UnitId = id,
				Serial = serial,
				IsCancel = cancel,
				IsFinal = final,
				Place = place,
				Magnitude = mag,
				DepthKm = depth,
				Latitude = lat,
				Longitude = lon,
				OriginUtc = origin,
				LastUpdatedUtc = lastUpdated,
				Intensity = intensity,
				IntensityMmi = intensityMmi,
				Fingerprint = fp,
			};
		}

		static string? Text(JsonElement o, params string[] keys) {
			foreach (var k in keys) {
				if (!o.TryGetProperty(k, out var v)) {
					continue;
				}
				if (v.ValueKind == JsonValueKind.String) {
					var s = v.GetString()?.Trim();
					if (!string.IsNullOrEmpty(s)) {
						return s;
					}
				}
				if (v.ValueKind == JsonValueKind.Number) {
					return v.GetRawText();
				}
			}
			return null;
		}

		static double? Num(JsonElement o, params string[] keys) {
			foreach (var k in keys) {
				if (!o.TryGetProperty(k, out var v)) {
					continue;
				}
				if (v.ValueKind == JsonValueKind.Number && v.TryGetDouble(out var n)) {
					return n;
				}
				if (v.ValueKind == JsonValueKind.String
					&& double.TryParse(v.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var s)) {
					return s;
				}
			}
			return null;
		}

		static int? Int(JsonElement o, params string[] keys) {
			var n = Num(o, keys);
			if (n == null) {
				return null;
			}
			var i = (int)n.Value;
			return i > 0 ? i : null;
		}

		/// <summary>优先 revisionId；忽略像毫秒时间戳的巨大 revision。</summary>
		static int? Revision(JsonElement o) {
			var id = Int(o, "revisionId", "updates", "reportNum");
			if (id != null) {
				return id;
			}
			var rev = Num(o, "revision");
			if (rev == null || rev <= 0 || rev > 100_000) {
				return null;
			}
			return (int)rev.Value;
		}

		static string? Intensity(JsonElement o) {
			var s = Text(o, "intensity", "maxIntensity");
			if (string.IsNullOrEmpty(s) || s is "-" or "—" or "N/A" or "n/a" or "null") {
				return null;
			}
			return s;
		}

		static DateTime? Time(JsonElement o, params string[] keys) {
			foreach (var k in keys) {
				if (!o.TryGetProperty(k, out var v)) {
					continue;
				}
				if (v.ValueKind == JsonValueKind.Number && v.TryGetDouble(out var n)) {
					var ms = n > 1e12 ? (long)n : n > 1e9 ? (long)(n * 1000) : 0L;
					if (ms > 0) {
						return DateTime.UnixEpoch.AddMilliseconds(ms);
					}
				}
				if (v.ValueKind == JsonValueKind.String
					&& DateTime.TryParse(v.GetString(), CultureInfo.InvariantCulture,
						DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dt)) {
					return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
				}
			}
			return null;
		}

		static bool Flag(JsonElement o, params string[] keys) {
			foreach (var k in keys) {
				if (!o.TryGetProperty(k, out var v)) {
					continue;
				}
				if (v.ValueKind == JsonValueKind.True) {
					return true;
				}
				if (v.ValueKind == JsonValueKind.Number && v.TryGetDouble(out var n) && n != 0) {
					return true;
				}
				if (v.ValueKind == JsonValueKind.String
					&& v.GetString() is "1" or "true" or "True" or "cancelled" or "canceled" or "archived") {
					return true;
				}
			}
			return false;
		}

		static string Fmt(double? n) => n?.ToString("G", CultureInfo.InvariantCulture) ?? "";
	}
}
