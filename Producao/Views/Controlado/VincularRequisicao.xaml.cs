﻿using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using Producao.Views.CentralModelos;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Persistence.Core;

namespace Producao.Views.Controlado
{
    /// <summary>
    /// Interação lógica para VincularRequisicao.xam
    /// </summary>
    public partial class VincularRequisicao : UserControl
    {
        public VincularRequisicao()
        {
            InitializeComponent();
            DataContext = new VincularRequisicaoViewModel();
        }

        private async void OnBuscaProdutos(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                    long requisicao = long.Parse(((TextBox)sender).Text);
                    VincularRequisicaoViewModel vm = (VincularRequisicaoViewModel)DataContext;
                    vm.Requisicao = await Task.Run(() => vm.GetRequisicaoAsync(requisicao));
                    if (vm.Requisicao == null)
                    {
                        MessageBox.Show("Requisição não encontrado", "Busca de requisição");
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                        return;
                    }
                    vm.Produtos = await Task.Run(() => vm.GetProdutosAsync(requisicao));
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    codigoProduto.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
            }
        }

        private async void OnAdicionarProduto(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    long codigo = long.Parse(((TextBox)sender).Text);
                    VincularRequisicaoViewModel vm = (VincularRequisicaoViewModel)DataContext;
                    vm.Etiqueta = await Task.Run(() => vm.GetEtiquetaAsync(codigo));
                    if (vm.Etiqueta == null)
                    {
                        MessageBox.Show("Etiqueta não encontrado", "Busca de etiqueta");
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                        return;
                    }

                    vm.Barcode = await Task.Run(() => vm.GetBarcodeAsync(codigo));
                    if (vm.Barcode == null)
                    {
                        MessageBox.Show("Código de barras não encontrado", "Busca de Barcode");
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                        return;
                    }

                    var confirma = MessageBox.Show("Deseja Adicionar o produto na lista?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
                    if (confirma == MessageBoxResult.Yes)
                    {
                        await Task.Run(() => vm.AddControladoAsync(
                            new ControladoShoppingModel 
                            { 
                                barcode = vm.Barcode.barcode, 
                                inserido_por = Environment.UserName, 
                                inserido_em = DateTime.Now, 
                                num_requisicao = vm.Requisicao.num_requisicao
                            }));
                    }

                    long requisicao = long.Parse(txtRequisicao.Text);
                    vm.Produtos = await Task.Run(() => vm.GetProdutosAsync(requisicao));

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private async void OnBaixaRequisicaoClick(object sender, RoutedEventArgs e)
        {
            try
            {
                VincularRequisicaoViewModel vm = (VincularRequisicaoViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                var query = vm.Produtos
                  .GroupBy(x => new { x.codcompladicional, x.num_requisicao, x.planilha, x.descricao, x.descricao_adicional, x.complementoadicional, x.descricao_completa })
                  .Select(g => new TransformaRequisicaoModel
                  {
                      codcompladicional = g.Key.codcompladicional,
                      quantidade = g.Sum(x => x.quantidade),
                      num_requisicao = g.Key.num_requisicao,
                      planilha = g.Key.planilha,
                      descricao = g.Key.descricao,
                      descricao_adicional = g.Key.descricao_adicional,
                      complementoadicional = g.Key.complementoadicional,
                      descricao_completa = g.Key.descricao_completa,
                  }).ToList();

                foreach (var item in query)
                {
                    await Task.Run(() => vm.BaixaReceitaAsync(item));
                }

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show("Baixa efetuada com sucesso!!!", "Baixa controlado.");
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void SfDataGrid_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {

        }

        private async void SfDataGrid_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            VincularRequisicaoViewModel vm = (VincularRequisicaoViewModel)DataContext;
            var confirma = MessageBox.Show("Deseja Deletar o produto da lista?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
            var item = e.Items[0] as TransformaRequisicaoModel;
            if (confirma == MessageBoxResult.Yes)
            {
                await Task.Run(() => vm.DeleteControladoAsync((long)item.num_requisicao, item.barcode));
                   
            }
            else
            {
                e.Cancel = true;
            }
        }
    }

    public class VincularRequisicaoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<TransformaRequisicaoModel> _produtos;
        public ObservableCollection<TransformaRequisicaoModel> Produtos
        {
            get { return _produtos; }
            set { _produtos = value; RaisePropertyChanged("Produtos"); }
        }

        private TransformaRequisicaoModel _produto;
        public TransformaRequisicaoModel Produto
        {
            get { return _produto; }
            set { _produto = value; RaisePropertyChanged("Produto"); }
        }

        private RequisicaoModel _requisicao;
        public RequisicaoModel Requisicao
        {
            get { return _requisicao; }
            set { _requisicao = value; RaisePropertyChanged("Requisicao"); }
        }

        private ControladoZebraModel _etiqueta;
        public ControladoZebraModel Etiqueta
        {
            get { return _etiqueta; }
            set { _etiqueta = value; RaisePropertyChanged("Etiqueta"); }
        }

        private BarcodeModel _barcode;
        public BarcodeModel Barcode
        {
            get { return _barcode; }
            set { _barcode = value; RaisePropertyChanged("Barcode"); }
        }
        
        private ControladoShoppingModel _controladoShopping;
        public ControladoShoppingModel ControladoShopping
        {
            get { return _controladoShopping; }
            set { _controladoShopping = value; RaisePropertyChanged("ControladoShopping"); }
        }

        public async Task<RequisicaoModel> GetRequisicaoAsync(long num_requisicao)
        {
            try
            {
                using DatabaseContext db = new();
                return await db.Requisicoes.FindAsync(num_requisicao);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<TransformaRequisicaoModel>> GetProdutosAsync(long nRequisicao)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.TransformaRequisicoes.Where(c => c.num_requisicao == nRequisicao).ToListAsync();
                return new ObservableCollection<TransformaRequisicaoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ControladoZebraModel> GetEtiquetaAsync(long codigo)
        {
            try
            {
                using DatabaseContext db = new();
                return await db.ControladosZebra.FindAsync(codigo);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BarcodeModel> GetBarcodeAsync(long codigo)
        {
            try
            {
                using DatabaseContext db = new();
                return await db.Barcodes.FindAsync(codigo);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddControladoAsync(ControladoShoppingModel controlado)
        {
            try
            {
                using DatabaseContext db = new();
                await db.ControladoShoppings.AddAsync(controlado);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteControladoAsync(long num_requisicao, string barcode)
        {
            try
            {
                using DatabaseContext db = new();

                var controlado = await db.ControladoShoppings.Where(x => x.num_requisicao == num_requisicao && x.barcode == barcode).FirstOrDefaultAsync();

                await db.ControladoShoppings.SingleDeleteAsync(controlado);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task BaixaReceitaAsync(TransformaRequisicaoModel model)
        {
            try
            {
                using DatabaseContext db = new();

                var detReq = await db.RequisicaoDetalhes.Where(r => r.num_requisicao == model.num_requisicao && r.codcompladicional == model.codcompladicional).FirstOrDefaultAsync();
                if (detReq != null)
                {
                    detReq.codcompladicional = model.codcompladicional;
                    detReq.quantidade = model.quantidade;
                    detReq.data = DateTime.Now;
                    detReq.alterado_por = Environment.UserName;
                    db.RequisicaoDetalhes.Update(detReq);
                }
                else
                {
                    var det = new DetalheRequisicaoModel
                    {
                        num_requisicao = model.num_requisicao,
                        codcompladicional = model.codcompladicional,
                        quantidade = model.quantidade,
                        data = DateTime.Now,
                        alterado_por = Environment.UserName,
                    };
                    await db.RequisicaoDetalhes.AddAsync(det);
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
