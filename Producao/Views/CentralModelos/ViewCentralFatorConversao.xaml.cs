using Microsoft.EntityFrameworkCore;
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
    /// Interação lógica para ViewCentralFatorConversao.xam
    /// </summary>
    public partial class ViewCentralFatorConversao : UserControl
    {
        public ViewCentralFatorConversao()
        {
            InitializeComponent();
            this.DataContext = new ViewCentralFatorConversaoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewCentralFatorConversaoViewModel vm = (ViewCentralFatorConversaoViewModel)DataContext;
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

        private void dgTabela_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {

        }

        private async void dgTabela_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            ModeloTabelaConversaoModel model = (ModeloTabelaConversaoModel)e.RowData;
            if (!model.codcompladicional.HasValue)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("codcompladicional", "Seleciona o a P.A.");
                return;
            }

            ViewCentralFatorConversaoViewModel vm = (ViewCentralFatorConversaoViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                await Task.Run(() => vm.SaveAsync(model));
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

                MessageBox.Show("Fator cadastrado!!!");
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

    public class ViewCentralFatorConversaoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private ModeloTabelaConversaoModel item;
        public ModeloTabelaConversaoModel Item
        {
            get { return item; }
            set { item = value; RaisePropertyChanged("Item"); }
        }
        private ObservableCollection<ModeloTabelaConversaoModel> itens;
        public ObservableCollection<ModeloTabelaConversaoModel> Itens
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

        public async Task<ObservableCollection<ModeloTabelaConversaoModel>> GetItensAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.TabelaConversoes.ToListAsync();
                return new ObservableCollection<ModeloTabelaConversaoModel>(data);
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
                                     where s.inativo != "-1   "
                                     select new ProdutoPAModel
                                     {
                                         codcompladicional = s.codcompladicional,
                                         descricao = s.descricao_completa
                                     }).ToListAsync();

                return new ObservableCollection<ProdutoPAModel>(results);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SaveAsync(ModeloTabelaConversaoModel model)
        {
            try
            {
                using DatabaseContext db = new();
                var result = await db.TabelaConversoes.FindAsync(model.codcompladicional);
                if (result == null)
                    await db.TabelaConversoes.AddAsync(model);
                else
                    await db.TabelaConversoes.SingleUpdateAsync(model);

                await db.SaveChangesAsync();

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
