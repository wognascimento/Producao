using System;
using System.Collections.Generic;
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

namespace Producao.Views.CadastroProduto
{
    /// <summary>
    /// Interação lógica para CadastroDescricaoEspanhol.xam
    /// </summary>
    public partial class CadastroDescricaoEspanhol : UserControl
    {
        public CadastroDescricaoEspanhol()
        {
            InitializeComponent();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow)._mdi.Items.Remove(this);
        }
    }
}
