using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.ComponentModel;

using GrammerMaterialOrder.MVVM.Models;

namespace GrammerMaterialOrder.MVVM.ViewModels
{
    public class OrdersViewModel
    {
        public OrdersViewModel()
        {
            cvsOrders.Source = GetDataOrders();
            cvsEmployees.Source = GetDataEmployees();
        }

        //private readonly ObservableCollection<Data> colData = new ObservableCollection<Data>();
        private readonly CollectionViewSource cvsOrders = new();
        private readonly CollectionViewSource cvsEmployees = new();
        public ICollectionView ViewOrders { get => cvsOrders.View; }
        public ICollectionView ViewEmployees { get => cvsEmployees.View; }

        private static ObservableCollection<DataOrders> GetDataOrders()
        {
            var colProducts = LoadProducts();
            var colProductionOrders = LoadProductionOrders();
            var query = from productionOrder in colProductionOrders
                        join product in colProducts on productionOrder.ProductId equals product.Id
                        select new DataOrders() { OrderName = productionOrder.Order, Quantity = productionOrder.Quantity, ProductName = product.Name };
            return new ObservableCollection<DataOrders>(query);
        }

        private static ObservableCollection<DataEmployees> GetDataEmployees()
        {
            var colEmployees = LoadEmployees();
            var query = from employee in colEmployees
                        select new DataEmployees() { FirstName = employee.FirstName, LastName = employee.LastName, IsEmployee = employee.IsEmployee, ProductionOfSeatsWarehouseman = employee.ProductionOfSeatsWarehouseman, ProductionOfSeatsManager = employee.ProductionOfSeatsManager };
            return new ObservableCollection<DataEmployees>(query);
        }

        private static ObservableCollection<Product> LoadProducts()
        {
            using var db = new MaterialOrderContext();
            List<Product> products = db.Products.ToList();
            return new ObservableCollection<Product>(products);
        }

        private static ObservableCollection<ProductionOrder> LoadProductionOrders()
        {
            using var db = new MaterialOrderContext();
            List<ProductionOrder> productionOrders = db.ProductionOrders.ToList();
            return new ObservableCollection<ProductionOrder>(productionOrders);
        }

        private static ObservableCollection<Employee> LoadEmployees()
        {
            using var db = new EmployeeContext();
            List<Employee> employees = db.Employees.ToList();
            return new ObservableCollection<Employee>(employees);
        }

        private struct DataOrders
        {
            public string OrderName { get; set; }
            public int Quantity { get; set; }
            public string ProductName { get; set; }
        }

        private struct DataEmployees
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public bool IsEmployee { get; set; }
            public bool ProductionOfSeatsWarehouseman { get; set; }
            public bool ProductionOfSeatsManager { get; set; }
        }
    }
}
