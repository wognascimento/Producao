using Dapper;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace Producao.Views.CentralModelos
{
    public partial class ViewCentralTabelaPA : UserControl
    {
        public ViewCentralTabelaPA()
        {
            InitializeComponent();
            DataContext = new ViewCentralTabelaPAViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewCentralTabelaPAViewModel vm = (ViewCentralTabelaPAViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                vm.Produtos = await vm.GetProdutosAsync();
                vm.Itens = await vm.GetItensAsync();
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
        }

        private void OnRowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.Row?.Item is not ModeloTabelaPAModel model)
            {
                return;
            }

            if (!model.codcompladicional.HasValue)
            {
                e.IsValid = false;
                AddValidation(e, nameof(ModeloTabelaPAModel.codcompladicional), "Selecione a P.A.");
            }
        }

        private async void OnRowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            if (e.EditedItem is not ModeloTabelaPAModel model)
            {
                return;
            }

            ViewCentralTabelaPAViewModel vm = (ViewCentralTabelaPAViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                await vm.SaveAsync(model);
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
                MessageBox.Show("Fator P.A cadastrado!!!");
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
        }

        private static void AddValidation(GridViewRowValidatingEventArgs e, string propertyName, string message)
        {
            e.ValidationResults.Add(new GridViewCellValidationResult
            {
                PropertyName = propertyName,
                ErrorMessage = message
            });
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }
    }

    public class ViewCentralTabelaPAViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ModeloTabelaPAModel item;
        public ModeloTabelaPAModel Item
        {
            get => item;
            set { item = value; RaisePropertyChanged(nameof(Item)); }
        }

        private ObservableCollection<ModeloTabelaPAModel> itens;
        public ObservableCollection<ModeloTabelaPAModel> Itens
        {
            get => itens;
            set { itens = value; RaisePropertyChanged(nameof(Itens)); }
        }

        private ProdutoPAModel produto;
        public ProdutoPAModel Produto
        {
            get => produto;
            set { produto = value; RaisePropertyChanged(nameof(Produto)); }
        }

        private ObservableCollection<ProdutoPAModel> produtos;
        public ObservableCollection<ProdutoPAModel> Produtos
        {
            get => produtos;
            set { produtos = value; RaisePropertyChanged(nameof(Produtos)); }
        }

        public async Task<ObservableCollection<ModeloTabelaPAModel>> GetItensAsync()
        {
            using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
            var data = await conn.QueryAsync<ModeloTabelaPAModel>(
                @"SELECT *
                  FROM modelos.tbl_pa;");
            return new ObservableCollection<ModeloTabelaPAModel>(data);
        }

        public async Task<ObservableCollection<ProdutoPAModel>> GetProdutosAsync()
        {
            using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
            var results = await conn.QueryAsync<ProdutoPAModel>(
                @"SELECT codcompladicional,
                         CONCAT(descricao_adicional, ' ', complementoadicional) AS descricao
                  FROM producao.qry3descricoes
                  WHERE planilha = 'KIT ENF PA'
                    AND descricao = 'PA'
                    AND inativo <> '-1   '
                  ORDER BY descricao;");

            return new ObservableCollection<ProdutoPAModel>(results);
        }

        public async Task SaveAsync(ModeloTabelaPAModel model)
        {
            using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
            await conn.ExecuteAsync(
                @"INSERT INTO modelos.tbl_pa
                    (codcompladicional, ponga, tripe, anel_1, anel_2, anel_3, anel_4, anel_5,
                     anel_6, anel_7, anel_8, anel_9, anel_10, anel_11, anel_12, anel_13,
                     anel_14, anel_15, anel_16, anel_17, anel_18, anel_19, anel_20, anel_21, anel_22)
                  VALUES
                    (@codcompladicional, @ponga, @tripe, @anel_1, @anel_2, @anel_3, @anel_4, @anel_5,
                     @anel_6, @anel_7, @anel_8, @anel_9, @anel_10, @anel_11, @anel_12, @anel_13,
                     @anel_14, @anel_15, @anel_16, @anel_17, @anel_18, @anel_19, @anel_20, @anel_21, @anel_22)
                  ON CONFLICT (codcompladicional) DO UPDATE SET
                    ponga = EXCLUDED.ponga,
                    tripe = EXCLUDED.tripe,
                    anel_1 = EXCLUDED.anel_1,
                    anel_2 = EXCLUDED.anel_2,
                    anel_3 = EXCLUDED.anel_3,
                    anel_4 = EXCLUDED.anel_4,
                    anel_5 = EXCLUDED.anel_5,
                    anel_6 = EXCLUDED.anel_6,
                    anel_7 = EXCLUDED.anel_7,
                    anel_8 = EXCLUDED.anel_8,
                    anel_9 = EXCLUDED.anel_9,
                    anel_10 = EXCLUDED.anel_10,
                    anel_11 = EXCLUDED.anel_11,
                    anel_12 = EXCLUDED.anel_12,
                    anel_13 = EXCLUDED.anel_13,
                    anel_14 = EXCLUDED.anel_14,
                    anel_15 = EXCLUDED.anel_15,
                    anel_16 = EXCLUDED.anel_16,
                    anel_17 = EXCLUDED.anel_17,
                    anel_18 = EXCLUDED.anel_18,
                    anel_19 = EXCLUDED.anel_19,
                    anel_20 = EXCLUDED.anel_20,
                    anel_21 = EXCLUDED.anel_21,
                    anel_22 = EXCLUDED.anel_22;",
                model);
        }
    }
}
