using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MesProject.Controls
{
    /// <summary>
    /// RadarChartControl.xaml 的交互逻辑
    /// </summary>
    public partial class RadarChartControl : UserControl
    {
        #region 私有属性，用于绘图

        /// <summary>
        /// 每个扇区的角度
        /// </summary>
        private double Angle { set; get; }

        /// <summary>
        /// 用于绘制雷达图的层数的多边形
        /// </summary>
        private List<Polygon> _radarChartLayersPolygon = new List<Polygon>();

        /// <summary>
        /// 用于绘制雷达图的射线
        /// </summary>
        private List<Polyline> _radarChartRadialsPolyline = new List<Polyline>();

        /// <summary>
        /// 用于绘制雷达图射线上实际值的圆点，使用多边形绘制，以实际值为圆心扩展多变形
        /// </summary>
        private List<Polygon> _radarChartRadialsValuesPolygons = new List<Polygon>();

        /// <summary>
        /// 所有的雷达图的多边形
        /// </summary>
        private Polygon _radarChartRadialsValuesPolygon = new Polygon();

        #endregion

        #region 雷达图图层

        /// <summary>
        /// 雷达图的层数
        /// </summary>
        public int Layers
        {
            get { return (int)GetValue(LayersProperty); }
            set
            {
                if (value < 1)
                    value = 1;

                SetValue(LayersProperty, value);
            }
        }

        public static readonly DependencyProperty LayersProperty = DependencyProperty.Register(
            "Layers",
            typeof(int),
            typeof(RadarChartControl),
            new PropertyMetadata(4, new PropertyChangedCallback(OnCurrentLayersChanged))
        );

        private static void OnCurrentLayersChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            RadarChartControl userControl = (RadarChartControl)d;
            bool needRefresh = false;
            if (userControl._radarChartLayersPolygon.Count > userControl.Layers)
            {
                int nCnt = userControl._radarChartLayersPolygon.Count - userControl.Layers;
                userControl._radarChartLayersPolygon.RemoveRange(
                    userControl._radarChartLayersPolygon.Count - 1 - nCnt,
                    nCnt
                );
                needRefresh = true;
            }
            else if (userControl._radarChartLayersPolygon.Count < userControl.Layers)
            {
                int nCnt = userControl.Layers - userControl._radarChartLayersPolygon.Count;
                for (int i = 0; i < nCnt; i++)
                {
                    userControl._radarChartLayersPolygon.Add(
                        new Polygon() { Stroke = userControl.LayerStroke, StrokeThickness = 1 }
                    );
                }

                needRefresh = true;
            }

            if (needRefresh)
            {
                userControl.RefreshRadarChart();
            }
        }

        /// <summary>
        /// 雷达图分层的规则,这里使用0-1之间的数据标识，主要是用比例来表示
        /// 在使用者未指定的情况下，则根据Layers的层数来均分
        /// 设置举例：雷达图分4层，均分每层面积，则LayersPercentList设置为：
        /// LayersPercentList[0] = 0.25;
        /// LayersPercentList[1] = 0.5;
        /// LayersPercentList[2] = 0.75;
        /// LayersPercentList[3] = 1;
        /// </summary>
        public IEnumerable<double> LayersPercentList
        {
            get { return (IEnumerable<double>)GetValue(LayersPercentListProperty); }
            set { SetValue(LayersPercentListProperty, value); }
        }

        public static readonly DependencyProperty LayersPercentListProperty =
            DependencyProperty.Register(
                "LayersPercentList",
                typeof(IEnumerable<double>),
                typeof(RadarChartControl),
                new FrameworkPropertyMetadata(
                    null,
                    new PropertyChangedCallback(OnChangedToRefreshRadarChart)
                )
            );

        /// <summary>
        /// 每层边框的粗细
        /// </summary>
        public double LayerStrokeThickness
        {
            get { return (double)GetValue(LayerStrokeThicknessProperty); }
            set { SetValue(LayerStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty LayerStrokeThicknessProperty =
            DependencyProperty.Register(
                "LayerStrokeThickness",
                typeof(double),
                typeof(RadarChartControl),
                new PropertyMetadata(
                    1.0,
                    new PropertyChangedCallback(OnCurrentLayersFillBrushAndStockThicknessChanged)
                )
            );

        /// <summary>
        /// 每层的边框颜色
        /// </summary>
        public SolidColorBrush LayerStroke
        {
            get { return (SolidColorBrush)GetValue(LayerStrokeProperty); }
            set { SetValue(LayerStrokeProperty, value); }
        }

        public static readonly DependencyProperty LayerStrokeProperty = DependencyProperty.Register(
            "LayerStroke",
            typeof(SolidColorBrush),
            typeof(RadarChartControl),
            new PropertyMetadata(
                Brushes.White,
                new PropertyChangedCallback(OnCurrentLayersFillBrushAndStockThicknessChanged)
            )
        );

        /// <summary>
        /// 雷达图从内到外渐变色，内部颜色
        /// </summary>
        public Color InnerColor
        {
            get { return (Color)GetValue(InnerColorProperty); }
            set { SetValue(InnerColorProperty, value); }
        }

        public static readonly DependencyProperty InnerColorProperty = DependencyProperty.Register(
            "InnerColor",
            typeof(Color),
            typeof(RadarChartControl),
            new PropertyMetadata(
                Colors.White,
                new PropertyChangedCallback(OnCurrentLayersFillBrushAndStockThicknessChanged)
            )
        );

        /// <summary>
        /// 雷达图从内到外渐变色，外部颜色
        /// </summary>
        public Color OutColor
        {
            get { return (Color)GetValue(OutColorProperty); }
            set { SetValue(OutColorProperty, value); }
        }

        public static readonly DependencyProperty OutColorProperty = DependencyProperty.Register(
            "OutColor",
            typeof(Color),
            typeof(RadarChartControl),
            new PropertyMetadata(
                Colors.Purple,
                new PropertyChangedCallback(OnCurrentLayersFillBrushAndStockThicknessChanged)
            )
        );

        private static void OnCurrentLayersFillBrushAndStockThicknessChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            RadarChartControl userControl = (RadarChartControl)d;
            userControl.RefreshLayersFillBrushAndThickness();
        }

        #endregion

        #region 雷达图射线

        /// <summary>
        /// 雷达图的射线数
        /// </summary>
        public int Radials
        {
            get { return (int)GetValue(RadialsProperty); }
            set { SetValue(RadialsProperty, value); }
        }

        public static readonly DependencyProperty RadialsProperty = DependencyProperty.Register(
            "Radials",
            typeof(int),
            typeof(RadarChartControl),
            new PropertyMetadata(9, new PropertyChangedCallback(OnCurrentRadialsChanged))
        );

        private static void OnCurrentRadialsChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            RadarChartControl userControl = (RadarChartControl)d;

            bool needRefresh = false;
            if (userControl._radarChartRadialsPolyline.Count > userControl.Radials)
            {
                int nCnt = userControl._radarChartRadialsPolyline.Count - userControl.Radials;
                userControl._radarChartRadialsPolyline.RemoveRange(
                    userControl._radarChartRadialsPolyline.Count - 1 - nCnt,
                    nCnt
                );
                needRefresh = true;
            }
            else if (userControl._radarChartRadialsPolyline.Count < userControl.Radials)
            {
                int nCnt = userControl.Radials - userControl._radarChartRadialsPolyline.Count;
                for (int i = 0; i < nCnt; i++)
                {
                    userControl._radarChartRadialsPolyline.Add(
                        new Polyline() { Stroke = userControl.RadialBrush, StrokeThickness = 2 }
                    );
                }
                needRefresh = true;
            }

            if (userControl._radarChartRadialsValuesPolygons.Count > userControl.Radials)
            {
                int nCnt = userControl._radarChartRadialsValuesPolygons.Count - userControl.Radials;
                userControl._radarChartRadialsValuesPolygons.RemoveRange(
                    userControl._radarChartRadialsValuesPolygons.Count - 1 - nCnt,
                    nCnt
                );
                needRefresh = true;
            }
            else if (userControl._radarChartRadialsValuesPolygons.Count < userControl.Radials)
            {
                int nCnt = userControl.Radials - userControl._radarChartRadialsValuesPolygons.Count;
                for (int i = 0; i < nCnt; i++)
                {
                    userControl._radarChartRadialsValuesPolygons.Add(
                        new Polygon() { Stroke = userControl.LayerStroke, StrokeThickness = 1 }
                    );
                }
                needRefresh = true;
            }

            if (needRefresh)
                userControl.RefreshRadarChart();
        }

        /// <summary>
        /// 雷达图半径,决定雷达图的半径
        /// </summary>
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
            "Radius",
            typeof(double),
            typeof(RadarChartControl),
            new PropertyMetadata(100.0, new PropertyChangedCallback(OnChangedToRefreshRadarChart))
        );

        /// <summary>
        /// 射线颜色
        /// </summary>
        public SolidColorBrush RadialBrush
        {
            get { return (SolidColorBrush)GetValue(RadialBrushProperty); }
            set { SetValue(RadialBrushProperty, value); }
        }

        public static readonly DependencyProperty RadialBrushProperty = DependencyProperty.Register(
            "RadialBrush",
            typeof(SolidColorBrush),
            typeof(RadarChartControl),
            new PropertyMetadata(
                Brushes.White,
                new PropertyChangedCallback(OnCurrentRadialBrushAndThicknessChanged)
            )
        );

        /// <summary>
        /// 射线粗细
        /// </summary>
        public double RadialThickness
        {
            get { return (double)GetValue(RadialThicknessProperty); }
            set { SetValue(RadialThicknessProperty, value); }
        }

        public static readonly DependencyProperty RadialThicknessProperty =
            DependencyProperty.Register(
                "RadialThickness",
                typeof(double),
                typeof(RadarChartControl),
                new PropertyMetadata(
                    1.0,
                    new PropertyChangedCallback(OnCurrentRadialBrushAndThicknessChanged)
                )
            );

        private static void OnCurrentRadialBrushAndThicknessChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            RadarChartControl userControl = (RadarChartControl)d;
            userControl.RefreshRadialBrushAndThinkness();
        }

        #endregion

        #region 雷达图上点

        /// <summary>
        /// 射线上的所有值点
        /// 1. 注意在使用绑定时，要先将Binding对象设置为null，然后将数据整合好的ObservableCollection再赋值给绑定对象，否则不更新
        /// </summary>
        public IEnumerable<double> Values
        {
            get { return (IEnumerable<double>)GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register(
            "Values",
            typeof(IEnumerable<double>),
            typeof(RadarChartControl),
            new FrameworkPropertyMetadata(
                null,
                new PropertyChangedCallback(OnChangedToRefreshValues)
            )
        );

        private static void OnChangedToRefreshValues(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            RadarChartControl userControl = (RadarChartControl)d;
            userControl.DrawRadarChartRadialsValues();
        }

        /// <summary>
        /// 普通值点的绘制半径
        /// </summary>
        public double ValueRadius
        {
            get { return (double)GetValue(ValueRadiusProperty); }
            set { SetValue(ValueRadiusProperty, value); }
        }

        public static readonly DependencyProperty ValueRadiusProperty = DependencyProperty.Register(
            "ValueRadius",
            typeof(double),
            typeof(RadarChartControl),
            new PropertyMetadata(
                4.0,
                new PropertyChangedCallback(OnChangedToRefreshValueRadiusAndBrush)
            )
        );

        /// <summary>
        /// 普通值点的颜色
        /// </summary>
        public SolidColorBrush ValueBrush
        {
            get { return (SolidColorBrush)GetValue(ValueBrushProperty); }
            set { SetValue(ValueBrushProperty, value); }
        }

        public static readonly DependencyProperty ValueBrushProperty = DependencyProperty.Register(
            "ValueBrush",
            typeof(SolidColorBrush),
            typeof(RadarChartControl),
            new PropertyMetadata(
                Brushes.Red,
                new PropertyChangedCallback(OnChangedToRefreshValueRadiusAndBrush)
            )
        );

        /// <summary>
        /// 需要高亮点的索引
        /// 1. 注意在使用绑定时，要先将Binding对象设置为null，然后将数据整合好的ObservableCollection再赋值给绑定对象，否则不更新
        /// </summary>
        public IEnumerable<double> HeightLightValues
        {
            get { return (IEnumerable<double>)GetValue(HeightLightValuesProperty); }
            set { SetValue(HeightLightValuesProperty, value); }
        }

        public static readonly DependencyProperty HeightLightValuesProperty =
            DependencyProperty.Register(
                "HeightLightValues",
                typeof(IEnumerable<double>),
                typeof(RadarChartControl),
                new FrameworkPropertyMetadata(
                    null,
                    new PropertyChangedCallback(OnChangedToRefreshValues)
                )
            );

        /// <summary>
        /// 光亮点的半径
        /// </summary>
        public double HeighLightRadius
        {
            get { return (double)GetValue(HeighLightRadiusProperty); }
            set { SetValue(HeighLightRadiusProperty, value); }
        }

        public static readonly DependencyProperty HeighLightRadiusProperty =
            DependencyProperty.Register(
                "HeighLightRadius",
                typeof(double),
                typeof(RadarChartControl),
                new PropertyMetadata(
                    6.0,
                    new PropertyChangedCallback(OnChangedToRefreshValueRadiusAndBrush)
                )
            );

        /// <summary>
        /// 高亮点的颜色
        /// </summary>
        public SolidColorBrush HeighLightBrush
        {
            get { return (SolidColorBrush)GetValue(HeighLightBrushProperty); }
            set { SetValue(HeighLightBrushProperty, value); }
        }

        public static readonly DependencyProperty HeighLightBrushProperty =
            DependencyProperty.Register(
                "HeighLightBrush",
                typeof(SolidColorBrush),
                typeof(RadarChartControl),
                new PropertyMetadata(
                    Brushes.Yellow,
                    new PropertyChangedCallback(OnChangedToRefreshValueRadiusAndBrush)
                )
            );

        private static void OnChangedToRefreshValueRadiusAndBrush(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            RadarChartControl userControl = (RadarChartControl)d;
            userControl.RefreshValuesRadiusAndBrush();
        }

        #endregion

        #region 雷达图值区域

        /// <summary>
        /// 雷达图值区域填充色
        /// </summary>
        public SolidColorBrush ValuesAreaFill
        {
            get { return (SolidColorBrush)GetValue(ValuesAreaFillProperty); }
            set { SetValue(ValuesAreaFillProperty, value); }
        }

        public static readonly DependencyProperty ValuesAreaFillProperty =
            DependencyProperty.Register(
                "ValuesAreaFill",
                typeof(SolidColorBrush),
                typeof(RadarChartControl),
                new PropertyMetadata(
                    Brushes.Red,
                    new PropertyChangedCallback(OnChangedToRefreshValuesAreaFillAndStrokeBrush)
                )
            );

        /// <summary>
        /// 雷达图值区域边框色
        /// </summary>
        public SolidColorBrush ValuesAreaStroke
        {
            get { return (SolidColorBrush)GetValue(ValuesAreaStrokeProperty); }
            set { SetValue(ValuesAreaStrokeProperty, value); }
        }

        public static readonly DependencyProperty ValuesAreaStrokeProperty =
            DependencyProperty.Register(
                "ValuesAreaStroke",
                typeof(SolidColorBrush),
                typeof(RadarChartControl),
                new PropertyMetadata(
                    Brushes.Gray,
                    new PropertyChangedCallback(OnChangedToRefreshValuesAreaFillAndStrokeBrush)
                )
            );

        private static void OnChangedToRefreshValuesAreaFillAndStrokeBrush(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            RadarChartControl userControl = (RadarChartControl)d;
            userControl.RefreshValuesAreaBrushAndStroke();
        }

        #endregion

        #region 雷达图的射线标题文字

        /// <summary>
        /// 是否显示Title
        /// </summary>
        public bool ShowTitle
        {
            get { return (bool)GetValue(ShowTitleProperty); }
            set { SetValue(ShowTitleProperty, value); }
        }

        public static readonly DependencyProperty ShowTitleProperty = DependencyProperty.Register(
            "ShowTitle",
            typeof(bool),
            typeof(RadarChartControl),
            new PropertyMetadata(false, new PropertyChangedCallback(OnChangedToRefreshTitles))
        );

        /// <summary>
        /// 文字的前景色
        /// </summary>
        public SolidColorBrush TitleForground
        {
            get { return (SolidColorBrush)GetValue(TitleForgroundProperty); }
            set { SetValue(TitleForgroundProperty, value); }
        }

        public static readonly DependencyProperty TitleForgroundProperty =
            DependencyProperty.Register(
                "TitleForground",
                typeof(SolidColorBrush),
                typeof(RadarChartControl),
                new PropertyMetadata(
                    Brushes.Black,
                    new PropertyChangedCallback(OnChangedToRefreshTitles)
                )
            );

        /// <summary>
        /// 文字的字号
        /// </summary>
        public int TitleFontSize
        {
            get { return (int)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        public static readonly DependencyProperty TitleFontSizeProperty =
            DependencyProperty.Register(
                "TitleFontSize",
                typeof(int),
                typeof(RadarChartControl),
                new PropertyMetadata(14, new PropertyChangedCallback(OnChangedToRefreshTitles))
            );

        /// <summary>
        /// FontWeight
        /// </summary>
        public FontWeight TitleFontWeight
        {
            get { return (FontWeight)GetValue(TitleFontWeightProperty); }
            set { SetValue(TitleFontWeightProperty, value); }
        }

        public static readonly DependencyProperty TitleFontWeightProperty =
            DependencyProperty.Register(
                "TitleFontWeights",
                typeof(FontWeight),
                typeof(RadarChartControl),
                new PropertyMetadata(
                    FontWeights.Normal,
                    new PropertyChangedCallback(OnChangedToRefreshTitles)
                )
            );

        /// <summary>
        /// Title要显示的文字
        /// </summary>
        public IEnumerable<string> Titles
        {
            get { return (IEnumerable<string>)GetValue(TitlesProperty); }
            set { SetValue(TitlesProperty, value); }
        }

        public static readonly DependencyProperty TitlesProperty = DependencyProperty.Register(
            "Titles",
            typeof(IEnumerable<string>),
            typeof(RadarChartControl),
            new FrameworkPropertyMetadata(
                null,
                new PropertyChangedCallback(OnChangedToRefreshTitles)
            )
        );

        private static void OnChangedToRefreshTitles(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            RadarChartControl userControl = (RadarChartControl)d;
            userControl.RefreshRadarChart();
        }

        #endregion

        private static void OnChangedToRefreshRadarChart(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            RadarChartControl userControl = (RadarChartControl)d;
            userControl.RefreshRadarChart();
        }

        public RadarChartControl()
        {
            //绘制图层
            List<Color> colors = GetSingleColorList(OutColor, InnerColor, Layers);
            for (int i = 0; i < Layers; i++)
            {
                _radarChartLayersPolygon.Add(
                    new Polygon()
                    {
                        //Fill = new SolidColorBrush(colors[i]),
                        Stroke = LayerStroke,
                        StrokeThickness = LayerStrokeThickness,
                    }
                );
            }

            //绘制射线以及线上值
            for (int i = 0; i < Radials; i++)
            {
                _radarChartRadialsPolyline.Add(
                    new Polyline() { Stroke = RadialBrush, StrokeThickness = RadialThickness }
                );
                _radarChartRadialsValuesPolygons.Add(
                    new Polygon() { Fill = ValueBrush, StrokeThickness = 1 }
                );
            }

            //雷达图值组成的区域
            _radarChartRadialsValuesPolygon = new Polygon()
            {
                Fill = ValuesAreaFill,
                Stroke = ValuesAreaStroke,
                Opacity = 0.5,
            };

            InitializeComponent();
        }

        private void RadarChartControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshRadarChart();
        }

        /// <summary>
        /// 刷新雷达图的层的填充色和层线粗细
        /// </summary>
        private void RefreshLayersFillBrushAndThickness()
        {
            //绘制雷达图层的多边形
            List<Color> colors = GetSingleColorList(OutColor, InnerColor, Layers);
            for (int i = 0; i < Layers; i++)
            {
                //RadarChartLayersPolygon[i].Fill = new SolidColorBrush(colors[i]);
                _radarChartLayersPolygon[i].Stroke = LayerStroke;
                _radarChartLayersPolygon[i].StrokeThickness = LayerStrokeThickness;
            }
        }

        /// <summary>
        /// 刷新射线的颜色和粗细
        /// </summary>
        private void RefreshRadialBrushAndThinkness()
        {
            foreach (var item in _radarChartRadialsPolyline)
            {
                item.Stroke = RadialBrush;
                item.StrokeThickness = RadialThickness;
            }
        }

        /// <summary>
        /// 刷新雷达图
        /// </summary>
        private void RefreshRadarChart()
        {
            Grid_RadarChart.Children.Clear();

            //首先清除一下polygon里存储的数据
            for (int i = 0; i < Layers; i++)
            {
                _radarChartLayersPolygon[i]?.Points?.Clear();
            }

            for (int i = 0; i < Radials; i++)
            {
                _radarChartRadialsPolyline[i]?.Points?.Clear();
            }

            //如果设置了LayersPercentList，并且LayersPercentList的元素个数与层数相同则按照LayersPercentList画每层的占比，否则均分每层占比
            List<double> layersPercents = new List<double>();
            if (
                LayersPercentList != null
                && LayersPercentList.Count() == Layers
                && LayersPercentList.Max() < 1
            )
            {
                foreach (var item in LayersPercentList)
                {
                    layersPercents.Add(item);
                }
            }
            else
            {
                double gap = 1.0 / Layers;
                for (int i = 0; i < Layers; i++)
                {
                    layersPercents.Add(gap * i + gap); //计算每层的默认占比
                }
            }

            //计算每个扇区的角度
            Angle = 360 / Radials;

            //计算并添加雷达图的区域线和射线上的点
            for (int i = 0; i < Radials; i++)
            {
                //射线上每层的点,从外到内
                List<Point> points = new List<Point>();
                for (int j = 0; j < Layers; j++)
                {
                    Point p = new Point(
                        Radius
                            + (Radius * layersPercents[Layers - j - 1])
                                * Math.Cos((Angle * i - 90) * Math.PI / 180),
                        Radius
                            + (Radius * layersPercents[Layers - j - 1])
                                * Math.Sin((Angle * i - 90) * Math.PI / 180)
                    );

                    points.Add(p);

                    //添加到区域线中
                    _radarChartLayersPolygon[j].Points.Add(p);
                }

                //添加到射线中
                foreach (var item in points)
                {
                    _radarChartRadialsPolyline[i].Points.Add(item);
                }

                //计算原点并添加到射线中
                Point p_origin = new Point(
                    Radius + Radius * 0 * Math.Cos((Angle * i - 90) * Math.PI / 180),
                    Radius + Radius * 0 * Math.Sin((Angle * i - 90) * Math.PI / 180)
                );

                _radarChartRadialsPolyline[i].Points.Add(p_origin);
            }

            //绘制区域层
            foreach (var polygon in _radarChartLayersPolygon)
            {
                if (!Grid_RadarChart.Children.Contains(polygon))
                    Grid_RadarChart.Children.Add(polygon);
            }

            //绘制雷达图射线
            foreach (var polyline in _radarChartRadialsPolyline)
            {
                if (!Grid_RadarChart.Children.Contains(polyline))
                    Grid_RadarChart.Children.Add(polyline);
            }

            //绘制雷达图上的文字
            if (ShowTitle && Titles != null && Titles.Count() == Radials)
            {
                List<string> titleList = Titles.ToList();
                for (int i = 0; i < Radials; i++)
                {
                    Point point = _radarChartLayersPolygon[0].Points[i];
                    string title = titleList[i];
                    TextBlock textBlock = RefreshRadiusTitles(point, title);

                    if (!Grid_RadarChart.Children.Contains(textBlock))
                        Grid_RadarChart.Children.Add(textBlock);
                }
            }

            DrawRadarChartRadialsValues();
        }

        /// <summary>
        /// 刷新雷达图上值的点的半径和填充色，以及高亮点的半径和填充色
        /// </summary>
        private void RefreshValuesRadiusAndBrush()
        {
            if (Values == null)
                return;

            bool drawHeight = false;
            if (HeightLightValues != null && HeightLightValues.Count() > 0)
                drawHeight = true;

            List<double> values = Values.ToList();
            for (int i = 0; i < _radarChartRadialsValuesPolygon.Points.Count; i++)
            {
                _radarChartRadialsValuesPolygons[i].Points.Clear();
                _radarChartRadialsValuesPolygons[i].Fill = ValueBrush;

                double radius = ValueRadius;

                if (drawHeight)
                {
                    if (HeightLightValues.Contains(values[i]))
                    {
                        radius = HeighLightRadius;
                        _radarChartRadialsValuesPolygons[i].Fill = HeighLightBrush;

                        if (ShowTitle && Titles != null && Titles.Count() > i)
                        {
                            List<string> titleList = Titles.ToList();
                            string heightTitle = titleList[i];
                            foreach (var item in Grid_RadarChart.Children)
                            {
                                if (item is TextBlock)
                                {
                                    TextBlock textBlock = (TextBlock)item;
                                    if (textBlock.Text == heightTitle)
                                    {
                                        textBlock.Foreground = HeighLightBrush;
                                    }
                                }
                            }
                        }
                    }
                }

                Point valuePoint = _radarChartRadialsValuesPolygon.Points[i];
                Point[] calc_points = GetEllipsePoints(valuePoint, radius);

                foreach (var p in calc_points)
                {
                    _radarChartRadialsValuesPolygons[i].Points.Add(p);
                }

                if (!Grid_RadarChart.Children.Contains(_radarChartRadialsValuesPolygons[i]))
                    Grid_RadarChart.Children.Add(_radarChartRadialsValuesPolygons[i]);
            }
        }

        /// <summary>
        /// 刷新雷达图值区域的填充色和边框色
        /// </summary>
        private void RefreshValuesAreaBrushAndStroke()
        {
            _radarChartRadialsValuesPolygon.Fill = ValuesAreaFill;
            _radarChartRadialsValuesPolygon.Stroke = ValuesAreaStroke;
        }

        /// <summary>
        /// 刷新射线上的文字标题
        /// </summary>
        /// <param name="point">图层最外层的点</param>
        /// <returns></returns>
        private TextBlock RefreshRadiusTitles(Point point, string title)
        {
            TextBlock textBlock = new TextBlock();

            textBlock.FontSize = 20;
            textBlock.Text = title;
            textBlock.Foreground = TitleForground;
            textBlock.FontWeight = FontWeights.Normal;
            textBlock.FontSize = TitleFontSize;

            //计算文字的实际像素值
            Rect rect1 = new Rect();
            textBlock.Arrange(rect1);
            double textLength = textBlock.ActualWidth;

            Thickness thickness = new Thickness(point.X + 10, point.Y - 10, 0, 0);
            if (point.X == Radius && point.Y < Radius)
            {
                thickness = new Thickness(point.X - textLength / 2, point.Y - 30, 0, 0);
            }
            else if (point.X == Radius && point.Y >= Radius)
            {
                thickness = new Thickness(point.X - textLength / 2, point.Y + 10, 0, 0);
            }
            else if (point.X < Radius)
            {
                thickness = new Thickness(point.X - 20 - textLength, point.Y - 10, 0, 0);
            }
            else
            {
                thickness = new Thickness(point.X + 10, point.Y - 10, 0, 0);
            }

            textBlock.Margin = thickness;

            return textBlock;
        }

        /// <summary>
        /// 绘制雷达图上的点
        /// </summary>
        /// <param name="Values"></param>
        /// <param name="mainType"></param>
        /// <param name="secondType"></param>
        public void DrawRadarChartRadialsValues()
        {
            if (Values == null || Values.Count() != Radials)
                return;

            int fullScore = 100;

            _radarChartRadialsValuesPolygon.Points.Clear();
            for (int i = 0; i < Radials; i++)
            {
                double temp = Values.ToList()[i];

                if (temp <= 0)
                    continue;

                Point value = new Point(
                    Radius
                        + Radius
                            * (temp * 1.0 / fullScore)
                            * Math.Cos((Angle * i - 90) * Math.PI / 180),
                    Radius
                        + Radius
                            * (temp * 1.0 / fullScore)
                            * Math.Sin((Angle * i - 90) * Math.PI / 180)
                );

                _radarChartRadialsValuesPolygon.Points.Add(value);
            }

            if (!Grid_RadarChart.Children.Contains(_radarChartRadialsValuesPolygon))
                Grid_RadarChart.Children.Add(_radarChartRadialsValuesPolygon);

            RefreshValuesRadiusAndBrush();
        }

        #region 工具类

        /// <summary>
        /// 根据圆心，扩展绘制圆
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private Point[] GetEllipsePoints(Point origin, double radius)
        {
            int count = 10;
            Point[] points = new Point[count];

            double angle = 360 / count;

            for (int i = 0; i < count; i++)
            {
                Point p1 = new Point(
                    origin.X + radius * Math.Cos((angle * i - 90) * Math.PI / 180),
                    origin.Y + radius * Math.Sin((angle * i - 90) * Math.PI / 180)
                );

                points[i] = p1;
            }

            return points;
        }

        /// <summary>
        /// 获得某一颜色区间的颜色集合
        /// </summary>
        /// <param name="sourceColor">起始颜色</param>
        /// <param name="destColor">终止颜色</param>
        /// <param name="count">分度数</param>
        /// <returns>返回颜色集合</returns>
        private List<Color> GetSingleColorList(Color srcColor, Color desColor, int count)
        {
            List<Color> colorFactorList = new List<Color>();
            int redSpan = desColor.R - srcColor.R;
            int greenSpan = desColor.G - srcColor.G;
            int blueSpan = desColor.B - srcColor.B;
            for (int i = 0; i < count; i++)
            {
                Color color = Color.FromRgb(
                    (byte)(srcColor.R + (int)((double)i / count * redSpan)),
                    (byte)(srcColor.G + (int)((double)i / count * greenSpan)),
                    (byte)(srcColor.B + (int)((double)i / count * blueSpan))
                );
                colorFactorList.Add(color);
            }
            return colorFactorList;
        }
        #endregion
    }
}
