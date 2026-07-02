using Dapper;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views.OrdemServico.Produto
{
    /// <summary>
    /// Interação lógica para Relplan.xam
    /// </summary>
    public partial class Relplan : UserControl
    {
        public Relplan()
        {
            InitializeComponent();
            DataContext = new RelplanViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                RelplanViewModel vm = (RelplanViewModel)DataContext;
                vm.Relplans = await vm.GetRelplanAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }
    }

    class RelplanViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private RelplanModel _relplan;
        public RelplanModel Relplan
        {
            get { return _relplan; }
            set { _relplan = value; RaisePropertyChanged("Relplan"); }
        }

        private ObservableCollection<RelplanModel> _relplans;
        public ObservableCollection<RelplanModel> Relplans
        {
            get { return _relplans; }
            set { _relplans = value; RaisePropertyChanged("Relplans"); }
        }

        public async Task<ObservableCollection<RelplanModel>> GetRelplanAsync()
        {
            try
            {
                using var conn = new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
                var data = await conn.QueryAsync<RelplanModel>(
                    @"SELECT *
                      FROM producao.relplan
                      ORDER BY planilha;");
                return new ObservableCollection<RelplanModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
