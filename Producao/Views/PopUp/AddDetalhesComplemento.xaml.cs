using System;
using System.Windows;

namespace Producao.Views.popup
{
    /// <summary>
    /// Logica interna para AddDetalhesComplemento.xaml
    /// </summary>
    public partial class AddDetalhesComplemento : Window
    {
        public AddDetalhesComplemento(object vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private async void OnGravarDetalhesComplemento(object sender, RoutedEventArgs e)
        {
            CheckListViewModel vm = (CheckListViewModel)DataContext;
            try
            {
                vm.DetCompl.codcompl = vm.ComplementoCheckList.codcompl;
                vm.DetCompl.confirmado_data = vm.DetCompl.confirmado == "-1" ? DateTime.Now : null;
                vm.DetCompl.confirmado_por = vm.DetCompl.confirmado == "-1" ? Environment.UserName : null;
                vm.DetCompl = await vm.AddDetalhesComplementoCheckListAsync(vm.DetCompl);
                vm.CheckListGeralComplementos = await vm.GetCheckListGeralComplementoAsync(vm.DetCompl.codcompl);

                compelemntos.SelectedItem = null;
                quantidade.Value = null;
                confirmado.IsChecked = false;
                compelemntos.Focus();
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }
    }
}
