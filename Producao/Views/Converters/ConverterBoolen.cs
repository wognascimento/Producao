using System;
using System.Globalization;
using System.Windows.Data;

namespace Producao
{
    public class ConverterBoolen : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            if (value is bool booleano)
                return booleano;

            switch (value.ToString()?.Trim().ToUpperInvariant())
            {
                case "-1":
                case "1":
                case "TRUE":
                case "SIM":
                    return true;
                case "0":
                case "FALSE":
                case "NAO":
                case "NÃO":
                    return false;
            }

            return false;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var marcado = value is true;
            var destino = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (destino == typeof(bool))
                return marcado;

            return marcado ? "-1" : "0";
        }
    }
}
