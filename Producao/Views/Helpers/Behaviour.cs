//using Microsoft.Xaml.Behaviors;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace SearchPanel //Producao.Views.Helpers
{
    class Behaviour : Behavior<UserControl>
    {
        SfDataGrid dataGrid;
        SearchControl searchControl;
        protected override void OnAttached()
        {
            var window = this.AssociatedObject;
            this.dataGrid = (SfDataGrid)window.FindName("dataGrid");
            this.dataGrid.KeyDown += OnDataGridKeyDown;
            this.searchControl = (SearchControl)window.FindName("searchControl");
        }

        /// <summary>
        /// Invokes thid Event to show the AdonerDecorator in the view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataGridKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != ModifierKeys.None && e.Key == Key.F)
                searchControl.UpdateSearchControlVisiblity(true);
            else
                searchControl.UpdateSearchControlVisiblity(false);
        }
        protected override void OnDetaching()
        {
            this.dataGrid.KeyDown -= OnDataGridKeyDown;
        }
    }
}
