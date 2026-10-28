using System;
using System.Globalization;

namespace Pancakes.CysTerra {
	/// <summary>
	/// PancakesAPI/gq 的 intensity 为 MMI 罗马数字；Cryville.EEW 的 IntensityMMI 要求数值。
	/// </summary>
	public static class MmiIntensity {
		/// <summary>
		/// 将罗马数字或阿拉伯数字解析为 1–12；无法识别则返回 null。
		/// </summary>
		public static float? TryParse(string? text) {
			if (string.IsNullOrWhiteSpace(text)) {
				return null;
			}
			var t = text.Trim();
			if (t is "-" or "—" or "N/A" or "n/a" or "null") {
				return null;
			}
			if (float.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out var n)
				&& n >= 1 && n <= 12) {
				return n;
			}
			var roman = t.ToUpperInvariant()
				.Replace("Ⅷ", "VIII", StringComparison.Ordinal)
				.Replace("Ⅶ", "VII", StringComparison.Ordinal)
				.Replace("Ⅵ", "VI", StringComparison.Ordinal)
				.Replace("Ⅴ", "V", StringComparison.Ordinal)
				.Replace("Ⅳ", "IV", StringComparison.Ordinal)
				.Replace("Ⅲ", "III", StringComparison.Ordinal)
				.Replace("Ⅱ", "II", StringComparison.Ordinal)
				.Replace("Ⅰ", "I", StringComparison.Ordinal)
				.Replace("Ⅹ", "X", StringComparison.Ordinal)
				.Replace("Ⅸ", "IX", StringComparison.Ordinal)
				.Replace("Ⅺ", "XI", StringComparison.Ordinal)
				.Replace("Ⅻ", "XII", StringComparison.Ordinal);
			return roman switch {
				"I" => 1f,
				"II" => 2f,
				"III" => 3f,
				"IV" => 4f,
				"V" => 5f,
				"VI" => 6f,
				"VII" => 7f,
				"VIII" => 8f,
				"IX" => 9f,
				"X" => 10f,
				"XI" => 11f,
				"XII" => 12f,
				_ => null,
			};
		}
	}
}
