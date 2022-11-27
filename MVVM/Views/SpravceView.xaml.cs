using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using GrammerMaterialOrder.MVVM.Models;
using GrammerMaterialOrder.MVVM.ViewModels;
using GrammerMaterialOrder.Utilities;

namespace GrammerMaterialOrder.MVVM.Views
{
    /// <summary>
    /// Interakční logika pro SpravceView.xaml
    /// </summary>
    public partial class SpravceView : UserControl
    {
        public SpravceView()
        {
            InitializeComponent();
            //SpravceViewModel vm = new();
            //DataContext = vm;
            NoveStanovisteAdminRadioButton.IsChecked = true;
            OpravaStanovisteAdminRadioButton.IsChecked = false;
            ProgramState.NoveStanovisteAdminRadioButton = true;
            ProgramState.OpravaStanovisteAdminRadioButton = false;
            StanovisteAdministraceComboBox.IsEnabled = false;
            noveOpravaStanovisteAdministraceLabel.Content = "Nové stanoviště";

            SkladnikVolbaFunkceAdminRadioButton.IsChecked = true;
            SpravceVolbyFunkceAdminRadioButton.IsChecked = false;

            ProgramState.HesloPotvrzeno = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        #region Plánovač
        private void UlozitPlanButton_Click(object sender, RoutedEventArgs e)
        {
            bool ulozit = true;

            int employeeId = 0;
            if (SkladnikPlanovacComboBox.SelectedValue == null)
            {
                ulozit = false;
                _ = MessageBox.Show("Nebyl vybrán skladník.", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                employeeId = Convert.ToInt32(SkladnikPlanovacComboBox.SelectedValue);
            }
            //var selectedItem = SkladnikComboBox.SelectedItem;
            //var a = hermes.GetType().FullName;

            int stationId = 0;
            if (StanovisteComboBox.SelectedValue == null)
            {
                ulozit = false;
                _ = MessageBox.Show("Nebylo vybráno stanoviště.", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                stationId = Convert.ToInt32(StanovisteComboBox.SelectedValue);
            }

            if (casovaZnackaDatePicker.SelectedDate == null)
            {
                ulozit = false;
                _ = MessageBox.Show("Nebyl vybrán datum naplánování.", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            using (MaterialOrderContext db = new())
            {
                if (ulozit)
                {
                    EmployeePlanning employeePlanning = new()
                    {
                        EmployeeId = employeeId,
                        StationId = stationId,
                        TimeStampFrom = (DateTime)casovaZnackaDatePicker.SelectedDate,
                        TimeStampTo = (DateTime)casovaZnackaDatePicker.SelectedDate
                    };
                    _ = db.EmployeePlanning.Add(employeePlanning);
                    _ = db.SaveChanges();
                }
            }
        }
        #endregion

        #region Administrace
        private void NoveStanovisteAdminRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ProgramState.NoveStanovisteAdminRadioButton = true;
            ProgramState.OpravaStanovisteAdminRadioButton = false;

            if (NoveStanovisteAdminRadioButton.IsChecked == true)
            {
                StanovisteAdministraceComboBox.IsEnabled = false;
                noveOpravaStanovisteAdministraceLabel.Content = "Nové stanoviště";
                StanovisteAdministraceTextBox.Text = string.Empty;
            }
        }

        private void OpravaStanovisteAdminRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ProgramState.NoveStanovisteAdminRadioButton = false;
            ProgramState.OpravaStanovisteAdminRadioButton = true;

            if (OpravaStanovisteAdminRadioButton.IsChecked == true)
            {
                StanovisteAdministraceComboBox.IsEnabled = true;
                noveOpravaStanovisteAdministraceLabel.Content = "Oprava názvu stanoviště";

                if (StanovisteAdministraceComboBox.SelectedValue != null)
                {
                    Station textComboBox = (Station)StanovisteAdministraceComboBox.SelectedItem;
                    StanovisteAdministraceTextBox.Text = textComboBox.Name;
                }
            }
        }

        private void StanovisteAdministraceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (StanovisteAdministraceComboBox.SelectedItem != null)
            //{
            //    Station textComboBox = (Station)StanovisteAdministraceComboBox.SelectedItem;
            //    StanovisteAdministraceTextBox.Text = textComboBox.Name;
            //}
        }

        private void UlozitStanovisteAdministrace_Click(object sender, RoutedEventArgs e)
        {
            using (MaterialOrderContext db = new())
            {
                if (NoveStanovisteAdminRadioButton.IsChecked == true)
                {
                    Station station = new()
                    {
                        Name = StanovisteAdministraceTextBox.Text,
                        StateObject = 1
                    };

                    _ = db.Stations.Add(station);
                    _ = db.SaveChanges();
                }
                if (OpravaStanovisteAdminRadioButton.IsChecked == true)
                {
                    var s = (Station)StanovisteComboBox.SelectedItem;

                    Station station = new()
                    {
                        Id = s.Id,
                        Name = StanovisteAdministraceTextBox.Text,
                        StateObject = 1
                    };

                    _ = db.Stations.Update(station);
                    _ = db.SaveChanges();

                    //((SpravceViewModel)DataContext).StationEntriesCmb.Name = StanovisteAdministraceTextBox.Text;
                }
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
    }
}
