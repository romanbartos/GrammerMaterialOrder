using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.ComponentModel;

using GrammerMaterialOrder.MVVM.Models;

namespace GrammerMaterialOrder.MVVM.ViewModels
{
    public class ProductionOrdersViewModel
    {
        public ProductionOrdersViewModel()
        {
            cvs.Source = GetData();
        }

        //private readonly ObservableCollection<Data> colData = new ObservableCollection<Data>();
        private readonly CollectionViewSource cvs = new();
        public ICollectionView View { get => cvs.View; }

        private static ObservableCollection<Data> GetData()
        {
            var colProducts = LoadProducts();
            var colProductionOrders = LoadProductionOrders();
            var query = from productionOrder in colProductionOrders
                        join product in colProducts on productionOrder.ProductId equals product.Id
                        select new Data() { OrderName = productionOrder.Order, Quantity = productionOrder.Quantity, ProductName = product.Name, StateObject = product.StateObject};
            return new ObservableCollection<Data>(query);
        }

        private static ObservableCollection<Product> LoadProducts()
        {
            using var db = new MaterialOrderContext();
            var products = db.Products.ToList();
            return new ObservableCollection<Product>(products);
        }

        private static ObservableCollection<ProductionOrder> LoadProductionOrders()
        {
            using var db = new MaterialOrderContext();
            var productionOrders = db.ProductionOrders.ToList();
            return new ObservableCollection<ProductionOrder>(productionOrders);
        }

        private struct Data
        {
            public string OrderName { get; set; }
            public int Quantity { get; set; }
            public string ProductName { get; set; }
            public byte StateObject { get; set; }
        }
    }
}
