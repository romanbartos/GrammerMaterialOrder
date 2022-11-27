using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using GrammerMaterialOrder.MVVM.Models;

namespace GrammerMaterialOrder.MVVM.ViewModels
{
    public class VlackarViewModel
    {
        public VlackarViewModel()
        {
            IEnumerable<DataOrders> productionOrders = GetAllProductionOrders();
            _productionOrderEntries = new CollectionView(productionOrders);
        }

        private readonly CollectionView _productionOrderEntries;

        public CollectionView ProductionOrderEntries
        {
            get { return _productionOrderEntries; }
        }

        private static IEnumerable<DataOrders> GetAllProductionOrders()
        {
            using (var db = new MaterialOrderContext())
            {
                var products = db.Products.ToList();
                var productionOrders = db.ProductionOrders.ToList();

                var query = from productionOrder in productionOrders
                            join product in products on productionOrder.ProductId equals product.Id
                            select new DataOrders() { ProductionOrder = productionOrder, Product = product };

                return query;
            }
        }

        public struct DataOrders
        {
            public ProductionOrder ProductionOrder { get; set; }
            public Product Product { get; set; }
        }

    }
}
