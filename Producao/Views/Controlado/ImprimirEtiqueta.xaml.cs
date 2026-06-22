using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
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
using Telerik.Windows.Controls.GridView;

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
                vm.Produtos = await vm.GetProdutosAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnImprimirEtiquetaClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is not ImprimirEtiquetaViewModel vm || ProdutosGrid.SelectedItem is not ControladoEtiquetaModel record)
            {
                return;
            }

            if (record.etiquetas == 0 || record.impressas == record.etiquetas)
            {
                MostrarAlertaEtiqueta("Não existe etiqueas para este produto.");
                return;
            }

            var printer = ThermalPrinterConfiguration.Load();
            TcpClient? client = new();
            try
            {
                await client.ConnectAsync(printer.IpAddress, printer.Port);
                await using var writer = new StreamWriter(client.GetStream());

                RadWindow.Prompt(new DialogParameters
                {
                    Content = "Informa a quantidade de etiquetas:",
                    Header = "Imprimir Etiqueta(s)",
                    OkButtonContent = "IMPRIMIR",
                    DefaultPromptResultValue = "1",
                    Closed = async (_, args) =>
                    {
                        if (string.IsNullOrWhiteSpace(args.PromptResult) || !int.TryParse(args.PromptResult, out var quantidade))
                        {
                            MessageBox.Show("Por favor, insira um número válido.");
                            return;
                        }

                        if (quantidade > record.etiquetas)
                        {
                            MostrarAlertaEtiqueta("Está informando uma quantidade maior do que as etiquetas disponíveis.");
                            return;
                        }

                        for (var i = 0; i < quantidade; i++)
                        {
                            var etiqueta = await vm.GetImprimirAsync(record.codcompladicional);
                            EscreverEtiqueta(writer, etiqueta);

                            using DatabaseContext db = new();
                            await db.Database.ExecuteSqlRawAsync("UPDATE producao.tbl_barcodes SET impresso = '-1' WHERE codigo = {0}", etiqueta.codigo);
                            record.impressas += 1;
                        }

                        await writer.FlushAsync();
                        ProdutosGrid.Items.Refresh();
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnAdicionarEtiquetaClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is not ImprimirEtiquetaViewModel vm || ProdutosGrid.SelectedItem is not ControladoEtiquetaModel record)
            {
                return;
            }

            RadWindow.Prompt(new DialogParameters
            {
                Content = "Informa a quantidade de etiquetas:",
                Header = "Adicionar Etiqueta(s)",
                OkButtonContent = "ADICIONAR",
                Closed = async (_, args) =>
                {
                    if (string.IsNullOrWhiteSpace(args.PromptResult) || !args.PromptResult.All(char.IsDigit))
                    {
                        RadWindow.Alert("Informa número para adicionar etiqueta.");
                        return;
                    }

                    var limit = int.Parse(args.PromptResult);
                    var saldo = (int)((record.saldo_estoque ?? 0) - (record.etiquetas ?? 0));
                    if (saldo < 1 || limit > saldo)
                    {
                        RadWindow.Alert(new DialogParameters
                        {
                            Content = "Não é possivel adicionar quantidade maior que o saldo de estoque.",
                            Header = "Adicionar Etiqueta(s)"
                        });
                        return;
                    }

                    try
                    {
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                        vm.Livres = await vm.GetLivresAsync(limit);
                        foreach (var item in vm.Livres)
                        {
                            await vm.AddEtiquetaAsync(new ControladoZebraModel
                            {
                                codcompladicional = record.codcompladicional,
                                codigo = item.codigo
                            });
                        }

                        record.etiquetas += limit;
                        ProdutosGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    }
                }
            });
        }

        private void OnRemoverEtiquetaClick(object sender, RoutedEventArgs e)
        {
        }

        private void OnImpressasEtiquetaClick(object sender, RoutedEventArgs e)
        {
            if (ProdutosGrid.SelectedItem is not ControladoEtiquetaModel item)
            {
                return;
            }

            var radWindow = new Impressas(item.codcompladicional)
            {
                Width = 400,
                Height = 300,
                ResizeMode = ResizeMode.NoResize,
                CanMove = false
            };
            StyleManager.SetTheme(radWindow, new Windows8Theme());
            radWindow.ShowDialog();
        }

        private static void MostrarAlertaEtiqueta(string mensagem)
        {
            var alert = new RadDesktopAlert
            {
                Header = "NOTIFICAÇÃO IMPRESSÃO ETIQUETA",
                Content = mensagem,
                ShowDuration = 3000
            };

            var manager = new RadDesktopAlertManager();
            StyleManager.SetTheme(alert, new Windows8Theme());
            manager.ShowAlert(alert);
        }

        private static void EscreverEtiqueta(StreamWriter writer, QryImpressaoModel etiqueta)
        {
            writer.WriteLine(@"^XA");
            writer.WriteLine(@"^PW184");
            writer.WriteLine(@"^CI28");
            writer.WriteLine($@"^FT24,313^BQN,2,6");
            writer.WriteLine($@"^FH\^FDHA,{etiqueta.barcode}^FS");
            writer.WriteLine($@"^FT160,295^AAB,9,5^FH\^FDPRODUTO^FS");
            writer.WriteLine($@"^FT175,295^A0B,11,19^FH\^FD{etiqueta.codcompladicional}^FS");
            writer.WriteLine($@"^FT160,229^AAB,9,5^FH\^FDETIQUETA^FS");
            writer.WriteLine($@"^FT175,229^A0B,11,19^FH\^FD{etiqueta.codigo}^FS");
            writer.WriteLine($@"^FT141,160^A0B,15^FB121,8,0,C^FH\^FD{etiqueta.descricao_completa?.Replace("ÚNICO", "")}^FS");
            writer.WriteLine(@"^PQ1,0,1,Y^XZ");
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
}
