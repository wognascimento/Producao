using iText.Commons.Actions.Contexts;
using Microsoft.EntityFrameworkCore;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.XlsIO.Implementation.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace Producao.Views.Estoque
{
    /// <summary>
    /// Interação lógica para DesbloqueioAcertoEstoque.xam
    /// </summary>
    public partial class DesbloqueioAcertoEstoque : UserControl
    {
        public DesbloqueioAcertoEstoque()
        {
            InitializeComponent();
            DataContext = new DesbloqueioAcertoEstoqueViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                DesbloqueioAcertoEstoqueViewModel vm = (DesbloqueioAcertoEstoqueViewModel)DataContext;
                vm.Itens = await Task.Run(vm.GetListAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private async void itens_CurrentCellValueChanged(object sender, CurrentCellValueChangedEventArgs e)
        {
            try
            {
                var sfdatagrid = sender as SfDataGrid;
                DesbloqueioAcertoEstoqueViewModel vm = (DesbloqueioAcertoEstoqueViewModel)DataContext;
                AcertoEstoque acerto = (AcertoEstoque)e.Record;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                await Task.Run(() => vm.UpdateAsync(acerto));
                sfdatagrid.View.Refresh();
                vm.Itens = await Task.Run(vm.GetListAsync);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }
    }

    class DesbloqueioAcertoEstoqueViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private List<AcertoEstoque> _itens;
        public List<AcertoEstoque> Itens
        {
            get { return _itens; }
            set { _itens = value; RaisePropertyChanged("Itens"); }
        }

        public async Task<List<AcertoEstoque>> GetListAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var itens = await (
                    from ai in db.ControleAcertoEstoques
                    join al in db.Descricoes on ai.codcompladicional equals al.codcompladicional
                    where (ai.bloqueado == "-1")
                    select new AcertoEstoque
                    {
                        codigo = ai.codigo,
                        cod_movimentacao = ai.cod_movimentacao,
                        codcompladicional = al.codcompladicional,
                        planilha = al.planilha,
                        descricao_completa = al.descricao_completa,
                        unidade = al.unidade,
                        quantidade = ai.quantidade,
                        bloqueado = ai.bloqueado,
                        processo = ai.processo

                    }).ToListAsync();

                return itens;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task UpdateAsync(AcertoEstoque acerto)
        {
            try
            {
                using DatabaseContext db = new();
                var produto = db.ControleAcertoEstoques.FirstOrDefault(p => p.codigo == acerto.codigo);
                if (produto != null)
                {

                    produto.bloqueado = acerto.bloqueado;
                    db.Entry(produto).Property(p => p.bloqueado).IsModified = true;
                    
                    produto.liberado_por = Environment.UserName;
                    db.Entry(produto).Property(p => p.liberado_por).IsModified = true;
                    
                    produto.liberado_em = DateTime.Now;
                    db.Entry(produto).Property(p => p.liberado_em).IsModified = true;


                    db.SaveChanges();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

    }

    public class AcertoEstoque
    {
        public long? codigo { get; set; }
        public long? cod_movimentacao { get; set; }
        public long? codcompladicional { get; set; }
        public string? planilha { get; set; }
        public string? descricao_completa { get; set; }
        public string? unidade { get; set; }
        public double? quantidade { get; set; }
        public string? bloqueado { get; set; }
        public string? processo { get; set; }
    }
}
