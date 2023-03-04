using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Prism.Commands;

using GrammerMaterialOrder.Core;
using GrammerMaterialOrder.MVVM.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Windows.Media.Media3D;
using System.Windows;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using GrammerMaterialOrder.MVVM.Views;
using System.Windows.Controls;

//using GrammerMaterialOrder.MVVM.Views;
using GrammerMaterialOrder.Utilities;

namespace GrammerMaterialOrder.MVVM.ViewModels
{
    public partial class SpravceViewModel : INotifyPropertyChanged
    {
        public ICollectionView MaterialForStationPridelenoEntriesView { get; set; }

        private int? _selectedStationId;

        public ObservableCollection<Station> StationsList { get; set; }
        public ObservableCollection<Station> ActivateStationsList { get; set; }
        public ObservableCollection<DataSchedule> ScheduleList { get; set; }

        public SpravceViewModel()
        {
            List<Employee> employeesList = LoadWarehousemanEmployees();
            _employeeWarehousemanEntries = new ObservableCollection<Employee>(employeesList);
            //_employeeWarehousemanEntries = new CollectionView(employeesList);

            employeesList = LoadAdminEmployees();
            _employeeAdminEntries = new CollectionView(employeesList);

            List<Employee> employeesAllList = LoadEmployeesAll();
            _employeeAllEntries = new CollectionView(employeesAllList);

            List<Station> stationsList = LoadStations();
            _stationEntries = new ObservableCollection<Station>(stationsList);
            _stationPridelenoEntries = new ObservableCollection<Station>(stationsList);

            List<Station> activateStationList = LoadActivateStation();
            _activateStationEntries = new ObservableCollection<Station>(activateStationList);

            IEnumerable<DataOrders> productionOrders = GetAllProductionOrders();
            _productionOrderEntries = new CollectionView(productionOrders);

            List<DataSchedule> schedule = LoadSchedule();
            _scheduleEntries = new ObservableCollection<DataSchedule>(schedule);
            ScheduleList = new ObservableCollection<DataSchedule>(schedule);

            IEnumerable<DataMFS> materialForStationList = LoadMaterialForStation();
            _materialForStationPridelitEntries = new ObservableCollection<DataMFS>(materialForStationList);

            IEnumerable<DataMFSPrideleno> materialForStationPridelenoList = LoadMaterialForStationPrideleno(-1);
            _materialForStationPridelenoEntries = new ObservableCollection<DataMFSPrideleno>(materialForStationPridelenoList);

            MaterialForStationPridelenoEntriesView = new CollectionViewSource { Source = MaterialForStationPridelenoEntries }.View;
            MaterialForStationPridelenoEntriesView.Filter = station => SelectedStationId == null || SelectedStationId == ((DataMFSPrideleno)station).StationId;

            StationsList = new ObservableCollection<Station>(stationsList);
            ActivateStationsList = new ObservableCollection<Station>(activateStationList);
        }

        public int? SelectedStationId
        {
            get
            {
                return _selectedStationId;
            }
            set
            {
                _selectedStationId = value;
                MaterialForStationPridelenoEntriesView.Refresh();
            }
        }

        public void UpdateSchedule()
        {
            IEnumerable<DataSchedule> schedule = LoadSchedule();
            _scheduleEntries = new ObservableCollection<DataSchedule>(schedule);
        }

        //private readonly CollectionView _employeeWarehousemanEntries;
        private readonly ObservableCollection<Employee> _employeeWarehousemanEntries;
        private readonly CollectionView _employeeAdminEntries;
        private readonly CollectionView _employeeAllEntries;

        // skladníci
        //public CollectionView EmployeeWarehousemanEntries
        public ObservableCollection<Employee> EmployeeWarehousemanEntries
        {
            get { return _employeeWarehousemanEntries; }
        }
        public CollectionView EmployeeAdminEntries
        {
            get { return _employeeAdminEntries; }
        }

        // všichni zaměstnanci
        public CollectionView EmployeeAllEntries
        {
            get { return _employeeAllEntries; }
        }

        private static List<Employee> LoadWarehousemanEmployees()
        {
            using var db = new EmployeeContext();
            var employees = db.Employees.ToList();

            var query = from employee in employees
                        where employee.IsEmployee && employee.ProductionOfSeatsWarehouseman
                        orderby employee.LastName, employee.FirstName
                        select new Employee() { Id = employee.Id, LastName = employee.LastName, FirstName = employee.FirstName };
            //new DataEmployees() { Employee = employee }

            return query.ToList();
            //db.Employees.ToList()
        }

        private static List<Employee> LoadAdminEmployees()
        {
            using var db = new EmployeeContext();
            var employees = db.Employees.ToList();

            var query = from employee in employees
                        where employee.IsEmployee && employee.ProductionOfSeatsManager
                        orderby employee.LastName, employee.FirstName
                        select new Employee() { Id = employee.Id, LastName = employee.LastName, FirstName = employee.FirstName };
            //new DataEmployees() { Employee = employee }

            return query.ToList();
            //db.Employees.ToList()
        }

        private static List<Employee> LoadEmployeesAll()
        {
            using var db = new EmployeeContext();
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


        private ObservableCollection<Station> _stationEntries;
        private ObservableCollection<Station> _stationPridelenoEntries;

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

        private ObservableCollection<Station> _activateStationEntries;

        public ObservableCollection<Station> ActivateStationEntries
        {
            get { return _activateStationEntries; }
            set
            {
                if (_activateStationEntries == value) return;
                _activateStationEntries = value;
                OnPropertyChanged(nameof(ActivateStationEntries));
            }
        }

        public void AddNewStation()
        {
            List<Station>stationsList = LoadStations();
            this.StationsList.Clear();
            foreach (var item in stationsList)
            {
                StationsList.Add(item);
            }
        }

        public void ActivateStation()
        {
            List<Station> activateStationsList = LoadActivateStation();
            this.ActivateStationsList.Clear();
            foreach (var item in activateStationsList)
            {
                ActivateStationsList.Add(item);
            }
        }

        public ObservableCollection<Station> StationPridelenoEntries
        {
            get { return _stationPridelenoEntries; }
            set
            {
                if (_stationPridelenoEntries == value) return;
                _stationPridelenoEntries = value;
                OnPropertyChanged(nameof(StationPridelenoEntries));
            }
        }

        //public DelegateCommand<Station> AddNewStationEntries
        //{
        //    get
        //    {
        //        return new DelegateCommand<Station>(Execute_AddNewStationEntries);
        //    }
        //}

        //public void Execute_AddNewStationEntries(Station param)
        //{
        //    using var db = new MaterialOrderContext();

        //    //_stationEntries.Add(new Station() { param });
        //    //db.GetContext().SaveChanges();
        //}   

        public static List<Station> LoadStations()
        {
            using var db = new MaterialOrderContext();
            var stations = db.Stations.ToList();

            var query = from station in stations
                        where station.StateObject == 1
                        select station;

            return query.ToList();
        }

        public static List<Station> LoadActivateStation()
        {
            using var db = new MaterialOrderContext();
            var stations = db.Stations.ToList();

            var query = from station in stations
                        where station.StateObject == 2
                        select station;

            return query.ToList();
        }

        private readonly CollectionView _productionOrderEntries;

        public CollectionView ProductionOrderEntries
        {
            get { return _productionOrderEntries; }
        }

        private IEnumerable<DataOrders> GetAllProductionOrders()
        {
            using var db = new MaterialOrderContext();
            var products = db.Products.ToList();
            var productionOrders = db.ProductionOrders.ToList();

            var query = from productionOrder in productionOrders
                        join product in products on productionOrder.ProductId equals product.Id
                        select new DataOrders() { ProductionOrder = productionOrder, Product = product };

            return query;
        }

        public struct DataOrders
        {
            public ProductionOrder ProductionOrder { get; set; }
            public Product Product { get; set; }
        }

        //private static List<Product> LoadProductsMaterials()
        //{
        //    using var db = new ProductMaterialContext();
        //    var productsmaterials = db.
        //    return new List<DataProductsMaterials>(productsmaterials);
        //}

        //public static List<DataProductsMaterials> GetDataProductIdMaterialId()
        //{
        //    using var db = new ProductMaterialContext();
        //    var productsMaterials = db.ProductsMaterials;
        //    var query = from productMaterial in productsMaterials
        //                select new DataProductsMaterials() { ProductId = productMaterial.ProductId, MaterialId = productMaterial.MaterialId };
        //    return new List<DataProductsMaterials>(query);
        //}

        public static List<DataProductsMaterials> GetDataProductsMaterials()
        {
            using var db = new ProductMaterialContext();
            var productsMaterials = db.ProductsMaterials;
            var materials = db.Materials;
            var query = from productMaterial in productsMaterials
                        join material in materials on productMaterial.MaterialId equals material.Id
                        select new DataProductsMaterials() { ProductId = productMaterial.ProductId, MaterialId = productMaterial.MaterialId, MaterialName = material.Name  };
            return new List<DataProductsMaterials>(query);
        }

        public struct DataProductsMaterials
        {
            public int ProductId { get; set; }
            public int MaterialId { get; set; }
            public string MaterialName { get; set; }
        }

        public static List<DataProducts> GetDataProducts()
        {
            using var db = new ProductMaterialContext();
            var products = db.Products;
            var query = from product in products
                        select new DataProducts() { ProductId = product.Id, ProductName = product.Name };
            return new List<DataProducts>(query);
        }

        public struct DataProducts
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
        }

        public static List<DataMaterials> GetDataMaterials()
        {
            using var db = new ProductMaterialContext();
            var materials = db.Materials;
            var query = from material in materials
                        select new DataMaterials() { MaterialId = material.Id, MaterialName = material.Name };
            return new List<DataMaterials>(query);
        }

        public struct DataMaterials
        {
            public int MaterialId { get; set; }
            public string MaterialName { get; set; }
        }

        public static List<DataMaterials> GetMaterials(List<string> vybraneMaterialy, List<DataMaterials> vsechnyMaterialy)
        {
            // vybraneMaterialy - co vybírám
            // vsechnyMaterialy - z čeho vybírám

            // vrátí materiály, jejichž indexy se nachází v proměnné vybraneMaterialy a patří jednomu produktu
            var query = from vm in vsechnyMaterialy
                        where vybraneMaterialy.Contains(vm.MaterialName)
                        select new DataMaterials() { MaterialId = vm.MaterialId, MaterialName = vm.MaterialName };
            return new List<DataMaterials>(query);
        }

        public static List<DataProduct> GetProduct(string vyrobek, List<DataProducts> produkty)
        {
            //var a = v.Vyrobek;
            var query = from product in produkty
                        where product.ProductName == vyrobek
                        select new DataProduct() { ProductId = product.ProductId, ProductName = product.ProductName };
            return new List<DataProduct> (query);
        }

        public struct DataProduct
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
        }

        #region Plánovač
        //public PlanovacViewModel()
        //{
        //    IEnumerable<DataOrders> productionOrders = GetAllProductionOrders();
        //    _productionOrderEntries = new CollectionView(productionOrders);
        //}

        private ObservableCollection<DataSchedule> _scheduleEntries;

        public ObservableCollection<DataSchedule> ScheduleEntries
        {
            get { return _scheduleEntries; }
        }

        private static List<DataSchedule> LoadSchedule()
        {
            using var db = new MaterialOrderContext();
            using var dbEmployee = new EmployeeContext();

            var schedule = db.EmployeePlanning.ToList();
            var stations = db.Stations.ToList();
            var employee = dbEmployee.Employees.ToList();

            var query = from s in schedule
                        join st in stations on s.StationId equals st.Id
                        join e in employee on s.EmployeeId equals e.Id
                        where s.StateObject == 1
                        orderby s.TimeStampFrom ascending, st.Id ascending
                        select new DataSchedule() { ScheduleId = s.Id, Schedule = s, Employee = e.FullName, Station = st.Name, StateObject = s.StateObject, EmployeeId = e.Id, StationId = st.Id };

            return query.ToList();
        }

        public static List<DataSchedule> GetSelectedSchedule(DateTime vybranyDatum, string pracoviste)
        {
            using var db = new MaterialOrderContext();
            using var dbEmployee = new EmployeeContext();

            var schedule = db.EmployeePlanning.ToList();
            var stations = db.Stations.ToList();
            var employee = dbEmployee.Employees.ToList();

            var query = from s in schedule
                        join st in stations on s.StationId equals st.Id
                        join e in employee on s.EmployeeId equals e.Id
                        where s.StateObject == 1 && s.TimeStampFrom.Date == vybranyDatum.Date && st.Name == pracoviste
                        orderby s.TimeStampFrom ascending, st.Id ascending
                        select new DataSchedule() { ScheduleId = s.Id, Schedule = s, Employee = e.FullName, Station = st.Name, StateObject = s.StateObject };

            return query.ToList();
        }

        public void GetSchedule()
        {
            List<DataSchedule> scheduleList = LoadSchedule();
            this.ScheduleList.Clear();
            //StationEntries.Clear();
            foreach (var item in scheduleList)
            {
                ScheduleList.Add(item);
            }
        }

        public struct DataSchedule
        {
            public int ScheduleId { get; set; }
            public EmployeePlanning Schedule { get; set; }
            public string Employee { get; set; }
            public string Station { get; set; }
            public byte StateObject { get; set; }
            public int EmployeeId { get; set; }
            public int StationId { get; set; }
        }
        #endregion

        #region Materiál na stanoviště
        private ObservableCollection<DataMFS> _materialForStationPridelitEntries;

        public ObservableCollection<DataMFS> MaterialForStationPridelitEntries
        {
            get { return _materialForStationPridelitEntries; }
            set
            {
                if (_materialForStationPridelitEntries == value) return;
                _materialForStationPridelitEntries = value;
                OnPropertyChanged(nameof(MaterialForStationPridelitEntries));
            }
        }

        private ObservableCollection<DataMFSPrideleno> _materialForStationPridelenoEntries;

        public ObservableCollection<DataMFSPrideleno> MaterialForStationPridelenoEntries
        {
            get { return _materialForStationPridelenoEntries; }
            set
            {
                if (_materialForStationPridelenoEntries == value) return;
                _materialForStationPridelenoEntries = value;
                OnPropertyChanged(nameof(MaterialForStationPridelenoEntries));
            }
        }

        private static IEnumerable<DataMFS> LoadMaterialForStation()
        {
            using var db = new MaterialOrderContext();
            var materials = db.Materials.ToList();

            var materialsForStation = db.MaterialForStation.ToList();

            var mat = materials.Where(y => !materialsForStation.Any(z => z.MaterialId == y.Id && z.StateObject == 1));
            //var rozdil = materials.Except(materialsForStation);

            var query = from m in mat
                        where m.StateObject == 1
                        orderby m.Name
                        select new DataMFS() { MaterialId = m.Id, MaterialName = m.Name, MaterialPridatOdebrat = false };

            ProgramState.RadkyPridelitMaterialDGV = false;
            if (query.Any())
            {
                ProgramState.RadkyPridelitMaterialDGV = true;
            }

            return query;
        }

        private static IEnumerable<DataMFSPrideleno> LoadMaterialForStationPrideleno(int stationId)
        {
            using var db = new MaterialOrderContext();
            var materials = db.Materials.ToList();
            var materialsForStation = db.MaterialForStation.ToList();

            var query = from mfs in materialsForStation
                        join m in materials on mfs.MaterialId equals m.Id
                        where mfs.StateObject == 1 && mfs.StationId == stationId
                        orderby m.Name
                        select new DataMFSPrideleno() { StationId = mfs.StationId, MaterialId = m.Id, MaterialName = m.Name, MaterialPridatOdebrat = false };

            ProgramState.RadkyMaterialPridelenDGV = false;
            if (query.Any())
            {
                ProgramState.RadkyMaterialPridelenDGV = true;
            }

            return query;
        }

        public void GetMaterialForStationPridelit()
        {
            IEnumerable<DataMFS> materialForStationPridelitList = LoadMaterialForStation();
            this.MaterialForStationPridelitEntries.Clear();

            foreach (var item in materialForStationPridelitList)
            {
                MaterialForStationPridelitEntries.Add(item);
            }
        }

        public void GetMaterialForStationPrideleno(int stationId)
        {
            IEnumerable<DataMFSPrideleno> materialForStationPridelenoList = LoadMaterialForStationPrideleno(stationId);
            this.MaterialForStationPridelenoEntries.Clear();

            foreach (var item in materialForStationPridelenoList)
            {
                MaterialForStationPridelenoEntries.Add(item);
            }
        }

        public static IEnumerable<DataMFSPrideleno> GetMaterialForStationOdebrat(int materialId, int stationId)
        {
            using var db = new MaterialOrderContext();
            var materialsForStation = db.MaterialForStation.ToList();

            var query = from mfs in materialsForStation
                        where mfs.MaterialId == materialId && mfs.StationId == stationId
                        select new DataMFSPrideleno() { MaterialNaStanovisteId = mfs.Id };

            return query;
        }

        public struct DataMFS
        {
            public string MaterialName { get; set; }
            public int MaterialId { get; set; }
            public bool MaterialPridatOdebrat { get; set; }
        }

        public struct DataMFSPrideleno
        {
            public int MaterialNaStanovisteId { get; set; }
            public string MaterialName { get; set; }
            public int MaterialId { get; set; }
            public bool MaterialPridatOdebrat { get; set; }
            public int StationId { get; set; }
        }

        public struct DataStation
        {
            public string StationName { get; set; }
            public int StationId { get; set; }
            public string MaterialName { get; set; }
            public int MaterialId { get; set; }
            public bool MaterialPridatOdebrat { get; set; }
        }

        public static IEnumerable<DataPZ> GetPlanovaniZamestnancu(int employeeId, int stationId)
        {
            using var db = new MaterialOrderContext();
            var employeePlanning = db.EmployeePlanning.ToList();

            var query = from pz in employeePlanning
                        where pz.EmployeeId == employeeId && pz.StationId == stationId
                        select new DataPZ() { TimeStampFrom = pz.TimeStampFrom, TimeStampTo = pz.TimeStampTo };

            return query;
        }

        public struct DataPZ
        {
            public DateTime TimeStampFrom { get; set; }
            public DateTime TimeStampTo { get; set; }
        }

        //public class DataMFS : DependencyObject
        //{
        //    public static readonly DependencyProperty MaterialNameProperty = DependencyProperty.Register("MaterialName", typeof(string), typeof(DataMFS), new PropertyMetadata(default(string)));
        //    public string MaterialName { get { return (string)GetValue(MaterialNameProperty); } set { SetValue(MaterialNameProperty, value); } }

        //    public static readonly DependencyProperty MaterialIdProperty = DependencyProperty.Register("MaterialId", typeof(int), typeof(DataMFS), new PropertyMetadata(default(int)));
        //    public int MaterialId { get { return (int)GetValue(MaterialIdProperty); } set { SetValue(MaterialIdProperty, value); } }

        //    public static readonly DependencyProperty MaterialPridatOdebratProperty = DependencyProperty.Register("MaterialPridatOdebrat", typeof(bool), typeof(DataMFS), new PropertyMetadata(default(bool)));
        //    public bool MaterialPridatOdebrat { get { return (bool)GetValue(MaterialPridatOdebratProperty); } set { SetValue(MaterialPridatOdebratProperty, value); } }
        //}
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
