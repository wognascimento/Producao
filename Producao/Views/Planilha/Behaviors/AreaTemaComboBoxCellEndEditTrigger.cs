using Microsoft.Xaml.Behaviors;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producao.Views.Planilha
{
    public class AreaTemaComboBoxCellEndEditTrigger : TargetedTriggerAction<SfDataGrid>
    {
        protected override void Invoke(object parameter)
        {
            CurrentCellEndEditEventArgs? args = parameter as CurrentCellEndEditEventArgs;
            var sfDataGrid = this.Target as SfDataGrid;

            var datarow = sfDataGrid.RowGenerator.Items.FirstOrDefault(dr => dr.RowIndex == args.RowColumnIndex.RowIndex);
            datarow.Element.DataContext = null;
            sfDataGrid.UpdateDataRow(args.RowColumnIndex.RowIndex);
        }
    }
}
