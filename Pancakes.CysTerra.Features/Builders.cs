using Cryville.EEW;
using Cryville.EEW.Features;
using Pancakes.CysTerra;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Pancakes.CysTerra.Features {
	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class FeatureBuilder : SimpleBuilder<LineFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
