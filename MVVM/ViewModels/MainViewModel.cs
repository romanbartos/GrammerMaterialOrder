//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
using System.Windows;
using GrammerMaterialOrder.Core;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using GrammerMaterialOrder.Utilities;

using GrammerMaterialOrder.MVVM.Models;

namespace GrammerMaterialOrder.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public HomeViewModel HomeVM { get; set; }
        public SkladnikViewModel SkladnikVM { get; set; }
        public VlackarViewModel VlackarVM { get; set; }
        public SpravceViewModel SpravceVM { get; set; }
        public List<Employee> Zamestnanci { get; set; }

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
            using (var db = new EmployeeContext())
            {
                var employees = db.Employees.ToList();

                var query = from employee in employees
                            where employee.IsEmployee && (employee.ProductionOfSeatsWarehouseman || employee.ProductionOfSeatsManager)
                            select new Employee() { Id = employee.Id, 
                                LastName = employee.LastName, 
                                FirstName = employee.FirstName, 
                                PersonalNumber = employee.PersonalNumber,
                                ProductionOfSeatsWarehouseman = employee.ProductionOfSeatsWarehouseman,
                                ProductionOfSeatsManager = employee.ProductionOfSeatsManager };
                //new DataEmployees() { Employee = employee }

                return query.ToList();
                //db.Employees.ToList()
            }
        }

    }
}
