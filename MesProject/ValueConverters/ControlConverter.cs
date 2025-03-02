using System.Globalization;
using MesProject.Controls;
using MesProject.Models;

namespace MesProject.ValueConverters
{
    public class ControlConverter : ValueConverterBase<ControlConverter>
    {
        public override object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            switch ((ControlEnum)value)
            {
                case ControlEnum.Index:
                    return new MainControl();
                case ControlEnum.WorkshopDetail:
                    return new WorkshopDetailControl();
                default:
                    return null;
            }
        }

        public override object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            throw new NotImplementedException();
        }
    }
}
