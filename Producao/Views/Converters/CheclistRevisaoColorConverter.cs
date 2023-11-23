using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Producao
{
    public class CheclistRevisaoColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //value == null || ((value != null) && double.Parse(value.ToString()) == 0)
            var data = value as QryCheckListGeralModel;
            if (data?.condicao == 1)
                return new SolidColorBrush(Colors.Yellow);
            else if (data?.condicao == 2)
                return new SolidColorBrush(Colors.LightGreen);
            else if (data?.condicao == 3)
                return new SolidColorBrush(Colors.LightSkyBlue);
            else
                return DependencyProperty.UnsetValue;
            //return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
