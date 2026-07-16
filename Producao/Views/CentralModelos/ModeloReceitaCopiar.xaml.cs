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
    public partial class ModeloReceitaCopiar : Window
    {
        private QryModeloModel Modelo { get; set; }
        public ObservableCollection<ModeloReceitaModel> itens { get; set; }

        public ModeloReceitaCopiar(QryModeloModel modelo)
        {
            InitializeComponent();
            Modelo = modelo;
            DataContext = new ModeloReceitaCopiarViewModel();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                ModeloReceitaCopiarViewModel vm = (ModeloReceitaCopiarViewModel)DataContext;
                vm.ItensReceita = await vm.GetModelosAsync(Modelo);
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
        }

        private void dgModelos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgModelos.SelectedItem is not HistoricoModeloCompletaModel modeloSelecionado)
            {
                return;
            }

            ModeloReceitaCopiarViewModel vm = (ModeloReceitaCopiarViewModel)DataContext;
            itens = new ObservableCollection<ModeloReceitaModel>(
                vm.ItensReceita
                    .Where(item => item.id_modelo == modeloSelecionado.id_modelo && item.ano == modeloSelecionado.ano)
                    .Select(item => new ModeloReceitaModel
                    {
                        id_modelo = Modelo.id_modelo,
                        codcompladicional = item.itens_receita,
                        qtd_modelo = item.qtd_modelo_receita,
                        qtd_producao = item.qtd_producao_receita,
                        observacao = item.observacao,
                        cadastrado_por = Environment.UserName,
                        data_cadastro = DateTime.Now,
                    }));

            DialogResult = true;
        }
    }

    public class ModeloReceitaCopiarViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private HistoricoModeloCompletaModel _itemReceita;
        public HistoricoModeloCompletaModel ItemReceita
        {
            get => _itemReceita;
            set { _itemReceita = value; RaisePropertyChanged(nameof(ItemReceita)); }
        }

        private ObservableCollection<HistoricoModeloCompletaModel> _itensReceita;
        public ObservableCollection<HistoricoModeloCompletaModel> ItensReceita
        {
            get => _itensReceita;
            set { _itensReceita = value; RaisePropertyChanged(nameof(ItensReceita)); }
        }

        public async Task<ObservableCollection<HistoricoModeloCompletaModel>> GetModelosAsync(QryModeloModel modelo)
        {
            await using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
            const string sql = """
                SELECT *
                FROM modelos.view_historico_modelo_completa;
                """;

            var data = await conn.QueryAsync<HistoricoModeloCompletaModel>(sql);
            return new ObservableCollection<HistoricoModeloCompletaModel>(data);
        }
    }
}

