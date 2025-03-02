using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using System.Windows;

namespace MesProject
{

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // 设置LiveCharts正常显示中文
            LiveCharts.Configure(config => config.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('汉')));

            base.OnStartup(e);
        }
    }

}
