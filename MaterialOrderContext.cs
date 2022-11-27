using Microsoft.EntityFrameworkCore;

using GrammerMaterialOrder.MVVM.Models;

namespace GrammerMaterialOrder
{
    public class MaterialOrderContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductionOrder> ProductionOrders { get; set; }
        public DbSet<ProdOrderEmployeePlan> ProdOrdersEmployeePlan { get; set; }
        public DbSet<EmployeePlanning> EmployeePlanning { get; set; }
        public DbSet<Station> Stations { get; set; }

        //proč je tady ta metoda
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=ntb-bartos\sqlexpress2014;Database=Zakazky;Trusted_Connection=True;");
        }
    }
}
