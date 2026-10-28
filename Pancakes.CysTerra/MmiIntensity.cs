using System;
using System.Globalization;
using Cryville.EEW.Report;

namespace Pancakes.CysTerra {
	/// <summary>
	/// PancakesAPI/gq 的 intensity 常为罗马数字；Severity / IntensityMMI 须传数值。
	/// 转换走官方 <see cref="RomanNumerals.RomanToInteger"/>。
	/// </summary>
	public static class MmiIntensity {
		/// <summary>解析为 MMI 1–12；无法识别则返回 null。</summary>
		public static float? TryParse(string? text) {
			if (string.IsNullOrWhiteSpace(text)) {
				return null;
			}
			var t = text.Trim();
			if (t is "-" or "—" or "N/A" or "n/a" or "null") {
				return null;
			}
			// 已是数值则直接用（上游偶发阿拉伯数字）
			if (float.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out var n)
				&& n >= 1 && n <= 12) {
				return n;
			}
			// RomanToInteger 只认多字符 ASCII（I…XII）；先把 Unicode 罗马字折成 ASCII
			t = NormalizeUnicodeRoman(t).ToUpperInvariant();
			try {
				var i = RomanNumerals.RomanToInteger(t);
				if (i >= 1 && i <= 12) {
					return i;
				}
			}
			catch (Exception) {
				// KeyNotFoundException 等：非法罗马串
			}
			return null;
		}

		static string NormalizeUnicodeRoman(string s) {
			return s
				.Replace("Ⅻ", "XII", StringComparison.Ordinal)
				.Replace("Ⅺ", "XI", StringComparison.Ordinal)
				.Replace("Ⅹ", "X", StringComparison.Ordinal)
				.Replace("Ⅸ", "IX", StringComparison.Ordinal)
				.Replace("Ⅷ", "VIII", StringComparison.Ordinal)
				.Replace("Ⅶ", "VII", StringComparison.Ordinal)
				.Replace("Ⅵ", "VI", StringComparison.Ordinal)
				.Replace("Ⅴ", "V", StringComparison.Ordinal)
				.Replace("Ⅳ", "IV", StringComparison.Ordinal)
				.Replace("Ⅲ", "III", StringComparison.Ordinal)
				.Replace("Ⅱ", "II", StringComparison.Ordinal)
				.Replace("Ⅰ", "I", StringComparison.Ordinal);
		}
	}
}
