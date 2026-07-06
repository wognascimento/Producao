using Dapper;
using Microsoft.Win32;
using Npgsql;
using Producao.DataBase.Model;
using Producao.Utils;
using Producao.Views.CentralModelos.Compat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Telerik.Windows.Controls;

namespace Producao.Views.CheckList
{
    /// <summary>
    /// Interação lógica para ViewMemorial.xam
    /// </summary>
    public partial class ViewMemorial : UserControl
    {
        PropostaFechaSiglaModel sigla;
        PropostaFechaTemaModel tema;
        private bool _dadosCarregados;

        static ViewMemorial()
        {
            SqlMapper.AddTypeHandler(new DateOnlyToDateTimeHandler());
        }


        public ViewMemorial()
        {
            InitializeComponent();
            DataContext = new ViewMemorialViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_dadosCarregados)
                return;

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ViewMemorialViewModel vm = (ViewMemorialViewModel)DataContext;
                vm.Siglas = await vm.GetSiglasAsync();
                _dadosCarregados = true;
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
                ViewMemorialViewModel vm = (ViewMemorialViewModel)DataContext;
                if (e.AddedItems.Count > 0 && e.AddedItems[0] is PropostaFechaSiglaModel selectedSigla)
                {
                    sigla = selectedSigla;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    vm.Temas = await vm.GetTemasAsync(sigla);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                else
                {
                    vm.Temas = null;
                    vm.Itens = null;
                    vm.Links = null;
                    txtTema.Text = string.Empty;
                }
                    
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }

        private async void OnTemaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ViewMemorialViewModel vm = (ViewMemorialViewModel)DataContext;
                if (e.AddedItems.Count > 0 && e.AddedItems[0] is PropostaFechaTemaModel selectedTema)
                {
                    tema = selectedTema;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    vm.Itens = await vm.GetFechaAsync(tema);
                    vm.Links = await vm.GetFechaLinksAsync(sigla.sigla, tema.tema);

                    if (vm.Links.Count == 0)
                    {
                        vm.Link = new FechaLinkModel { links = "CLIQUE PARA ADICIONAR LINK" };
                    }
                    else
                    {
                        vm.Link = vm.Links.FirstOrDefault();
                    }

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                else
                {
                    vm.Itens = null;
                    vm.Links = null;
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }
        //private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) => System.Diagnostics.Process.Start(e.Uri.ToString());
        private async void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                ViewMemorialViewModel vm = (ViewMemorialViewModel)DataContext;
                if (e.Uri.OriginalString == "CLIQUE PARA ADICIONAR LINK")
                {
                    var folderDialog = new OpenFolderDialog
                    {
                        Title = "Selecione a pasta",
                        InitialDirectory = @"\\192.168.0.4\clientes" //Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
                    };

                    if (folderDialog.ShowDialog() == true)
                    {
                        var folderName = folderDialog.FolderName;

                       vm.Link =  await vm.SaveLinksAsync( new FechaLinkModel { idtema = vm.Tema.idtema, tema = vm.Tema.tema, sigla = vm.Sigla.sigla, data_link = DateTime.Now, links = folderName } );
                        
                    }
                    return;
                }

                Process.Start(new ProcessStartInfo()
                {
                    FileName = e.Uri.LocalPath,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }

        }

        private async void OnPrintMemorial(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                ViewMemorialViewModel vm = (ViewMemorialViewModel)DataContext;
                DataBaseSettings BaseSettings = DataBaseSettings.Instance;
                using ExcelEngine excelEngine = new();
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];
                Producao.Utils.PrintPageSetupHelper.ApplyA4Margins(worksheet);
                worksheet.IsGridLinesVisible = false;

                IStyle headerStyle;
                IStyle bodyStyle;

                bodyStyle = workbook.Styles.Add("BodyStyle");
                bodyStyle.BeginUpdate();
                bodyStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                bodyStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                bodyStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                bodyStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                bodyStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Grey_25_percent;
                bodyStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Grey_25_percent;
                bodyStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Grey_25_percent;
                bodyStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Grey_25_percent;
                bodyStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                bodyStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                bodyStyle.Font.Bold = true;
                bodyStyle.WrapText = true;
                bodyStyle.EndUpdate();

                headerStyle = workbook.Styles.Add("headerStyle");
                headerStyle.BeginUpdate();
                headerStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Grey_25_percent;
                headerStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Grey_25_percent;
                headerStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Grey_25_percent;
                headerStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Grey_25_percent;
                headerStyle.WrapText = true;
                headerStyle.EndUpdate();

                //vm.Itens

                worksheet.Range["A1"].Text = $"{vm.Sigla.sigla} MEMORIAL {BaseSettings.Database}";
                worksheet.Range["A1"].CellStyle.Font.Bold = true;
                worksheet.Range["A1"].CellStyle.Font.Size = 25;

                worksheet.Range["A2"].Text = $"{vm.Tema.tema}";
                worksheet.Range["A2"].CellStyle.Font.Bold = true;
                worksheet.Range["A2"].CellStyle.Font.Size = 20;

                worksheet.Range["A3"].Text = $"ITEM";
                worksheet.Range["A3"].ColumnWidth = 5;

                worksheet.Range["B3"].Text = $"LOCAL";
                worksheet.Range["B3"].ColumnWidth = 20;

                worksheet.Range["C3"].Text = $"DESCRIÇÃO";
                worksheet.Range["C3"].ColumnWidth = 25;
                worksheet.Range["C3"].WrapText = true;
                                  
                worksheet.Range["D3"].Text = $"QTDE";
                worksheet.Range["D3"].ColumnWidth = 5;
                worksheet.Range["D3"].WrapText = true;
                                  
                worksheet.Range["E3"].Text = $"DIMENSÃO";
                worksheet.Range["E3"].ColumnWidth = 25;
                worksheet.Range["E3"].WrapText = true;
                                  
                worksheet.Range["F3"].Text = $"Nº CAMINHÃO";
                worksheet.Range["F3"].ColumnWidth = 10;
                worksheet.Range["F3"].WrapText = true;
                                  
                worksheet.Range["G3"].Text = $"OBSERVAÇÃO";
                worksheet.Range["G3"].ColumnWidth = 20;
                worksheet.Range["G3"].WrapText = true;
                                  
                worksheet.Range["H3"].Text = $"OBS INTERNA";
                worksheet.Range["H3"].ColumnWidth = 15;
                worksheet.Range["H3"].WrapText = true;
                                  
                worksheet.Range["I3"].Text = $"OBS ALTERAÇÃO";
                worksheet.Range["I3"].ColumnWidth = 15;
                worksheet.Range["I3"].WrapText = true;

                worksheet.Rows[2].CellStyle = bodyStyle;

                var dados = vm.Itens.Select(m => new {m.item, m.localitem, m.descricao, m.qtd, m.dimensao, m.baia_caminhao, m.obs, m.obs_interna, m.obs_alteracao}).ToList(); // await vm.GetChkGeralRelatorioAsync(vm.Sigla.id_aprovado);
                worksheet.ImportData(dados, 4, 1, false);

                worksheet.Range[$"A4:I{dados.Count + 3}"].CellStyle = headerStyle;

                worksheet.Range[$"A4:A{dados.Count + 3}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"A4:A{dados.Count + 3}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"B4:B{dados.Count + 3}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"C4:C{dados.Count + 3}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"D4:D{dados.Count + 3}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"D4:D{dados.Count + 3}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;


                //worksheet.Range[$"E4:E{dados.Count + 3}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"E4:E{dados.Count + 3}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"F4:G{dados.Count + 3}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"F4:G{dados.Count + 3}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"G4:G{dados.Count + 3}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                //worksheet.Range[$"H4:H{dados.Count + 3}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"H4:H{dados.Count + 3}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"I4:I{dados.Count + 3}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.PageSetup.PrintTitleColumns = "$A:$H";
                worksheet.PageSetup.PrintTitleRows = "$1:$2";
                worksheet.PageSetup.Orientation = ExcelPageOrientation.Landscape;
                worksheet.PageSetup.LeftMargin = Producao.Utils.PrintPageSetupHelper.LeftMargin;
                worksheet.PageSetup.RightMargin = Producao.Utils.PrintPageSetupHelper.RightMargin;
                worksheet.PageSetup.TopMargin = Producao.Utils.PrintPageSetupHelper.TopMargin;
                worksheet.PageSetup.BottomMargin = Producao.Utils.PrintPageSetupHelper.BottomMargin;
                worksheet.PageSetup.RightFooter = "&P";
                worksheet.PageSetup.LeftFooter = "&D";
                //worksheet.PageSetup.CenterVertically = true;
                worksheet.PageSetup.CenterHorizontally = true;

                workbook.SaveAs(BaseSettings.ResolveImpressosPath($"MEMORIAL.xlsx"));
                Process.Start(new ProcessStartInfo(BaseSettings.ResolveImpressosPath($"MEMORIAL.xlsx"))
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });


            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void Itens_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            try
            {
                ViewMemorialViewModel vm = (ViewMemorialViewModel)DataContext;
                var data = e.Cell?.DataContext as ViewFechaModel;
                if (data is null)
                {
                    return;
                }

                if (e.Cell?.Column?.UniqueName == "baia_caminhao")
                {
                    await vm.SaveBaiaCaminhaoAsync(new ControleBaiaEnderecamentoModel { sigla_serv = data.sigla_serv, id_aprovado = data.id_aprovado, item_memorial = data.item, baia_caminhao = data.baia_caminhao, inserido_por = Environment.UserName, inserido_em = DateTime.Now.Date });
                }
            }
            catch (PostgresException ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }
    }

    public class ViewMemorialViewModel : INotifyPropertyChanged
    {
        static ViewMemorialViewModel() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        private static NpgsqlConnection CreateConnection() => new(DataBaseSettings.Instance.ConnectionString);

        private static async Task<List<T>> QueryAsync<T>(string sql, object? param = null)
        {
            await using var conn = CreateConnection();
            var data = await conn.QueryAsync<T>(sql, param);
            return data.ToList();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        /*
        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        */

        private ObservableCollection<PropostaFechaSiglaModel> _siglas;
        public ObservableCollection<PropostaFechaSiglaModel> Siglas { get { return _siglas; } set { _siglas = value; RaisePropertyChanged("Siglas"); } }

        private PropostaFechaSiglaModel _sigla;
        public PropostaFechaSiglaModel Sigla { get { return _sigla; } set { _sigla = value; RaisePropertyChanged("Sigla"); } }

        private ObservableCollection<PropostaFechaTemaModel> _temas;
        public ObservableCollection<PropostaFechaTemaModel> Temas { get { return _temas; } set { _temas = value; RaisePropertyChanged("Temas"); } }

        private PropostaFechaTemaModel _tema;
        public PropostaFechaTemaModel Tema { get { return _tema; } set { _tema = value; RaisePropertyChanged("Tema"); } }

        private ObservableCollection<ViewFechaModel> _itens;
        public ObservableCollection<ViewFechaModel> Itens { get { return _itens; } set { _itens = value; RaisePropertyChanged("Itens"); } }

        private ViewFechaModel _item;
        public ViewFechaModel Item { get { return _item; } set { _item = value; RaisePropertyChanged("Item"); } }


        private ObservableCollection<FechaLinkModel> _links;
        public ObservableCollection<FechaLinkModel> Links { get { return _links; } set { _links = value; RaisePropertyChanged("Links"); } }

        private FechaLinkModel _link;
        public FechaLinkModel Link { get { return _link; } set { _link = value; RaisePropertyChanged("Link"); } }

        private ControleBaiaEnderecamentoModel _baiaCaminhao;
        public ControleBaiaEnderecamentoModel BaiaCaminhao { get { return _baiaCaminhao; } set { _baiaCaminhao = value; RaisePropertyChanged("BaiaCaminhao"); } }


        public async Task<ObservableCollection<PropostaFechaSiglaModel>> GetSiglasAsync()
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM comercial.proposta_fecha_siglas
                    ORDER BY sigla;
                    """;

                var data = await QueryAsync<PropostaFechaSiglaModel>(sql);
                return new ObservableCollection<PropostaFechaSiglaModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<PropostaFechaTemaModel>> GetTemasAsync(PropostaFechaSiglaModel sigla)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM comercial.proposta_fecha_tema
                    WHERE cod_brief = @codbriefing
                    ORDER BY tema;
                    """;

                var data = await QueryAsync<PropostaFechaTemaModel>(sql, new { sigla.codbriefing });
                return new ObservableCollection<PropostaFechaTemaModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<ViewFechaModel>> GetFechaAsync(PropostaFechaTemaModel tema)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM comercial.proposta_view_fecha
                    WHERE cod_brief = @cod_brief
                      AND idtema = @idtema
                    ORDER BY item;
                    """;

                var data = await QueryAsync<ViewFechaModel>(sql, new { tema.cod_brief, tema.idtema });
                return new ObservableCollection<ViewFechaModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<FechaLinkModel>> GetFechaLinksAsync(string sigla, string tema)
        {
            try
            {
                const string sql = """
                    SELECT *
                    FROM comercial.tbl_fecha_links
                    WHERE sigla = @sigla
                      AND tema = @tema;
                    """;

                var data = await QueryAsync<FechaLinkModel>(sql, new { sigla, tema });
                return new ObservableCollection<FechaLinkModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FechaLinkModel> SaveLinksAsync(FechaLinkModel fechaLink)
        {
            try
            {
                await using var conn = CreateConnection();
                const string sql = """
                    INSERT INTO comercial.tbl_fecha_links
                        (sigla, tema, links, data_link, idtema)
                    VALUES
                        (@sigla, @tema, @links, @data_link, @idtema)
                    RETURNING codlinkfecha;
                    """;

                fechaLink.codlinkfecha = await conn.ExecuteScalarAsync<long>(sql, fechaLink);

                return fechaLink;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ControleBaiaEnderecamentoModel> SaveBaiaCaminhaoAsync(ControleBaiaEnderecamentoModel controleBaia)
        {
            try
            {
                await using var conn = CreateConnection();

                const string updateSql = """
                    UPDATE expedicao.tbl_controle_baia_enderecamento
                    SET baia_caminhao = @baia_caminhao,
                        alterado_por = @alterado_por,
                        alterado_em = @alterado_em
                    WHERE id_aprovado = @id_aprovado
                      AND item_memorial = @item_memorial;
                    """;

                controleBaia.alterado_por ??= Environment.UserName;
                controleBaia.alterado_em ??= DateTime.Now;

                var affected = await conn.ExecuteAsync(updateSql, controleBaia);
                if (affected == 0)
                {
                    const string insertSql = """
                        INSERT INTO expedicao.tbl_controle_baia_enderecamento
                            (sigla_serv, baia_caminhao, endereco, item_memorial, id_aprovado, inserido_por, inserido_em,
                             alterado_por, alterado_em)
                        VALUES
                            (@sigla_serv, @baia_caminhao, @endereco, @item_memorial, @id_aprovado, @inserido_por, @inserido_em,
                             @alterado_por, @alterado_em)
                        RETURNING id_controle;
                        """;

                    controleBaia.inserido_por ??= Environment.UserName;
                    controleBaia.inserido_em ??= DateTime.Now;
                    controleBaia.id_controle = await conn.ExecuteScalarAsync<long>(insertSql, controleBaia);
                }

                return controleBaia;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}

