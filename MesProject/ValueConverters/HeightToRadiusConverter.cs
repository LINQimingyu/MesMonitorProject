using System.Globalization;

namespace MesProject.ValueConverters
{
    public class HeightToRadiusConverter : ValueConverterBase<HeightToRadiusConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value - 50) / 2;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
