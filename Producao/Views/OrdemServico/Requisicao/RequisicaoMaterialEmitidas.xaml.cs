using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Producao.Views.OrdemServico.Requisicao
{
    /// <summary>
    /// Interação lógica para RequisicaoMaterialEmitidas.xam
    /// </summary>
    public partial class RequisicaoMaterialEmitidas : UserControl
    {
        public RequisicaoMaterialEmitidas()
        {
            DataContext = new RequisicaoMaterialEmitidasViewModel();
            InitializeComponent();
        }

        private async void UserControl_Initialized(object sender, EventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                RequisicaoMaterialEmitidasViewModel vm = (RequisicaoMaterialEmitidasViewModel)DataContext;
                vm.Itens = await Task.Run(vm.GetRequisicaoDetalhesAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }
    }

    class RequisicaoMaterialEmitidasViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private GeralRequisicaoProducaoModel _item;
        public GeralRequisicaoProducaoModel Item
        {
            get { return _item; }
            set { _item = value; RaisePropertyChanged("Item"); }
        }
        private ObservableCollection<GeralRequisicaoProducaoModel> _itens;
        public ObservableCollection<GeralRequisicaoProducaoModel> Itens
        {
            get { return _itens; }
            set { _itens = value; RaisePropertyChanged("Itens"); }
        }

        public async Task<ObservableCollection<GeralRequisicaoProducaoModel>> GetRequisicaoDetalhesAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.RequisicoesProducao.ToListAsync();
                return new ObservableCollection<GeralRequisicaoProducaoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
