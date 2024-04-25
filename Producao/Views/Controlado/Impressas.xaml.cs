using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Utility;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
            var record = obj as ControladoEtiquetaImpressaModel;
        }
    }
}
