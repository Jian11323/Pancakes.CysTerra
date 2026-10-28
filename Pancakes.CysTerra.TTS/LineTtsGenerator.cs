using Cryville.Common.Compat;
using Pancakes.CysTerra;
using Cryville.EEW.TTS;
using System.Globalization;
using System.Text;
using Cryville.EEW;

namespace Pancakes.CysTerra.TTS {
	public sealed class LineTtsGenerator : IContextedGenerator<LineEvent, ITTSMessageGeneratorContext, TTSEntry?> {
		public TTSEntry? Generate(LineEvent e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;

			var spoken = new StringBuilder();
			spoken.Append(res.GetStringRequired("TtsLead"));
			spoken.Append(res.GetStringRequired(TitleKey(e)));
			if (!string.IsNullOrEmpty(e.Place)) {
				spoken.Append(string.Format(culture, res.GetStringRequired("TtsPlace"), e.Place));
			}
			if (e.Magnitude is double mag) {
				spoken.Append(string.Format(culture, res.GetStringRequired("TtsMagnitude"), mag.ToString("0.0", culture)));
			}
			if (e.DepthKm is double depth) {
				spoken.Append(string.Format(culture, res.GetStringRequired("TtsDepth"), depth.ToString("0", culture)));
			}
			if (!string.IsNullOrEmpty(e.Intensity)) {
				spoken.Append(string.Format(culture, res.GetStringRequired("TtsIntensity"), e.Intensity));
			}

			// CysTerra：数值越小优先级越高；普通 0，紧急 -100。
			var priority = e.IsCancel ? -50 : -100;
			CultureInfo? cultureForName = culture;
			var repeatTitle = SharedResources.SourceName(ref cultureForName)
				?? res.GetStringRequired("TtsRepeatTitle");
			return new TTSEntry(culture, repeatTitle, spoken.ToString(), priority, SoundFor(e));
		}

		static string? SoundFor(LineEvent e) {
			if (e.IsCancel) {
				return "eew_update_cancel";
			}
			if (e.IsFinal) {
				return "eew_update_final";
			}
			if (e.Serial is > 1) {
				return "eew_update";
			}
			return IntensitySound(e.Intensity);
		}

		static string IntensitySound(string? intensity) {
			if (string.IsNullOrEmpty(intensity)) {
				return "eew_2";
			}
			var t = intensity.Trim().ToUpperInvariant();
			// MMI 罗马数字粗分档
			if (t is "X" or "XI" or "XII" or "IX") {
				return "eew_5";
			}
			if (t is "VII" or "VIII") {
				return "eew_4";
			}
			if (t is "V" or "VI") {
				return "eew_3";
			}
			if (t is "III" or "IV") {
				return "eew_2";
			}
			return "eew_1";
		}

		static string TitleKey(LineEvent e) {
			if (e.IsCancel) {
				return "TitleCancel";
			}
			if (e.IsFinal) {
				return "TitleFinal";
			}
			return e.Serial is > 1 ? "TitleUpdate" : "Title";
		}
	}
}
