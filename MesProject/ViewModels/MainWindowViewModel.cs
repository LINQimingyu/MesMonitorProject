using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using MesProject.Models;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace MesProject.ViewModels
{
    public class MainWindowViewModel : NotificationObject
    {
        private string _timeStr = string.Empty;

        public string TimeStr
        {
            get { return _timeStr; }
            set
            {
                _timeStr = value;
                RaisePropertyChanged();
            }
        }

        private string _dateStr = string.Empty;

        public string DateStr
        {
            get { return _dateStr; }
            set
            {
                _dateStr = value;
                RaisePropertyChanged();
            }
        }

        private string _weekStr = string.Empty;

        public string WeekStr
        {
            get { return _weekStr; }
            set
            {
                _weekStr = value;
                RaisePropertyChanged();
            }
        }

        private string _machineCount;

        // 机台总数
        public string MachineCount
        {
            get { return _machineCount; }
            set
            {
                _machineCount = value;
                RaisePropertyChanged();
            }
        }

        private string _productCount;

        // 生产计数
        public string ProductCount
        {
            get { return _productCount; }
            set
            {
                _productCount = value;
                RaisePropertyChanged();
            }
        }

        private string _unqualifiedCount;

        // 不良计数
        public string UnqualifiedCount
        {
            get { return _unqualifiedCount; }
            set
            {
                _unqualifiedCount = value;
                RaisePropertyChanged();
            }
        }

        public List<EnvironmentModel> EnvironmentModels { get; set; }

        private List<AlarmModel> _alarmModels;

        // 报警集合
        public List<AlarmModel> AlarmModels
        {
            get { return _alarmModels; }
            set
            {
                _alarmModels = value;
                RaisePropertyChanged();
            }
        }

        private List<DeviceModel> _deviceModels;

        public List<DeviceModel> DeviceModels
        {
            get { return _deviceModels; }
            set
            {
                _deviceModels = value;
                RaisePropertyChanged();
            }
        }

        // 产能柱状图数据集
        public ISeries[] CapacitySeries { get; set; }

        public Axis[] CapacityXAxes { get; set; }
        public Axis[] CapacityYAxes { get; set; }

        // 质量折线图数据集
        public ISeries[] QualitySeries { get; set; }

        public Axis[] QualityXAxes { get; set; }
        public Axis[] QualityYAxes { get; set; }

        // 数据异常报警比例饼图数据集
        public ISeries[] AlarmSeries { get; set; }

        private ObservableCollection<double>? _radarValues;

        public ObservableCollection<double>? RadarValues
        {
            get { return _radarValues; }
            set
            {
                _radarValues = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<string>? _radarTitles;

        public ObservableCollection<string>? RadarTitles
        {
            get { return _radarTitles; }
            set
            {
                _radarTitles = value;
                RaisePropertyChanged();
            }
        }

        public List<WorkshopModel> WorkshopModels { get; set; }

        private ControlEnum _currentControl;

        public ControlEnum CurrentControl
        {
            get { return _currentControl; }
            set
            {
                _currentControl = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 显示车间详情页面
        /// </summary>
        public DelegateCommand ShowWorkshopDetailCommand { get; set; }

        /// <summary>
        /// 显示首页
        /// </summary>
        public DelegateCommand ShowIndexCommand { get; set; }

        public MainWindowViewModel()
        {
            _currentControl = ControlEnum.Index;
            UpdateTime();
            StartTimer();
            
            _machineCount = "298";
            _productCount = "1643";
            _unqualifiedCount = "34";

            EnvironmentModels = new List<EnvironmentModel>()
            {
                new EnvironmentModel() { Name = "光照(Lux)", Value = 123 },
                new EnvironmentModel() { Name = "噪音(db)", Value = 55 },
                new EnvironmentModel() { Name = "温度(℃)", Value = 80 },
                new EnvironmentModel() { Name = "湿度(%)", Value = 43 },
                new EnvironmentModel() { Name = "PM2.5", Value = 20 },
                new EnvironmentModel() { Name = "硫化氢(PPM)", Value = 15 },
                new EnvironmentModel() { Name = "氮气(PPM)", Value = 18 },
            };

            _alarmModels = new List<AlarmModel>()
            {
                new AlarmModel()
                {
                    Num = "01",
                    Message = "设备温度过高",
                    DateTime = DateTime.Now,
                    Duration = 7,
                },
                new AlarmModel()
                {
                    Num = "02",
                    Message = "车间温度过高",
                    DateTime = DateTime.Now,
                    Duration = 10,
                },
                new AlarmModel()
                {
                    Num = "03",
                    Message = "设备转速过快",
                    DateTime = DateTime.Now,
                    Duration = 12,
                },
                new AlarmModel()
                {
                    Num = "04",
                    Message = "设备气压偏低",
                    DateTime = DateTime.Now,
                    Duration = 90,
                },
            };

            _deviceModels = new List<DeviceModel>()
            {
                new DeviceModel() { Name = "电能(Kw.h)", Value = 60.8 },
                new DeviceModel() { Name = "电压(V)", Value = 390 },
                new DeviceModel() { Name = "电流(A)", Value = 5 },
                new DeviceModel() { Name = "压差(kpa)", Value = 13 },
                new DeviceModel() { Name = "温度(℃)", Value = 36 },
                new DeviceModel() { Name = "振动(mm/s)", Value = 4.1 },
                new DeviceModel() { Name = "转速(r/min)", Value = 2600 },
                new DeviceModel() { Name = "气压(kpa)", Value = 0.5 },
            };

            var radarChartModels = new List<RadarChartModel>()
            {
                new RadarChartModel() { Name = "排烟风机", Value = 90 },
                new RadarChartModel() { Name = "稳压设备", Value = 30 },
                new RadarChartModel() { Name = "供水机", Value = 34.89 },
                new RadarChartModel() { Name = "喷淋水泵", Value = 69.59 },
                new RadarChartModel() { Name = "客梯", Value = 20 },
            };

            RadarValues = new ObservableCollection<double>();
            RadarTitles = new ObservableCollection<string>();
            foreach (var model in radarChartModels)
            {
                RadarValues.Add(model.Value);
                RadarTitles.Add(model.Name);
            }

            CapacitySeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = new int[] { 100, 200, 480, 450, 380, 450, 450, 330, 340 },
                    Name = "生产计数",
                    MaxBarWidth = 15,
                },
                new ColumnSeries<int>
                {
                    Values = new int[] { 15, 55, 15, 40, 38, 45 },
                    Name = "不良计数",
                    MaxBarWidth = 15,
                    Fill = new SolidColorPaint(SKColors.IndianRed),
                },
            };

            CapacityXAxes = new Axis[]
            {
                new Axis
                {
                    Labels = new string[]
                    {
                        "8:00",
                        "9:00",
                        "10:00",
                        "11:00",
                        "12:00",
                        "13:00",
                        "14:00",
                        "15:00",
                        "16:00",
                    },
                    LabelsPaint = new SolidColorPaint(SKColors.White),
                    TextSize = 10,
                },
            };

            CapacityYAxes = new Axis[]
            {
                new Axis
                {
                    LabelsPaint = new SolidColorPaint(SKColors.White),
                    TextSize = 10,
                    ForceStepToMin = true,
                    MinStep = 100,
                    MinLimit = 0,
                    SeparatorsPaint = new SolidColorPaint(SKColors.White.WithAlpha(50))
                    {
                        PathEffect = new DashEffect(new float[] { 3, 3 }),
                    },
                },
            };

            QualitySeries = new ISeries[]
            {
                new LineSeries<int> { Values = new int[] { 8, 2, 7, 6, 4, 14 }, Name = "质量" },
            };

            QualityXAxes = new Axis[]
            {
                new Axis
                {
                    Labels = new string[] { "1#", "2#", "3#", "4#", "5#", "6#" },
                    LabelsPaint = new SolidColorPaint(SKColors.White),
                    TextSize = 10,
                },
            };

            QualityYAxes = new Axis[]
            {
                new Axis
                {
                    LabelsPaint = new SolidColorPaint(SKColors.White),
                    TextSize = 10,
                    MinStep = 5,
                    ForceStepToMin = true,
                    MinLimit = 0,
                    SeparatorsPaint = new SolidColorPaint(SKColors.White.WithAlpha(50))
                    {
                        PathEffect = new DashEffect(new float[] { 3, 3 }),
                    },
                },
            };

            AlarmSeries = new ISeries[]
            {
                new PieSeries<double>
                {
                    Values = new double[] { 20 },
                    Name = "压差",
                    DataLabelsSize = 10,
                    DataLabelsPosition = PolarLabelsPosition.Middle,
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsFormatter = point =>
                        "压差 " + point.Coordinate.PrimaryValue.ToString(),
                },
                new PieSeries<double>
                {
                    Values = new double[] { 40 },
                    Name = "振动",
                    DataLabelsSize = 10,
                    DataLabelsPosition = PolarLabelsPosition.Middle,
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsFormatter = point =>
                        "振动 " + point.Coordinate.PrimaryValue.ToString(),
                },
                new PieSeries<double>
                {
                    Values = new double[] { 10 },
                    Name = "设备温度",
                    DataLabelsSize = 10,
                    DataLabelsPosition = PolarLabelsPosition.Middle,
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsFormatter = point =>
                        "设备温度 " + point.Coordinate.PrimaryValue.ToString(),
                },
                new PieSeries<double>
                {
                    Values = new double[] { 30 },
                    Name = "光照",
                    DataLabelsSize = 10,
                    DataLabelsPosition = PolarLabelsPosition.Middle,
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsFormatter = point =>
                        "光照 " + point.Coordinate.PrimaryValue.ToString(),
                },
            };

            WorkshopModels = new List<WorkshopModel>()
            {
                new WorkshopModel()
                {
                    Name = "贴片车间",
                    WorkingNum = 32,
                    WaitingNum = 8,
                    BreakdownNum = 4,
                    HaltNum = 0,
                },
                new WorkshopModel()
                {
                    Name = "封装车间",
                    WorkingNum = 20,
                    WaitingNum = 8,
                    BreakdownNum = 4,
                    HaltNum = 0,
                },
                new WorkshopModel()
                {
                    Name = "焊接车间",
                    WorkingNum = 32,
                    WaitingNum = 8,
                    BreakdownNum = 4,
                    HaltNum = 0,
                },
                new WorkshopModel()
                {
                    Name = "贴片车间",
                    WorkingNum = 68,
                    WaitingNum = 8,
                    BreakdownNum = 4,
                    HaltNum = 0,
                },
            };

            ShowWorkshopDetailCommand = new DelegateCommand();
            ShowWorkshopDetailCommand.ExecuteAction = ShowWorkshopDetail;

            ShowIndexCommand = new DelegateCommand();
            ShowIndexCommand.ExecuteAction = ShowIndex;
        }

        private void ShowIndex(object? param)
        {
            CurrentControl = ControlEnum.Index;
        }

        private void ShowWorkshopDetail(object? param)
        {
            CurrentControl = ControlEnum.WorkshopDetail;
        }

        private string GetChineseWeek()
        {
            int weekIndex = (int)DateTime.Now.DayOfWeek;
            string[] weeks = new string[7]
            {
                "星期日",
                "星期一",
                "星期二",
                "星期三",
                "星期四",
                "星期五",
                "星期六",
            };
            return weeks[weekIndex];
        }

        private async void StartTimer()
        {
            while (true)
            {
                UpdateTime();
                await Task.Delay(1000);
            }
        }

        private void UpdateTime()
        {
            TimeStr = DateTime.Now.ToString("HH:mm:ss");
            DateStr = DateTime.Now.ToString("yyyy-MM-dd");
            WeekStr = GetChineseWeek();
        }
    }
}
