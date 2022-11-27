using System.ComponentModel.DataAnnotations.Schema;

namespace GrammerMaterialOrder.MVVM.Models
{
    [Table("Material", Schema = "dbo")]
    public class Material
    {
        [Column("MaterialID")]
        public int Id { get; set; }

        [Column("Nazev")]
        public string Name { get; set; }
    }
}
