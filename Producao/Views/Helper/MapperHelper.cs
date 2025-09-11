using System;
using System.Linq;
using System.Reflection;

namespace Producao.Views.Helper;

public static class MapperHelper
{
    public static void CopyMatchingProperties(object source, object target)
    {
        if (source == null || target == null) return;

        var srcType = source.GetType();
        var tgtType = target.GetType();

        var srcProps = srcType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var tgtProps = tgtType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             .Where(p => p.CanWrite)
                             .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var sp in srcProps)
        {
            if (!tgtProps.TryGetValue(sp.Name, out var tp)) continue;

            var sval = sp.GetValue(source);
            if (sval == null)
            {
                tp.SetValue(target, null);
                continue;
            }

            if (tp.PropertyType.IsAssignableFrom(sp.PropertyType))
            {
                tp.SetValue(target, sval);
                continue;
            }

            try
            {
                var targetType = Nullable.GetUnderlyingType(tp.PropertyType) ?? tp.PropertyType;
                var converted = Convert.ChangeType(sval, targetType);
                tp.SetValue(target, converted);
            }
            catch
            {
                // opcional: log de conversão falhada
            }
        }
    }

}
