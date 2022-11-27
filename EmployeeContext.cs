using Microsoft.EntityFrameworkCore;

using GrammerMaterialOrder.MVVM.Models;

namespace GrammerMaterialOrder
{
    public class EmployeeContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        //proč je tady ta metoda
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _ = optionsBuilder.UseSqlServer(@"Server=ntb-bartos\sqlexpress2014;Database=Zamestnanci;Trusted_Connection=True;");
        }
    }
}
