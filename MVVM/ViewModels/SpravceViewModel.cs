using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using GrammerMaterialOrder.Core;
using GrammerMaterialOrder.MVVM.Models;

namespace GrammerMaterialOrder.MVVM.ViewModels
{
    public partial class SpravceViewModel : INotifyPropertyChanged
    {
        public SpravceViewModel()
        {
            List<Employee> employeesList = LoadEmployees();
            _employeeEntries = new CollectionView(employeesList);

            List<Employee> employeesAllList = LoadEmployeesAll();
            _employeeAllEntries = new CollectionView(employeesAllList);

            List<Station> stationsList = LoadStations();
            //CollectionView
            _stationEntries = new ObservableCollection<Station>(stationsList);
            _stationEntriesCmb = new ObservableCollection<Station>(stationsList);

            IEnumerable<DataOrders> productionOrders = GetAllProductionOrders();
            _productionOrderEntries = new CollectionView(productionOrders);
        }

        private readonly CollectionView _employeeEntries;
        private readonly CollectionView _employeeAllEntries;

        public CollectionView EmployeeEntries
        {
            get { return _employeeEntries; }
        }

        public CollectionView EmployeeAllEntries
        {
            get { return _employeeAllEntries; }
        }

        private static List<Employee> LoadEmployees()
        {
            using (var db = new EmployeeContext())
            {
                var employees = db.Employees.ToList();

                var query = from employee in employees
                            where employee.IsEmployee && (employee.ProductionOfSeatsWarehouseman || employee.ProductionOfSeatsManager)
                            orderby employee.LastName, employee.FirstName
                            select new Employee() { Id = employee.Id, LastName = employee.LastName, FirstName = employee.FirstName };
                //new DataEmployees() { Employee = employee }

                return query.ToList();
                //db.Employees.ToList()
            }
        }

        private static List<Employee> LoadEmployeesAll()
        {
            using (var db = new EmployeeContext()) 
            {
                var employees = db.Employees.ToList();

                var query = from employee in employees
                            where employee.IsEmployee
                            orderby employee.LastName, employee.FirstName
                            select new Employee() { Id = employee.Id, LastName = employee.LastName, FirstName = employee.FirstName };
                //new DataEmployees() { Employee = employee }
                //  && employee.ProductionOfSeatsWarehouseman is false || (employee.IsEmployee && employee.ProductionOfSeatsManager is false)

                return query.ToList();
                //db.Employees.ToList()
            }
        }

        // ObservableCollection<Station>
        private ObservableCollection<Station> _stationEntries;
        //List<Station>
        private ObservableCollection<Station> _stationEntriesCmb;

        // ObservableCollection<Station>
        public ObservableCollection<Station> StationEntries
        {
            get { return _stationEntries; }
            set
            {
                if (_stationEntries == value) return;
                _stationEntries = value;
                OnPropertyChanged(nameof(StationEntries));
            }
        }
        //List<Station>
        public ObservableCollection<Station> StationEntriesCmb
        {
            get { return _stationEntriesCmb; }
            set
            {
                if (_stationEntriesCmb == value)
                {
                    return;
                }
                _stationEntriesCmb = value;
                OnPropertyChanged(nameof(StationEntriesCmb));
            }
        }

        private static List<Station> LoadStations()
        {
            using (var db = new MaterialOrderContext())
            {
                var stations = db.Stations.ToList();

                var query = from station in stations
                            where station.StateObject == 1
                            select new Station() { Id = station.Id, Name = station.Name };

                return query.ToList();
            }
        }

        private readonly CollectionView _productionOrderEntries;

        public CollectionView ProductionOrderEntries
        {
            get { return _productionOrderEntries; }
        }

        private IEnumerable<DataOrders> GetAllProductionOrders()
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
