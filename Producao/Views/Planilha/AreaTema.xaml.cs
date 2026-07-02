using Dapper;
using Npgsql;
using Producao.DataBase.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.Planilha
{
    /// <summary>
    /// Interação lógica para AreaTema.xam
    /// </summary>
    public partial class AreaTema : UserControl
    {
        public AreaTema()
        {
            InitializeComponent();
            this.DataContext = new AreaTemaViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                AreaTemaViewModel vm = (AreaTemaViewModel)DataContext;
                vm.Siglas = await vm.GetSiglasAsync();
                vm.AreaTemas = await vm.GeAreaTemasAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnRowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            var registro = e.EditedItem as TblAreaTemaModel;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                AreaTemaViewModel vm = (AreaTemaViewModel)DataContext;
                if (registro is null)
                {
                    return;
                }

                await vm.SaveAsync(registro);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");

                if (registro is not null && registro.codareatema == 0)
                    ((AreaTemaViewModel)DataContext).AreaTemas.Remove(registro);
                else
                    gridAreaTema.Rebind();
            }
        }

        private void OnRowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
        }

        private void gridAreaTema_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            if (e.Cell?.DataContext is not TblAreaTemaModel registro)
            {
                return;
            }

            if (e.Cell.Column.UniqueName == "sigla")
            {
                var funcionario = ((AreaTemaViewModel)DataContext).Siglas.FirstOrDefault(f => f.sigla_serv == registro.sigla);
                if (funcionario != null)
                {
                    registro.tema = funcionario.tema;
                }
            }

            if (e.Cell.Column.UniqueName == "construcao_total")
            {
                registro.cenografia_planta = registro.area_total_planta - registro.trilha_planta - registro.pa - registro.construcao_total;
            }

            gridAreaTema.Rebind();
        }
    }

    public class AreaTemaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<AprovadoModel>? _siglas;
        public ObservableCollection<AprovadoModel> Siglas
        {
            get { return _siglas; }
            set { _siglas = value; RaisePropertyChanged("Siglas"); }
        }

        private ObservableCollection<string>? _locais = ["Praça principal", "Trono", "Presépio", "Mini-Cenário", "Oficina", "Fachada"];
        public ObservableCollection<string> Locais
        {
            get { return _locais; }
            set { _locais = value; RaisePropertyChanged("Locais"); }
        }

        private ObservableCollection<TblAreaTemaModel>? _areaTemas;
        public ObservableCollection<TblAreaTemaModel> AreaTemas
        {
            get { return _areaTemas; }
            set { _areaTemas = value; RaisePropertyChanged("AreaTemas"); }
        }

        private TblAreaTemaModel? _areaTema;
        public TblAreaTemaModel AreaTema
        {
            get { return _areaTema; }
            set { _areaTema = value; RaisePropertyChanged("AreaTema"); }
        }

        public async Task<ObservableCollection<AprovadoModel>> GetSiglasAsync()
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                var data = await conn.QueryAsync<AprovadoModel>(
                    @"SELECT *
                      FROM producao.qry_aprovados
                      ORDER BY sigla_serv;");
                return new ObservableCollection<AprovadoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<string>> GeTemasAsync(string sigla)
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                var data = await conn.QueryAsync<string>(
                    @"SELECT tema
                      FROM producao.qry_aprovados
                      WHERE sigla_serv = @sigla
                      GROUP BY tema
                      ORDER BY tema;",
                    new { sigla });
                return new ObservableCollection<string>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<TblAreaTemaModel>> GeAreaTemasAsync()
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                var data = await conn.QueryAsync<TblAreaTemaModel>(
                    @"SELECT *
                      FROM producao.tbl_area_temas;");
                return new ObservableCollection<TblAreaTemaModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TblAreaTemaModel> SaveAsync(TblAreaTemaModel areaTema)
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                if (areaTema.codareatema == 0)
                {
                    areaTema.codareatema = await conn.ExecuteScalarAsync<long>(
                        @"INSERT INTO producao.tbl_area_temas
                            (sigla, tema, ano, local, trem, area_total_memorial, area_total_planta,
                             trilha_memorial, trilha_planta, pa, planta_liquida, perimetro_planta,
                             construcao_total, cenografia_planta, incluido_por, data_inclusao,
                             alterad_por, data_altera)
                          VALUES
                            (@sigla, @tema, @ano, @local, @trem, @area_total_memorial, @area_total_planta,
                             @trilha_memorial, @trilha_planta, @pa, @planta_liquida, @perimetro_planta,
                             @construcao_total, @cenografia_planta, @incluido_por, @data_inclusao,
                             @alterad_por, @data_altera)
                          RETURNING codareatema;",
                        areaTema);
                }
                else
                {
                    await conn.ExecuteAsync(
                        @"UPDATE producao.tbl_area_temas
                          SET sigla = @sigla,
                              tema = @tema,
                              ano = @ano,
                              local = @local,
                              trem = @trem,
                              area_total_memorial = @area_total_memorial,
                              area_total_planta = @area_total_planta,
                              trilha_memorial = @trilha_memorial,
                              trilha_planta = @trilha_planta,
                              pa = @pa,
                              planta_liquida = @planta_liquida,
                              perimetro_planta = @perimetro_planta,
                              construcao_total = @construcao_total,
                              cenografia_planta = @cenografia_planta,
                              incluido_por = @incluido_por,
                              data_inclusao = @data_inclusao,
                              alterad_por = @alterad_por,
                              data_altera = @data_altera
                          WHERE codareatema = @codareatema;",
                        areaTema);
                }

                return areaTema;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
