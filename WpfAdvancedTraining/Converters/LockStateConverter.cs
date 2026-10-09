using System.Globalization;
using System.Windows.Data;

namespace WpfAdvancedTraining.Converters;

public sealed class LockStateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is true ? "Locked" : "Active";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
