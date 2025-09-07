using SistemaEstoquePDV.Helpers;
using SistemaEstoquePDV.Models;

namespace SistemaEstoquePDV.Services
{
    public class PDVService
    {
        private readonly SupabaseService _supabaseService;

        public PDVService(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task NovaVenda()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══ NOVA VENDA ═══\n");
            Console.ForegroundColor = ConsoleColor.White;

            var venda = new Venda
            {
                NumeroVenda = GerarNumeroVenda(),
                DataVenda = DateTime.Now
            };

            var itens = new List<ItemVenda>();
            decimal valorTotal = 0;

            Console.WriteLine($"Venda #{venda.NumeroVenda}");
            Console.WriteLine($"Data: {venda.DataVenda:dd/MM/yyyy HH:mm}\n");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=== ADICIONAR ITEM ===");
                Console.ForegroundColor = ConsoleColor.White;
                
                Console.Write("Código do produto (ou 'F' para finalizar): ");
                var codigo = Console.ReadLine()?.ToUpper();

                if (codigo == "F") break;

                var produtos = await _supabaseService.Select<Produto>();
                var produto = produtos.FirstOrDefault(p => p.Codigo == codigo && p.Ativo);

                if (produto == null)
                {
                    MenuHelper.ExibirMensagem("Produto não encontrado!", ConsoleColor.Red);
                    continue;
                }

                Console.WriteLine($"Produto: {produto.Nome} - R$ {produto.Preco:F2}");
                Console.WriteLine($"Estoque disponível: {produto.QuantidadeEstoque}");

                Console.Write("Quantidade: ");
                if (!int.TryParse(Console.ReadLine(), out int quantidade) || quantidade <= 0)
                {
                    MenuHelper.ExibirMensagem("Quantidade inválida!", ConsoleColor.Red);
                    continue;
                }

                if (quantidade > produto.QuantidadeEstoque)
                {
                    MenuHelper.ExibirMensagem("Quantidade indisponível em estoque!", ConsoleColor.Red);
                    continue;
                }

                var item = new ItemVenda
                {
                    ProdutoId = produto.Id,
                    Quantidade = quantidade,
                    PrecoUnitario = produto.Preco,
                    Subtotal = quantidade * produto.Preco,
                    Produto = produto
                };

                itens.Add(item);
                valorTotal += item.Subtotal;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✓ Item adicionado! Subtotal: R$ {item.Subtotal:F2}");
                Console.WriteLine($"Total atual: R$ {valorTotal:F2}\n");
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (!itens.Any())
            {
                MenuHelper.ExibirMensagem("Venda cancelada - nenhum item adicionado.", ConsoleColor.Yellow);
                return;
            }

            // Resumo da venda
            ExibirResumoVenda(itens, valorTotal);

            // Aplicar desconto
            Console.Write("Desconto (R$ ou %): ");
            var descontoStr = Console.ReadLine() ?? "";
            decimal desconto = 0;

            if (!string.IsNullOrEmpty(descontoStr))
            {
                if (descontoStr.EndsWith("%"))
                {
                    if (decimal.TryParse(descontoStr.Replace("%", ""), out decimal percentual))
                        desconto = valorTotal * (percentual / 100);
                }
                else
                {
                    decimal.TryParse(descontoStr, out desconto);
                }
            }

            var valorFinal = valorTotal - desconto;

            // Forma de pagamento
            var formaPagamento = EscolherFormaPagamento();

            // Finalizar venda
            venda.ValorTotal = valorTotal;
            venda.Desconto = desconto;
            venda.ValorFinal = valorFinal;
            venda.FormaPagamento = formaPagamento;

            Console.Write("Observações (opcional): ");
            venda.Observacoes = Console.ReadLine();

            // Confirmar venda
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n=== CONFIRMAR VENDA ===");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Total: R$ {valorTotal:F2}");
            Console.WriteLine($"Desconto: R$ {desconto:F2}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"VALOR FINAL: R$ {valorFinal:F2}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Pagamento: {formaPagamento}");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nConfirmar venda? (S/N): ");
            var confirmacao = Console.ReadLine()?.ToUpper();

            if (confirmacao == "S")
            {
                // Salvar venda
                var vendaSalva = await _supabaseService.Insert(venda);
                
                if (vendaSalva != null)
                {
                    // Salvar itens da venda
                    foreach (var item in itens)
                    {
                        item.VendaId = vendaSalva.Id;
                        await _supabaseService.Insert(item);

                        // Atualizar estoque
                        var produto = item.Produto!;
                        produto.QuantidadeEstoque -= item.Quantidade;
                        produto.DataAtualizacao = DateTime.Now;
                        await _supabaseService.Update(produto);
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n✓ Venda #{venda.NumeroVenda} finalizada com sucesso!");
                    Console.WriteLine($"Total: R$ {valorFinal:F2}");
                }
                else
                {
                    MenuHelper.ExibirMensagem("✗ Erro ao processar venda!", ConsoleColor.Red);
                }
            }
            else
            {
                MenuHelper.ExibirMensagem("Venda cancelada.", ConsoleColor.Yellow);
            }

            MenuHelper.PausarTela();
        }

        public async Task VendasDoDia()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══ VENDAS DO DIA ═══\n");
            Console.ForegroundColor = ConsoleColor.White;

            var vendas = await _supabaseService.Select<Venda>();
            var vendasDoDia = vendas.Where(v => v.DataVenda.Date == DateTime.Today && v.Status != "Cancelada").ToList();

            if (!vendasDoDia.Any())
            {
                MenuHelper.ExibirMensagem("Nenhuma venda realizada hoje.", ConsoleColor.Yellow);
                return;
            }

            decimal totalDia = 0;
            int quantidadeVendas = vendasDoDia.Count;

            Console.WriteLine($"📅 {DateTime.Today:dd/MM/yyyy}\n");

            foreach (var venda in vendasDoDia.OrderByDescending(v => v.DataVenda))
            {
                venda.ExibirResumo();
                totalDia += venda.ValorFinal;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== RESUMO DO DIA ===");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Quantidade de vendas: {quantidadeVendas}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"TOTAL FATURADO: R$ {totalDia:F2}");

            MenuHelper.PausarTela();
        }

        public async Task ConsultarVenda()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══ CONSULTAR VENDA ═══\n");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write("Número da venda: ");
            var numeroVenda = Console.ReadLine() ?? "";

            var vendas = await _supabaseService.Select<Venda>();
            var venda = vendas.FirstOrDefault(v => v.NumeroVenda == numeroVenda);

            if (venda == null)
            {
                MenuHelper.ExibirMensagem("Venda não encontrada!", ConsoleColor.Red);
                return;
            }

            // Buscar itens da venda
            var itensVenda = await _supabaseService.Select<ItemVenda>();
            var itens = itensVenda.Where(i => i.VendaId == venda.Id).ToList();

            // Buscar produtos dos itens
            var produtos = await _supabaseService.Select<Produto>();
            foreach (var item in itens)
            {
                item.Produto = produtos.FirstOrDefault(p => p.Id == item.ProdutoId);
            }

            Console.WriteLine("\n=== DETALHES DA VENDA ===");
            venda.ExibirResumo();

            Console.WriteLine("\n=== ITENS DA VENDA ===");
            foreach (var item in itens)
            {
                item.ExibirItem();
            }

            if (!string.IsNullOrEmpty(venda.Observacoes))
            {
                Console.WriteLine($"\nObservações: {venda.Observacoes}");
            }

            MenuHelper.PausarTela();
        }

        public async Task CancelarVenda()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══ CANCELAR VENDA ═══\n");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write("Número da venda para cancelar: ");
            var numeroVenda = Console.ReadLine() ?? "";

            var vendas = await _supabaseService.Select<Venda>();
            var venda = vendas.FirstOrDefault(v => v.NumeroVenda == numeroVenda && v.Status != "Cancelada");

            if (venda == null)
            {
                MenuHelper.ExibirMensagem("Venda não encontrada ou já cancelada!", ConsoleColor.Red);
                return;
            }

            Console.WriteLine("\n=== VENDA A SER CANCELADA ===");
            venda.ExibirResumo();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\nConfirma o cancelamento? (S/N): ");
            var confirmacao = Console.ReadLine()?.ToUpper();

            if (confirmacao == "S")
            {
                // Buscar itens da venda para estornar estoque
                var itensVenda = await _supabaseService.Select<ItemVenda>();
                var itens = itensVenda.Where(i => i.VendaId == venda.Id).ToList();

                // Estornar estoque
                var produtos = await _supabaseService.Select<Produto>();
                foreach (var item in itens)
                {
                    var produto = produtos.FirstOrDefault(p => p.Id == item.ProdutoId);
                    if (produto != null)
                    {
                        produto.QuantidadeEstoque += item.Quantidade;
                        produto.DataAtualizacao = DateTime.Now;
                        await _supabaseService.Update(produto);
                    }
                }

                // Cancelar venda
                venda.Status = "Cancelada";
                var resultado = await _supabaseService.Update(venda);

                if (resultado != null)
                {
                    MenuHelper.ExibirMensagem("✓ Venda cancelada e estoque estornado!", ConsoleColor.Green);
                }
                else
                {
                    MenuHelper.ExibirMensagem("✗ Erro ao cancelar venda!", ConsoleColor.Red);
                }
            }
            else
            {
                MenuHelper.ExibirMensagem("Operação cancelada.", ConsoleColor.Yellow);
            }

            MenuHelper.PausarTela();
        }

        public async Task VendasPorPeriodo()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══ VENDAS POR PERÍODO ═══\n");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write("Data inicial (dd/MM/yyyy): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial))
            {
                MenuHelper.ExibirMensagem("Data inválida!", ConsoleColor.Red);
                return;
            }

            Console.Write("Data final (dd/MM/yyyy): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal))
            {
                MenuHelper.ExibirMensagem("Data inválida!", ConsoleColor.Red);
                return;
            }

            dataFinal = dataFinal.AddDays(1).AddTicks(-1); // Até o final do dia

            var vendas = await _supabaseService.Select<Venda>();
            var vendasPeriodo = vendas.Where(v => v.DataVenda >= dataInicial && v.DataVenda <= dataFinal && v.Status != "Cancelada").ToList();

            if (!vendasPeriodo.Any())
            {
                MenuHelper.ExibirMensagem("Nenhuma venda encontrada no período.", ConsoleColor.Yellow);
                return;
            }

            Console.WriteLine($"\n📅 Período: {dataInicial:dd/MM/yyyy} a {dataFinal.AddTicks(1).AddDays(-1):dd/MM/yyyy}\n");

            decimal totalPeriodo = 0;
            foreach (var venda in vendasPeriodo.OrderByDescending(v => v.DataVenda))
            {
                venda.ExibirResumo();
                totalPeriodo += venda.ValorFinal;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== RESUMO DO PERÍODO ===");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Quantidade de vendas: {vendasPeriodo.Count}");
            Console.WriteLine($"Ticket médio: R$ {(vendasPeriodo.Count > 0 ? totalPeriodo / vendasPeriodo.Count : 0):F2}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"TOTAL FATURADO: R$ {totalPeriodo:F2}");

            MenuHelper.PausarTela();
        }

        public async Task ProdutosMaisVendidos()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══ PRODUTOS MAIS VENDIDOS ═══\n");
            Console.ForegroundColor = ConsoleColor.White;

            var itensVenda = await _supabaseService.Select<ItemVenda>();
            var produtos = await _supabaseService.Select<Produto>();
            var vendas = await _supabaseService.Select<Venda>();

            var itensVendasFinalizadas = itensVenda.Where(i => 
                vendas.Any(v => v.Id == i.VendaId && v.Status != "Cancelada")).ToList();

            var produtosMaisVendidos = itensVendasFinalizadas
                .GroupBy(i => i.ProdutoId)
                .Select(g => new
                {
                    ProdutoId = g.Key,
                    Produto = produtos.FirstOrDefault(p => p.Id == g.Key),
                    QuantidadeVendida = g.Sum(i => i.Quantidade),
                    ValorTotal = g.Sum(i => i.Subtotal)
                })
                .OrderByDescending(x => x.QuantidadeVendida)
                .Take(10)
                .ToList();

            if (!produtosMaisVendidos.Any())
            {
                MenuHelper.ExibirMensagem("Nenhuma venda registrada.", ConsoleColor.Yellow);
                return;
            }

            Console.WriteLine("🏆 TOP 10 PRODUTOS MAIS VENDIDOS:\n");

            int posicao = 1;
            foreach (var item in produtosMaisVendidos)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{posicao}º - {item.Produto?.Nome ?? "Produto"}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"    Quantidade vendida: {item.QuantidadeVendida}");
                Console.WriteLine($"    Valor total: R$ {item.ValorTotal:F2}");
                Console.WriteLine($"    Preço médio: R$ {(item.QuantidadeVendida > 0 ? item.ValorTotal / item.QuantidadeVendida : 0):F2}");
                Console.WriteLine();
                posicao++;
            }

            MenuHelper.PausarTela();
        }

        public async Task ResumoFinanceiro()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══ RESUMO FINANCEIRO ═══\n");
            Console.ForegroundColor = ConsoleColor.White;

            var vendas = await _supabaseService.Select<Venda>();
            var vendasFinalizadas = vendas.Where(v => v.Status != "Cancelada").ToList();

            var hoje = DateTime.Today;
            var vendasHoje = vendasFinalizadas.Where(v => v.DataVenda.Date == hoje).ToList();
            var vendasMes = vendasFinalizadas.Where(v => v.DataVenda.Month == hoje.Month && v.DataVenda.Year == hoje.Year).ToList();

            Console.WriteLine("📊 RESUMO FINANCEIRO\n");

            // Vendas de hoje
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== HOJE ===");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Vendas: {vendasHoje.Count}");
            Console.WriteLine($"Faturamento: R$ {vendasHoje.Sum(v => v.ValorFinal):F2}");
            Console.WriteLine($"Ticket médio: R$ {(vendasHoje.Count > 0 ? vendasHoje.Sum(v => v.ValorFinal) / vendasHoje.Count : 0):F2}");

            // Vendas do mês
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n=== MÊS ATUAL ===");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Vendas: {vendasMes.Count}");
            Console.WriteLine($"Faturamento: R$ {vendasMes.Sum(v => v.ValorFinal):F2}");
            Console.WriteLine($"Ticket médio: R$ {(vendasMes.Count > 0 ? vendasMes.Sum(v => v.ValorFinal) / vendasMes.Count : 0):F2}");

            // Formas de pagamento (mês atual)
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n=== FORMAS DE PAGAMENTO (MÊS) ===");
            Console.ForegroundColor = ConsoleColor.White;
            
            var formasPagamento = vendasMes
                .GroupBy(v => v.FormaPagamento)
                .Select(g => new { Forma = g.Key, Quantidade = g.Count(), Valor = g.Sum(v => v.ValorFinal) })
                .OrderByDescending(x => x.Valor);

            foreach (var forma in formasPagamento)
            {
                Console.WriteLine($"{forma.Forma}: {forma.Quantidade} vendas - R$ {forma.Valor:F2}");
            }

            // Produtos com estoque baixo
            var produtos = await _supabaseService.Select<Produto>();
            var produtosBaixoEstoque = produtos.Where(p => p.QuantidadeEstoque <= p.EstoqueMinimo && p.Ativo).Count();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n=== ALERTAS ===");
            Console.ForegroundColor = ConsoleColor.White;
            
            if (produtosBaixoEstoque > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"⚠️ {produtosBaixoEstoque} produto(s) com estoque baixo");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Todos os produtos com estoque adequado");
            }

            MenuHelper.PausarTela();
        }

        private void ExibirResumoVenda(List<ItemVenda> itens, decimal valorTotal)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n=== RESUMO DA VENDA ===");
            Console.ForegroundColor = ConsoleColor.White;
            
            foreach (var item in itens)
            {
                item.ExibirItem();
            }
            
            Console.WriteLine(new string('-', 50));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"TOTAL: R$ {valorTotal:F2}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private string EscolherFormaPagamento()
        {
            Console.WriteLine("\n=== FORMA DE PAGAMENTO ===");
            Console.WriteLine("[1] Dinheiro");
            Console.WriteLine("[2] Cartão de Débito");
            Console.WriteLine("[3] Cartão de Crédito");
            Console.WriteLine("[4] PIX");
            Console.WriteLine("[5] Outros");
            Console.Write("Escolha: ");

            var opcao = Console.ReadLine();
            
            return opcao switch
            {
                "1" => "Dinheiro",
                "2" => "Cartão de Débito",
                "3" => "Cartão de Crédito",
                "4" => "PIX",
                "5" => "Outros",
                _ => "Não informado"
            };
        }

        private string GerarNumeroVenda()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}