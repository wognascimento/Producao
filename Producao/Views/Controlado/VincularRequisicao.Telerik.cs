using System;
using System.Windows;

namespace Producao.Views.Controlado
{
    public partial class VincularRequisicao
    {
        private async void OnRemoverProdutoClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is null)
            {
                return;
            }

            dynamic vm = DataContext;
            if (vm.Produto is null)
            {
                return;
            }

            var confirmacao = MessageBox.Show(
                "Deseja remover este produto da requisição?",
                "Remover produto",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmacao != MessageBoxResult.Yes)
            {
                return;
            }

            await vm.DeleteControladoAsync((long)vm.Produto.num_requisicao, (string)vm.Produto.barcode);

            if (long.TryParse(txtRequisicao.Text, out var requisicao))
            {
                vm.Produtos = await vm.GetProdutosAsync(requisicao);
            }
        }
    }
}
