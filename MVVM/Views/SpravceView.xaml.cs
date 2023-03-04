using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System.Runtime.InteropServices;

using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;

using System.IO;
using System.Linq;
using NPOI.Util;
using System.Windows.Media.Media3D;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using NPOI.SS.Formula.Functions;
using static NPOI.POIFS.Crypt.CryptoFunctions;
using NPOI.HPSF;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Numerics;

using GrammerMaterialOrder.MVVM.Models;
using GrammerMaterialOrder.MVVM.ViewModels;
using GrammerMaterialOrder.Utilities;
using static GrammerMaterialOrder.MVVM.ViewModels.SpravceViewModel;
using NPOI.DDF;
using System.Collections.Immutable;
using static System.Net.Mime.MediaTypeNames;
using NPOI.HSSF.Util;

namespace GrammerMaterialOrder.MVVM.Views
{
    /// <summary>
    /// Interakční logika pro SpravceView.xaml
    /// </summary>
    public partial class SpravceView : UserControl
    {
        // nový plán = 1; oprava plánu = 2
        private byte novyPlanOpravaPlanu;

        private List<CasOdDo> casNaplanovaniOd = new();
        private List<CasOdDo> casNaplanovaniDo = new();

        private struct CasOdDo
        {
            public string CasNaplanovaniOdDoHodnota { get; set; }
            public string CasNaplanovaniOdDo { get; set; }
        }

        public SpravceView()
        {
            //DataContext = this;
            InitializeComponent();
            //SpravceViewModel vm = new();
            //DataContext = vm;
            NoveStanovisteAdminRadioButton.IsChecked = true;
            OpravaStanovisteAdminRadioButton.IsChecked = false;
            ProgramState.NoveStanovisteAdminRadioButton = true;
            ProgramState.OpravaStanovisteAdminRadioButton = false;
            StanovisteAdministraceComboBox.IsEnabled = false;

            OdebratStanovisteAdministrace.IsEnabled = false;
            StanovisteAdministraceTextBox.IsEnabled = false;
            UlozitOpravaStanovisteAdministrace.IsEnabled = false;

            AktivovatStanovisteAdministraceComboBox.IsEnabled = false;
            AktivovatStanovisteAdministrace.IsEnabled = false;

            SkladnikVolbaFunkceAdminRadioButton.IsChecked = true;
            SpravceVolbyFunkceAdminRadioButton.IsChecked = false;
            UlozitNoveStanovisteAdministrace.IsEnabled = false;
            ProgramState.HesloPotvrzeno = false;

            novyPlanOpravaPlanu = 1;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //if (SkladnikPlanovacComboBox.Items.Count > 0)
            //{
            //    SkladnikPlanovacComboBox.SelectedIndex = 0;
            //}
            //if (StanovistePlanovacComboBox.Items.Count > 0)
            //{
            //    StanovistePlanovacComboBox.SelectedIndex = 0;
            //}
            //if (VolbaStanovisteMaterialOdebratComboBox.Items.Count > 0)
            //{
            //    VolbaStanovisteMaterialOdebratComboBox.SelectedIndex = 0;
            //}
            if (StanovisteAdministraceComboBox.Items.Count > 0)
            {
                //StanovisteAdministraceComboBox.SelectedIndex = 0;
                if (StanovisteAdministraceComboBox.SelectedValue != null)
                {
                    Station textComboBox = (Station)StanovisteAdministraceComboBox.SelectedItem;
                    StanovisteAdministraceTextBox.Text = textComboBox.Name;
                }
            }

            // nastavení handlerů pro comboboxy na kartě administrace
            StanovisteAdministraceComboBox.SelectionChanged += new SelectionChangedEventHandler(StanovisteAdministraceComboBoxSelectionChanged);
            AktivovatStanovisteAdministraceComboBox.SelectionChanged += new SelectionChangedEventHandler(AktivovatStanovisteAdministraceComboBoxSelectionChanged);

            // nastavení handlerů pro combobox na kartě materiál na stanoviště
            VolbaStanovisteMaterialPridelenoComboBox.SelectionChanged += new SelectionChangedEventHandler(VolbaStanovisteMaterialPridelenoComboBoxSelectionChanged);
        }

        #region Plánovač
        private void CasPlanovacOdComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 24; i++)
            {
                casNaplanovaniOd.Add(new() { CasNaplanovaniOdDoHodnota = i.ToString("D2") + ":" + "00",
                                            CasNaplanovaniOdDo = i.ToString("D2") + ":" + "00" });
                casNaplanovaniOd.Add(new() { CasNaplanovaniOdDoHodnota = i.ToString("D2") + ":" + "30",
                                            CasNaplanovaniOdDo = i.ToString("D2") + ":" + "30" });
                //if (i % 2 == 0)
                //{
                //    casNaplanovaniOd.Add(new() { IndexCasNaplanovaniOdDo = i, CasNaplanovaniOdDo = i.ToString("D2") + ":" + "00" });
                //}
                //else
                //{
                //    casNaplanovaniOd.Add(new() { IndexCasNaplanovaniOdDo = i, CasNaplanovaniOdDo = i.ToString("D2") + ":" + "30" });
                //}
            }

            var combo = sender as ComboBox;
            combo.ItemsSource = casNaplanovaniOd;
            combo.SelectedIndex = 0;
        }

        private void CasPlanovacDoComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 24; i++)
            {
                casNaplanovaniDo.Add(new() { CasNaplanovaniOdDoHodnota = i.ToString("D2") + ":" + "00",
                                            CasNaplanovaniOdDo = i.ToString("D2") + ":" + "00" });
                casNaplanovaniDo.Add(new() { CasNaplanovaniOdDoHodnota = i.ToString("D2") + ":" + "30",
                                            CasNaplanovaniOdDo = i.ToString("D2") + ":" + "30" });
            }

            var combo = sender as ComboBox;
            combo.ItemsSource = casNaplanovaniDo;
            combo.SelectedIndex = 0;
        }

        private void UlozitPlanButton_Click(object sender, RoutedEventArgs e)
        {
            bool ulozit = true;

            int employeeId = 0;
            if (SkladnikPlanovacComboBox.SelectedValue == null)
            {
                ulozit = false;
                _ = MessageBox.Show(@"Nebyl vybrán skladník.", @"Upozornění", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                employeeId = Convert.ToInt32(SkladnikPlanovacComboBox.SelectedValue);
            }

            int stationId = 0;
            if (StanovistePlanovacComboBox.SelectedValue == null)
            {
                ulozit = false;
                _ = MessageBox.Show(@"Nebylo vybráno stanoviště.", @"Upozornění", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                stationId = Convert.ToInt32(StanovistePlanovacComboBox.SelectedValue);
            }

            if (CasovaZnackaDatePicker.SelectedDate == null)
            {
                ulozit = false;
                _ = MessageBox.Show(@"Nebyl vybrán datum naplánování.", @"Upozornění", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            var casOd = DateTime.Parse(CasPlanovacOdComboBox.SelectedValue.ToString());
            var casDo = DateTime.Parse(CasPlanovacDoComboBox.SelectedValue.ToString());
            if (casDo <= casOd)
            {
                ulozit = false;
                _ = MessageBox.Show(@"Čas Do musí být větší než Od.", @"Upozornění", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            // ještě musím zkontrolovat, jestli na stejné stanoviště neukládám podobná data
            DateTime TimeStampFrom = Convert.ToDateTime(CasovaZnackaDatePicker.SelectedDate.Value.ToShortDateString() + " " + CasPlanovacOdComboBox.SelectedValue.ToString());
            DateTime TimeStampTo = Convert.ToDateTime(CasovaZnackaDatePicker.SelectedDate.Value.ToShortDateString() + " " + CasPlanovacDoComboBox.SelectedValue.ToString());

            // zkongtrolovat, jestli se nepřekrývá plán
            // test, jestli se náhodou nový plán nepřekrývá s novým
            // nesmí se shodovat stanoviště a překrývat čas
            Station stanovisteItem = (Station)StanovistePlanovacComboBox.SelectedItem;
            string stanovisteNazev = stanovisteItem.Name;
            // výběr dat na základě datumu a stanoviště
            var vybranaData = SpravceViewModel.GetSelectedSchedule(Convert.ToDateTime(CasovaZnackaDatePicker.SelectedDate.Value.ToShortDateString()), stanovisteNazev);

            foreach (var item in vybranaData)
            {
                // budu testovat průnik časových intervalů
                if ((TimeStampFrom >= item.Schedule.TimeStampFrom && TimeStampFrom < item.Schedule.TimeStampTo) ||
                    (TimeStampTo > item.Schedule.TimeStampFrom && TimeStampTo <= item.Schedule.TimeStampTo))
                {
                    ulozit = false;
                    MessageBox.Show(@"Uvedený časový interval se překrývá s již uloženým.", @"Upozornění", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            if (ulozit == false)
            {
                return;
            }

            //var planovaniZamestnancu = SpravceViewModel.GetPlanovaniZamestnancu(Convert.ToInt32(SkladnikPlanovacComboBox.SelectedValue),
            //    Convert.ToInt32(StanovistePlanovacComboBox.SelectedValue));

            // uložit nový plán
            if (novyPlanOpravaPlanu == 1)
            {
                using MaterialOrderContext db = new();

                EmployeePlanning employeePlanning = new()
                {
                    EmployeeId = employeeId,
                    StationId = stationId,
                    TimeStampFrom = Convert.ToDateTime(CasovaZnackaDatePicker.SelectedDate.Value.ToShortDateString() + " " + CasPlanovacOdComboBox.SelectedValue.ToString()),
                    TimeStampTo = Convert.ToDateTime(CasovaZnackaDatePicker.SelectedDate.Value.ToShortDateString() + " " + CasPlanovacDoComboBox.SelectedValue.ToString()),
                    StateObject = 1
                };
                _ = db.EmployeePlanning.Add(employeePlanning);
                _ = db.SaveChanges();

                // aktualizace DGV
                ((SpravceViewModel)this.DataContext).GetSchedule();
            }
            // aktualizace plánu 
            if (novyPlanOpravaPlanu == 2)
            {
                using MaterialOrderContext db = new();

                DataSchedule schedule = (DataSchedule)PlanovacDataGrid.SelectedItem;

                int pracovisteId = schedule.ScheduleId;

                var scheduleUpdate = db.EmployeePlanning.Where(ep => ep.Id == pracovisteId).First();
                scheduleUpdate.EmployeeId = Convert.ToInt32(SkladnikPlanovacComboBox.SelectedValue);
                scheduleUpdate.StationId = Convert.ToInt32(StanovistePlanovacComboBox.SelectedValue);
                scheduleUpdate.TimeStampFrom = Convert.ToDateTime(CasovaZnackaDatePicker.SelectedDate.Value.ToShortDateString() + " " + CasPlanovacOdComboBox.SelectedValue.ToString());
                scheduleUpdate.TimeStampTo = Convert.ToDateTime(CasovaZnackaDatePicker.SelectedDate.Value.ToShortDateString() + " " + CasPlanovacDoComboBox.SelectedValue.ToString());

                _ = db.EmployeePlanning.Update(scheduleUpdate);
                _ = db.SaveChanges();

                ((SpravceViewModel)this.DataContext).GetSchedule();
            }
        }

        private void NovyPlanButton_Click(object sender, RoutedEventArgs e)
        {
            novyPlanOpravaPlanu = 1;

            SkladnikPlanovacComboBox.Text = "Vybrat skladníka";
            StanovistePlanovacComboBox.Text = "Vybrat stanoviště";
            CasovaZnackaDatePicker.SelectedDate = null;

            CasPlanovacOdComboBox.SelectedValue = "00:00";
            CasPlanovacDoComboBox.SelectedValue = "00:00";
        }

        private void AktualizovatPlanButton_Click(object sender, RoutedEventArgs e)
        {
            if (PlanovacDataGrid.SelectedItem == null)
            {
                MessageBox.Show(@"Nebyl vybrán žádný plán.", @"Upozornění", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            novyPlanOpravaPlanu = 2;

            DataSchedule schedule = (DataSchedule)PlanovacDataGrid.SelectedItem;

            SkladnikPlanovacComboBox.SelectedValue = schedule.EmployeeId;
            StanovistePlanovacComboBox.SelectedValue = schedule.StationId;
            CasovaZnackaDatePicker.SelectedDate = DateTime.Parse(schedule.Schedule.TimeStampFrom.ToShortDateString());
            string s = schedule.Schedule.TimeStampFrom.ToShortTimeString();
            string[] words = s.Split(':');
            string hodina = words[0];
            if (hodina.Length == 1)
            {
                s = s.Insert(0, "0");
            }
            CasPlanovacOdComboBox.SelectedValue = s;

            s = schedule.Schedule.TimeStampTo.ToShortTimeString();
            words = s.Split(':');
            hodina = words[0];
            if (hodina.Length == 1)
            {
                s = s.Insert(0, "0");
            }
            CasPlanovacDoComboBox.SelectedValue = s;

            //if (PlanovacDataGrid.SelectedItem == null) return;
        }

        private void OdebratNovyPlanButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show(@"Chcete opravdu odebrat vybraný plán.", @"Plánování skladníků", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dialogResult == MessageBoxResult.Yes)
            {
                if (PlanovacDataGrid.SelectedIndex != -1)
                {
                    using MaterialOrderContext db = new();

                    DataSchedule schedule = (DataSchedule)PlanovacDataGrid.SelectedItem;

                    var employeePlanning = db.EmployeePlanning.Where(ep => ep.Id == schedule.ScheduleId).First();
                    employeePlanning.StateObject = 2;

                    _ = db.EmployeePlanning.Update(employeePlanning);
                    _ = db.SaveChanges();

                    ((SpravceViewModel)this.DataContext).GetSchedule();
                }
                else
                {
                    MessageBox.Show(@"Nebyl vybrán žádný řádek.", @"Plánování skladníků", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (dialogResult == MessageBoxResult.No)
            {
                //do something else
            }
        }
        #endregion

        #region Administrace stanovišť a skladníků
        private void NoveStanovisteAdminRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ProgramState.NoveStanovisteAdminRadioButton = true;
            ProgramState.OpravaStanovisteAdminRadioButton = false;

            if (NoveStanovisteAdminRadioButton.IsChecked == true)
            {
                StanovisteAdministraceComboBox.IsEnabled = false;
                StanovisteAdministraceComboBox.Text = "Vybrat stanoviště";
                StanovisteAdministraceTextBox.Text = string.Empty;

                OdebratStanovisteAdministrace.IsEnabled = false;
                StanovisteAdministraceTextBox.IsEnabled = false;
                UlozitOpravaStanovisteAdministrace.IsEnabled = false;

                AktivovatStanovisteAdministraceComboBox.IsEnabled = false;
                AktivovatStanovisteAdministrace.IsEnabled = false;
            }
        }

        private void OpravaStanovisteAdminRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ProgramState.NoveStanovisteAdminRadioButton = false;
            ProgramState.OpravaStanovisteAdminRadioButton = true;

            if (OpravaStanovisteAdminRadioButton.IsChecked == true)
            {
                StanovisteAdministraceComboBox.IsEnabled = true;

                if (StanovisteAdministraceComboBox.SelectedValue != null)
                {
                    OdebratStanovisteAdministrace.IsEnabled = true;
                    StanovisteAdministraceTextBox.IsEnabled = true;
                    UlozitOpravaStanovisteAdministrace.IsEnabled = true;
                    Station textComboBox = (Station)StanovisteAdministraceComboBox.SelectedItem;
                    StanovisteAdministraceTextBox.Text = textComboBox.Name;
                }
                if (AktivovatStanovisteAdministraceComboBox.Items.Count > 0)
                {

                    if (AktivovatStanovisteAdministraceComboBox.SelectedIndex == -1)
                    {
                        AktivovatStanovisteAdministraceComboBox.IsEnabled = true;
                        AktivovatStanovisteAdministrace.IsEnabled = false;
                    }
                    else
                    {
                        AktivovatStanovisteAdministraceComboBox.IsEnabled = true;
                        AktivovatStanovisteAdministrace.IsEnabled = true;
                    }
                }
                if (AktivovatStanovisteAdministraceComboBox.Items.Count == 0)
                {
                    AktivovatStanovisteAdministraceComboBox.IsEnabled = false;
                    AktivovatStanovisteAdministrace.IsEnabled = false;
                }
            }
        }

        private void UlozitNoveStanovisteAdministrace_Click(object sender, RoutedEventArgs e)
        {
            using MaterialOrderContext db = new();

            Station newStation = new()
            {
                Name = NoveStanovisteAdministraceTextBox.Text,
                StateObject = 1
            };

            _ = db.Stations.Add(newStation);
            _ = db.SaveChanges();

            ((SpravceViewModel)this.DataContext).AddNewStation();
            NoveStanovisteAdministraceTextBox.Text = "";
            UlozitNoveStanovisteAdministrace.IsEnabled = false;
        }

        private void UlozitOpravaStanovisteAdministrace_Click(object sender, RoutedEventArgs e)
        {
            var index = StanovisteAdministraceComboBox.SelectedIndex;

            using MaterialOrderContext db = new();

            if (OpravaStanovisteAdminRadioButton.IsChecked == true)
            {
                var stanoviste = db.Stations.Where(s => s.Id == Convert.ToInt32(StanovisteAdministraceComboBox.SelectedValue)).First();
                stanoviste.Name = StanovisteAdministraceTextBox.Text;

                _ = db.Stations.Update(stanoviste);
                _ = db.SaveChanges();
            }

            //BindingExpression binding = BindingOperations.GetBindingExpression(StanovisteAdministraceTextBox, TextBox.TextProperty);
            //binding.UpdateSource();
            ((SpravceViewModel)this.DataContext).AddNewStation();

            StanovisteAdministraceComboBox.SelectedIndex = -1;
            StanovisteAdministraceComboBox.SelectedIndex = index;
        }

        private void OdebratStanovisteAdministrace_Click(object sender, RoutedEventArgs e)
        {
            var index = StanovisteAdministraceComboBox.SelectedIndex;

            using MaterialOrderContext db = new();

            var stanoviste = db.Stations.Where(s => s.Id == Convert.ToInt32(StanovisteAdministraceComboBox.SelectedValue)).First();
            stanoviste.StateObject = 2;

            _ = db.Stations.Update(stanoviste);
            _ = db.SaveChanges();

            ((SpravceViewModel)this.DataContext).AddNewStation();

            StanovisteAdministraceComboBox.SelectedIndex = -1;
            if (StanovisteAdministraceComboBox.Items.Count == 0)
            {
                StanovisteAdministraceComboBox.SelectedIndex = -1;
                StanovisteAdministraceComboBox.Text = "Vybrat stanoviště";
                OdebratStanovisteAdministrace.IsEnabled = false;
                StanovisteAdministraceTextBox.IsEnabled = false;
                UlozitOpravaStanovisteAdministrace.IsEnabled = false;
            }
            if (StanovisteAdministraceComboBox.Items.Count > 0)
            {
                if (index == StanovisteAdministraceComboBox.Items.Count)
                {
                    StanovisteAdministraceComboBox.SelectedIndex = index - 1;
                }
                else
                {
                    StanovisteAdministraceComboBox.SelectedIndex = index;
                }
            }

            if (AktivovatStanovisteAdministraceComboBox.Items.Count == -1)
            {
                ((SpravceViewModel)this.DataContext).ActivateStation();
            }
            if (AktivovatStanovisteAdministraceComboBox.Items.Count >= 0)
            {
                var selectedValue = AktivovatStanovisteAdministraceComboBox.SelectedValue;
                ((SpravceViewModel)this.DataContext).ActivateStation();
                AktivovatStanovisteAdministraceComboBox.SelectedValue = selectedValue;
            }
            if (AktivovatStanovisteAdministraceComboBox.Items.Count > 0)
            {
                AktivovatStanovisteAdministraceComboBox.IsEnabled = true;
                AktivovatStanovisteAdministrace.IsEnabled = true;
            }
            if (AktivovatStanovisteAdministraceComboBox.Items.Count == 0)
            {
                AktivovatStanovisteAdministraceComboBox.IsEnabled = false;
                AktivovatStanovisteAdministrace.IsEnabled = false;
            }
        }

        private void AktivovatStanovisteAdministrace_Click(object sender, RoutedEventArgs e)
        {
            var index = AktivovatStanovisteAdministraceComboBox.SelectedIndex;

            using MaterialOrderContext db = new();

            var stanoviste = db.Stations.Where(s => s.Id == Convert.ToInt32(AktivovatStanovisteAdministraceComboBox.SelectedValue)).First();
            stanoviste.StateObject = 1;

            _ = db.Stations.Update(stanoviste);
            _ = db.SaveChanges();

            ((SpravceViewModel)this.DataContext).ActivateStation();

            AktivovatStanovisteAdministraceComboBox.SelectedIndex = -1;
            if (AktivovatStanovisteAdministraceComboBox.Items.Count == 0)
            {
                AktivovatStanovisteAdministraceComboBox.SelectedIndex = -1;
                AktivovatStanovisteAdministraceComboBox.Text = "Vybrat stanoviště";
                AktivovatStanovisteAdministrace.IsEnabled = false;
            }
            if (AktivovatStanovisteAdministraceComboBox.Items.Count > 0)
            {
                if (index == AktivovatStanovisteAdministraceComboBox.Items.Count)
                {
                    AktivovatStanovisteAdministraceComboBox.SelectedIndex = index - 1;
                }
                else
                {
                    AktivovatStanovisteAdministraceComboBox.SelectedIndex = index;
                }
            }

            if (StanovisteAdministraceComboBox.SelectedIndex >= 0)
            {
                var selectedValue = StanovisteAdministraceComboBox.SelectedValue;
                ((SpravceViewModel)this.DataContext).AddNewStation();
                StanovisteAdministraceComboBox.SelectedValue = selectedValue;
            }
            if (StanovisteAdministraceComboBox.SelectedIndex == -1)
            {
                ((SpravceViewModel)this.DataContext).AddNewStation();
                StanovisteAdministraceComboBox.SelectedIndex = -1;
            }

            if (AktivovatStanovisteAdministraceComboBox.Items.Count > 0)
            {
                AktivovatStanovisteAdministraceComboBox.IsEnabled = true;
                AktivovatStanovisteAdministrace.IsEnabled = true;
            }
            if (AktivovatStanovisteAdministraceComboBox.Items.Count == 0)
            {
                AktivovatStanovisteAdministraceComboBox.IsEnabled = false;
                AktivovatStanovisteAdministrace.IsEnabled = false;
            }
        }

        private void NoveStanovisteAdministraceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NoveStanovisteAdministraceTextBox.Text.Length == 0)
            {
                UlozitNoveStanovisteAdministrace.IsEnabled = false;
            }
            else
            {
                UlozitNoveStanovisteAdministrace.IsEnabled = true;
            }
        }

        private void StanovisteAdministraceComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OpravaStanovisteAdminRadioButton.IsEnabled == true && StanovisteAdministraceComboBox.SelectedValue != null)
            {
                OdebratStanovisteAdministrace.IsEnabled = true;
                StanovisteAdministraceTextBox.IsEnabled = true;
                UlozitOpravaStanovisteAdministrace.IsEnabled = true;
            }
        }

        private void AktivovatStanovisteAdministraceComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OpravaStanovisteAdminRadioButton.IsEnabled == true && AktivovatStanovisteAdministraceComboBox.SelectedValue != null)
            {
                AktivovatStanovisteAdministrace.IsEnabled = true;
            }
        }

        private void OdebratSpravceAdministraceButton_Click(object sender, RoutedEventArgs e)
        {
            using EmployeeContext db = new();

            var odebratManazera = db.Employees.Where(z => z.Id == Convert.ToInt32(SpravciAdministraceComboBox.SelectedValue)).First();
            odebratManazera.ProductionOfSeatsManager = false;

            _ = db.Employees.Update(odebratManazera);
            _ = db.SaveChanges();
        }

        private void OdebratSkladnikaAdministraceButton_Click(object sender, RoutedEventArgs e)
        {
            using EmployeeContext db = new();

            var odebratSkladnika = db.Employees.Where(z => z.Id == Convert.ToInt32(SkladniciAdministraceComboBox.SelectedValue)).First();
            odebratSkladnika.ProductionOfSeatsWarehouseman = false;

            _ = db.Employees.Update(odebratSkladnika);
            _ = db.SaveChanges();
        }

        private void PridatSkladnikaManazeraAdministraceButton_Click(object sender, RoutedEventArgs e)
        {
            using EmployeeContext db = new();

            var pridatSkladnikaManazera = db.Employees.Where(z => z.Id == Convert.ToInt32(ZamestnanciAdministratceComboBox.SelectedValue)).First();
            if (SkladnikVolbaFunkceAdminRadioButton.IsChecked == true)
            {
                pridatSkladnikaManazera.ProductionOfSeatsWarehouseman = true;
            }
            if (SpravceVolbyFunkceAdminRadioButton.IsChecked == true)
            {
                pridatSkladnikaManazera.ProductionOfSeatsManager = true;
            }

            _ = db.Employees.Update(pridatSkladnikaManazera);
            _ = db.SaveChanges();
        }
        #endregion

        #region Materiál na stanoviště
        private void UlozitPridatMaterialButton_Click(object sender, RoutedEventArgs e)
        {
            using MaterialOrderContext db = new();

            int stationId = Convert.ToInt32(VolbaStanovisteMaterialPridatComboBox.SelectedValue);
            //PridatMaterialDataGrid.Items.Count
            for (int i = 0; i < PridatMaterialDataGrid.Items.Count; i++)
            {
                DataGridRow row = (DataGridRow)PridatMaterialDataGrid.ItemContainerGenerator.ContainerFromIndex(i);
                if(row != null)
                {
                    var radekData = (DataMFS)row.Item;

                    int materialId = radekData.MaterialId;
                    string materialName = radekData.MaterialName;
                    bool materialAddToStation = radekData.MaterialPridatOdebrat;

                    if (materialAddToStation == true)
                    {
                        // vyhledám již přidělený materiál, který má nastavenou hodnotu stavObjektu na 2
                        // if (db.MaterialForStation.Where(mfs => mfs.MaterialId == materialId && mfs.StationId == stationId && mfs.StateObject == 2).Any() == true)
                        if (db.MaterialForStation.Where(mfs => mfs.MaterialId == materialId && mfs.StateObject == 2).Any() == true)
                        {
                            //var odebratMaterial = db.MaterialForStation.Where(mfs => mfs.MaterialId == materialId && mfs.StationId == stationId && mfs.StateObject == 2).First();
                            var odebratMaterial = db.MaterialForStation.Where(mfs => mfs.MaterialId == materialId && mfs.StateObject == 2).First();
                            odebratMaterial.StationId = stationId;
                            odebratMaterial.StateObject = 1;

                            _ = db.MaterialForStation.Update(odebratMaterial);
                            _ = db.SaveChanges();
                        }
                        else
                        {
                            //tady uložím nový materiál na vybrané stanoviště
                            MaterialForStation materialForStation = new()
                            {
                                MaterialId = materialId,
                                StationId = Convert.ToInt32(VolbaStanovisteMaterialPridatComboBox.SelectedValue),
                                StateObject = 1
                            };

                            _ = db.MaterialForStation.Add(materialForStation);
                            _ = db.SaveChanges();
                        }
                    }
                }
            }

            // zaktualizovat PridatMaterialDataGrid
            if (VolbaStanovisteMaterialPridelenoComboBox.SelectedValue != null)
            {
                var stanoviste = db.Stations.Where(s => s.Id == Convert.ToInt32(VolbaStanovisteMaterialPridelenoComboBox.SelectedValue)).First();
                ((SpravceViewModel)this.DataContext).GetMaterialForStationPrideleno(stanoviste.Id);
            }

            // aktualizace DGV PridelenyMaterialDataGrid
            ((SpravceViewModel)this.DataContext).GetMaterialForStationPridelit();
        }

        private void UlozitOdebratMaterialButton_Click(object sender, RoutedEventArgs e)
        {
            using MaterialOrderContext db = new();

            int stationId = Convert.ToInt32(VolbaStanovisteMaterialPridelenoComboBox.SelectedValue);
            //PridatMaterialDataGrid.Items.Count
            for (int i = 0; i < PridelenyMaterialDataGrid.Items.Count; i++)
            {
                DataGridRow row = (DataGridRow)PridelenyMaterialDataGrid.ItemContainerGenerator.ContainerFromIndex(i);
                if (row != null)
                {
                    var radekData = (DataMFSPrideleno)row.Item;

                    int materialId = radekData.MaterialId;
                    bool materialAddToStation = radekData.MaterialPridatOdebrat;

                    if (materialAddToStation == true)
                    {
                        var odebratMaterial = db.MaterialForStation.Where(mfs => mfs.MaterialId == materialId && mfs.StationId == stationId).First();
                        odebratMaterial.StateObject = 2;

                        _ = db.MaterialForStation.Update(odebratMaterial);
                        _ = db.SaveChanges();
                    }
                }
            }

            // odebere vybraný materiál z DGV
            var stanoviste = db.Stations.Where(s => s.Id == Convert.ToInt32(VolbaStanovisteMaterialPridelenoComboBox.SelectedValue)).First();
            ((SpravceViewModel)this.DataContext).GetMaterialForStationPrideleno(stanoviste.Id);

            // přidá vybraný materiál do DGV
            ((SpravceViewModel)this.DataContext).GetMaterialForStationPridelit();

            if (ProgramState.RadkyMaterialPridelenDGV == false)
            {
                UlozitOdebratMaterialButton.IsEnabled = false;
            }
            else
            {
                UlozitOdebratMaterialButton.IsEnabled = true;
            }
        }

        private void VolbaStanovisteMaterialPridelenoComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var index = StanovisteAdministraceComboBox.SelectedIndex;

            if (VolbaStanovisteMaterialPridelenoComboBox.SelectedIndex == -1)
            {
                return;
            }

            using MaterialOrderContext db = new();

            var stanoviste = db.Stations.Where(s => s.Id == Convert.ToInt32(VolbaStanovisteMaterialPridelenoComboBox.SelectedValue)).First();

            ((SpravceViewModel)this.DataContext).GetMaterialForStationPrideleno(stanoviste.Id);

            //if (ProgramState.RadkyPridelitMaterialDGV == false)
            //{
            //    UlozitPridatMaterialButton.IsEnabled = false;
            //}
            //else
            //{
            //    UlozitPridatMaterialButton.IsEnabled = true;
            //}
            if (ProgramState.RadkyMaterialPridelenDGV == false)
            {
                UlozitOdebratMaterialButton.IsEnabled = false;
            }
            else
            {
                UlozitOdebratMaterialButton.IsEnabled = true;
            }
        }

        #endregion
        //private void SkladnikComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (SkladnikComboBox.SelectedItem != null)
        //    {
        //        ComboBoxItem cbi = (ComboBoxItem)SkladnikComboBox.SelectedItem;
        //        string selectedText = cbi.Content.ToString();
        //        MessageBox.Show(selectedText);
        //    }
        //}

        #region Import výrobků
        protected virtual bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }
        private void ImportVyrobkuButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> seznamVyrobkuExcel = new();
            List<string> seznamMaterialuExcel = new();

            _ = MessageBox.Show(@"Import dat bude zahájen.", @"Import dat", MessageBoxButton.OK, MessageBoxImage.Information);

            string filePath = Helper.path + @"ZPP_BESTK.XLSX";

            if (!System.IO.File.Exists(filePath))
            {
                MessageBox.Show(@"Soubor neexistuje.", @"Upozornění", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            DirectoryInfo di = new (Helper.path);
            // Create an array representing the files in the directory.
            FileInfo[] fi = di.GetFiles();

            // kontrola, jestli je soubor otevřen
            foreach (var f in fi)
            {
                // test na otevření souboru
                if (IsFileLocked(f) == true)
                {
                    if (f.FullName == filePath)
                    {
                        MessageBox.Show(@"Pro pokračování musí být soubor nejdřív uzavřen.", @"Upozornění", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }
            }

            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                int pocetRadek = 0;
                string strExt = System.IO.Path.GetExtension(filePath);

                IWorkbook wb;

                #region Check extension to define the Workbook
                if (strExt.Equals(".xls"))
                {
                    // formát xls
                    wb = new HSSFWorkbook(file);
                }
                else
                {
                    // formát xlsx
                    wb = new XSSFWorkbook(file);
                }
                #endregion

                ISheet sheet = wb.GetSheetAt(0);//Start reading at index 0

                List<VyrobekMaterialy> vyrobkyMaterialy = new();

                // v první řádce jsou názvy sloupců, proto začínám od 1
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);

                    ICell cellVyrobek = row.GetCell(1);
                    if (cellVyrobek == null)
                    {
                        continue;
                    }
                    object cellValueVyrobek = null;
                    cellValueVyrobek = cellVyrobek.StringCellValue;
                    
                    ICell cellMaterial = row.GetCell(2);
                    object cellValueMaterial = null;
                    cellValueMaterial = cellMaterial.StringCellValue;

                    // pokud by byla jedna z buněk prázdná, budu pokračovat další řádkou
                    if (cellVyrobek.CellType == CellType.Blank || cellMaterial.CellType == CellType.Blank)
                        continue;

                    if (seznamVyrobkuExcel.IndexOf(Convert.ToString(cellValueVyrobek)) == -1)
                    {
                        // nový výrobek
                        vyrobkyMaterialy.Add(new VyrobekMaterialy() { Vyrobek = Convert.ToString(cellValueVyrobek) });
                        seznamVyrobkuExcel.Add(Convert.ToString(cellValueVyrobek));
                        var lastItem = vyrobkyMaterialy.LastOrDefault();

                        lastItem.Materialy = new()
                            {
                                Convert.ToString(cellValueMaterial)
                            };

                        pocetRadek++;
                    }
                    else
                    {
                        // již existující výrobek
                        int index = vyrobkyMaterialy.FindIndex(vm => vm.Vyrobek == Convert.ToString(cellValueVyrobek));
                        if (index != -1)
                        {
                            vyrobkyMaterialy[index].Materialy.Add(Convert.ToString(cellValueMaterial));
                            pocetRadek++;
                        }
                    }

                    if (seznamMaterialuExcel.IndexOf(Convert.ToString(cellValueMaterial)) == -1)
                    {
                        seznamMaterialuExcel.Add(Convert.ToString(cellValueMaterial));
                    }
                }

                //using MaterialOrderContext db = new();
                List<DataProducts> products = SpravceViewModel.GetDataProducts();
                List<DataMaterials> materials = SpravceViewModel.GetDataMaterials();
                var productsMaterials = SpravceViewModel.GetDataProductsMaterials();
                using ProductMaterialContext db = new();

                // to, co je v databázi, porovnám s tím, co je v excelu
                List<string> vyrobkyDBF;
                List<string> materialDBF;

                if (products.Any() == false)
                {
                    SaveProducts(db, seznamVyrobkuExcel);
                }
                else
                {
                    vyrobkyDBF = new();

                    foreach (var v in products)
                    {
                        vyrobkyDBF.Add(v.ProductName);
                    }

                    // vyberu výrobky, které nejsou uloženy v databázi
                    var vybraneVyrobky = seznamVyrobkuExcel.Except(vyrobkyDBF);
                    if (vybraneVyrobky.Any() == true)
                    {
                        SaveProducts(db, vybraneVyrobky);
                    }
                }

                if (materials.Any() == false)
                {
                    SaveMaterials(db, seznamMaterialuExcel);
                }
                else
                {
                    materialDBF = new();

                    foreach (var m in materials)
                    {
                        materialDBF.Add(m.MaterialName);
                    }

                    // vyberu výrobky, které nejsou uloženy v databázi
                    var vybranyMaterial = seznamMaterialuExcel.Except(materialDBF);
                    if (vybranyMaterial.Any() == true)
                    {
                        SaveMaterials(db, vybranyMaterial);
                    }
                }

                // všechny produkty uložené v databázi
                products = SpravceViewModel.GetDataProducts();
                // všechny materiály uložené v databázi
                materials = SpravceViewModel.GetDataMaterials();
                //List<DataMaterials> vybraneMaterialy;

                // natáhne aktuální stav produktů a na ně navázané materiály
                var prodIdMatId = SpravceViewModel.GetDataProductsMaterials();

                // projíždím všechny výrobky z excelu a na to navázané materiály
                foreach (var v in vyrobkyMaterialy)
                {
                    // pro daný výrobek vyhledat navázené materiály
                    var vyrobky = SpravceViewModel.GetProduct(v.Vyrobek, products);
                    int productId = 0;
                    foreach (var vyrobek in vyrobky)
                    {
                        productId = vyrobek.ProductId;

                        if (productId > 0)
                        {
                            // všechny materiály pro jeden výrobek získané z excelu
                            var materialyJednohoVyrobkuExcel = v.Materialy;
                            // vybrané materiály pro daný produkt z databáze
                            var idProdMatDBF = prodIdMatId.Where(x => x.ProductId == productId).Select(x => x.MaterialName).Distinct().ToList();

                            var rozdilVMaterialu = materialyJednohoVyrobkuExcel.Except(idProdMatDBF).ToList();
                            // rozdíl přidám do databáze a přidělím vybranému výrobku
                            // pro daný výrobek se objevil nový myteriál, který nejdřív uožím a pak přidám k výrobku
                            SaveMaterialsForProduct(db, productId, rozdilVMaterialu, materials);

                            // nastaví StavObjektu na hodnotu 2 pro materiál, který již není součástí výrobku
                            rozdilVMaterialu = idProdMatDBF.Except(materialyJednohoVyrobkuExcel).ToList();
                            DeleteMaterialsFromProduct(db, productId, rozdilVMaterialu, materials);
                        }
                        else
                        {
                            // špatný index, musím vymyslet, jak zareagovat
                        }
                    }
                }
            }

            _ = MessageBox.Show(@"Import výrobků a materiálu proběhl.", @"Import dat", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static void SaveProducts(ProductMaterialContext db, IEnumerable<string> vyrobky)
        {
            foreach (var v in vyrobky)
            {
                Product product = new()
                {
                    Name = v,
                    StateObject = 1
                };

                // uložím nový výrobek
                _ = db.Products.Add(product);
                _ = db.SaveChanges();
            }
        }

        private static void SaveMaterials(ProductMaterialContext db, IEnumerable<string> material)
        {
            foreach (var m in material)
            {
                Models.Material mat = new()
                {
                    Name = m,
                    StateObject = 1
                };

                // uložím nový materiál
                _ = db.Materials.Add(mat);
                _ = db.SaveChanges();
            }
        }

        private void DeleteMaterialsFromProduct(ProductMaterialContext db, int indexVyrobek, List<string> materialy, List<DataMaterials> allMaterials)
        {
            var materials = SpravceViewModel.GetMaterials(materialy, allMaterials);

            foreach (var m in materials)
            {
                // vyhledat materiál podle indexVyrobek a m.MaterialId
                // podle získaného Id provedu Update
                var stavObjektu = db.ProductsMaterials.Where(p => p.ProductId == indexVyrobek && p.MaterialId == m.MaterialId).First();
                stavObjektu.StateObject = 2;

                // zneaktivní materiál pro vybraný výrobek
                _ = db.SaveChanges();
            }
        }

        private void SaveMaterialsForProduct(ProductMaterialContext db, int indexVyrobek, List<string> materialy, List<DataMaterials> materials)
        {
            var mat = SpravceViewModel.GetMaterials(materialy, materials);

            foreach (var m in mat)
            {
                // materiál není uložen
                //Models.Material mat = new()
                //{
                //    Name = m,
                //    StateObject = 1
                //};

                //_ = db.Materials.Add(mat);
                //_ = db.SaveChanges();

                // vrátí id nového záznamu
                //int indexMaterial = mat.Id;
                int indexMaterial = m.MaterialId;

                ProductMaterial productMat = new()
                {
                    ProductId = indexVyrobek,
                    MaterialId = indexMaterial,
                    StateObject = 1
                };

                _ = db.ProductsMaterials.Add(productMat);
                _ = db.SaveChanges();
            }
        }

        //private void SaveProductAndMaterial(ProductMaterialContext db, List<string> materialy, List<SpravceViewModel.DataMaterials> materials, int indexVyrobek)
        //{
        //    foreach (var m in materialy)
        //    {
        //        if (materials.Exists(x => x.MaterialName == m) == false)
        //        {
        //            // materiál není uložen
        //            Models.Material mat = new()
        //            {
        //                Name = m,
        //                StateObject = 1
        //            };

        //            _ = db.Materials.Add(mat);
        //            _ = db.SaveChanges();

        //            // vrátí id nového záznamu
        //            int indexMaterial = mat.Id;

        //            materials = SpravceViewModel.GetDataMaterials();

        //            ProductMaterial productMat = new()
        //            {
        //                ProductId = indexVyrobek,
        //                MaterialId = indexMaterial
        //            };

        //            _ = db.ProductsMaterials.Add(productMat);
        //            _ = db.SaveChanges();
        //        }
        //        else
        //        {
        //            // získá index již uloženého materiálu
        //            int indexMaterial = materials.FindIndex(m => m.MaterialName == Convert.ToString(m));

        //            ProductMaterial productMat = new()
        //            {
        //                ProductId = indexVyrobek,
        //                MaterialId = indexMaterial
        //            };

        //            _ = db.ProductsMaterials.Add(productMat);
        //            _ = db.SaveChanges();
        //        }
        //    }
        //}

        public class VyrobekMaterialy
        {
            public string Vyrobek { get; set; }
            public List<string> Materialy { get; set; }
            public VyrobekMaterialy()
            { 
            }
        }
        #endregion
    }
}