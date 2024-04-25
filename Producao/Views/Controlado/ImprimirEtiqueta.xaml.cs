using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Utility;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

        public async Task<ObservableCollection<ControladoEtiquetaModel>> GetProdutosAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ControladoEtiquetas.ToListAsync();
                return new ObservableCollection<ControladoEtiquetaModel>(data);
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
                return new ObservableCollection<ControladoEtiquetaLivreModel>(data);
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
                            if (limit > record.saldo_estoque)
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
                                record.impressas += limit;
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
