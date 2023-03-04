using System;
using System.Windows;
using System.Windows.Controls;

using GrammerMaterialOrder.MVVM.Models;
using GrammerMaterialOrder.MVVM.ViewModels;
using static GrammerMaterialOrder.MVVM.ViewModels.SkladnikViewModel;

namespace GrammerMaterialOrder.MVVM.Views
{
    /// <summary>
    /// Interakční logika pro SkladnikView.xaml
    /// </summary>
    public partial class SkladnikView : UserControl
    {
        public SkladnikView()
        {
            InitializeComponent();
            //var vm = new SkladnikViewModel();
            //DataContext = vm;
        }

        private void Hotovo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBox.Show(@"Je opravdu všechno připraveno?", @"Příprava materiálu", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // přepíšu slouoec hotovo
                using MaterialOrderContext db = new();

                DataOrders zakazka = (DataOrders)ZakazkySkladnikDataGrid.SelectedItem;

                var objednavka = new ProdOrderEmployeePlan() { Id = zakazka.ProductionOrderEmployeePlanId, Done = true };
                db.ProdOrdersEmployeePlan.Attach(objednavka);
                db.Entry(objednavka).Property(x => x.Done).IsModified = true;

                //zakazka.Done = true;
                //ProdOrderEmployeePlan prodOrderEmployeePlan = new()
                //{
                //    Id = zakazka.ProductionOrderEmployeePlanId,
                //    ProductionOrderId = zakazka.ProductionOrderId,
                //    StationId = zakazka.StationId,
                //    EmployeePlanningId = zakazka.EmployeePlanningId,
                //    Done = true,
                //    Note = zakazka.Note,
                //    StateObject = 1

                //};
                //_ = db.ProdOrdersEmployeePlan.Update(prodOrderEmployeePlan);
                _ = db.SaveChanges();
            }
        }

        private void Nedokonceno_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(@"Je opravdu všechno připraveno?", @"Příprava materiálu", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                DataOrders zakazka = (DataOrders)ZakazkySkladnikDataGrid.SelectedItem;

                //int objednavkaId = zakazka.;

                var newPoznamkaSkladniView = new PoznamkaSkladnikView(1);
                newPoznamkaSkladniView.ShowDialog();
                

            }
        }
    }
}