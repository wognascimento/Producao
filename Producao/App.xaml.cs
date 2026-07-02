using Dapper;
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
            RegistrarHandlersDapper();
            StyleManager.ApplicationTheme = new FluentTheme();
            AplicarCulturaPadrao();

            LocalizationManager.Manager = new LocalizationManager
            {
                ResourceManager = GridViewResources.ResourceManager
            };

            DispatcherUnhandledException += OnDispatcherUnhandledException;
            System.AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private static void RegistrarHandlersDapper()
        {
            SqlMapper.AddTypeHandler(new Utils.DateOnlyToDateTimeHandler());
            SqlMapper.AddTypeHandler(new Utils.DateOnlyToNullableDateTimeHandler());
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

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ErrorDialog.Show(e.Exception, "Erro inesperado");
            e.Handled = true;
        }

        private void OnUnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is System.Exception ex)
                ErrorDialog.Show(ex, "Erro critico", MessageBoxImage.Stop);
        }
    }
}
