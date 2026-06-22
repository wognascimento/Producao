using Producao.Localization;
using System.Globalization;
using System.Threading;
using System.Windows;
using Telerik.Windows.Controls;

namespace Producao
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private readonly DataBaseSettings BaseSettings = DataBaseSettings.Instance;
        public App()
        {
            BaseSettings.LoadFromConfiguration();
            StyleManager.ApplicationTheme = new Windows11Theme();
            AplicarCulturaPadrao();

            if (!string.IsNullOrWhiteSpace(BaseSettings.SyncfusionLicense))
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(BaseSettings.SyncfusionLicense);

            LocalizationManager.Manager = new LocalizationManager
            {
                ResourceManager = GridViewResources.ResourceManager
            };
        }

        private static void AplicarCulturaPadrao()
        {
            var culture = CultureInfo.GetCultureInfo("pt-BR");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            GridViewResources.Culture = culture;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Verificação de atualização em segundo plano
            //await CheckForUpdatesAsync();
        }
    }
}
