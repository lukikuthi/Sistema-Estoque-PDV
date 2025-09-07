using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SistemaEstoquePDV.Models
{
    [Table("vendas")]
    public class Venda : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("numero_venda")]
        public string NumeroVenda { get; set; } = string.Empty;

        [Column("data_venda")]
        public DateTime DataVenda { get; set; } = DateTime.Now;

        [Column("valor_total")]
        public decimal ValorTotal { get; set; }

        [Column("desconto")]
        public decimal Desconto { get; set; } = 0;

        [Column("valor_final")]
        public decimal ValorFinal { get; set; }

        [Column("forma_pagamento")]
        public string FormaPagamento { get; set; } = string.Empty;

        [Column("status")]
        public string Status { get; set; } = "Finalizada";

        [Column("observacoes")]
        public string? Observacoes { get; set; }

        // Propriedade de navegação - não é mapeada para o banco
        [Reference(typeof(ItemVenda))]
        public List<ItemVenda>? Itens { get; set; }

        public void ExibirResumo()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Venda #{NumeroVenda} - {DataVenda:dd/MM/yyyy HH:mm}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Status: {Status}");
            Console.WriteLine($"Forma de Pagamento: {FormaPagamento}");
            Console.WriteLine($"Valor Total: R$ {ValorTotal:F2}");
            Console.WriteLine($"Desconto: R$ {Desconto:F2}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"VALOR FINAL: R$ {ValorFinal:F2}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new string('-', 50));
        }
    }
}