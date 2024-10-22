using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Npgsql;
using Producao.DataBase.Model;
using Syncfusion.XlsIO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Telerik.Windows.Persistence.Core;

namespace Producao.Views.CheckList
{
    /// <summary>
    /// Interação lógica para ViewMemorial.xam
    /// </summary>
    public partial class ViewMemorial : UserControl
    {
        PropostaFechaSiglaModel sigla;
        PropostaFechaTemaModel tema;
        public ViewMemorial()
        {
            InitializeComponent();
            DataContext = new ViewMemorialViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ViewMemorialViewModel vm = (ViewMemorialViewModel)DataContext;
                vm.Siglas = await Task.Run(vm.GetSiglasAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnSiglaSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ViewMemorialViewModel vm = (ViewMemorialViewModel)DataContext;
                if (e.NewValue != null)
                {
                    sigla = (PropostaFechaSiglaModel)e.NewValue;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    vm.Temas = await Task.Run(async () => await vm.GetTemasAsync(sigla));
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
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnTemaSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ViewMemorialViewModel vm = (ViewMemorialViewModel)DataContext;
                if (e.NewValue != null) 
                {
                    tema = (PropostaFechaTemaModel)e.NewValue;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    vm.Itens = await Task.Run(async () => await vm.GetFechaAsync(tema));
                    vm.Links = await Task.Run(async () => await vm.GetFechaLinksAsync(sigla.sigla, tema.tema));

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
                MessageBox.Show(ex.Message);
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
                        InitialDirectory = @"\\servidor\clientes" //Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
                    };

                    if (folderDialog.ShowDialog() == true)
                    {
                        var folderName = folderDialog.FolderName;

                       vm.Link =  await Task.Run(async () => await vm.SaveLinksAsync( new FechaLinkModel { idtema = vm.Tema.idtema, tema = vm.Tema.tema, sigla = vm.Sigla.sigla, data_link = DateTime.Now, links = folderName } ));
                        
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
                MessageBox.Show(ex.Message);
            }

        }

        private void SfDataGrid_CurrentCellRequestNavigate(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellRequestNavigateEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                Process explorer = new Process();
                explorer.StartInfo.FileName = "explorer.exe";
                explorer.StartInfo.Arguments = e.NavigateText.Replace("#", null);
                explorer.Start();

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
            
        }

        private async void OnPrintMemorial(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                ViewMemorialViewModel vm = (ViewMemorialViewModel)DataContext;
                DataBaseSettings BaseSettings = DataBaseSettings.Instance;
                using ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];
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

                worksheet.Range["A2"].Text = $"ITEM";
                worksheet.Range["A2"].ColumnWidth = 5;

                worksheet.Range["B2"].Text = $"LOCAL";
                worksheet.Range["B2"].ColumnWidth = 20;

                worksheet.Range["C2"].Text = $"DESCRIÇÃO";
                worksheet.Range["C2"].ColumnWidth = 25;
                worksheet.Range["C2"].WrapText = true;

                worksheet.Range["D2"].Text = $"QTDE";
                worksheet.Range["D2"].ColumnWidth = 5;
                worksheet.Range["D2"].WrapText = true;

                worksheet.Range["E2"].Text = $"DIMENSÃO";
                worksheet.Range["E2"].ColumnWidth = 25;
                worksheet.Range["E2"].WrapText = true;

                worksheet.Range["F2"].Text = $"Nº CAMINÃO";
                worksheet.Range["F2"].ColumnWidth = 10;
                worksheet.Range["F2"].WrapText = true;

                worksheet.Range["G2"].Text = $"OBSERVAÇÃO";
                worksheet.Range["G2"].ColumnWidth = 20;
                worksheet.Range["G2"].WrapText = true;

                worksheet.Range["H2"].Text = $"OBS INTERNA";
                worksheet.Range["H2"].ColumnWidth = 15;
                worksheet.Range["H2"].WrapText = true;

                worksheet.Range["I2"].Text = $"OBS ALTERAÇÃO";
                worksheet.Range["I2"].ColumnWidth = 15;
                worksheet.Range["I2"].WrapText = true;

                worksheet.Rows[1].CellStyle = bodyStyle;

                var dados = vm.Itens.Select(m => new {m.item, m.localitem, m.descricao, m.qtd, m.dimensao, m.baia_caminhao, m.obs, m.obs_interna, m.obs_alteracao}).ToList(); // await Task.Run(() => vm.GetChkGeralRelatorioAsync(vm.Sigla.id_aprovado));
                worksheet.ImportData(dados, 3, 1, false);

                worksheet.Range[$"A3:I{dados.Count + 2}"].CellStyle = headerStyle;

                worksheet.Range[$"A3:A{dados.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"A3:A{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"B3:B{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"C3:C{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"D3:D{dados.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"D3:D{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;


                //worksheet.Range[$"E3:E{dados.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"E3:E{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"F3:G{dados.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"F3:G{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"G3:G{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                //worksheet.Range[$"H3:H{dados.Count + 2}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[$"H3:H{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.Range[$"I3:I{dados.Count + 2}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                worksheet.PageSetup.PrintTitleColumns = "$A:$H";
                worksheet.PageSetup.PrintTitleRows = "$1:$2";
                worksheet.PageSetup.Orientation = ExcelPageOrientation.Landscape;
                worksheet.PageSetup.LeftMargin = 0;
                worksheet.PageSetup.RightMargin = 0;
                worksheet.PageSetup.TopMargin = 0;
                worksheet.PageSetup.BottomMargin = 0.5;
                worksheet.PageSetup.RightFooter = "&P";
                worksheet.PageSetup.LeftFooter = "&D";
                //worksheet.PageSetup.CenterVertically = true;
                worksheet.PageSetup.CenterHorizontally = true;

                workbook.SaveAs("Impressos/MEMORIAL.xlsx");
                Process.Start(new ProcessStartInfo("Impressos\\MEMORIAL.xlsx")
                {
                    UseShellExecute = true
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void SfDataGrid_CurrentCellValidated(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellValidatedEventArgs e)
        {
            try
            {
                ViewMemorialViewModel vm = (ViewMemorialViewModel)DataContext;
                var column = e.Column; //Column = {Syncfusion.UI.Xaml.Grid.GridTextColumn}
                var data = e.RowData as ViewFechaModel;
                if (e.Column.MappingName == "baia_caminhao")
                {
                    if (e.NewValue != e.OldValue)
                    {
                        await vm.SaveBaiaCaminhaoAsync(new ControleBaiaEnderecamentoModel { sigla_serv = data.sigla_serv, id_aprovado = data.id_aprovado, item_memorial = data.item, baia_caminhao = data.baia_caminhao, inserido_por = Environment.UserName, inserido_em = DateTime.Now.Date });
                    }
                }
            }
            catch (PostgresException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    public class ViewMemorialViewModel : INotifyPropertyChanged
    {
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
                using DatabaseContext db = new();
                var data = await db.PropostaFechaSiglas.OrderBy(c => c.sigla).ToListAsync();
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
                using DatabaseContext db = new();
                var data = await db.PropostaFechaTemas.OrderBy(c => c.tema).Where(c => c.cod_brief == sigla.codbriefing).ToListAsync();
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
                using DatabaseContext db = new();
                var data = await db.ViewFechas.OrderBy(c => c.item).Where(c => c.cod_brief == tema.cod_brief && c.idtema == tema.idtema).ToListAsync();
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
                using DatabaseContext db = new();
                var data = await db.FechaLinks.Where(c => c.sigla == sigla && c.tema == tema).ToListAsync();
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
                using DatabaseContext db = new();
                await db.FechaLinks.AddAsync(fechaLink);
                await db.SaveChangesAsync();

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
                using DatabaseContext db = new();
                using (var context = db)
                {
                    var baia = await context.ControleBaias.Where(x => x.id_aprovado == controleBaia.id_aprovado && x.item_memorial == controleBaia.item_memorial).FirstOrDefaultAsync();
                    if (baia != null)
                    {
                        baia.baia_caminhao = controleBaia.baia_caminhao;
                        context.Entry(baia).Property(f => f.baia_caminhao).IsModified = true;
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        context.Add(controleBaia).Property(f => f.sigla_serv).IsModified = true;
                        context.Add(controleBaia).Property(f => f.id_aprovado).IsModified = true;
                        context.Add(controleBaia).Property(f => f.item_memorial).IsModified = true;
                        context.Add(controleBaia).Property(f => f.baia_caminhao).IsModified = true;
                        context.Add(controleBaia).Property(f => f.inserido_por).IsModified = true;
                        context.Add(controleBaia).Property(f => f.inserido_em).IsModified = true;
                        await context.SaveChangesAsync();
                    }
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
