using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Producao
{
    class RequisicaoViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<RelplanModel> _planilhas;
        public ObservableCollection<RelplanModel> Planilhas
        {
            get { return _planilhas; }
            set { _planilhas = value; RaisePropertyChanged("Planilhas"); }
        }
        private RelplanModel _planilha;
        public RelplanModel Planilha
        {
            get { return _planilha; }
            set { _planilha = value; RaisePropertyChanged("Planilha"); }
        }

        private ObservableCollection<ProdutoModel> _produtos;
        public ObservableCollection<ProdutoModel> Produtos
        {
            get { return _produtos; }
            set { _produtos = value; RaisePropertyChanged("Produtos"); }
        }
        private ProdutoModel _produto;
        public ProdutoModel Produto
        {
            get { return _produto; }
            set { _produto = value; RaisePropertyChanged("Produto"); }
        }

        private ObservableCollection<TabelaDescAdicionalModel> _descAdicionais;
        public ObservableCollection<TabelaDescAdicionalModel> DescAdicionais
        {
            get { return _descAdicionais; }
            set { _descAdicionais = value; RaisePropertyChanged("DescAdicionais"); }
        }
        private TabelaDescAdicionalModel _descAdicional;
        public TabelaDescAdicionalModel DescAdicional
        {
            get { return _descAdicional; }
            set { _descAdicional = value; RaisePropertyChanged("DescAdicional"); }
        }

        private ObservableCollection<TblComplementoAdicionalModel> _compleAdicionais;
        public ObservableCollection<TblComplementoAdicionalModel> CompleAdicionais
        {
            get { return _compleAdicionais; }
            set { _compleAdicionais = value; RaisePropertyChanged("CompleAdicionais"); }
        }
        private TblComplementoAdicionalModel _compledicional;
        public TblComplementoAdicionalModel Compledicional
        {
            get { return _compledicional; }
            set { _compledicional = value; RaisePropertyChanged("Compledicional"); }
        }

        private ProdutoServicoModel _produtoServico;
        public ProdutoServicoModel ProdutoServico
        {
            get { return _produtoServico; }
            set { _produtoServico = value; RaisePropertyChanged("ProdutoServico"); }
        }
        private ObservableCollection<ProdutoServicoModel> _produtoServicos;
        public ObservableCollection<ProdutoServicoModel> ProdutoServicos
        {
            get { return _produtoServicos; }
            set { _produtoServicos = value; RaisePropertyChanged("ProdutoServicos"); }
        }

        private RequisicaoModel _requisicao;
        public RequisicaoModel Requisicao
        {
            get { return _requisicao; }
            set { _requisicao = value; RaisePropertyChanged("Requisicao"); }
        }
        private ObservableCollection<RequisicaoModel> _requisicoes;
        public ObservableCollection<RequisicaoModel> Requisicoes
        {
            get { return _requisicoes; }
            set { _requisicoes = value; RaisePropertyChanged("Requisicoes"); }
        }

        private DetalheRequisicaoModel _requisicaoDetalhe;
        public DetalheRequisicaoModel RequisicaoDetalhe
        {
            get { return _requisicaoDetalhe; }
            set { _requisicaoDetalhe = value; RaisePropertyChanged("RequisicaoDetalhe"); }
        }
        private ObservableCollection<DetalheRequisicaoModel> _requisicaoDetalhes;
        public ObservableCollection<DetalheRequisicaoModel> RequisicaoDetalhes
        {
            get { return _requisicaoDetalhes; }
            set { _requisicaoDetalhes = value; RaisePropertyChanged("RequisicaoDetalhes"); }
        }

        private QryRequisicaoDetalheModel _qryRequisicaoDetalhe;
        public QryRequisicaoDetalheModel QryRequisicaoDetalhe
        {
            get { return _qryRequisicaoDetalhe; }
            set { _qryRequisicaoDetalhe = value; RaisePropertyChanged("QryRequisicaoDetalhe"); }
        }
        private ObservableCollection<QryRequisicaoDetalheModel> _qryRequisicaoDetalhes;
        public ObservableCollection<QryRequisicaoDetalheModel> QryRequisicaoDetalhes
        {
            get { return _qryRequisicaoDetalhes; }
            set { _qryRequisicaoDetalhes = value; RaisePropertyChanged("QryRequisicaoDetalhes"); }
        }

        private QryDescricao _descricao;
        public QryDescricao Descricao
        {
            get { return _descricao; }
            set { _descricao = value; RaisePropertyChanged("Descricao"); }
        }
        private ObservableCollection<QryDescricao> _descricoes;
        public ObservableCollection<QryDescricao> Descricoes
        {
            get { return _descricoes; }
            set { _descricoes = value; RaisePropertyChanged("Descricoes"); }
        }

        private ObservableCollection<ReqDetalhesModel> _reqDetalhes;
        public ObservableCollection<ReqDetalhesModel> ReqDetalhes
        {
            get { return _reqDetalhes; }
            set { _reqDetalhes = value; RaisePropertyChanged("ReqDetalhes"); }
        }

        public RequisicaoViewModel()
        {
            /*
            try
            {
                Task.Run(async () => await GetPlanilhasAsync());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            */

            Requisicao = new RequisicaoModel();
        }

        public async Task<QryDescricao> GetDescricaoAsync(long codcompladicional)
        {//e.FirstName.StartsWith(employeeName) || e.LastName.StartsWith(employeeName)
            try
            {
                using DatabaseContext db = new();
                return await db.Descricoes.Where(d => d.inativo.Equals("0") && d.codcompladicional == codcompladicional).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<RelplanModel>> GetPlanilhasAsync()
        {
            try
            {
                using DatabaseContext db = new();
                return new ObservableCollection<RelplanModel>(await db.Relplans.OrderBy(c => c.planilha).Where(c => c.ativo.Equals("1")).ToListAsync());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<ProdutoModel>> GetProdutosAsync(string? planilha)
        {
            try
            {
                Produtos = new ObservableCollection<ProdutoModel>();
                using DatabaseContext db = new();
                var data = await db.Produtos
                    .OrderBy(c => c.descricao)
                    .Where(c => c.planilha.Equals(planilha))
                    .Where(c => c.inativo != "-1")
                    .ToListAsync();
                //Produtos = new ObservableCollection<ProdutoModel>(data);

                return new ObservableCollection<ProdutoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<TabelaDescAdicionalModel>> GetDescAdicionaisAsync(long? codigo)
        {
            try
            {
                DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>();
                using DatabaseContext db = new();
                var data = await db.DescAdicionais
                    .OrderBy(c => c.descricao_adicional)
                    .Where(c => c.codigoproduto.Equals(codigo))
                    .Where(c => c.inativo != "-1")
                    .ToListAsync();
                //DescAdicionais = new ObservableCollection<TabelaDescAdicionalModel>(data);
                return new ObservableCollection<TabelaDescAdicionalModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<TblComplementoAdicionalModel>> GetCompleAdicionaisAsync(long? coduniadicional)
        {
            try
            {
                CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>();
                using DatabaseContext db = new();
                var data = await db.ComplementoAdicionais
                    .OrderBy(c => c.complementoadicional)
                    .Where(c => c.coduniadicional.Equals(coduniadicional))
                    .Where(c => c.inativo != "-1")
                    .ToListAsync();
                //CompleAdicionais = new ObservableCollection<TblComplementoAdicionalModel>(data);
                return new ObservableCollection<TblComplementoAdicionalModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<RequisicaoModel> GetByRequisicaoAsync(long? num_requisicao)
        {
            try
            {
                using DatabaseContext db = new();
                return await db.Requisicoes.FindAsync(num_requisicao); //Where(r => r.num_os_servico == num_os_servico).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<RequisicaoModel> GetRequisicaoAsync(long? num_os_servico)
        {
            try
            {
                using DatabaseContext db = new();
                return await db.Requisicoes.OrderBy(u => u.num_requisicao).Where(r => r.num_os_servico == num_os_servico).LastOrDefaultAsync();
                //await _context.TaskOrder.OrderBy(u => u.TaskOrderId).LastOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DetalheRequisicaoModel> AddProdutoRequisicaoAsync(DetalheRequisicaoModel requisicaoDetalhe)
        {
            try
            {
                using DatabaseContext db = new();
                /*db.Entry(requisicaoDetalhe).State = requisicaoDetalhe.cod_det_req == null ?
                                   EntityState.Added :
                                   EntityState.Modified;*/

                await db.RequisicaoDetalhes.SingleMergeAsync(requisicaoDetalhe);
                await db.SaveChangesAsync();
                return requisicaoDetalhe;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<ReqDetalhesModel>> GetByRequisicaoDetalhesAsync(long? num_requisicao)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ReqDetalhes.Where(r => r.num_requisicao == num_requisicao).ToListAsync();
                return new ObservableCollection<ReqDetalhesModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DetalheRequisicaoModel> GetItemRequisicaoAsync(long? codDetReq)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.RequisicaoDetalhes.FindAsync(codDetReq);
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<QryRequisicaoDetalheModel>> GetRequisicaoDetalhesAsync(long? num_requisicao)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.QryRequisicaoDetalhes.Where(r => r.num_requisicao == num_requisicao).ToListAsync();
                return new ObservableCollection<QryRequisicaoDetalheModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ChecklistPrdutoRequisicaoModel> GetPrdutoRequisicaoAsync(long? num_requisicao)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ChecklistPrdutooRequisicoes.Where(r => r.num_requisicao == num_requisicao).FirstOrDefaultAsync();
                return data;
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
