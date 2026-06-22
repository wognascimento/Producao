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
    public partial class ViewCentralTabelaPA : UserControl
    {
        public ViewCentralTabelaPA()
        {
            InitializeComponent();
            DataContext = new ViewCentralTabelaPAViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewCentralTabelaPAViewModel vm = (ViewCentralTabelaPAViewModel)DataContext;
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
            if (e.Row?.Item is not ModeloTabelaPAModel model)
            {
                return;
            }

            if (!model.codcompladicional.HasValue)
            {
                e.IsValid = false;
                AddValidation(e, nameof(ModeloTabelaPAModel.codcompladicional), "Selecione a P.A.");
            }
        }

        private async void OnRowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            if (e.EditedItem is not ModeloTabelaPAModel model)
            {
                return;
            }

            ViewCentralTabelaPAViewModel vm = (ViewCentralTabelaPAViewModel)DataContext;
            try
            {
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                await vm.SaveAsync(model);
                Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
                MessageBox.Show("Fator P.A cadastrado!!!");
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

    public class ViewCentralTabelaPAViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ModeloTabelaPAModel item;
        public ModeloTabelaPAModel Item
        {
            get => item;
            set { item = value; RaisePropertyChanged(nameof(Item)); }
        }

        private ObservableCollection<ModeloTabelaPAModel> itens;
        public ObservableCollection<ModeloTabelaPAModel> Itens
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

        public async Task<ObservableCollection<ModeloTabelaPAModel>> GetItensAsync()
        {
            using DatabaseContext db = new();
            var data = await db.TabelaPAs.ToListAsync();
            return new ObservableCollection<ModeloTabelaPAModel>(data);
        }

        public async Task<ObservableCollection<ProdutoPAModel>> GetProdutosAsync()
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

        public async Task SaveAsync(ModeloTabelaPAModel model)
        {
            using DatabaseContext db = new();
            var result = await db.TabelaPAs.FindAsync(model.codcompladicional);
            if (result == null)
            {
                await db.TabelaPAs.AddAsync(model);
            }
            else
            {
                await db.TabelaPAs.SingleUpdateAsync(model);
            }

            await db.SaveChangesAsync();
        }
    }
}
