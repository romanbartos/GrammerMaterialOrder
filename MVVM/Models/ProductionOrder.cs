using System.ComponentModel.DataAnnotations.Schema;

namespace GrammerMaterialOrder.MVVM.Models
{
    [Table("Objednavka", Schema = "dbo")]
    public class ProductionOrder
    {
        [Column("ObjednavkaID")]
        public int Id { get; set; }

        [Column("Zakazka")]
        public string Order { get; set; }

        [Column("Pocet")]
        public int Quantity { get; set; }

        [Column("VyrobekID")]
        public int ProductId { get; set; }
    }
}