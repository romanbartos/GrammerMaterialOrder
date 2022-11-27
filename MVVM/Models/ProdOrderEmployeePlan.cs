using System.ComponentModel.DataAnnotations.Schema;

namespace GrammerMaterialOrder.MVVM.Models
{
    [Table("ObjednavkaPlanZam", Schema = "dbo")]
    public class ProdOrderEmployeePlan
    {
        [Column("ObjednavkaPlanZamID")]
        public int Id { get; set; }

        [Column("ObjednavkaID")]
        public int ProductionOrderId { get; set; }

        [Column("PlanovaniZamestnancuID")]
        public int EmployeePlanningId { get; set; }

        [Column("Hotovo")]
        public bool Done { get; set; }

        [Column("Poznamka")]
        public string Note { get; set; }
    }
}
