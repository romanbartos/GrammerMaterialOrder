using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using GrammerMaterialOrder.Utilities;
using GrammerMaterialOrder.MVVM.Models;
using GrammerMaterialOrder.MVVM.ViewModels;
using static GrammerMaterialOrder.MVVM.ViewModels.MainViewModel;

using System.Windows.Documents;
using static GrammerMaterialOrder.Utilities.Helper;

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
            //var vm = new MainViewModel();
            //DataContext = vm;

            //nastavení pro skladníka
            ProgramState.SwitchRadioButton = 1;
            ProgramState.Logout = false;
            ProgramState.HesloPotvrzeno = false;
        }

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
            using var db = new MaterialOrderContext();
            var productionOrders = db.ProductionOrders.ToList();
            return productionOrders;

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

                _ = MessageBox.Show(@"Musí být vyplněno personální číslo.", @"Přihlášení", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            ProgramState.Logout = false;

            ProgramState.HesloPotvrzeno = false;
            bool hesloPotvrzeno = false;
            bool skladnikPovolen = false;
            if (SkladnikRadioButton.IsChecked == true)
            {
                foreach (Employee employee in ProgramState.employees)
                {
                    /// <summary>
                    /// Porovná, jestli zadané personální číslo se rovná personálnímu číslu skladníka
                    /// </summary>
                    if (employee.ProductionOfSeatsWarehouseman)
                    {
                        // zkontrolovat, jestli se skladník může přihlásit na daném stanovišti
                        // jestli má naplánovaný čas vyskladňování
                        // porovnat aktuální čas s tím naplánovaným, může se o deset minut předcházet, nesmí být pozdější ani o minutu
                        ProgramState.EmployeePlanningId = -1;
                        
                        var employeePlanning = GetPlanOfEmployee(employee.Id, 1);
                        if (employeePlanning != null)
                        {
                            foreach (var ep in employeePlanning)
                            {
                                // přihlášený skladník má naplánovaný čas vyskladňování
                                skladnikPovolen = true;
                                ProgramState.EmployeePlanningId = ep.Id;
                                ProgramState.StationId = 1;
                                break;
                            }
                        }

                        if (loginTextBox.Text == Convert.ToString(employee.PersonalNumber))
                        {
                            if (skladnikPovolen == false)
                            {
                                MessageBox.Show(@"Nebyl zadán plán.", @"Upozornění", MessageBoxButton.OK, MessageBoxImage.Warning);
                                break;
                            }

                            loginLabel.Content = employee.FullName;
                            int id = employee.Id;
                            // vyhledat PlanovaniZamestnancuID podle employee.Id a stationID
                            // dočasně nastavím ID stanoviště na jedna


                            ProgramState.HesloPotvrzeno = true;
                            //_ = MessageBox.Show(@"To je skladník.");

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
                            loginLabel.Content = employee.FullName;
                            ProgramState.HesloPotvrzeno = true;
                            //_ = MessageBox.Show(@"To je vláčkař.");

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
                            loginLabel.Content = employee.FullName;
                            ProgramState.HesloPotvrzeno = true;
                            //_ = MessageBox.Show(@"To je správce.");

                            hesloPotvrzeno = true;
                            break;
                        }
                    }
                }
            }
            if (skladnikPovolen == true)
            {
                if (hesloPotvrzeno == true)
                {
                    vyberFunkce.Visibility = Visibility.Hidden;
                    //loginLabel.Visibility = Visibility.Hidden;
                    loginTextBox.Visibility = Visibility.Hidden;
                    LoginButton.Visibility = Visibility.Hidden;
                    LogoutButton.Visibility = Visibility.Visible;
                }
                else
                {
                    vyberFunkce.Visibility = Visibility.Visible;
                    //loginLabel.Visibility = Visibility.Visible;
                    loginTextBox.Visibility = Visibility.Visible;
                    LoginButton.Visibility = Visibility.Visible;
                    LogoutButton.Visibility = Visibility.Hidden;

                    _ = MessageBox.Show(@"Zadané heslo není platné.", @"Personální číslo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            // vyprázdním pole se zadaným personálním číslem
            loginTextBox.Text = string.Empty;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ProgramState.Logout = true;
            vyberFunkce.Visibility = Visibility.Visible;
            //loginLabel.Visibility = Visibility.Visible;
            loginTextBox.Text = string.Empty;
            loginTextBox.Visibility = Visibility.Visible;
            LoginButton.Visibility = Visibility.Visible;
            LogoutButton.Visibility = Visibility.Hidden;

            loginLabel.Content = "Personální číslo";

        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (Helper.ConvertFile.ConvertFileWithoutZero() == false)
            {
                _ = MessageBox.Show(@"Došlo k problému při vytváření kopie souboru.", @"Upozornění", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (Helper.ConvertFile.GetValueZakazka() == false)
            {
                _ = MessageBox.Show(@"Došlo k problému při načítání zakázek", @"Upozornění", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // zakázky z databáze, které se musí provést
            var zak = MainViewModel.GetZakazky();

            using MaterialOrderContext db = new();

            // vyberu nepřidělené plány a spolu s ID objednávky založím do tabulky ObjednavkaPlanZam
            var plany = MainViewModel.GetPlanId();

            // zakázky načtené ze souboru (jsou to ty, které se musí udělat
            foreach (var z in Helper.zakazky)
            {
                string zakazka = z.VyrobniZakazka;

                var vyhledanaZakazka = zak.Find(x => x.Order == zakazka);
                if (vyhledanaZakazka == null)
                {
                    // zakázka nebyla vyhledána v databázi
                    // bude založena do tabulky

                    // vyhledám Id výrobku
                    string vyrobek = z.Vyrobek;
                    var vyrobekId = MainViewModel.GetVyrobekId(vyrobek);
                    // pokud bude vyrobekId null, tak není daný výrobek uložen v databázi
                    // jak bude uložen nový výrobek do databáze

                    if (vyrobekId == null)
                    {
                        // jak bude zareagováno, musí se nahrát nový výrobek ze SAPu
                    }
                    else
                    {
                        int pocetKusu = z.PocetKusu;
                        string cisloDisponenta = z.CisloDisponenta;
                        string cisloSedacky = z.CisloSedacky;

                        ProductionOrder productionOrder = new()
                        {
                            Order = zakazka,
                            Quantity = pocetKusu,
                            ProductId = vyrobekId[0].Id,
                            StateObject = 1
                        };

                        // uložím novou zakázku
                        _ = db.ProductionOrders.Add(productionOrder);
                        _ = db.SaveChanges();

                        // do tabulky ObjednavkaPlanZam zapíšu novou zakázku pro všechny stanoviště
                        int id = productionOrder.Id;

                        foreach (var p in plany)
                        {
                            ProdOrderEmployeePlan prodOrderEmplPlan = new()
                            {
                                ProductionOrderId = id,
                                StationId = p.StationId,
             
                                Done = false,
                                Note = null,
                                StateObject = 1
                            };
                            //EmployeePlanningId = null,

                            // uložím novou zakázku pro vybrané stanoviště
                            _ = db.ProdOrdersEmployeePlan.Add(prodOrderEmplPlan);
                            _ = db.SaveChanges();
                        }
                    }
                }

                // projedu seznam uložených zakázek
                // nové zakázky uložím do tabulky Objednavka
            }
        }

        private void SkladnikRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (SkladnikRadioButton.IsChecked == true)
            {
                ProgramState.SwitchRadioButton = 1;
                vyberStanoviste.Visibility = Visibility.Visible;
            }
        }

        private void VlackarRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (VlackarRadioButton.IsChecked == true)
            {
                ProgramState.SwitchRadioButton = 2;
                vyberStanoviste.Visibility = Visibility.Hidden;
            }
        }

        private void SpravceRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (SpravceRadioButton.IsChecked == true)
            {
                ProgramState.SwitchRadioButton = 3;
                vyberStanoviste.Visibility = Visibility.Hidden;
            }
        }
    }
}