using Dapper;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.CentralModelos
{
    public partial class ModeloFiada : Window
    {
        private QryModeloModel? modelo;

        public ModeloFiada(QryModeloModel? modelo)
        {
            InitializeComponent();
            this.modelo = modelo;
            DataContext = new ModeloFiadaViewModel();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModeloFiadaViewModel vm = (ModeloFiadaViewModel)DataContext;
            vm.Modelo = modelo;
            vm.Modelos = new ObservableCollection<string> { "MOD. 01", "MOD. 02", "MOD. 03", "MOD. 04", "MOD. 05", "MOD. 06", "MOD. 07", "MOD. 08", "MOD. 09", "MOD. 10" };

            try
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                vm.ModeloFiada = await vm.GetModelosFiadaAsync(modelo);
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private void OnAddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            if (modelo == null)
            {
                return;
            }

            ((ModeloFiadaModel)e.NewObject).id_modelo = modelo.id_modelo;
        }

        private void OnRowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.Row?.Item is not ModeloFiadaModel rowData)
            {
                return;
            }

            if (rowData.id_modelo == null)
            {
                e.IsValid = false;
                AddValidation(e, nameof(ModeloFiadaModel.modelofiada), "Modelo não selecionado. Feche e abra a janela novamente.");
            }
            else if (string.IsNullOrWhiteSpace(rowData.modelofiada))
            {
                e.IsValid = false;
                AddValidation(e, nameof(ModeloFiadaModel.modelofiada), "Selecione o modelo da fiada.");
            }
            else if (rowData.qtdmodelofiada == null)
            {
                e.IsValid = false;
                AddValidation(e, nameof(ModeloFiadaModel.qtdmodelofiada), "Informe a quantidade de enfeites do modelo.");
            }
        }

        private async void OnRowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            if (e.EditedItem is not ModeloFiadaModel data)
            {
                return;
            }

            try
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                ModeloFiadaViewModel vm = (ModeloFiadaViewModel)DataContext;
                await vm.SaveModelosFiadaAsync(data);
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
                Producao.ErrorDialog.Show(ex, "Erro");
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

        private async void QtdFiada_ValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            if (!IsLoaded || modelo?.id_modelo == null)
            {
                return;
            }

            try
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                var valor = e.NewValue.HasValue ? Convert.ToInt32(e.NewValue.Value) : (int?)null;
                ModeloFiadaViewModel vm = (ModeloFiadaViewModel)DataContext;
                await vm.AddModeloAsync(modelo.id_modelo, valor);
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }
    }

    public class ModeloFiadaViewModel : INotifyPropertyChanged
    {
        static ModeloFiadaViewModel() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        private static NpgsqlConnection CreateConnection() => new(DataBaseSettings.Instance.ConnectionString);

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<string>? modelos;
        public ObservableCollection<string> Modelos
        {
            get => modelos;
            set { modelos = value; RaisePropertyChanged(nameof(Modelos)); }
        }

        private ObservableCollection<ModeloFiadaModel>? modeloFiada;
        public ObservableCollection<ModeloFiadaModel> ModeloFiada
        {
            get => modeloFiada;
            set { modeloFiada = value; RaisePropertyChanged(nameof(ModeloFiada)); }
        }

        private QryModeloModel? modelo;
        public QryModeloModel Modelo
        {
            get => modelo;
            set { modelo = value; RaisePropertyChanged(nameof(Modelo)); }
        }

        public async Task<ObservableCollection<ModeloFiadaModel>> GetModelosFiadaAsync(QryModeloModel? modelo)
        {
            const string sql = """
                SELECT *
                FROM modelos.tbl_modelo_fiada
                WHERE id_modelo = @id_modelo
                ORDER BY modelofiada;
                """;

            await using var conn = CreateConnection();
            var data = await conn.QueryAsync<ModeloFiadaModel>(sql, new { modelo.id_modelo });
            return new ObservableCollection<ModeloFiadaModel>(data);
        }

        public async Task<ModeloFiadaModel> SaveModelosFiadaAsync(ModeloFiadaModel modelo)
        {
            await using var conn = CreateConnection();
            if (modelo.id is null or 0)
            {
                modelo.id = await conn.ExecuteScalarAsync<long>(
                    """
                    INSERT INTO modelos.tbl_modelo_fiada
                        (id_modelo, modelofiada, qtdmodelofiada)
                    VALUES
                        (@id_modelo, @modelofiada, @qtdmodelofiada)
                    RETURNING id;
                    """,
                    modelo);
            }
            else
            {
                await conn.ExecuteAsync(
                    """
                    UPDATE modelos.tbl_modelo_fiada
                    SET id_modelo = @id_modelo,
                        modelofiada = @modelofiada,
                        qtdmodelofiada = @qtdmodelofiada
                    WHERE id = @id;
                    """,
                    modelo);
            }

            return modelo;
        }

        public async Task<ModeloModel> AddModeloAsync(long? id_modelo, int? qtd_fiada_cascata)
        {
            await using var conn = CreateConnection();
            await conn.ExecuteAsync(
                """
                UPDATE modelos.tbl_modelos
                SET qtd_fiada_cascata = @qtd_fiada_cascata
                WHERE id_modelo = @id_modelo;
                """,
                new { id_modelo, qtd_fiada_cascata });

            var modelo = await conn.QueryFirstOrDefaultAsync<ModeloModel>(
                """
                SELECT *
                FROM modelos.tbl_modelos
                WHERE id_modelo = @id_modelo
                LIMIT 1;
                """,
                new { id_modelo });

            return modelo;
        }
    }
}

