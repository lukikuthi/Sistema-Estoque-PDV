using SistemaEstoquePDV.Helpers;
using SistemaEstoquePDV.Models;

namespace SistemaEstoquePDV.Services
{
    public class EstoqueService
    {
        private readonly SupabaseService _supabaseService;

        public EstoqueService(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task CadastrarProduto()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══ CADASTRAR NOVO PRODUTO ═══\n");
            Console.ForegroundColor = ConsoleColor.White;

            var produto = new Produto();

            Console.Write("Código do Produto: ");
            produto.Codigo = Console.ReadLine() ?? "";

            if (await ProdutoExiste(produto.Codigo))
            {
                MenuHelper.ExibirMensagem("Produto com este código já existe!", ConsoleColor.Red);
                return;
            }

            Console.Write("Nome do Produto: ");
            produto.Nome = Console.ReadLine() ?? "";

            Console.Write("Descrição (opcional): ");
            produto.Descricao = Console.ReadLine();

            Console.Write("Preço (R$): ");
            if (decimal.TryParse(Console.ReadLine(), out decimal preco))
                produto.Preco = preco;

            Console.Write("Quantidade em Estoque: ");
            if (int.TryParse(Console.ReadLine(), out int quantidade))
                produto.QuantidadeEstoque = quantidade;

            Console.Write("Estoque Mínimo: ");
            if (int.TryParse(Console.ReadLine(), out int minimo))
                produto.EstoqueMinimo = minimo;

            Console.Write("Categoria (opcional): ");
            produto.Categoria = Console.ReadLine();

            var resultado = await _supabaseService.Insert(produto);
            
            if (resultado != null)
            {
                MenuHelper.ExibirMensagem("✓ Produto cadastrado com sucesso!", ConsoleColor.Green);
            }
            else
            {
                MenuHelper.ExibirMensagem("✗ Erro ao cadastrar produto!", ConsoleColor.Red);
            }
        }

        public async Task ListarProdutos()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══ LISTA DE PRODUTOS ═══\n");
            Console.ForegroundColor = ConsoleColor.White;

            var produtos = await _supabaseService.Select<Produto>();

            if (!produtos.Any())
            {
                MenuHelper.ExibirMensagem("Nenhum produto cadastrado.", ConsoleColor.Yellow);
                return;
            }

            foreach (var produto in produtos.Where(p => p.Ativo))
            {
                produto.ExibirInfo();
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nTotal de produtos ativos: {produtos.Count(p => p.Ativo)}");
            MenuHelper.PausarTela();
        }

        public async Task EditarProduto()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══ EDITAR PRODUTO ═══\n");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write("Digite o código do produto para editar: ");
            var codigo = Console.ReadLine() ?? "";

            var produtos = await _supabaseService.Select<Produto>();
            var produto = produtos.FirstOrDefault(p => p.Codigo == codigo && p.Ativo);

            if (produto == null)
            {
                MenuHelper.ExibirMensagem("Produto não encontrado!", ConsoleColor.Red);
                return;
            }

            Console.WriteLine("\nProduto encontrado:");
            produto.ExibirInfo();

            Console.WriteLine("\nDeixe em branco para manter o valor atual:\n");

            Console.Write($"Nome ({produto.Nome}): ");
            var nome = Console.ReadLine();
            if (!string.IsNullOrEmpty(nome)) produto.Nome = nome;

            Console.Write($"Descrição ({produto.Descricao}): ");
            var descricao = Console.ReadLine();
            if (!string.IsNullOrEmpty(descricao)) produto.Descricao = descricao;

            Console.Write($"Preço ({produto.Preco:F2}): ");
            var precoStr = Console.ReadLine();
            if (!string.IsNullOrEmpty(precoStr) && decimal.TryParse(precoStr, out decimal preco))
                produto.Preco = preco;

            Console.Write($"Estoque Mínimo ({produto.EstoqueMinimo}): ");
            var minimoStr = Console.ReadLine();
            if (!string.IsNullOrEmpty(minimoStr) && int.TryParse(minimoStr, out int minimo))
                produto.EstoqueMinimo = minimo;

            Console.Write($"Categoria ({produto.Categoria}): ");
            var categoria = Console.ReadLine();
            if (!string.IsNullOrEmpty(categoria)) produto.Categoria = categoria;

            produto.DataAtualizacao = DateTime.Now;

            var resultado = await _supabaseService.Update(produto);
            
            if (resultado != null)
            {
                MenuHelper.ExibirMensagem("✓ Produto atualizado com sucesso!", ConsoleColor.Green);
            }
            else
            {
                MenuHelper.ExibirMensagem("✗ Erro ao atualizar produto!", ConsoleColor.Red);
            }
        }

        public async Task RemoverProduto()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══ REMOVER PRODUTO ═══\n");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write("Digite o código do produto para remover: ");
            var codigo = Console.ReadLine() ?? "";

            var produtos = await _supabaseService.Select<Produto>();
            var produto = produtos.FirstOrDefault(p => p.Codigo == codigo && p.Ativo);

            if (produto == null)
            {
                MenuHelper.ExibirMensagem("Produto não encontrado!", ConsoleColor.Red);
                return;
            }

            Console.WriteLine("\nProduto a ser removido:");
            produto.ExibirInfo();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\nConfirma a remoção? (S/N): ");
            var confirmacao = Console.ReadLine()?.ToUpper();

            if (confirmacao == "S")
            {
                produto.Ativo = false;
                produto.DataAtualizacao = DateTime.Now;
                
                var resultado = await _supabaseService.Update(produto);
                
                if (resultado != null)
                {
                    MenuHelper.ExibirMensagem("✓ Produto removido com sucesso!", ConsoleColor.Green);
                }
                else
                {
                    MenuHelper.ExibirMensagem("✗ Erro ao remover produto!", ConsoleColor.Red);
                }
            }
            else
            {
                MenuHelper.ExibirMensagem("Operação cancelada.", ConsoleColor.Yellow);
            }
        }

        public async Task AtualizarEstoque()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══ ATUALIZAR ESTOQUE ═══\n");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write("Digite o código do produto: ");
            var codigo = Console.ReadLine() ?? "";

            var produtos = await _supabaseService.Select<Produto>();
            var produto = produtos.FirstOrDefault(p => p.Codigo == codigo && p.Ativo);

            if (produto == null)
            {
                MenuHelper.ExibirMensagem("Produto não encontrado!", ConsoleColor.Red);
                return;
            }

            Console.WriteLine("\nProduto encontrado:");
            produto.ExibirInfo();

            Console.WriteLine("\n[1] Entrada de Estoque");
            Console.WriteLine("[2] Saída de Estoque");
            Console.WriteLine("[3] Definir Quantidade Exata");
            Console.Write("\nEscolha uma opção: ");

            var opcao = Console.ReadLine();
            int novaQuantidade = produto.QuantidadeEstoque;

            switch (opcao)
            {
                case "1":
                    Console.Write("Quantidade a adicionar: ");
                    if (int.TryParse(Console.ReadLine(), out int adicionar))
                        novaQuantidade += adicionar;
                    break;

                case "2":
                    Console.Write("Quantidade a remover: ");
                    if (int.TryParse(Console.ReadLine(), out int remover))
                    {
                        novaQuantidade -= remover;
                        if (novaQuantidade < 0) novaQuantidade = 0;
                    }
                    break;

                case "3":
                    Console.Write("Nova quantidade: ");
                    if (int.TryParse(Console.ReadLine(), out int nova))
                        novaQuantidade = nova;
                    break;

                default:
                    MenuHelper.ExibirMensagem("Opção inválida!", ConsoleColor.Red);
                    return;
            }

            produto.QuantidadeEstoque = novaQuantidade;
            produto.DataAtualizacao = DateTime.Now;

            var resultado = await _supabaseService.Update(produto);
            
            if (resultado != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✓ Estoque atualizado! Nova quantidade: {novaQuantidade}");
                
                if (novaQuantidade <= produto.EstoqueMinimo)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("⚠️ ATENÇÃO: Estoque abaixo do mínimo!");
                }
            }
            else
            {
                MenuHelper.ExibirMensagem("✗ Erro ao atualizar estoque!", ConsoleColor.Red);
            }

            MenuHelper.PausarTela();
        }

        public async Task BuscarProduto()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══ BUSCAR PRODUTO ═══\n");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("[1] Buscar por Código");
            Console.WriteLine("[2] Buscar por Nome");
            Console.Write("\nEscolha uma opção: ");

            var opcao = Console.ReadLine();
            var produtos = await _supabaseService.Select<Produto>();

            switch (opcao)
            {
                case "1":
                    Console.Write("Digite o código: ");
                    var codigo = Console.ReadLine() ?? "";
                    var produtoPorCodigo = produtos.FirstOrDefault(p => p.Codigo.Contains(codigo) && p.Ativo);
                    
                    if (produtoPorCodigo != null)
                    {
                        Console.WriteLine("\nProduto encontrado:");
                        produtoPorCodigo.ExibirInfo();
                    }
                    else
                    {
                        MenuHelper.ExibirMensagem("Produto não encontrado!", ConsoleColor.Red);
                    }
                    break;

                case "2":
                    Console.Write("Digite o nome ou parte do nome: ");
                    var nome = Console.ReadLine() ?? "";
                    var produtosPorNome = produtos.Where(p => p.Nome.ToLower().Contains(nome.ToLower()) && p.Ativo).ToList();
                    
                    if (produtosPorNome.Any())
                    {
                        Console.WriteLine($"\n{produtosPorNome.Count} produto(s) encontrado(s):\n");
                        foreach (var produto in produtosPorNome)
                        {
                            produto.ExibirInfo();
                        }
                    }
                    else
                    {
                        MenuHelper.ExibirMensagem("Nenhum produto encontrado!", ConsoleColor.Red);
                    }
                    break;

                default:
                    MenuHelper.ExibirMensagem("Opção inválida!", ConsoleColor.Red);
                    break;
            }

            MenuHelper.PausarTela();
        }

        public async Task ProdutosBaixoEstoque()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══ PRODUTOS EM BAIXO ESTOQUE ═══\n");
            Console.ForegroundColor = ConsoleColor.White;

            var produtos = await _supabaseService.Select<Produto>();
            var produtosBaixoEstoque = produtos.Where(p => p.QuantidadeEstoque <= p.EstoqueMinimo && p.Ativo).ToList();

            if (!produtosBaixoEstoque.Any())
            {
                MenuHelper.ExibirMensagem("✓ Nenhum produto com estoque baixo!", ConsoleColor.Green);
                return;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"⚠️ {produtosBaixoEstoque.Count} produto(s) com estoque baixo:\n");
            Console.ForegroundColor = ConsoleColor.White;

            foreach (var produto in produtosBaixoEstoque)
            {
                produto.ExibirInfo();
            }

            MenuHelper.PausarTela();
        }

        private async Task<bool> ProdutoExiste(string codigo)
        {
            var produtos = await _supabaseService.Select<Produto>();
            return produtos.Any(p => p.Codigo == codigo && p.Ativo);
        }
    }
}