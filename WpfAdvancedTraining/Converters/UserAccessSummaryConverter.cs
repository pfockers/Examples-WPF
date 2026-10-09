using System.Globalization;
using System.Windows.Data;

namespace WpfAdvancedTraining.Converters;

public sealed class UserAccessSummaryConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2)
        {
            return "Unknown access";
        }

        var role = values[0]?.ToString() ?? "Unknown role";
        var isLocked = values[1] is true;
        return $"{role} · {(isLocked ? "Locked" : "Active")}";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
