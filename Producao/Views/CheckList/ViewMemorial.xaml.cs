using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
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

    }
}
