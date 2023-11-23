using Microsoft.EntityFrameworkCore;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views.CentralModelos
{
    /// <summary>
    /// Interação lógica para ViewCentralTabelaPA.xam
    /// </summary>
    public partial class ViewCentralTabelaPA : UserControl
    {
        public ViewCentralTabelaPA()
        {
            InitializeComponent();
            this.DataContext = new ViewCentralTabelaPAViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewCentralTabelaPAViewModel vm = (ViewCentralTabelaPAViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                vm.Produtos = await Task.Run(vm.GetProdutosAsync);
                vm.Itens = await Task.Run(vm.GetItensAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void dgTabela_RowValidated(object sender, RowValidatedEventArgs e)
        {
            
        }

        private async void dgTabela_RowValidating(object sender, RowValidatingEventArgs e)
        {
            ModeloTabelaPAModel model = (ModeloTabelaPAModel)e.RowData;
            if (!model.codcompladicional.HasValue)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("codcompladicional", "Seleciona o a P.A.");
                return;
            }

            ViewCentralTabelaPAViewModel vm = (ViewCentralTabelaPAViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                await Task.Run(() => vm.SaveAsync(model));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

                MessageBox.Show("Fator P.A cadastrado!!!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

                //var addNewRowController = this.dgTabela.GetAddNewRowController();
                //addNewRowController.CancelAddNew();

            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }

    public class ViewCentralTabelaPAViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private ModeloTabelaPAModel item;
        public ModeloTabelaPAModel Item
        {
            get { return item; }
            set { item = value; RaisePropertyChanged("Item"); }
        }
        private ObservableCollection<ModeloTabelaPAModel> itens;
        public ObservableCollection<ModeloTabelaPAModel> Itens
        {
            get { return itens; }
            set { itens = value; RaisePropertyChanged("Itens"); }
        }

        private ProdutoPAModel produto;
        public ProdutoPAModel Produto
        {
            get { return produto; }
            set { produto = value; RaisePropertyChanged("Produto"); }
        }
        private ObservableCollection<ProdutoPAModel> produtos;
        public ObservableCollection<ProdutoPAModel> Produtos
        {
            get { return produtos; }
            set { produtos = value; RaisePropertyChanged("Produtos"); }
        }

        public async Task<ObservableCollection<ModeloTabelaPAModel>> GetItensAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.TabelaPAs.ToListAsync();
                return new ObservableCollection<ModeloTabelaPAModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<ProdutoPAModel>> GetProdutosAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var results = await (from s in db.Descricoes
                              where s.planilha == "KIT ENF PA" && s.descricao == "PA" && s.inativo != "-1   "
                              select new ProdutoPAModel 
                              { 
                                  codcompladicional = s.codcompladicional,
                                  descricao = s.descricao_adicional + " " + s.complementoadicional
                              }).ToListAsync();

                return new ObservableCollection<ProdutoPAModel>(results);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SaveAsync(ModeloTabelaPAModel model)
        {
            try
            {
                using DatabaseContext db = new();
                var result = await db.TabelaPAs.FindAsync(model.codcompladicional);
                if(result == null)
                    await db.TabelaPAs.AddAsync(model);
                else
                    await db.TabelaPAs.SingleUpdateAsync(model);

                await db.SaveChangesAsync();

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
