using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views.CheckList
{
    /// <summary>
    /// Interação lógica para ViewEmitirEtiquetaCheckList.xam
    /// </summary>
    public partial class ViewEmitirEtiquetaCheckList : UserControl
    {
        public ViewEmitirEtiquetaCheckList()
        {
            this.DataContext = new EmitirEtiquetaViewModel();
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                EmitirEtiquetaViewModel vm = (EmitirEtiquetaViewModel)DataContext;
                vm.Siglas = await vm.GetSiglasAsync();
                vm.Itens = await vm.GetItensAsync("");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnSiglaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                EmitirEtiquetaViewModel vm = (EmitirEtiquetaViewModel)DataContext;
                //vm.Itens = await vm.GetItensAsync(vm.Sigla.sigla_serv);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

    }

    public class EmitirEtiquetaViewModel : INotifyPropertyChanged
    {
        static EmitirEtiquetaViewModel() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        private static NpgsqlConnection CreateConnection() => new(DataBaseSettings.Instance.ConnectionString);

        private static async Task<List<T>> QueryAsync<T>(string sql, object? param = null)
        {
            await using var conn = CreateConnection();
            var data = await conn.QueryAsync<T>(sql, param);
            return data.ToList();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        
        public DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        private ObservableCollection<SiglaChkListModel> _siglas;
        public ObservableCollection<SiglaChkListModel> Siglas
        {
            get { return _siglas; }
            set { _siglas = value; RaisePropertyChanged("Siglas"); }
        }
        private SiglaChkListModel _sigla;
        public SiglaChkListModel Sigla
        {
            get { return _sigla; }
            set { _sigla = value; RaisePropertyChanged("Sigla"); }
        }

        private ObservableCollection<EtiquetaCheckListModel> _itens;
        public ObservableCollection<EtiquetaCheckListModel> Itens
        {
            get { return _itens; }
            set { _itens = value; RaisePropertyChanged("Itens"); }
        }
        private EtiquetaCheckListModel _item;
        public EtiquetaCheckListModel Item
        {
            get { return _item; }
            set { _item = value; RaisePropertyChanged("Item"); }
        }
        /*
        private ObservableCollection<EtiquetaProducaoModel> _etiquetas;
        public ObservableCollection<EtiquetaProducaoModel> Etiquetas
        {
            get { return _etiquetas; }
            set { _etiquetas = value; RaisePropertyChanged("Etiquetas"); }
        }
        private EtiquetaProducaoModel _etiqueta;
        public EtiquetaProducaoModel Etiqueta
        {
            get { return _etiqueta; }
            set { _etiqueta = value; RaisePropertyChanged("Etiqueta"); }
        }
        */

        public async Task<ObservableCollection<SiglaChkListModel>> GetSiglasAsync()
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM producao.view_sigla_chkgeral
                    ORDER BY sigla_serv;
                    """;

                var data = await QueryAsync<SiglaChkListModel>(sql);
                return new ObservableCollection<SiglaChkListModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<EtiquetaCheckListModel>> GetItensAsync(string? sigla_serv)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM producao.qryetiquetachkgeral
                    ORDER BY item_memorial;
                    """;

                var data = await QueryAsync<EtiquetaCheckListModel>(sql);
                return new ObservableCollection<EtiquetaCheckListModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<EtiquetaProducaoModel>> GetEtiquetasAsync(long? coddetalhescompl)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM producao.tbl_etiqueta_producao
                    WHERE coddetalhescompl = @coddetalhescompl
                    ORDER BY codvol;
                    """;

                var data = await QueryAsync<EtiquetaProducaoModel>(sql, new { coddetalhescompl });
                return new ObservableCollection<EtiquetaProducaoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<EtiquetaProducaoModel> AddEtiquetaAsync(EtiquetaProducaoModel etiqueta)
        {
            try
            {
                await using var conn = CreateConnection();

                if (etiqueta.codvol is null or 0)
                {
                    const string insertSql = """
                        INSERT INTO producao.tbl_etiqueta_producao
                            (coddetalhescompl, volumes, volumes_total, qtd, largura, altura, profundidade,
                             peso_bruto, peso_liquido, impresso, impresso_por, impresso_em, criado_por, criado_em)
                        VALUES
                            (@coddetalhescompl, @volumes, @volumes_total, @qtd, @largura, @altura, @profundidade,
                             @peso_bruto, @peso_liquido, @impresso, @impresso_por, @impresso_em, @criado_por, @criado_em)
                        RETURNING codvol;
                        """;

                    etiqueta.codvol = await conn.ExecuteScalarAsync<long>(insertSql, etiqueta);
                }
                else
                {
                    const string updateSql = """
                        UPDATE producao.tbl_etiqueta_producao
                        SET coddetalhescompl = @coddetalhescompl,
                            volumes = @volumes,
                            volumes_total = @volumes_total,
                            qtd = @qtd,
                            largura = @largura,
                            altura = @altura,
                            profundidade = @profundidade,
                            peso_bruto = @peso_bruto,
                            peso_liquido = @peso_liquido,
                            impresso = @impresso,
                            impresso_por = @impresso_por,
                            impresso_em = @impresso_em,
                            criado_por = @criado_por,
                            criado_em = @criado_em
                        WHERE codvol = @codvol;
                        """;

                    await conn.ExecuteAsync(updateSql, etiqueta);
                }

                return etiqueta;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /*
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        */

    }
}

