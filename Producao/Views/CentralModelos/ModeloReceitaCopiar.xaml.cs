using Microsoft.EntityFrameworkCore;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
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
    /// Lógica interna para ModeloReceitaCopiar.xaml
    /// </summary>
    public partial class ModeloReceitaCopiar : Window
    {

        private QryModeloModel Modelo { get; set; }

        //public ObservableCollection<ModeloReceitaAnoAnterior> itens { get; set; }
        public ObservableCollection<ModeloReceitaModel> itens { get; set; }

        public ModeloReceitaCopiar(QryModeloModel Modelo)
        {
            InitializeComponent();
            this.Modelo = Modelo;
            this.DataContext = new ModeloReceitaCopiarViewModel();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                ModeloReceitaCopiarViewModel? vm = (ModeloReceitaCopiarViewModel)DataContext;
                vm.ItensReceita = await Task.Run(() => vm.GetModelosAsync(Modelo));
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void dgModelos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var modelo = (HistoricoModeloCompletaModel)dgModelos.SelectedItem;

            dgModelos.Columns["id_modelo"].FilteredFrom = FilteredFrom.FilterRow;
            dgModelos.Columns["id_modelo"].FilterPredicates.Add(new FilterPredicate()
            {
                FilterType = FilterType.Equals,
                FilterValue = modelo.id_modelo
            });

            this.itens = new ObservableCollection<ModeloReceitaModel>();
            var filteredResult = this.dgModelos.View.Records.Select(recordentry => recordentry.Data);
            foreach (HistoricoModeloCompletaModel item in filteredResult)
                this.itens.Add(
                    new ModeloReceitaModel
                    {
                        id_modelo = Modelo.id_modelo,
                        codcompladicional = item.itens_receita,
                        qtd_modelo = item.qtd_modelo_receita,
                        qtd_producao = item.qtd_producao_receita,
                        observacao = item.observacao,
                        cadastrado_por = Environment.UserName,
                        data_cadastro = DateTime.Now,
                    });

            this.DialogResult = true;
        }
    }

    public class ModeloReceitaCopiarViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private HistoricoModeloCompletaModel _itemReceita;
        public HistoricoModeloCompletaModel ItemReceita
        {
            get { return _itemReceita; }
            set { _itemReceita = value; RaisePropertyChanged("ItemReceita"); }
        }
        private ObservableCollection<HistoricoModeloCompletaModel> _itensReceita;
        public ObservableCollection<HistoricoModeloCompletaModel> ItensReceita
        {
            get { return _itensReceita; }
            set { _itensReceita = value; RaisePropertyChanged("ItensReceita"); }
        }

        public async Task<ObservableCollection<HistoricoModeloCompletaModel>> GetModelosAsync(QryModeloModel Modelo)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.HistoricoModeloCompletas
                    //.Where(c => c.planilha == Modelo.planilha && c.descricao == Modelo.descricao)
                    .ToListAsync();
                return new ObservableCollection<HistoricoModeloCompletaModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
