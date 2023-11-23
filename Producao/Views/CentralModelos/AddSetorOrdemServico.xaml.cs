using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Producao.Views.CentralModelos
{
    /// <summary>
    /// Interação lógica para AddSetorOrdemServico.xam
    /// </summary>
    public partial class AddSetorOrdemServico : UserControl
    {
        
        public AddSetorOrdemServico(object datacontex)
        {
            InitializeComponent();
            this.DataContext = datacontex;

            ModeloSetoresOrdemServicoViewModel vm = (ModeloSetoresOrdemServicoViewModel)DataContext;
        }

        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ModeloSetoresOrdemServicoViewModel vm = (ModeloSetoresOrdemServicoViewModel)DataContext;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ModeloSetoresOrdemServicoViewModel vm = (ModeloSetoresOrdemServicoViewModel)DataContext;
            vm.Itens?.Add(new HistoricoSetorModel() { codigo_setor = vm.Setor.codigo_setor, observacao = txtObservacao.Text, selesao = true, setor = vm.Setor.setor });
            vm.Setor = null;
            txtObservacao.Text = null;
        }
    }
}
