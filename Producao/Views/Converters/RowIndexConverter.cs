using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.Converters;

public class RowIndexConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not GridViewRow row)
            return "";

        var grid = row.ParentOfType<RadGridView>();

        if (grid?.ItemsSource == null || row.Item == null)
            return "";

        ICollectionView view = CollectionViewSource.GetDefaultView(grid.ItemsSource);

        int index = 0;

        foreach (var item in view)
        {
            if (Equals(item, row.Item))
                return index + 1;

            index++;
        }

        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
