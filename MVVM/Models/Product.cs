using System.ComponentModel.DataAnnotations.Schema;

namespace GrammerMaterialOrder.MVVM.Models
{
    [Table("Vyrobek", Schema = "dbo")]
    public class Product
    {
        [Column("VyrobekID")]
        public int Id { get; set; }

        [Column("Nazev")]
        public string Name { get; set; }
    }
}
