using System.Globalization;

namespace MesProject.ValueConverters
{
    public class ChartHeightConverter : ValueConverterBase<ChartHeightConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((double)value == 0)
                return 0;
            return (double)value - 25;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
