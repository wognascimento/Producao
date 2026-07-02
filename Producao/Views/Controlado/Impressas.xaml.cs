using Dapper;
using Npgsql;
using Producao.DataBase.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace Producao.Views.Controlado
{
    /// <summary>
    /// Lógica interna para Impressas.xaml
    /// </summary>
    public partial class Impressas : RadWindow
    {
        private long Codcompladicional;
        public Impressas(long? codcompladicional)
        {
            InitializeComponent();
            DataContext = new ImpressasViewModel();
            Codcompladicional = (long)codcompladicional;  
        }

        private async void RadWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ImpressasViewModel vm = (ImpressasViewModel)DataContext;
                vm.Impressas = await vm.GetImpressasAsync(Codcompladicional);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnPrintAll(object sender, RoutedEventArgs e)
        {
            ImpressasViewModel vm = (ImpressasViewModel)DataContext;
            if (!vm.Impressas.Any(f => f.vinculado == false))
            {
                RadWindow.Alert(new DialogParameters()
                {
                    Theme = new CrystalTheme(),
                    Content = "Não existem etiquetas não vinculadas a uma requisição.",
                    Header = "Atenção",
                });
                return;
            }

            RadWindow.Confirm(new DialogParameters()
            {
                Theme = new CrystalTheme(),
                OkButtonContent = "Sim",
                CancelButtonContent = "Não",
                Content = "Deseja imprimir todas as etiquetas não vinculadas a uma requisição ?",
                Header = "Atenção",
                Closed = OnPrintdAsync
            });
            
        }

        private async void OnPrintdAsync(object sender, WindowClosedEventArgs e)
        {
            var result = e.DialogResult;
            if (result == true)
            {
                try
                {
                    var printer = ThermalPrinterConfiguration.Load();

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    ImpressasViewModel vm = (ImpressasViewModel)DataContext;
                    using var client = new TcpClient();
                    await client.ConnectAsync(printer.IpAddress, printer.Port);
                    await using var writer = new StreamWriter(client.GetStream());
                    foreach (var impressa in vm.Impressas.Where(f => f.vinculado == false))
                    {
                        var record = impressa;
                        var etiqueta = await vm.GetImprimirAsync(record.codigo);
                        writer.WriteLine($@"^XA");
                        writer.WriteLine($@"^PW184");
                        writer.WriteLine($@"^CI28");
                        //SWriter.WriteLine($@"^FT24,313^BQN,2,6");
                        //SWriter.WriteLine($@"^FH\^FDHA,{etiqueta.barcode}^FS");
                        //SWriter.WriteLine($@"^FT124,159^AAB,9,5^FH\^FDPRODUTO^FS");
                        //SWriter.WriteLine($@"^FT139,159^A0B,11,19^FH\^FD{etiqueta.codcompladicional}^FS");
                        //SWriter.WriteLine($@"^FT124,93^AAB,9,5^FH\^FDETIQUETA^FS");
                        //SWriter.WriteLine($@"^FT139,93^A0B,11,19^FH\^FD{etiqueta.codigo}^FS");
                        //SWriter.WriteLine($@"^FT105,160^AAB,9,5^FB121,6,0,C^FH\^FD{etiqueta.descricao_completa}^FS");
                        //SWriter.WriteLine($@"^PQ1,0,1,Y^XZ");
                        writer.WriteLine($@"^FT24,313^BQN,2,6");
                        writer.WriteLine($@"^FH\^FDHA,{etiqueta.barcode}^FS");
                        writer.WriteLine($@"^FT160,295^AAB,9,5^FH\^FDPRODUTO^FS");
                        writer.WriteLine($@"^FT175,295^A0B,11,19^FH\^FD{etiqueta.codcompladicional}^FS");
                        writer.WriteLine($@"^FT160,229^AAB,9,5^FH\^FDETIQUETA^FS");
                        writer.WriteLine($@"^FT175,229^A0B,11,19^FH\^FD{etiqueta.codigo}^FS");
                        writer.WriteLine($@"^FT141,160^A0B,15^FB121,8,0,C^FH\^FD{etiqueta.descricao_completa?.Replace("ÚNICO", "")}^FS");
                        writer.WriteLine($@"^PQ1,0,1,Y^XZ");

                        //await db.Database.ExecuteSqlRawAsync("UPDATE producao.tbl_barcodes SET impresso = '-1' WHERE codigo = {0}", etiqueta.codigo);
                    }
                    await writer.FlushAsync();

                    RadWindow.Alert(new DialogParameters()
                    {
                        Theme = new CrystalTheme(),
                        Content = "Etiquetas impressas.",
                        Header = "Atenção",
                    });

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    Producao.ErrorDialog.Show(ex, "Erro");
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                
            }
        }

        private async void OnImprimirEtiquetaClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is not ImpressasViewModel vm || vm.Impressa is null)
            {
                return;
            }

            var printer = ThermalPrinterConfiguration.Load();
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(printer.IpAddress, printer.Port);
                await using var writer = new StreamWriter(client.GetStream());
                var etiqueta = await vm.GetImprimirAsync(vm.Impressa.codigo);

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

                await writer.FlushAsync();

                await vm.MarcarImpressoAsync(etiqueta.codigo);
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
            }
        }
    }

    public class ImpressasViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<ControladoEtiquetaImpressaModel> _impressas;
        public ObservableCollection<ControladoEtiquetaImpressaModel> Impressas
        {
            get { return _impressas; }
            set { _impressas = value; RaisePropertyChanged("Impressas"); }
        }
        private ControladoEtiquetaImpressaModel _impressa;
        public ControladoEtiquetaImpressaModel Impressa
        {
            get { return _impressa; }
            set { _impressa = value; RaisePropertyChanged("Impressa"); }
        }

        public async Task<ObservableCollection<ControladoEtiquetaImpressaModel>> GetImpressasAsync(long? codcompladicional)
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                var data = await conn.QueryAsync<ControladoEtiquetaImpressaModel>(
                    @"SELECT *
                      FROM producao.qry_codigo_impresso
                      WHERE codcompladicional = @codcompladicional;",
                    new { codcompladicional });
                return new ObservableCollection<ControladoEtiquetaImpressaModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<QryImpressaoModel> GetImprimirAsync(long? codigo)
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                return await conn.QueryFirstOrDefaultAsync<QryImpressaoModel>(
                    @"SELECT *
                      FROM producao.qry_impressao
                      WHERE codigo = @codigo
                      LIMIT 1;",
                    new { codigo });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task MarcarImpressoAsync(long? codigo)
        {
            using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
            await conn.ExecuteAsync(
                @"UPDATE producao.tbl_barcodes
                  SET impresso = '-1'
                  WHERE codigo = @codigo;",
                new { codigo });
        }
    }

}
