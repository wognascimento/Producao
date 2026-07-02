using System;
using System.Globalization;
using System.Windows.Data;

namespace Producao
{
    public class ConverterNumber : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is null)
                    return string.Empty;

                //var valorFormatado = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:D}", value);
                var valor = Convert.ToDouble(value) % 1 == 0 ? value : string.Format(CultureInfo.CurrentCulture, "{0:N}", value);
                return valor;
            }
            catch
            {
            }
            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return null;

            var texto = value.ToString();
            if (string.IsNullOrWhiteSpace(texto))
                return null;

            var destino = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (destino == typeof(long) && long.TryParse(texto, NumberStyles.Number, culture, out var longValue))
                return longValue;

            if (destino == typeof(int) && int.TryParse(texto, NumberStyles.Number, culture, out var intValue))
                return intValue;

            if (destino == typeof(double) && double.TryParse(texto, NumberStyles.Number, culture, out var doubleValue))
                return doubleValue;

            if (destino == typeof(decimal) && decimal.TryParse(texto, NumberStyles.Number, culture, out var decimalValue))
                return decimalValue;

            return value;
        }
    }
}
