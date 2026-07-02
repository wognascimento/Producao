using Dapper;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Producao.Views.CentralModelos
{
    /// <summary>
    /// Lógica interna para ModeloControleChecklist.xaml
    /// </summary>
    public partial class ModeloControleChecklist : Window
    {

        private QryModeloModel modelo;

        public ModeloControleChecklist(QryModeloModel modelo)
        {
            InitializeComponent();
            this.DataContext = new ModeloControleChecklistViewModel();
            this.modelo = modelo;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            ModeloControleChecklistViewModel vm = (ModeloControleChecklistViewModel)DataContext;
            try
            {
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                vm.ItensChkList = await vm.GetControlesAsync(modelo);
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;

                //MessageBox.Show("Precisona a tecla F3 para dar baixa na linha selecionada.","Info Baixa", MessageBoxButton.OK, MessageBoxImage.Question);
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
            }
        }

        private async void dgItens_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ModeloControleChecklistViewModel vm = (ModeloControleChecklistViewModel)DataContext;
            if (e.Key == Key.F3)
            {
                if (vm.ItemChkList == null)
                {
                    MessageBox.Show("Seleciona uma linha para dar baixa no modelo.");
                    return;
                }
                var confirm = MessageBox.Show($"Deseja confirmar a inclusão do modelo {modelo.id_modelo} na linha selecionada?", "Baixa modelo", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (confirm == MessageBoxResult.Yes)
                {
                    try
                    {
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                        await vm.BaixaModeloAsync(vm.ItemChkList.coddetalhescompl, modelo.id_modelo);
                        vm.ItensChkList = await vm.GetControlesAsync(modelo);
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    }
                    catch (Exception ex)
                    {
                        Producao.ErrorDialog.Show(ex, "Erro");
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    }
                }
            }
        }
    }

    public class ModeloControleChecklistViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private ControleModeloBaixa _itemChkList;
        public ControleModeloBaixa ItemChkList
        {
            get { return _itemChkList; }
            set { _itemChkList = value; RaisePropertyChanged("ItemChkList"); }
        }
        private ObservableCollection<ControleModeloBaixa> _itensChkList;
        public ObservableCollection<ControleModeloBaixa> ItensChkList
        {
            get { return _itensChkList; }
            set { _itensChkList = value; RaisePropertyChanged("ItensChkList"); }
        }

        public async Task<ObservableCollection<ControleModeloBaixa>> GetControlesAsync(QryModeloModel modelo)
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                var data = await conn.QueryAsync<ControleModeloBaixa>(
                    @"SELECT *
                      FROM modelos.qry_controle_modelo_baixa
                      WHERE tema = @tema
                        AND codcompladicional = @codcompladicional
                        AND id_modelo IS NULL
                        AND qtd_compl_chk > 0;",
                    new
                    {
                        modelo.tema,
                        modelo.codcompladicional
                    });
                return new ObservableCollection<ControleModeloBaixa>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task BaixaModeloAsync(long? coddetalhescompl, long? id_modelo)
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                await conn.ExecuteAsync(
                    @"UPDATE producao.tbldetalhescomplemento
                      SET id_modelo = @id_modelo
                      WHERE coddetalhescompl = @coddetalhescompl;",
                    new { coddetalhescompl, id_modelo });
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
