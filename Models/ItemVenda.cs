using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SistemaEstoquePDV.Models
{
    [Table("itens_venda")]
    public class ItemVenda : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("venda_id")]
        public int VendaId { get; set; }

        [Column("produto_id")]
        public int ProdutoId { get; set; }

        [Column("quantidade")]
        public int Quantidade { get; set; }

        [Column("preco_unitario")]
        public decimal PrecoUnitario { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        // Propriedade de navegação - não é mapeada para o banco
        [Reference(typeof(Produto))]
        public Produto? Produto { get; set; }

        public void ExibirItem()
        {
            Console.WriteLine($"{Produto?.Nome ?? "Produto"} | Qtd: {Quantidade} | Unit: R$ {PrecoUnitario:F2} | Total: R$ {Subtotal:F2}");
        }
    }
}