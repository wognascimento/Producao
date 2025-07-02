using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using Syncfusion.UI.Xaml.Utility;
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
                vm.Impressas = await Task.Run( () => vm.GetImpressasAsync(Codcompladicional));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                    string IPAdress = "192.168.0.113";
                    int Port = 9100;
                    StreamWriter? SWriter;
                    TcpClient? Client = new();

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    ImpressasViewModel vm = (ImpressasViewModel)DataContext;
                    await Client.ConnectAsync(IPAdress, Port);
                    SWriter = new(Client.GetStream());
                    foreach (var impressa in vm.Impressas.Where(f => f.vinculado == false))
                    {
                        var record = impressa;
                        var etiqueta = await vm.GetImprimirAsync(record.codigo);
                        SWriter.WriteLine($@"^XA");
                        SWriter.WriteLine($@"^CI28");
                        SWriter.WriteLine($@"^PW320");
                        SWriter.WriteLine($@"^FO0,160^GFA,01280,01280,00040,:Z64:eJxjYCAOiAS6EoMEiDRuFIyCUTAKBhwAAHykCLM=:ECE9");
                        SWriter.WriteLine($@"^FT20,168^BQN,2,6");
                        SWriter.WriteLine($@"^FH\^FDHA,{etiqueta.barcode}^FS");
                        SWriter.WriteLine($@"^FT156,126^AAN,5,5^FH\^FDPRODUTO^FS");
                        SWriter.WriteLine($@"^FT156,145^A0N,16,20^FH\^FD{etiqueta.codcompladicional}^FS");
                        SWriter.WriteLine($@"^FT241,126^AAN,5,5^FH\^FDETIQUETA^FS");
                        SWriter.WriteLine($@"^FT241,145^A0N,16,20^FH\^FD{etiqueta.codigo}^FS");
                        SWriter.WriteLine($@"^FT157,105^A0N,16^FB151,5,0^FH\^FD{etiqueta.descricao_completa}^FS");
                        SWriter.WriteLine($@"^PQ1,0,1,Y^XZ");
                        //using DatabaseContext db = new();
                        //await db.Database.ExecuteSqlRawAsync("UPDATE producao.tbl_barcodes SET impresso = '-1' WHERE codigo = {0}", etiqueta.codigo);
                    }
                    await SWriter.FlushAsync();
                    await SWriter.DisposeAsync();
                    SWriter.Close();

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
                    MessageBox.Show(ex.Message);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                
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
                using DatabaseContext db = new();
                var data = await db.ControladoEtiquetaImpressas.Where(x => x.codcompladicional == codcompladicional).ToListAsync();
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
                using DatabaseContext db = new();
                var data = await db.Impressoes.FirstOrDefaultAsync(i => i.codigo == codigo);
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public static class ContextMenuCommandEtiquetaImpressa
    {
        static BaseCommand? imprimir;
        public static BaseCommand Imprimir
        {
            get
            {
                imprimir ??= new BaseCommand(OnImprimir);
                return imprimir;
            }
        }
        private static async void OnImprimir(object obj)
        {
            ImpressasViewModel vm = (ImpressasViewModel)obj;
            var record = vm.Impressa;
            string IPAdress = "192.168.0.113";
            int Port = 9100;
            StreamWriter? SWriter;
            TcpClient? Client = new();
            try
            {
                await Client.ConnectAsync(IPAdress, Port);
                SWriter = new(Client.GetStream());
                //SWriter = new(@"C:\TEMP\ETIQUETA.TXT");
                var etiqueta = await vm.GetImprimirAsync(record.codigo);
                SWriter.WriteLine($@"^XA");
                SWriter.WriteLine($@"^CI28");
                SWriter.WriteLine($@"^PW320");
                SWriter.WriteLine($@"^FO0,160^GFA,01280,01280,00040,:Z64:eJxjYCAOiAS6EoMEiDRuFIyCUTAKBhwAAHykCLM=:ECE9");
                SWriter.WriteLine($@"^FT20,168^BQN,2,6");
                SWriter.WriteLine($@"^FH\^FDHA,{etiqueta.barcode}^FS");
                SWriter.WriteLine($@"^FT156,126^AAN,5,5^FH\^FDPRODUTO^FS");
                //SWriter.WriteLine($@"^FO156,129^GB63,13,13^FS");
                SWriter.WriteLine($@"^FT156,145^A0N,16,20^FH\^FD{etiqueta.codcompladicional}^FS");
                SWriter.WriteLine($@"^FT241,126^AAN,5,5^FH\^FDETIQUETA^FS");
                //SWriter.WriteLine($@"^FO241,129^GB63,13,13^FS");
                SWriter.WriteLine($@"^FT241,145^A0N,16,20^FH\^FD{etiqueta.codigo}^FS");
                /*
                SWriter.WriteLine($@"^FT157,33^AAN,9,5^FB151,1,0,C^FH\^FD{etiqueta.descricao_completa}^FS");
                SWriter.WriteLine($@"^FT157,42^AAN,9,5^FB151,1,0,C^FH\^FDDESCRI\80\C7O DO PRODUTO FORM^FS");
                SWriter.WriteLine($@"^FT157,51^AAN,9,5^FB151,1,0,C^FH\^FDDESCRI\80\C7O DO PRODUTO FORM^FS");
                SWriter.WriteLine($@"^FT157,60^AAN,9,5^FB151,1,0,C^FH\^FDDESCRI\80\C7O DO PRODUTO FORM^FS");
                SWriter.WriteLine($@"^FT157,69^AAN,9,5^FB151,1,0,C^FH\^FDDESCRI\80\C7O DO PRODUTO FORM^FS");
                SWriter.WriteLine($@"^FT157,78^AAN,9,5^FB151,1,0,C^FH\^FDDESCRI\80\C7O DO PRODUTO FORM^FS");
                SWriter.WriteLine($@"^FT157,87^AAN,9,5^FB151,1,0,C^FH\^FDDESCRI\80\C7O DO PRODUTO FORM^FS");
                SWriter.WriteLine($@"^FT157,96^AAN,9,5^FB151,1,0,C^FH\^FDDESCRI\80\C7O DO PRODUTO FORM^FS");
                */
                SWriter.WriteLine($@"^FT157,105^A0N,16^FB151,5,0^FH\^FD{etiqueta.descricao_completa}^FS");
                SWriter.WriteLine($@"^PQ1,0,1,Y^XZ");

                await SWriter.FlushAsync();
                await SWriter.DisposeAsync();
                SWriter.Close();

                using DatabaseContext db = new();
                await db.Database.ExecuteSqlRawAsync("UPDATE producao.tbl_barcodes SET impresso = '-1' WHERE codigo = {0}", etiqueta.codigo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
