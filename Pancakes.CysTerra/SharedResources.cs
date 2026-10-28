using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using Cryville.EEW;

namespace Pancakes.CysTerra {
	public static class SharedResources {
		/// <summary>设置里事件源等短名：PancakesAPI。</summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string? SourceName([NotNull] ref CultureInfo? culture) {
			using var lres = new LocalizedResource("", ref culture);
			return lres.RootMessageStringSet.GetStringRequired("SourceName");
		}

		/// <summary>事件摘要组件显示名：PancakesAPI（GlobalQuake）。</summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string? ComponentName([NotNull] ref CultureInfo? culture) {
			using var lres = new LocalizedResource("", ref culture);
			return lres.RootMessageStringSet.GetStringRequired("ComponentName");
		}

		/// <summary>报告 Source / 预警条：GlobalQuake。</summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string? AuthorityName([NotNull] ref CultureInfo? culture) {
			using var lres = new LocalizedResource("", ref culture);
			return lres.RootMessageStringSet.GetStringRequired("AuthorityName");
		}
	}
}
