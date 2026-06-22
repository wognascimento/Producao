using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.Planilha
{
    /// <summary>
    /// Interação lógica para AreaTema.xam
    /// </summary>
    public partial class AreaTema : UserControl
    {
        public AreaTema()
        {
            InitializeComponent();
            this.DataContext = new AreaTemaViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                AreaTemaViewModel vm = (AreaTemaViewModel)DataContext;
                vm.Siglas = await vm.GetSiglasAsync();
                vm.AreaTemas = await vm.GeAreaTemasAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void OnRowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            var registro = e.EditedItem as TblAreaTemaModel;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                AreaTemaViewModel vm = (AreaTemaViewModel)DataContext;
                if (registro is null)
                {
                    return;
                }

                await vm.SaveAsync(registro);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (DbUpdateException ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.InnerException?.Message);

                if (registro is not null && registro.codareatema == 0)
                    ((AreaTemaViewModel)DataContext).AreaTemas.Remove(registro);
                else
                    gridAreaTema.Rebind();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);

                if (registro is not null && registro.codareatema == 0)
                    ((AreaTemaViewModel)DataContext).AreaTemas.Remove(registro);
                else
                    gridAreaTema.Rebind();
            }
        }

        private void OnRowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
        }

        private void gridAreaTema_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            if (e.Cell?.DataContext is not TblAreaTemaModel registro)
            {
                return;
            }

            if (e.Cell.Column.UniqueName == "sigla")
            {
                var funcionario = ((AreaTemaViewModel)DataContext).Siglas.FirstOrDefault(f => f.sigla_serv == registro.sigla);
                if (funcionario != null)
                {
                    registro.tema = funcionario.tema;
                }
            }

            if (e.Cell.Column.UniqueName == "construcao_total")
            {
                registro.cenografia_planta = registro.area_total_planta - registro.trilha_planta - registro.pa - registro.construcao_total;
            }

            gridAreaTema.Rebind();
        }
    }

    public class AreaTemaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<AprovadoModel>? _siglas;
        public ObservableCollection<AprovadoModel> Siglas
        {
            get { return _siglas; }
            set { _siglas = value; RaisePropertyChanged("Siglas"); }
        }

        private ObservableCollection<string>? _locais = ["Praça principal", "Trono", "Presépio", "Mini-Cenário", "Oficina", "Fachada"];
        public ObservableCollection<string> Locais
        {
            get { return _locais; }
            set { _locais = value; RaisePropertyChanged("Locais"); }
        }

        private ObservableCollection<TblAreaTemaModel>? _areaTemas;
        public ObservableCollection<TblAreaTemaModel> AreaTemas
        {
            get { return _areaTemas; }
            set { _areaTemas = value; RaisePropertyChanged("AreaTemas"); }
        }

        private TblAreaTemaModel? _areaTema;
        public TblAreaTemaModel AreaTema
        {
            get { return _areaTema; }
            set { _areaTema = value; RaisePropertyChanged("AreaTema"); }
        }

        public async Task<ObservableCollection<AprovadoModel>> GetSiglasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Aprovados
                    .OrderBy(a => a.sigla_serv)
                    .ToListAsync();
                return new ObservableCollection<AprovadoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<string>> GeTemasAsync(string sigla)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.Aprovados
                    .Where(a => a.sigla_serv == sigla)
                    .OrderBy(a => a.tema)
                    .GroupBy(a => a.tema)
                    .Select(a => a.Key)
                    .ToListAsync();
                return new ObservableCollection<string>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<TblAreaTemaModel>> GeAreaTemasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.AreaTemas
                    .ToListAsync();
                return new ObservableCollection<TblAreaTemaModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TblAreaTemaModel> SaveAsync(TblAreaTemaModel areaTema)
        {
            try
            {
                using DatabaseContext db = new();

                db.Entry(areaTema).State = areaTema.codareatema == 0 ?
                                   EntityState.Added :
                                   EntityState.Modified;

                db.SaveChanges();

                return areaTema;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
