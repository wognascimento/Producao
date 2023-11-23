using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using Syncfusion.UI.Xaml.Grid;
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
    /// Interação lógica para ControladoRecebimento.xam
    /// </summary>
    public partial class ControladoRecebimento : UserControl
    {
        public ControladoRecebimento()
        {
            InitializeComponent();
            DataContext = new ControladoRecebimentoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ControladoRecebimentoViewModel vm = (ControladoRecebimentoViewModel)DataContext;
                vm.Produtos = await Task.Run(vm.GetProdutosAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void SfDataGrid_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var sfdatagrid = sender as SfDataGrid;
            ControladoRecebimentoViewModel vm = (ControladoRecebimentoViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ControladoRetornoGeralModel data = (ControladoRetornoGeralModel)e.RowData;
                vm.Retorno = new()
                {
                    id_aprovado = data.id_aprovado,
                    codcompladicional = data.codcompladicional,
                    qtd = data.retorno,
                    atualizado_por = Environment.UserName,
                    atualizado_em = DateTime.Now,
                };

                await Task.Run(() => vm.SaveRetornoAsync(vm.Retorno));
                sfdatagrid.View.Refresh();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }
    }

    public class ControladoRecebimentoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<ControladoRetornoGeralModel> _produtos;
        public ObservableCollection<ControladoRetornoGeralModel> Produtos
        {
            get { return _produtos; }
            set { _produtos = value; RaisePropertyChanged("Produtos"); }
        }

        private ControladoRetornoGeralModel _produto;
        public ControladoRetornoGeralModel Produto
        {
            get { return _produto; }
            set { _produto = value; RaisePropertyChanged("Produto"); }
        }
        
        private ControladoRecebidoModel _retorno;
        public ControladoRecebidoModel Retorno
        {
            get { return _retorno; }
            set { _retorno = value; RaisePropertyChanged("Retorno"); }
        }

        public async Task<ObservableCollection<ControladoRetornoGeralModel>> GetProdutosAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ControladoRetornoGeral.ToListAsync();
                return new ObservableCollection<ControladoRetornoGeralModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SaveRetornoAsync(ControladoRecebidoModel m)
        {
            try
            {
                using DatabaseContext db = new();
                var controlado = await db.ControladoRecebido.Where(w => w.id_aprovado == m.id_aprovado && w.codcompladicional == m.codcompladicional).FirstOrDefaultAsync();

                if (controlado == null)
                    await db.ControladoRecebido.AddAsync(m);
                else
                {
                    controlado.atualizado_em = m.atualizado_em;
                    controlado.atualizado_por = m.atualizado_por;
                    controlado.qtd = m.qtd;
                    db.ControladoRecebido.Update(controlado);
                }
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
