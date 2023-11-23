using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using Producao.Views.CentralModelos;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
    }

    public static class ContextMenuCommandsImprimirEtiqueta
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
            var record = ((GridRecordContextMenuInfo)obj).Record as ControladoEtiquetaModel;
            var grid = ((GridRecordContextMenuInfo)obj).DataGrid;
            var item = grid.SelectedItem as ControladoEtiquetaModel;
            ImprimirEtiquetaViewModel vm = (ImprimirEtiquetaViewModel)grid.DataContext;
        }

        static BaseCommand? adicionar;
        public static BaseCommand Adicionar
        {
            get
            {
                adicionar ??= new BaseCommand(OnAdicionar);
                return adicionar;
            }
        }
        private static async void OnAdicionar(object obj)
        {
            var record = ((GridRecordContextMenuInfo)obj).Record as ControladoEtiquetaModel;
            var grid = ((GridRecordContextMenuInfo)obj).DataGrid;
            var item = grid.SelectedItem as ControladoEtiquetaModel;
            ImprimirEtiquetaViewModel vm = (ImprimirEtiquetaViewModel)grid.DataContext;
        }

        static BaseCommand? remover;
        public static BaseCommand Remover
        {
            get
            {
                remover ??= new BaseCommand(OnRemover);
                return remover;
            }
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
            get
            {
                impressas ??= new BaseCommand(OnImpressas);
                return impressas;
            }
        }
        private static async void OnImpressas(object obj)
        {
            var record = ((GridRecordContextMenuInfo)obj).Record as ControladoEtiquetaModel;
            var grid = ((GridRecordContextMenuInfo)obj).DataGrid;
            var item = grid.SelectedItem as ControladoEtiquetaModel;
            ImprimirEtiquetaViewModel vm = (ImprimirEtiquetaViewModel)grid.DataContext;
        }

        static BaseCommand? gerar;
        public static BaseCommand Gerar
        {
            get
            {
                gerar ??= new BaseCommand(OnGerar);
                return gerar;
            }
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
