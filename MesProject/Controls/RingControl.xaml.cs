using System.Windows;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace MesProject.Controls
{
    /// <summary>
    /// RingControl.xaml 的交互逻辑
    /// </summary>
    public partial class RingControl : UserControl
    {
        public static readonly DependencyProperty PercentProperty = DependencyProperty.Register(
            "Percent",
            typeof(int),
            typeof(RingControl),
            new PropertyMetadata(0)
        );

        public int Percent
        {
            get { return (int)GetValue(PercentProperty); }
            set { SetValue(PercentProperty, value); }
        }

        public RingControl()
        {
            InitializeComponent();
            SizeChanged += RingControl_SizeChanged;
        }

        private void RingControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Draw();
        }

        private void Draw()
        {
            IEnumerable<ISeries> MachineSeries = GaugeGenerator.BuildSolidGauge(
                new GaugeItem(
                    Percent,
                    series =>
                    {
                        series.Name = "完成率";
                        series.InnerRadius = 60;
                        series.DataLabelsSize = 30;
                        series.DataLabelsFormatter = p => $"{p.Coordinate.PrimaryValue} %";
                        series.DataLabelsPaint = new SolidColorPaint(SKColors.White);
                        series.Fill = new SolidColorPaint(SKColors.Orange);
                    }
                ),
                new GaugeItem(
                    GaugeItem.Background,
                    series =>
                    {
                        series.InnerRadius = 60;
                        series.Fill = new SolidColorPaint(SKColors.Gray);
                    }
                )
            );

            chart.Series = MachineSeries;
        }
    }
}
