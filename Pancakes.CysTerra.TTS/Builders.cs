using Cryville.EEW;
using Pancakes.CysTerra;
using Cryville.EEW.TTS;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Pancakes.CysTerra.TTS {
	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class TtsBuilder : IBuilder<IGenerator<TTSEntry>> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);

		public IGenerator<TTSEntry> Build(ref CultureInfo? culture) =>
			(IGenerator<TTSEntry>)(IGenerator<TTSEntry?>)new LineTtsGenerator();
	}
}
