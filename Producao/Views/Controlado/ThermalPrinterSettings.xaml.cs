using System;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace Producao.Views.Controlado
{
    public partial class ThermalPrinterSettings : UserControl
    {
        public ThermalPrinterSettings()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var config = ThermalPrinterConfiguration.Load();
            IpAddressTextBox.Text = config.IpAddress;
            PortTextBox.Text = config.Port.ToString();
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (!TryReadConfiguration(out var config))
            {
                return;
            }

            config.Save();
            RadWindow.Alert("Configuração salva.");
        }

        private async void OnTestConnectionClick(object sender, RoutedEventArgs e)
        {
            if (!TryReadConfiguration(out var config))
            {
                return;
            }

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                using var client = new TcpClient();
                await client.ConnectAsync(config.IpAddress, config.Port);
                RadWindow.Alert("Conexão realizada com sucesso.");
            }
            catch (Exception ex)
            {
                RadWindow.Alert($"Não foi possível conectar na impressora térmica. {ex.Message}");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private bool TryReadConfiguration(out ThermalPrinterConfiguration config)
        {
            config = new ThermalPrinterConfiguration
            {
                IpAddress = IpAddressTextBox.Text?.Trim() ?? string.Empty
            };

            if (string.IsNullOrWhiteSpace(config.IpAddress))
            {
                RadWindow.Alert("Informe o IP ou nome da impressora.");
                return false;
            }

            if (!int.TryParse(PortTextBox.Text, out var port) || port <= 0 || port > 65535)
            {
                RadWindow.Alert("Informe uma porta válida.");
                return false;
            }

            config.Port = port;
            return true;
        }
    }
}
