using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SistemaEstoquePDV.Models
{
    [Table("produtos")]
    public class Produto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [Column("nome")]
        public string Nome { get; set; } = string.Empty;

        [Column("descricao")]
        public string? Descricao { get; set; }

        [Column("preco")]
        public decimal Preco { get; set; }

        [Column("quantidade_estoque")]
        public int QuantidadeEstoque { get; set; }

        [Column("estoque_minimo")]
        public int EstoqueMinimo { get; set; }

        [Column("categoria")]
        public string? Categoria { get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; } = true;

        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        [Column("data_atualizacao")]
        public DateTime DataAtualizacao { get; set; } = DateTime.Now;

        public void ExibirInfo()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Código: {Codigo} | Nome: {Nome}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Descrição: {Descricao ?? "N/A"}");
            Console.WriteLine($"Preço: R$ {Preco:F2}");
            Console.WriteLine($"Estoque: {QuantidadeEstoque} | Mínimo: {EstoqueMinimo}");
            Console.WriteLine($"Categoria: {Categoria ?? "N/A"}");

            if (QuantidadeEstoque <= EstoqueMinimo)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("⚠️ ESTOQUE BAIXO!");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new string('-', 50));
        }
    }
}