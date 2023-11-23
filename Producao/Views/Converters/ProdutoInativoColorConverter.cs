using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Producao
{
    public class ProdutoInativoColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = value as QryReceitaDetalheCriadoModel;
            if (data?.inativo?.Replace(" ", "") == "-1")
                return new SolidColorBrush(Colors.Orange);
            //else if (data.OrderID < 1007)
                //return new SolidColorBrush(Colors.Bisque);
            else
                return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
