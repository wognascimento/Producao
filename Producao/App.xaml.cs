using Dapper;
using Producao.Localization;
using System.Globalization;
using System.Threading;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Data;

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
            RegistrarPadraoFiltroRadGridView();
            StyleManager.ApplicationTheme = new Office2016Theme();
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

        private static void RegistrarPadraoFiltroRadGridView()
        {
            EventManager.RegisterClassHandler(
                typeof(RadGridView),
                FrameworkElement.LoadedEvent,
                new RoutedEventHandler(OnRadGridViewLoaded));
        }

        private static void OnRadGridViewLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is not RadGridView grid)
                return;

            grid.FilterOperatorsLoading -= OnRadGridViewFilterOperatorsLoading;
            grid.FilterOperatorsLoading += OnRadGridViewFilterOperatorsLoading;
        }

        private static void OnRadGridViewFilterOperatorsLoading(object sender, FilterOperatorsLoadingEventArgs e)
        {
            if (!e.AvailableOperators.Contains(FilterOperator.Contains))
                return;

            e.DefaultOperator1 = FilterOperator.Contains;
            e.DefaultOperator2 = FilterOperator.Contains;
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
