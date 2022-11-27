using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using GrammerMaterialOrder.Utilities;
using GrammerMaterialOrder.MVVM.Models;

namespace GrammerMaterialOrder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //DataContext = new MainViewModel();
            //nastavení pro skladníka
            ProgramState.SwitchRadioButton = 1;
            ProgramState.Logout = false;
            ProgramState.HesloPotvrzeno = false;

            //var dg = new DataGrid();
            //this.MainGrid.Children.Add(dg);

            //CreateColumns();
            //LoadData();
        }

        //private void CreateColumns()
        //{
        //    var idColumn = new DataGridTextColumn
        //    {
        //        Header = "ID",
        //        Width = 50,
        //        FontSize = 14,
        //        Binding = new Binding("Id")
        //    };
        //    zakazkyDataGrid.Columns.Add(idColumn);

        //    var orderColumn = new DataGridTextColumn
        //    {
        //        Header = "Order",
        //        Width = 100,
        //        FontSize = 14,
        //        Binding = new Binding("Order")
        //    };
        //    zakazkyDataGrid.Columns.Add(orderColumn);

        //    var quantityColumn = new DataGridTextColumn
        //    {
        //        Header = "Quantity",
        //        Width = 100,
        //        FontSize = 14,
        //        Binding = new Binding("Quantity")
        //    };
        //    zakazkyDataGrid.Columns.Add(quantityColumn);

        //    var productColumn = new DataGridTextColumn
        //    {
        //        Header = "Product",
        //        Width = 100,
        //        FontSize = 14,
        //        Binding = new Binding("Product.Name")
        //    };
        //    zakazkyDataGrid.Columns.Add(productColumn);
        //}

        //private static void LoadData()
        //{
        //    foreach (var productionOrder in GetAllProductionOrders())
        //    {
        //        zakazkySkladnikDataGrid.Items.Add(productionOrder);
        //    }
        //}

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //spravceTabControl.Visibility = Visibility.Hidden;
            loginTextBox.Text = string.Empty;
            loginTextBox.Visibility = Visibility.Visible;
            LoginButton.Visibility = Visibility.Visible;
            LogoutButton.Visibility = Visibility.Hidden;

            SkladnikRadioButton.IsChecked = true;
            VlackarRadioButton.IsChecked = false;
            SpravceRadioButton.IsChecked = false;

            // do employee natáhne všechny skladníky, vláčkaře a správce
            ProgramState.employees = (List<Employee>)DataContext.GetType().GetProperty("Zamestnanci").GetValue(DataContext, null);
        }

        public static List<ProductionOrder> GetAllProductionOrders()
        {
            using (var db = new MaterialOrderContext())
            {
                var productionOrders = db.ProductionOrders.ToList();
                return productionOrders;
            }

            //private ObservableCollection<Authors> LoadAuthors()
            //{
            //    ObservableCollection<Authors> col = new ObservableCollection<Authors>();
            //    for (int i = 1; i < 10; i++) col.Add(new Authors() { AuthorID = i, FirstName = $"FirstName{i}", LastName = $"LastName{i}" });
            //    return col;
            //}
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (loginTextBox.Text == string.Empty)
            {
                ProgramState.Logout = true;

                _ = MessageBox.Show("Musí být vyplněno personální číslo.", "Přihlášení", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                ProgramState.Logout = false;

                ProgramState.HesloPotvrzeno = false;
                bool hesloPotvrzeno = false;
                if (SkladnikRadioButton.IsChecked == true)
                {
                    foreach (Employee employee in ProgramState.employees)
                    {
                        /// <summary>
                        /// Porovná, jestli zadané personální číslo se rovná personálnímu číslu skladníka
                        /// </summary>
                        if (employee.ProductionOfSeatsWarehouseman)
                        {
                            if (loginTextBox.Text == Convert.ToString(employee.PersonalNumber))
                            {
                                ProgramState.HesloPotvrzeno = true;
                                _ = MessageBox.Show("To je skladník.");

                                hesloPotvrzeno = true;
                                break;
                            }
                        }
                    }
                }
                if (VlackarRadioButton.IsChecked == true)
                {
                    foreach (Employee employee in ProgramState.employees)
                    {
                        /// <summary>
                        /// Porovná, jestli zadané personální číslo se rovná personálnímu číslu vláčkaře
                        /// </summary>
                        if (employee.ProductionOfSeatsWarehouseman)
                        {
                            if (loginTextBox.Text == Convert.ToString(employee.PersonalNumber))
                            {
                                ProgramState.HesloPotvrzeno = true;
                                _ = MessageBox.Show("To je vláčkař.");

                                hesloPotvrzeno = true;
                                break;
                            }
                        }
                    }
                }
                if (SpravceRadioButton.IsChecked == true)
                {
                    foreach (Employee employee in ProgramState.employees)
                    {
                        /// <summary>
                        /// Porovná, jestli zadané personální číslo se rovná personálnímu číslu správce
                        /// </summary>
                        if (employee.ProductionOfSeatsManager)
                        {
                            if (loginTextBox.Text == Convert.ToString(employee.PersonalNumber))
                            {
                                ProgramState.HesloPotvrzeno = true;
                                _ = MessageBox.Show("To je správce.");

                                hesloPotvrzeno = true;
                                break;
                            }
                        }
                    }
                }
                if (hesloPotvrzeno == true)
                {
                    vyberFunkce.Visibility = Visibility.Hidden;
                    loginLabel.Visibility = Visibility.Hidden;
                    loginTextBox.Visibility = Visibility.Hidden;
                    LoginButton.Visibility = Visibility.Hidden;
                    LogoutButton.Visibility = Visibility.Visible;
                }
                else
                {
                    vyberFunkce.Visibility = Visibility.Visible;
                    loginLabel.Visibility = Visibility.Visible;
                    loginTextBox.Visibility = Visibility.Visible;
                    LoginButton.Visibility = Visibility.Visible;
                    LogoutButton.Visibility = Visibility.Hidden;

                    _ = MessageBox.Show("Špatné heslo.", "Personální číslo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                loginTextBox.Text = string.Empty;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ProgramState.Logout = true;
            vyberFunkce.Visibility = Visibility.Visible;
            loginLabel.Visibility = Visibility.Visible;
            loginTextBox.Text = string.Empty;
            loginTextBox.Visibility = Visibility.Visible;
            LoginButton.Visibility = Visibility.Visible;
            LogoutButton.Visibility = Visibility.Hidden;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (Helper.ConvertFile.ConvertFileWithoutZero() == false)
            {
                _ = MessageBox.Show("Došlo k problému při vytváření kopie souboru.", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (Helper.ConvertFile.GetValueZakazka() == false)
                {
                    _ = MessageBox.Show("Došlo k problému při načítání zakázek", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    foreach (var z in Helper.zakazky)
                    {
                        string s = z.VyrobniZakazka;
                    }
                }
            }
        }

        private void SkladnikRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (SkladnikRadioButton.IsChecked == true)
            {
                ProgramState.SwitchRadioButton = 1;
            }
        }

        private void VlackarRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (VlackarRadioButton.IsChecked == true)
            {
                ProgramState.SwitchRadioButton = 2;
            }
        }

        private void SpravceRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (SpravceRadioButton.IsChecked == true)
            {
                ProgramState.SwitchRadioButton = 3;
            }
        }
    }
}