//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
using System.Windows;
using GrammerMaterialOrder.Core;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Collections.ObjectModel;

using GrammerMaterialOrder.Utilities;
using GrammerMaterialOrder.MVVM.Models;
using NPOI.SS.Formula.Functions;
using System;

namespace GrammerMaterialOrder.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public HomeViewModel HomeVM { get; set; }
        public SkladnikViewModel SkladnikVM { get; set; }
        public VlackarViewModel VlackarVM { get; set; }
        public SpravceViewModel SpravceVM { get; set; }
        public List<Employee> Zamestnanci { get; set; }

        public ObservableCollection<Station> StationsList { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public ICommand ClickCommand { get; }

        public MainViewModel()
        {
            List<Station> stationsList = LoadStations();
            _stationEntries = new ObservableCollection<Station>(stationsList);

            List<Employee> employeesList = LoadEmployees();
            _employeeEntries = new CollectionView(employeesList);

            Zamestnanci = new List<Employee>();
            Zamestnanci = employeesList.ToList();

            HomeVM = new HomeViewModel();

            CurrentView = HomeVM;

            // Drží v paměti, je potřeba nastavit tak, aby to vždy reagovalo na danou situaci.
            // Musím se podívat, jak funguje RelayCommand
            ClickCommand = new RelayCommand(o =>
            {
                if (ProgramState.Logout == false)
                {
                    if (ProgramState.SwitchRadioButton == 1)
                    {
                        if (ProgramState.HesloPotvrzeno == true)
                        {
                            ProgramState.HesloPotvrzeno = false;
                            SkladnikVM = new SkladnikViewModel();
                            CurrentView = SkladnikVM;
                        }
                        else if (ProgramState.HesloPotvrzeno == false)
                        {
                            CurrentView = HomeVM;
                        }
                    }
                    if (ProgramState.SwitchRadioButton == 2)
                    {
                        if (ProgramState.HesloPotvrzeno == true)
                        {
                            ProgramState.HesloPotvrzeno = false;
                            VlackarVM = new VlackarViewModel();
                            CurrentView = VlackarVM;
                        }
                        else if (ProgramState.HesloPotvrzeno == false)
                        {
                            CurrentView = HomeVM;
                        }
                    }
                    if (ProgramState.SwitchRadioButton == 3)
                    {
                        if (ProgramState.HesloPotvrzeno == true)
                        {
                            ProgramState.HesloPotvrzeno = false;
                            SpravceVM = new SpravceViewModel();
                            CurrentView = SpravceVM;
                        }
                        else if (ProgramState.HesloPotvrzeno == false)
                        {
                            CurrentView = HomeVM;
                        }
                    }
                }
                else if (ProgramState.Logout == true)
                {
                    CurrentView = HomeVM;
                }
            });
        }

        private ObservableCollection<Station> _stationEntries;

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

        public static List<Station> LoadStations()
        {
            using var db = new MaterialOrderContext();
            var stations = db.Stations.ToList();

            var query = from station in stations
                        where station.StateObject == 1
                        select station;

            if (query.Any())
            {
                return query.ToList();
            }
            else
                return null;
        }

        //private void OnClick()
        //{
        //    _ = MessageBox.Show("Klik", "Klik", MessageBoxButton.OK, MessageBoxImage.Information);
        //}

        private readonly CollectionView _employeeEntries;

        public CollectionView EmployeeEntries
        {
            get { return _employeeEntries; }
        }

        private static List<Employee> LoadEmployees()
        {
            using var db = new EmployeeContext();
            var employees = db.Employees.ToList();

            var query = from employee in employees
                        where employee.IsEmployee && (employee.ProductionOfSeatsWarehouseman || employee.ProductionOfSeatsManager)
                        select new Employee()
                        {
                            Id = employee.Id,
                            LastName = employee.LastName,
                            FirstName = employee.FirstName,
                            PersonalNumber = employee.PersonalNumber,
                            ProductionOfSeatsWarehouseman = employee.ProductionOfSeatsWarehouseman,
                            ProductionOfSeatsManager = employee.ProductionOfSeatsManager
                        };

            return query.ToList();
        }

        public static List<ProductionOrder> GetZakazky()
        {
            using var db = new MaterialOrderContext();
            var zakazky = db.ProductionOrders.ToList();

            var query = from z in zakazky
                        select new ProductionOrder()
                        {
                            Id = z.Id,
                            Order = z.Order,
                            Quantity = z.Quantity,
                            ProductId = z.ProductId,
                            StateObject = z.StateObject
                        };

            return query.ToList();
        }

        public static List<Product> GetVyrobekId(string vyrobek)
        {
            using var db = new ProductMaterialContext();
            var vyrobky = db.Products.ToList();

            var query = from v in vyrobky
                        where v.Name == vyrobek
                        select new Product()
                        {
                            Id = v.Id,
                            Name = v.Name,
                            StateObject = v.StateObject
                        };

            if (query.Any())
                return query.ToList();
            else
                return null;
        }

        public static List<EmployeePlanning> GetPlanId()
        {
            using var db = new MaterialOrderContext();
            var plany = db.EmployeePlanning.ToList();

            var query = from p in plany
                        where !p.Assigned
                        select new EmployeePlanning()
                        {
                            Id = p.Id,
                            StationId = p.StationId
                        };


            if (query.Any())
            {
                return query.ToList();
            }
            else
                return null;
        }

        public static List<EmployeePlanning> GetPlanOfEmployee(int employeeId, int stationId)
        {
            using var db = new MaterialOrderContext();
            var plany = db.EmployeePlanning.ToList();

            var query = from p in plany
                        where employeeId == p.EmployeeId && stationId == p.StationId && DateTime.Now >= p.TimeStampFrom.AddMinutes(-10) && DateTime.Now <= p.TimeStampTo
                        select new EmployeePlanning()
                        {
                            Id = p.Id,
                            StationId = p.StationId
                        };


            if (query.Any())
            {
                return query.ToList();
            }
            else
                return null;
        }
    }
}
