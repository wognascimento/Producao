using Dapper;
using Npgsql;
using Producao.DataBase.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace Producao.Views.kit
{
    /// <summary>
    /// Interação lógica para ViewControleGeralSolicitacao.xam
    /// </summary>
    public partial class ViewControleGeralSolicitacao : UserControl
    {
        public ViewControleGeralSolicitacao()
        {
            InitializeComponent();
            DataContext = new ControleGeralSolicitacaoViewModel();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                ControleGeralSolicitacaoViewModel vm = (ControleGeralSolicitacaoViewModel)DataContext;
                vm.Controles = await vm.GetControlesAsync();
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }

        private void OnRowValidating(object sender, GridViewRowValidatingEventArgs e)
        {

        }

        private async void OnRowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            try
            {
                ControleGeralSolicitacaoViewModel vm = (ControleGeralSolicitacaoViewModel)DataContext;

                if (e.Row.Item is not ControleSolicaoGeralModel data)
                    return;

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                await vm.AddControleAsync(new ControleEnvioModel
                {
                    coddetalhescompl = data.coddetalhescompl,
                    data_envio = data.data_envio,
                    local_galpao = "JACAREÍ",
                    status = data.status,
                    placa = data.placa,
                    motorista = data.motorista,
                    horario_saida = data.horario_saida,
                    ordem = data.ordem,
                });

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
            catch (Exception ex)
            {
                Producao.ErrorDialog.Show(ex, "Erro");
                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
            }
        }
    }

    public class ControleGeralSolicitacaoViewModel : INotifyPropertyChanged
    {
        private static NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(DataBaseSettings.Instance.ConnectionString);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private ObservableCollection<ControleSolicaoGeralModel> _controles;
        public ObservableCollection<ControleSolicaoGeralModel> Controles
        {
            get { return _controles; }
            set { _controles = value; RaisePropertyChanged("Controles"); }
        }

        private ControleSolicaoGeralModel _controle;
        public ControleSolicaoGeralModel Controle
        {
            get { return _controle; }
            set { _controle = value; RaisePropertyChanged("Controle"); }
        }

        public async Task<ObservableCollection<ControleSolicaoGeralModel>> GetControlesAsync()
        {
            try
            {
                using var conn = CreateConnection();
                var data = await conn.QueryAsync<ControleSolicaoGeralModel>(
                    @"SELECT *
                      FROM kitsolucao.query_controle_solicao_geral;");
                return new ObservableCollection<ControleSolicaoGeralModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddControleAsync(ControleEnvioModel? controle)
        {
            try
            {
                if (controle?.coddetalhescompl == null)
                    return;

                using var conn = CreateConnection();
                await conn.ExecuteAsync(
                    @"INSERT INTO kitsolucao.tbl_controle_envio
                        (coddetalhescompl, data_envio, local_galpao, status, placa, motorista, horario_saida, ordem)
                      VALUES
                        (@coddetalhescompl, @data_envio, @local_galpao, @status, @placa, @motorista, @horario_saida, @ordem)
                      ON CONFLICT (coddetalhescompl) DO UPDATE SET
                        data_envio = EXCLUDED.data_envio,
                        local_galpao = EXCLUDED.local_galpao,
                        status = EXCLUDED.status,
                        placa = EXCLUDED.placa,
                        motorista = EXCLUDED.motorista,
                        horario_saida = EXCLUDED.horario_saida,
                        ordem = EXCLUDED.ordem;",
                    controle);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
