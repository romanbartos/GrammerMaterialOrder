using Microsoft.EntityFrameworkCore;

using GrammerMaterialOrder.MVVM.Models;

namespace GrammerMaterialOrder
{
    public class ProductMaterialContext : DbContext
    {
        //public ProductMaterialContext(DbContextOptions<ProductMaterialContext> options) : base(options)
        //{
        //}

        public DbSet<Product> Products { get; set; }
        //public DbSet<ProductMaterial> ProductsMaterials { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<ProductMaterial> ProductsMaterials { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=ntb-bartos\sqlexpress2014;Database=Zakazky;Trusted_Connection=True;");
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<ProductMaterial>()
        //        .HasKey(t => new { t.ProductId, t.MaterialId });

        //    modelBuilder.Entity<ProductMaterial>()
        //        .HasOne(pm => pm.Product)
        //        .WithMany(p => p.ProductsMaterials)
        //        .HasForeignKey(pm => pm.ProductId);

        //    modelBuilder.Entity<ProductMaterial>()
        //        .HasOne(pm => pm.Material)
        //        .WithMany(m => m.ProductsMaterials)
        //        .HasForeignKey(pm => pm.MaterialId);
        //}
    }
}
