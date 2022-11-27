using System.ComponentModel.DataAnnotations.Schema;

namespace GrammerMaterialOrder.MVVM.Models
{
    [Table("MaterialNaStanoviste", Schema = "dbo")]
    public class MaterialForStation
    {
        [Column("MaterialNaStanovisteID")]
        public int Id { get; set; }

        [Column("PlanovaniZamestnancuID")]
        public int EmploeePlannignId { get; set; }

        //[Column("ObjednavkaID")]
        //public int OrderId { get; set; }

        [Column("StanovisteID")]
        public int StationId { get; set; }

        //[Column("Poznamka")]
        //public string Note { get; set; }

        //[Column("Hotovo")]
        //public bool Done { get; set; }
    }
}
