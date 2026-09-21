using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao;

internal static class ContextMenuSelectionDefaults
{
    private static bool registered;

    public static void Register()
    {
        if (registered)
            return;

        EventManager.RegisterClassHandler(
            typeof(RadGridView),
            UIElement.PreviewMouseRightButtonDownEvent,
            new MouseButtonEventHandler(OnGridPreviewMouseRightButtonDown),
            true);

        EventManager.RegisterClassHandler(
            typeof(RadListBox),
            UIElement.PreviewMouseRightButtonDownEvent,
            new MouseButtonEventHandler(OnListPreviewMouseRightButtonDown),
            true);

        registered = true;
    }

    private static void OnGridPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not RadGridView grid || FindVisualParent<RadGridView>(e.OriginalSource as DependencyObject) != grid)
            return;

        var row = FindVisualParent<GridViewRow>(e.OriginalSource as DependencyObject);
        if (row?.Item != null)
        {
            grid.SelectedItem = row.Item;
            grid.CurrentItem = row.Item;
            row.IsSelected = true;
            row.Focus();
            return;
        }

        grid.SelectedItem = null;
        grid.CurrentItem = null;
    }

    private static void OnListPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not RadListBox listBox ||
            FindVisualParent<RadListBox>(e.OriginalSource as DependencyObject) != listBox)
        {
            return;
        }

        var container = ItemsControl.ContainerFromElement(listBox, e.OriginalSource as DependencyObject);
        if (container == null)
        {
            listBox.SelectedItem = null;
            return;
        }

        var item = listBox.ItemContainerGenerator.ItemFromContainer(container);
        listBox.SelectedItem = item == DependencyProperty.UnsetValue ? null : item;
        if (container is UIElement element)
            element.Focus();
    }

    private static T? FindVisualParent<T>(DependencyObject? element)
        where T : DependencyObject
    {
        while (element != null)
        {
            if (element is T typed)
                return typed;

            element = VisualTreeHelper.GetParent(element);
        }

        return null;
    }
}
