using SistemaEstoquePDV.Helpers;
using SistemaEstoquePDV.Services;

namespace SistemaEstoquePDV
{
    internal class Program
    {
        private static SupabaseService? _supabaseService;
        private static EstoqueService? _estoqueService;
        private static PDVService? _pdvService;

        static async Task Main(string[] args)
        {
            Console.Title = "Sistema de Estoque e PDV";
            Console.ForegroundColor = ConsoleColor.Cyan;
            
            try
            {
                // Inicializar serviços
                await InicializarServicos();
                
                // Menu principal
                await ExibirMenuPrincipal();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Erro crítico: {ex.Message}");
                Console.WriteLine("Pressione qualquer tecla para sair...");
                Console.ReadKey();
            }
        }

        private static async Task InicializarServicos()
        {
            Console.WriteLine("=== SISTEMA DE ESTOQUE E PDV ===");
            Console.WriteLine("Inicializando conexão com Supabase...");
            
            _supabaseService = new SupabaseService();
            await _supabaseService.Initialize();
            
            _estoqueService = new EstoqueService(_supabaseService);
            _pdvService = new PDVService(_supabaseService);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ Conexão estabelecida com sucesso!");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
            Console.Clear();
        }

        private static async Task ExibirMenuPrincipal()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║          SISTEMA ESTOQUE & PDV         ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.ForegroundColor = ConsoleColor.White;
                
                Console.WriteLine("\n📦 [1] Gerenciar Estoque");
                Console.WriteLine("💰 [2] Ponto de Venda (PDV)");
                Console.WriteLine("📊 [3] Relatórios");
                Console.WriteLine("❌ [0] Sair");
                
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("\nEscolha uma opção: ");
                
                var opcao = Console.ReadLine();
                
                switch (opcao)
                {
                    case "1":
                        await MenuEstoque();
                        break;
                    case "2":
                        await MenuPDV();
                        break;
                    case "3":
                        await MenuRelatorios();
                        break;
                    case "0":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nObrigado por usar o Sistema de Estoque e PDV!");
                        return;
                    default:
                        MenuHelper.ExibirMensagem("Opção inválida! Tente novamente.", ConsoleColor.Red);
                        break;
                }
            }
        }

        private static async Task MenuEstoque()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║           GERENCIAR ESTOQUE            ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.ForegroundColor = ConsoleColor.White;
                
                Console.WriteLine("\n➕ [1] Cadastrar Produto");
                Console.WriteLine("📋 [2] Listar Produtos");
                Console.WriteLine("✏️  [3] Editar Produto");
                Console.WriteLine("🗑️  [4] Remover Produto");
                Console.WriteLine("📈 [5] Atualizar Estoque");
                Console.WriteLine("🔍 [6] Buscar Produto");
                Console.WriteLine("⬅️  [0] Voltar ao Menu Principal");
                
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("\nEscolha uma opção: ");
                
                var opcao = Console.ReadLine();
                
                switch (opcao)
                {
                    case "1":
                        await _estoqueService!.CadastrarProduto();
                        break;
                    case "2":
                        await _estoqueService!.ListarProdutos();
                        break;
                    case "3":
                        await _estoqueService!.EditarProduto();
                        break;
                    case "4":
                        await _estoqueService!.RemoverProduto();
                        break;
                    case "5":
                        await _estoqueService!.AtualizarEstoque();
                        break;
                    case "6":
                        await _estoqueService!.BuscarProduto();
                        break;
                    case "0":
                        return;
                    default:
                        MenuHelper.ExibirMensagem("Opção inválida! Tente novamente.", ConsoleColor.Red);
                        break;
                }
            }
        }

        private static async Task MenuPDV()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║            PONTO DE VENDA              ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.ForegroundColor = ConsoleColor.White;
                
                Console.WriteLine("\n🛒 [1] Nova Venda");
                Console.WriteLine("📋 [2] Vendas do Dia");
                Console.WriteLine("🔍 [3] Consultar Venda");
                Console.WriteLine("❌ [4] Cancelar Venda");
                Console.WriteLine("⬅️  [0] Voltar ao Menu Principal");
                
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("\nEscolha uma opção: ");
                
                var opcao = Console.ReadLine();
                
                switch (opcao)
                {
                    case "1":
                        await _pdvService!.NovaVenda();
                        break;
                    case "2":
                        await _pdvService!.VendasDoDia();
                        break;
                    case "3":
                        await _pdvService!.ConsultarVenda();
                        break;
                    case "4":
                        await _pdvService!.CancelarVenda();
                        break;
                    case "0":
                        return;
                    default:
                        MenuHelper.ExibirMensagem("Opção inválida! Tente novamente.", ConsoleColor.Red);
                        break;
                }
            }
        }

        private static async Task MenuRelatorios()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║              RELATÓRIOS                ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.ForegroundColor = ConsoleColor.White;
                
                Console.WriteLine("\n📊 [1] Produtos em Baixo Estoque");
                Console.WriteLine("💰 [2] Vendas por Período");
                Console.WriteLine("🏆 [3] Produtos Mais Vendidos");
                Console.WriteLine("📈 [4] Resumo Financeiro");
                Console.WriteLine("⬅️  [0] Voltar ao Menu Principal");
                
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("\nEscolha uma opção: ");
                
                var opcao = Console.ReadLine();
                
                switch (opcao)
                {
                    case "1":
                        await _estoqueService!.ProdutosBaixoEstoque();
                        break;
                    case "2":
                        await _pdvService!.VendasPorPeriodo();
                        break;
                    case "3":
                        await _pdvService!.ProdutosMaisVendidos();
                        break;
                    case "4":
                        await _pdvService!.ResumoFinanceiro();
                        break;
                    case "0":
                        return;
                    default:
                        MenuHelper.ExibirMensagem("Opção inválida! Tente novamente.", ConsoleColor.Red);
                        break;
                }
            }
        }
    }
}