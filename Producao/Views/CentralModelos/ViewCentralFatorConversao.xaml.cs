using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace Producao.Views.CentralModelos
{
    public partial class ViewCentralFatorConversao : UserControl
    {
        public ViewCentralFatorConversao()
        {
            InitializeComponent();
            DataContext = new ViewCentralFatorConversaoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewCentralFatorConversaoViewModel vm = (ViewCentralFatorConversaoViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                vm.Produtos = await vm.GetProdutosAsync();
                vm.Itens = await vm.GetItensAsync();
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
        }

        private void OnRowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.Row?.Item is not ModeloTabelaConversaoModel model)
            {
                return;
            }

            if (!model.codcompladicional.HasValue)
            {
                e.IsValid = false;
                AddValidation(e, nameof(ModeloTabelaConversaoModel.codcompladicional), "Selecione a P.A.");
            }
        }

        private async void OnRowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            if (e.EditedItem is not ModeloTabelaConversaoModel model)
            {
                return;
            }

            ViewCentralFatorConversaoViewModel vm = (ViewCentralFatorConversaoViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                await vm.SaveAsync(model);
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
                MessageBox.Show("Fator cadastrado!!!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
            }
        }

        private static void AddValidation(GridViewRowValidatingEventArgs e, string propertyName, string message)
        {
            e.ValidationResults.Add(new GridViewCellValidationResult
            {
                PropertyName = propertyName,
                ErrorMessage = message
            });
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }
    }

    public class ViewCentralFatorConversaoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ModeloTabelaConversaoModel item;
        public ModeloTabelaConversaoModel Item
        {
            get => item;
            set { item = value; RaisePropertyChanged(nameof(Item)); }
        }

        private ObservableCollection<ModeloTabelaConversaoModel> itens;
        public ObservableCollection<ModeloTabelaConversaoModel> Itens
        {
            get => itens;
            set { itens = value; RaisePropertyChanged(nameof(Itens)); }
        }

        private ProdutoPAModel produto;
        public ProdutoPAModel Produto
        {
            get => produto;
            set { produto = value; RaisePropertyChanged(nameof(Produto)); }
        }

        private ObservableCollection<ProdutoPAModel> produtos;
        public ObservableCollection<ProdutoPAModel> Produtos
        {
            get => produtos;
            set { produtos = value; RaisePropertyChanged(nameof(Produtos)); }
        }

        public async Task<ObservableCollection<ModeloTabelaConversaoModel>> GetItensAsync()
        {
            using DatabaseContext db = new();
            var data = await db.TabelaConversoes.ToListAsync();
            return new ObservableCollection<ModeloTabelaConversaoModel>(data);
        }

        public async Task<ObservableCollection<ProdutoPAModel>> GetProdutosAsync()
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

        public async Task SaveAsync(ModeloTabelaConversaoModel model)
        {
            using DatabaseContext db = new();
            var result = await db.TabelaConversoes.FindAsync(model.codcompladicional);
            if (result == null)
            {
                await db.TabelaConversoes.AddAsync(model);
            }
            else
            {
                await db.TabelaConversoes.SingleUpdateAsync(model);
            }

            await db.SaveChangesAsync();
        }
    }
}
