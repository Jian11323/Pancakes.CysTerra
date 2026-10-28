using Cryville.EEW;
using Cryville.EEW.Features;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Pancakes.CysTerra.Features {
	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class FeatureBuilder : SimpleBuilder<LineFeatureGenerator> {
		// 事件详情：PancakesAPI（GlobalQuake）
		public override string? GetName([NotNull] ref CultureInfo? culture) {
			using var lres = new LocalizedResource("", ref culture);
			return lres.RootMessageStringSet.GetStringRequired("ComponentName");
		}
	}
}
