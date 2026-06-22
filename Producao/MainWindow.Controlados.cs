using Producao.Views.Controlado;

namespace Producao
{
    public partial class MainWindow
    {
        private void OnConfigurarImpressoraTermica(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            adicionarFilho(new ThermalPrinterSettings(), "IMPRESSORA TÉRMICA", "IMPRESSORA_TERMICA");
        }
    }
}
