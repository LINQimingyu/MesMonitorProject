using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MesProject.Models;
using SkiaSharp;

namespace MesProject.ViewModels
{
    public class WorkshopDetailViewModel : NotificationObject
    {
        private List<MachineModel> _machineModels;

        public List<MachineModel> MachineModels
        {
            get { return _machineModels; }
            set
            {
                _machineModels = value;
                RaisePropertyChanged();
            }
        }

        private bool _isShowStackedColumnChart;

        public bool IsShowStackedColumnChart
        {
            get { return _isShowStackedColumnChart; }
            set
            {
                _isShowStackedColumnChart = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand ShowStackedColumnChartCommand { get; private set; }
        public DelegateCommand CloseStackedColumnChartCommand { get; private set; }

        private ISeries[] _columnSeries;

        public ISeries[] ColumnSeries
        {
            get { return _columnSeries; }
            set
            {
                _columnSeries = value;
                RaisePropertyChanged();
            }
        }

        private ICartesianAxis[] _xAxes;

        public ICartesianAxis[] XAxes
        {
            get { return _xAxes; }
            set
            {
                _xAxes = value;
                RaisePropertyChanged();
            }
        }

        private ICartesianAxis[] _yAxes;

        public ICartesianAxis[] YAxes
        {
            get { return _yAxes; }
            set
            {
                _yAxes = value;
                RaisePropertyChanged();
            }
        }

        public WorkshopDetailViewModel()
        {
            _machineModels = new List<MachineModel>();
            GenerateMachineModels();

            ShowStackedColumnChartCommand = new DelegateCommand();
            ShowStackedColumnChartCommand.ExecuteAction = ShowStackedColumnChart;

            CloseStackedColumnChartCommand = new DelegateCommand();
            CloseStackedColumnChartCommand.ExecuteAction = CloseStackedColumnChart;
        }

        private void CloseStackedColumnChart(object? obj)
        {
            IsShowStackedColumnChart = false;
        }

        private void ShowStackedColumnChart(object? obj)
        {
            IsShowStackedColumnChart = true;
            RefreshStackColumnChart();
        }

        private void RefreshStackColumnChart()
        {
            XAxes = new ICartesianAxis[]
            {
                new Axis
                {
                    Labels = new List<string>
                    {
                        "2024-01",
                        "2024-02",
                        "2024-03",
                        "2024-04",
                        "2024-05",
                        "2024-06",
                        "2024-07",
                        "2024-08",
                        "2024-09",
                        "2024-10",
                        "2024-11",
                        "2024-12",
                    },
                },
            };

            YAxes = new ICartesianAxis[]
            {
                new Axis
                {
                    MinLimit = 0,
                    MaxLimit = 100,
                    SeparatorsPaint = new SolidColorPaint(SKColors.Gray.WithAlpha(30)),
                },
            };

            ColumnSeries = new ISeries[]
            {
                new StackedColumnSeries<double>
                {
                    Values = new double[]
                    {
                        34.02,
                        36.72,
                        73.80,
                        54.18,
                        87.73,
                        61.63,
                        71.22,
                        56.96,
                        47.21,
                        42.67,
                        49.14,
                        76.79,
                    },
                    Fill = new SolidColorPaint(SKColors.LightGreen),
                    Stroke = null,
                    MaxBarWidth = 15,
                    YToolTipLabelFormatter = p => $"作业 {p.Coordinate.PrimaryValue}%",
                },
                new StackedColumnSeries<double>
                {
                    Values = new double[]
                    {
                        7.91,
                        7.75,
                        4.30,
                        0.40,
                        4.82,
                        8.98,
                        5.07,
                        2.42,
                        6.15,
                        0.57,
                        8.91,
                        8.75,
                    },
                    Fill = new SolidColorPaint(SKColors.Orange),
                    Stroke = null,
                    MaxBarWidth = 15,
                    YToolTipLabelFormatter = p => $"等待 {p.Coordinate.PrimaryValue}%",
                },
                new StackedColumnSeries<double>
                {
                    Values = new double[]
                    {
                        19.50,
                        19.51,
                        7.87,
                        15.63,
                        5.57,
                        13.69,
                        10.25,
                        10.99,
                        15.45,
                        13.87,
                        0.94,
                        12.94,
                    },
                    Fill = new SolidColorPaint(SKColors.PaleVioletRed),
                    Stroke = null,
                    MaxBarWidth = 15,
                    YToolTipLabelFormatter = p => $"故障 {p.Coordinate.PrimaryValue}%",
                },
                new StackedColumnSeries<double>
                {
                    Values = new double[]
                    {
                        38.57,
                        36.02,
                        14.03,
                        29.79,
                        1.87,
                        15.69,
                        13.45,
                        29.63,
                        31.18,
                        42.89,
                        41.01,
                        1.52,
                    },
                    Fill = new SolidColorPaint(SKColors.LightGray),
                    Stroke = null,
                    MaxBarWidth = 15,
                    YToolTipLabelFormatter = p => $"停机 {p.Coordinate.PrimaryValue}%",
                },
            };
        }

        private void GenerateMachineModels()
        {
            var random = new Random();
            for (int i = 0; i < 20; i++)
            {
                int planNum = random.Next(100, 1000);
                MachineModels.Add(
                    new MachineModel()
                    {
                        Name = $"焊接机-{i + 1}",
                        PlanNum = planNum,
                        CompleteNum = random.Next(0, planNum),
                        Status = "作业中",
                        OrderNo = $"H00100-{i + 1}",
                    }
                );
            }
        }
    }
}
