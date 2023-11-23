using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

namespace Producao.Views.OrdemServico.Requisicao
{
    /// <summary>
    /// Interação lógica para RequisicaoMaterialEmitir.xam
    /// </summary>
    public partial class RequisicaoMaterialEmitir : UserControl
    {
        public RequisicaoMaterialEmitir()
        {
            DataContext = new RequisicaoMaterialEmitirViewModel();
            InitializeComponent();
        }

        private async void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    string text = ((TextBox)sender).Text;
                    RequisicaoMaterialEmitirViewModel vm = (RequisicaoMaterialEmitirViewModel)DataContext;
                    vm.ProdutoServico = await Task.Run(() => vm.GetProdutoServicoAsync(long.Parse(text)));
                    if (vm.ProdutoServico == null)
                    {
                        MessageBox.Show("Número de serviço não encontrado", "Busca de número de serviço");
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                        return;
                    }
                    txtData.Text = DateTime.Now.ToString("MM/dd/yyyy");
                    txtEmitente.Text = Environment.UserName;
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
            }
        }

        private async void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                RequisicaoMaterialEmitirViewModel vm = (RequisicaoMaterialEmitirViewModel)DataContext;
                var requisicao = await Task.Run(() => vm.SaveRequisicaoAsync(new RequisicaoModel { num_os_servico = vm.ProdutoServico.num_os_servico, data = DateTime.Now, alterado_por = Environment.UserName}));
                RequisicaoMaterial detailsWindow = new RequisicaoMaterial(vm.ProdutoServico); //ProdutoServico
                detailsWindow.Owner = Window.GetWindow((DependencyObject)sender);  //(Window)obj;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                detailsWindow.Width = 800;
                detailsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }

    class RequisicaoMaterialEmitirViewModel : INotifyPropertyChanged
    {
        private ProdutoServicoModel _produtoServico;
        public ProdutoServicoModel ProdutoServico
        {
            get { return _produtoServico; }
            set { _produtoServico = value; RaisePropertyChanged("ProdutoServico"); }
        }

        public async Task<ProdutoServicoModel> GetProdutoServicoAsync(long num_os_servico)
        {
            try
            {
                using DatabaseContext db = new();
                return await db.ProdutoServicos.FindAsync(num_os_servico);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<RequisicaoModel> SaveRequisicaoAsync(RequisicaoModel? requisicao)
        {
            try
            {
                using DatabaseContext db = new();
                await db.Requisicoes.SingleMergeAsync(requisicao);
                await db.SaveChangesAsync();
                return requisicao;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
