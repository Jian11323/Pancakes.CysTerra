using Cryville.EEW;
using Cryville.EEW.TTS;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Pancakes.CysTerra.TTS {
	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class TtsBuilder : IBuilder<IGenerator<TTSEntry>> {
		// 语音消息：PancakesAPI（GlobalQuake）
		public string? GetName([NotNull] ref CultureInfo? culture) {
			using var lres = new LocalizedResource("", ref culture);
			return lres.RootMessageStringSet.GetStringRequired("ComponentName");
		}

		public IGenerator<TTSEntry> Build(ref CultureInfo? culture) =>
			(IGenerator<TTSEntry>)(IGenerator<TTSEntry?>)new LineTtsGenerator();
	}
}
