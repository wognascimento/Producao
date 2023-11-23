using Microsoft.EntityFrameworkCore;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views.Estoque
{
    /// <summary>
    /// Interação lógica para BaixaRequisicao.xam
    /// </summary>
    public partial class BaixaRequisicao : UserControl
    {
        public BaixaRequisicao()
        {
            InitializeComponent();
            DataContext = new BaixaRequisicaoViewModel();
        }

        private async void OnBuscaRequisicao(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    BaixaRequisicaoViewModel vm = (BaixaRequisicaoViewModel)DataContext;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    string text = ((TextBox)sender).Text;
                    vm.Itens = await Task.Run(() => vm.GetItensAsync(long.Parse(text)));
                    if (vm.Itens.Count == 0)
                    {
                        MessageBox.Show("Não existe itens nesta requisição", "Busca de requisição");
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                        return;
                    }
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
            }
        }

        private async void itens_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var sfdatagrid = sender as SfDataGrid;
            BaixaRequisicaoViewModel vm = (BaixaRequisicaoViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                BaixaEstoqueRequisicaoModel data = (BaixaEstoqueRequisicaoModel)e.RowData;
                var saida = new SaidaEstoqueModel
                {
                    quantidade = data.qtd_baixa, //Convert.ToDouble(txtQuantidade.Text),
                    destino = "PRODUÇÃO",
                    saida_data = DateTime.Now,
                    saida_por = Environment.UserName,
                    codcompladicional = data.codcompladicional,
                    processado = "-1",
                    num_requisicao = long.Parse(tbCodproduto.Text),
                    caminho = "R",
                };


                await Task.Run(() => vm.SaveSaidaAsync(saida));
                var record = sfdatagrid.View.CurrentAddItem as SaidaEstoqueModel;
                sfdatagrid.View.Refresh();

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }

    class BaixaRequisicaoViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<BaixaEstoqueRequisicaoModel> _itens;
        public ObservableCollection<BaixaEstoqueRequisicaoModel> Itens
        {
            get { return _itens; }
            set { _itens = value; RaisePropertyChanged("Itens"); }
        }

        public async Task<ObservableCollection<BaixaEstoqueRequisicaoModel>> GetItensAsync(long num_requisicao)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.BaixaEstoqueRequisicoes
                    .Where(c => c.num_requisicao == num_requisicao)
                    .ToListAsync();
                return new ObservableCollection<BaixaEstoqueRequisicaoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<SaidaEstoqueModel> SaveSaidaAsync(SaidaEstoqueModel saida)
        {
            try
            {
                using DatabaseContext db = new();
                await db.Saidas.SingleMergeAsync(saida);
                await db.SaveChangesAsync();
                return saida;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
