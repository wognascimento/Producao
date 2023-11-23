using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Producao.Views.CheckList
{
    /// <summary>
    /// Interação lógica para ViewEtiquetaCheckListEmitida.xam
    /// </summary>
    public partial class ViewEtiquetaCheckListEmitida : UserControl
    {
        public ViewEtiquetaCheckListEmitida()
        {
            InitializeComponent();
            this.DataContext = new EtiquetaEmitidaViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Visible;
                EtiquetaEmitidaViewModel vm = (EtiquetaEmitidaViewModel)DataContext;
                await Task.Run(async () => await vm.GetEtiquetasAsync());
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ((MainWindow)Application.Current.MainWindow).PbLoading.Visibility = Visibility.Hidden;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }

    public class EtiquetaEmitidaViewModel : INotifyPropertyChanged
    {

        public DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        #region Etiqueta
        private ObservableCollection<EtiquetaEmitidaModel> _etiquetas;
        public ObservableCollection<EtiquetaEmitidaModel> Etiquetas
        {
            get { return _etiquetas; }
            set { _etiquetas = value; RaisePropertyChanged("Etiquetas"); }
        }
        private EtiquetaEmitidaModel _etiqueta;
        public EtiquetaEmitidaModel Etiqueta
        {
            get { return _etiqueta; }
            set { _etiqueta = value; RaisePropertyChanged("Etiqueta"); }
        }
        #endregion

        public EtiquetaEmitidaViewModel() { }

        public async Task GetEtiquetasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.EtiquetaEmitidas.ToListAsync();
                Etiquetas = new ObservableCollection<EtiquetaEmitidaModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
