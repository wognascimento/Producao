using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Utility;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace Producao.Views.Controlado
{
    /// <summary>
    /// Interação lógica para ImprimirEtiqueta.xam
    /// </summary>
    public partial class ImprimirEtiqueta : UserControl
    {
        public ImprimirEtiqueta()
        {
            InitializeComponent();
            DataContext = new ImprimirEtiquetaViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ImprimirEtiquetaViewModel vm = (ImprimirEtiquetaViewModel)DataContext;
                vm.Produtos = await Task.Run(vm.GetProdutosAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }
    }

    public class ImprimirEtiquetaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<ControladoEtiquetaModel> _produtos;
        public ObservableCollection<ControladoEtiquetaModel> Produtos
        { 
            get { return _produtos; } 
            set { _produtos = value; RaisePropertyChanged("Produtos"); }
        }

        private ControladoEtiquetaModel _produto;
        public ControladoEtiquetaModel Produto
        {
            get { return _produto; }
            set { _produto = value; RaisePropertyChanged("Produto"); }
        }

        private ObservableCollection<ControladoEtiquetaLivreModel> _livres;
        public ObservableCollection<ControladoEtiquetaLivreModel> Livres
        {
            get { return _livres; }
            set { _livres = value; RaisePropertyChanged("Livres"); }
        }

        private ObservableCollection<QryImpressaoModel> _impressos;
        public ObservableCollection<QryImpressaoModel> Impressos
        {
            get { return _impressos; }
            set { _impressos = value; RaisePropertyChanged("Impressos"); }
        }

        public async Task<ObservableCollection<ControladoEtiquetaModel>> GetProdutosAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ControladoEtiquetas.ToListAsync();
                return [.. data];
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<ControladoEtiquetaLivreModel>> GetLivresAsync(int limit)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ControladoEtiquetaLivres.Take(limit).ToListAsync();
                return [.. data];
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddEtiquetaAsync(ControladoZebraModel controlado)
        {
            try
            {
                using DatabaseContext db = new();
                await db.ControladosZebra.AddAsync(controlado);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<QryImpressaoModel> GetImprimirAsync(long? codcompladicional)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Impressoes.FirstOrDefaultAsync(i => i.codcompladicional == codcompladicional && i.impresso == "0");
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

    public static class ContextMenuCommandsImprimirEtiqueta
    {
        static BaseCommand? imprimir;
        public static BaseCommand Imprimir
        {
            get { imprimir ??= new BaseCommand(OnImprimir); return imprimir; }
        }
        private static async void OnImprimir(object obj)
        {
            var record = ((GridRecordContextMenuInfo)obj).Record as ControladoEtiquetaModel;
            var grid = ((GridRecordContextMenuInfo)obj).DataGrid;
            var item = grid.SelectedItem as ControladoEtiquetaModel;
            ImprimirEtiquetaViewModel vm = (ImprimirEtiquetaViewModel)grid.DataContext;

            if (record.etiquetas == 0 || record.impressas == record.etiquetas) 
            {
                var alert = new RadDesktopAlert
                {
                    Header = "NOTIFICAÇÃO IMPRESSÃO ETIQUETA",
                    Content = "Não existe etiqueas para este produto.",
                    ShowDuration = 3000
                };
                RadDesktopAlertManager manager = new();
                StyleManager.SetTheme(alert, new Windows8Theme());
                manager.ShowAlert(alert);

                return;
            }

            string IPAdress = "192.168.0.113";
            int Port = 9100;
            StreamWriter? SWriter;
            TcpClient? Client = new();
            try
            {
                await Client.ConnectAsync(IPAdress, Port);
                SWriter = new(Client.GetStream());
                //SWriter = new(@"C:\TEMP\ETIQUETA.TXT");

                RadWindow.Prompt(new DialogParameters()
                {
                    Content = "Informa a quantidade de etiquetas:",
                    Header = "Imprimir Etiqueta(s)",
                    OkButtonContent = "IMPRIMIR",
                    DefaultPromptResultValue = "1",

                    Closed = async (sender, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.PromptResult) && int.TryParse(e.PromptResult, out int quantidade))
                        {
                            //int saldo = (int)((record.etiquetas ?? 0) - (record.impressas ?? 0));

                            if (quantidade > record.etiquetas)
                            {
                                var alert = new RadDesktopAlert
                                {
                                    Header = "NOTIFICAÇÃO IMPRESSÃO ETIQUETA",
                                    Content = "Está informando uma quantidade maior do que as etiquetas disponíveis.",
                                    ShowDuration = 3000
                                };
                                RadDesktopAlertManager manager = new();
                                StyleManager.SetTheme(alert, new Windows8Theme());
                                manager.ShowAlert(alert);
                                return;
                            }
                            for (int i = 0; i < quantidade; i++)
                            {
                                var etiqueta = await vm.GetImprimirAsync(record.codcompladicional);
        
                                SWriter.WriteLine($@"^XA");
                                SWriter.WriteLine($@"^CI28");
                                SWriter.WriteLine($@"^FT24,318^BQN,2,6");
                                SWriter.WriteLine($@"^FH\^FDHA,{etiqueta.barcode}^FS");
                                SWriter.WriteLine($@"^FT124,164^AAB,9,5^FH\^FDPRODUTO^FS");
                                SWriter.WriteLine($@"^FT139,164^A0B,11,19^FH\^FD{etiqueta.codcompladicional}^FS");
                                SWriter.WriteLine($@"^FT124,79^AAB,9,5^FH\^FDETIQUETA^FS");
                                SWriter.WriteLine($@"^FT139,79^A0B,11,19^FH\^FD{etiqueta.codigo}^FS");
                                SWriter.WriteLine($@"^FT105,163^A0B,14^FB151,6,0,C^FH\^FD{etiqueta.descricao_completa}^FS");
                                SWriter.WriteLine($@"^PQ1,0,1,Y^XZ");

                                using DatabaseContext db = new();
                                await db.Database.ExecuteSqlRawAsync("UPDATE producao.tbl_barcodes SET impresso = '-1' WHERE codigo = {0}", etiqueta.codigo);
                                record.impressas += 1;
                                grid.View.Refresh();
                            }
                            await SWriter.FlushAsync();
                            await SWriter.DisposeAsync();
                            SWriter.Close();
                        }
                        else
                        {
                            MessageBox.Show("Por favor, insira um número válido.");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        static BaseCommand? adicionar;
        public static BaseCommand Adicionar
        {
            get { adicionar ??= new BaseCommand(OnAdicionar); return adicionar; }
        }
        private static async void OnAdicionar(object obj)
        {
            var record = ((GridRecordContextMenuInfo)obj).Record as ControladoEtiquetaModel;
            var grid = ((GridRecordContextMenuInfo)obj).DataGrid;
            //var item = grid.SelectedItem as ControladoEtiquetaModel;
            ImprimirEtiquetaViewModel vm = (ImprimirEtiquetaViewModel)grid.DataContext;

            RadWindow.Prompt(new DialogParameters()
            {
                Content = "Informa a quantidade de etiquetas:",
                Header = "Adicionar Etiqueta(s)",
                OkButtonContent = "ADICIONAR",
                Closed = async (object sender, WindowClosedEventArgs e) =>
                {
                    if (e.PromptResult != null)
                    {
                        //bool ehValido = Regex.IsMatch(e.PromptResult, @"^\d");
                        if (e.PromptResult.All(char.IsDigit)) 
                        {
                            int limit = int.Parse(e.PromptResult);
                            int saldo = (int)((record.saldo_estoque ?? 0) - (record.etiquetas ?? 0));
                            if (saldo < 1 || limit < saldo)
                            {
                                RadWindow.Alert(new DialogParameters()
                                {
                                    Content = "Não é possivel adicionar quantidade maior que o saldo de estoque.",
                                    Header = "Adicionar Etiqueta(s)"
                                });
                                return;
                            }

                            try
                            {
                                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                                vm.Livres = await Task.Run(() => vm.GetLivresAsync(limit));
                                foreach (var item in vm.Livres)
                                {
                                    await Task.Run(() => vm.AddEtiquetaAsync(new ControladoZebraModel { codcompladicional = record.codcompladicional, codigo = item.codigo }));

                                }

                                var filteredResult = grid.View.Records.Select(recordentry => recordentry.Data).ToList();

                                //vm.Produtos = await Task.Run(vm.GetProdutosAsync);
                                int i = filteredResult.IndexOf(record);
                                record.etiquetas += limit;
                                //record.impressas += limit;
                                filteredResult[i] = record;

                                grid.View.Refresh();

                                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                            }
                        }
                        else
                        {
                            RadWindow.Alert("Informa número para adicionar etiqueta.");
                        }
                    }
                }
            });
        }

        static BaseCommand? remover;
        public static BaseCommand Remover
        {
            get { remover ??= new BaseCommand(OnRemover); return remover; }
        }
        private static async void OnRemover(object obj)
        {
            var record = ((GridRecordContextMenuInfo)obj).Record as ControladoEtiquetaModel;
            var grid = ((GridRecordContextMenuInfo)obj).DataGrid;
            var item = grid.SelectedItem as ControladoEtiquetaModel;
            ImprimirEtiquetaViewModel vm = (ImprimirEtiquetaViewModel)grid.DataContext;
        }

        static BaseCommand? impressas;
        public static BaseCommand Impressas
        {
            get { impressas ??= new BaseCommand(OnImpressas); return impressas; }
        }
        private static async void OnImpressas(object obj)
        {
            var record = ((GridRecordContextMenuInfo)obj).Record as ControladoEtiquetaModel;
            var grid = ((GridRecordContextMenuInfo)obj).DataGrid;
            var item = grid.SelectedItem as ControladoEtiquetaModel;
            ImprimirEtiquetaViewModel vm = (ImprimirEtiquetaViewModel)grid.DataContext;

            Impressas radWindow = new(item.codcompladicional)
            {
                Width = 400,
                Height = 300,
                ResizeMode = ResizeMode.NoResize,
                CanMove = false
            };
            StyleManager.SetTheme(radWindow, new Windows8Theme());
            radWindow.ShowDialog();
        }

        static BaseCommand? gerar;
        public static BaseCommand Gerar
        {
            get { gerar ??= new BaseCommand(OnGerar); return gerar; }
        }
        private static async void OnGerar(object obj)
        {
            var record = ((GridRecordContextMenuInfo)obj).Record as ControladoEtiquetaModel;
            var grid = ((GridRecordContextMenuInfo)obj).DataGrid;
            var item = grid.SelectedItem as ControladoEtiquetaModel;
            ImprimirEtiquetaViewModel vm = (ImprimirEtiquetaViewModel)grid.DataContext;
        }

        

    }
}
