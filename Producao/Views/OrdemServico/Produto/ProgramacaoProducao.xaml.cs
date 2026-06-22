using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Producao.DataBase.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Producao.Views.OrdemServico.Produto
{
    /// <summary>
    /// Interação lógica para ProgramacaoProducao.xam
    /// </summary>
    public partial class ProgramacaoProducao : UserControl
    {
        DataBaseSettings BaseSettings = DataBaseSettings.Instance;

        public ProgramacaoProducao()
        {
            InitializeComponent();
            DataContext = new ProgramacaoProducaoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ProgramacaoProducaoViewModel vm = (ProgramacaoProducaoViewModel)DataContext;
                vm.Locais = await vm.GetLocalicacoesAsync();
                vm.Programacoes = await vm.GetProgramacaoItensAsync();

                txtFila.Text = vm.Programacoes.Where(p => p.programacao_status == "FILA/M.O").Count().ToString(); //DCount("[num_os]", "qry_programacao_producao_global_producao", "[programacao_status] = 'FILA/M.O'")
                txtDiretoria.Text = vm.Programacoes.Where(p => p.programacao_status == "ESPAÇO FÍSICO").Count().ToString();//DCount("[num_os]", "qry_programacao_producao_global_producao", "[programacao_status] = 'ESPAÇO FÍSICO'")
                txtProjeto.Text = vm.Programacoes.Where(p => p.programacao_status == "EMBALAGEM/EXPEDIÇÃO").Count().ToString();//DCount("[num_os]", "qry_programacao_producao_global_producao", "[programacao_status] = 'EMBALAGEM/EXPEDIÇÃO'")
                txtAndamento.Text = vm.Programacoes.Where(p => p.programacao_status == "EM ANDAMENTO").Count().ToString();//DCount("[num_os]", "qry_programacao_producao_global_producao", "[programacao_status] = 'EM ANDAMENTO'")
                txtIndefinido.Text = vm.Programacoes.Where(p => p.programacao_status == "INDEFINIDO").Count().ToString();//DCount("[num_os]", "qry_programacao_producao_global_producao", "[programacao_status] = 'INDEFINIDO'")
                txtProjetos.Text = vm.Programacoes.Where(p => p.programacao_status == "PROJETOS").Count().ToString();//DCount("[num_os]", "qry_programacao_producao_global_producao", "[programacao_status] = 'PROJETOS'")
                txtFaltaMaterialTranf.Text = vm.Programacoes.Where(p => p.programacao_status == "FALTA MATERIAL INTERNO / TRANSF").Count().ToString();//DCount("[num_os]", "qry_programacao_producao_global_producao", "[programacao_status] = 'FALTA MATERIAL INTERNO / TRANSF'")
                txtFaltaMaterialCompras.Text = vm.Programacoes.Where(p => p.programacao_status == "FALTA MATERIAL EXTERNO / COMPRAS").Count().ToString();//DCount("[num_os]", "qry_programacao_producao_global_producao", "[programacao_status] = 'FALTA MATERIAL EXTERNO / COMPRAS'")


                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void LocaisSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (RadComboBox)sender;
            var local = (SetorProducaoModel)comboBox.SelectedItem;
            if (local is null)
            {
                return;
            }

            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ProgramacaoProducaoViewModel vm = (ProgramacaoProducaoViewModel)DataContext;
                vm.Setores = await vm.GetSetorsAsync(local.localizacao);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private void SetoresSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //SelectedItems = Count = 2
            var comboBox = (RadComboBox)sender;
            var setor = comboBox.SelectedItem as SetorModel;
        }

        private void OnAddNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {

        }

        private void OnRowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            //UpdateProgramacaoAsync(ProdutoServicoModel produtoServico)

        }

        private async void OnRowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            try
            {
                if (e.EditOperationType == GridViewEditOperationType.None || e.Row.Item is not ProgramacaoProducaoModel data)
                {
                    return;
                }

                ProgramacaoProducaoViewModel vm = (ProgramacaoProducaoViewModel)DataContext;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                await vm.UpdateProgramacaoAsync(
                        new TGlobalModel
                        {
                            num_os = data.num_os, 
                            programacao_ordem = data.programacao_ordem,
                            programacao_observacao = data.programacao_observacao,
                            programacao_inserido_por = Environment.UserName, 
                            programacao_inserido_data = DateTime.Now
                        })
                    ;
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnPrintClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ProgramacaoProducaoViewModel vm = (ProgramacaoProducaoViewModel)DataContext;
                var filteredResult = programacao.Items.OfType<ProgramacaoProducaoModel>().ToList();
                var itens = filteredResult.Count;

                var filePath = BaseSettings.ResolveImpressosPath("PROGRAMACAO_PROGRAMACAO_MODELO.xlsx");
                using var workbook = new XLWorkbook(BaseSettings.ResolveModeloPath("PROGRAMACAO_PROGRAMACAO_MODELO.xlsx"));
                var worksheet = workbook.Worksheet(1);

                int _l = 9;

                foreach (ProgramacaoProducaoModel item in filteredResult)
                {

                    ApplyHeaderStyle(worksheet.Cell(_l, 2));
                    worksheet.Cell(_l, 2).Value = item.programacao_ordem.GetValueOrDefault(); //item.programacao_ordem.Value;

                    ApplyHeaderStyle(worksheet.Cell(_l, 3));
                    worksheet.Cell(_l, 3).Value = item.data_de_expedicao.GetValueOrDefault();
                    
                    ApplyHeaderStyle(worksheet.Cell(_l, 4));
                    worksheet.Cell(_l, 4).Value = item.cliente_os ?? string.Empty;
                    
                    ApplyHeaderStyle(worksheet.Cell(_l, 5));
                    worksheet.Cell(_l, 5).Value = item.cod_compl_adicional.GetValueOrDefault();
                    
                    ApplyHeaderStyle(worksheet.Cell(_l, 6));
                    worksheet.Cell(_l, 6).Value = item.planilha ?? string.Empty;
                    
                    var descricaoRange = worksheet.Range(_l, 7, _l, 12).Merge();
                    ApplyHeaderStyle(descricaoRange);
                    descricaoRange.Value = item.descricao_completa ?? string.Empty;
                    worksheet.Row(_l).Height = 26;

                    ApplyHeaderStyle(worksheet.Cell(_l, 13));
                    worksheet.Cell(_l, 13).Value = item.num_os.GetValueOrDefault();
                    
                    ApplyHeaderStyle(worksheet.Cell(_l, 14));
                    worksheet.Cell(_l, 14).Value = item.quantidade_os.GetValueOrDefault();

                    var statusRange = worksheet.Range(_l, 15, _l, 16).Merge();
                    ApplyHeaderStyle(statusRange);
                    statusRange.Value = item.programacao_status ?? string.Empty;

                    var observacaoRange = worksheet.Range(_l, 17, _l, 18).Merge();
                    ApplyHeaderStyle(observacaoRange);
                    observacaoRange.Value = item.programacao_observacao ?? string.Empty;

                    //worksheet.Range[$"R{_l}:S{_l}"].Merge();
                    ApplyHeaderStyle(worksheet.Cell(_l, 19));
                    worksheet.Cell(_l, 19).Value = item.meta_peca_hora ?? string.Empty;
                    
                    ApplyHeaderStyle(worksheet.Cell(_l, 20));
                    worksheet.Cell(_l, 20).Value = item.ht.GetValueOrDefault();

                    _l++;
                }

                workbook.SaveAs(filePath);
                
                Process.Start(new ProcessStartInfo(filePath)
                {
                    UseShellExecute = true
                });
                
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }

        }

        private static void ApplyHeaderStyle(IXLCell cell)
        {
            ApplyHeaderStyle(cell.Style);
        }

        private static void ApplyHeaderStyle(IXLRange range)
        {
            ApplyHeaderStyle(range.Style);
        }

        private static void ApplyHeaderStyle(IXLStyle style)
        {
            style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            style.Border.InsideBorder = XLBorderStyleValues.Thin;
            style.Border.OutsideBorderColor = XLColor.FromArgb(191, 191, 191);
            style.Border.InsideBorderColor = XLColor.FromArgb(191, 191, 191);
            style.Font.FontSize = 8;
            style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            style.Alignment.WrapText = true;
        }
    }

    class ProgramacaoProducaoViewModel : INotifyPropertyChanged
    {
        private SetorModel _setor;
        public SetorModel Setor
        {
            get { return _setor; }
            set { _setor = value; RaisePropertyChanged("Setor"); }
        }
        private ObservableCollection<SetorModel> _setores;
        public ObservableCollection<SetorModel> Setores
        {
            get { return _setores; }
            set { _setores = value; RaisePropertyChanged("Setores"); }
        }

        private SetorProducaoModel _local;
        public SetorProducaoModel Local
        {
            get { return _local; }
            set { _local = value; RaisePropertyChanged("Local"); }
        }
        private ObservableCollection<SetorProducaoModel> _locais;
        public ObservableCollection<SetorProducaoModel> Locais
        {
            get { return _locais; }
            set { _locais = value; RaisePropertyChanged("Locais"); }
        }

        private ProgramacaoProducaoModel _programacao;
        public ProgramacaoProducaoModel Programacao
        {
            get { return _programacao; }
            set { _programacao = value; RaisePropertyChanged("Programacao"); }
        }
        private ObservableCollection<ProgramacaoProducaoModel> _programacoes;
        public ObservableCollection<ProgramacaoProducaoModel> Programacoes
        {
            get { return _programacoes; }
            set { _programacoes = value; RaisePropertyChanged("Programacoes"); }
        }

        private ObservableCollection<string> _distribuirOS =
        [
            "FILA/M.O",
            "EM ANDAMENTO",
            "EMBALAGEM/EXPEDIÇÃO",
            "ESPAÇO FÍSICO",
            "INDEFINIDO",
            "PROJETOS",
            "FALTA MATERIAL INTERNO / TRANSF",
            "FALTA MATERIAL EXTERNO / COMPRAS"
        ];
        public ObservableCollection<string> DistribuirOS
        {
            get { return _distribuirOS; }
            set { _distribuirOS = value; RaisePropertyChanged("DistribuirOS"); }
        }

        public async Task<ObservableCollection<SetorProducaoModel>> GetLocalicacoesAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.SetorProducaos
                    .GroupBy(p => p.localizacao)
                    .Select(g => g.OrderBy(p => p.localizacao).FirstOrDefault())
                    .ToListAsync();

                return new ObservableCollection<SetorProducaoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<SetorModel>> GetSetorsAsync(string localizacao)
        {
            try
            {
                using DatabaseContext db = new();
                var data = await (from s in db.SetorProducaos orderby s.setor where s.inativo == "0    " && s.localizacao == localizacao select new SetorModel { setor = s.setor + " - " + s.galpao, codigo_setor = s.codigo_setor }).ToListAsync();
                return new ObservableCollection<SetorModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<ProgramacaoProducaoModel>> GetProgramacaoItensAsync()
        {
            try
            {
                using DatabaseContext db = new();
                var data = await db.ProgramacaoProducoes
                    .Where(p => p.quantidade_os > 0)
                    .ToListAsync();

                return new ObservableCollection<ProgramacaoProducaoModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateProgramacaoAsync(TGlobalModel global)
        {
            try
            {
                using DatabaseContext db = new();
                TGlobalModel servico = await db.Globais.FindAsync(global.num_os);
                servico.programacao_ordem = global.programacao_ordem;
                servico.programacao_observacao = global.programacao_observacao;
                servico.programacao_inserido_por = global.programacao_inserido_por;
                servico.programacao_inserido_data = global.programacao_inserido_data;

                await db.SaveChangesAsync();
                //await db.Globais.SingleUpdateAsync(servico);
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

    public class ProgramacaoProducaoColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //value == null || ((value != null) && double.Parse(value.ToString()) == 0)
            var data = value as ProgramacaoProducaoModel;
            if (data?.dias_expedicao < 0)
                return new SolidColorBrush(Colors.Red);
            //else if (data?.condicao == 2)
                //return new SolidColorBrush(Colors.LightGreen);
            //else if (data?.condicao == 3)
                //return new SolidColorBrush(Colors.LightSkyBlue);
            else
                return DependencyProperty.UnsetValue;
            //return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
