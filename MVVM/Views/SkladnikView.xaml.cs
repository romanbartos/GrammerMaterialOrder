using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

                DataOrdersWPF zakazka = (DataOrdersWPF)ZakazkySkladnikDataGrid.SelectedItem;

                //var objednavka = new ProdOrderEmployeePlan() { Id = zakazka.ProductionOrderEmployeePlanId, Done = true };
                //db.ProdOrdersEmployeePlan.Attach(objednavka);
                //db.Entry(objednavka).Property(x => x.Done).IsModified = true;

                var objednavka = new ProductionOrder() { Id = zakazka.ProductionOrderId, Quantity = 44 };
                db.ProductionOrders.Attach(objednavka);
                db.Entry(objednavka).Property(x => x.Quantity).IsModified = true;

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

                UpdateProductionOrder(zakazka);
                ZakazkySkladnikDataGrid.Items.Refresh();
            }
        }

        private void Poznamka_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(@"Je opravdu všechno připraveno?", @"Příprava materiálu", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                DataOrders zakazka = (DataOrders)ZakazkySkladnikDataGrid.SelectedItem;

                //int objednavkaId = zakazka.;

                var newPoznamkaSkladniView = new PoznamkaSkladnikView(1);
                newPoznamkaSkladniView.ShowDialog();


            }
        }

        public class WorkerToColorMultiConverter : IMultiValueConverter
        {
            public Brush ChefColor { get; set; }
            public Brush WaiterColor { get; set; }
            public Brush DefaultColor { get; set; }

            public WorkerToColorMultiConverter()
            {
                //Default Colors
                ChefColor = Brushes.Aqua;
                WaiterColor = Brushes.Yellow;
                DefaultColor = Brushes.Transparent;
            }

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if (values != null && values.Count() == 2)
                {
                    var cellValue = values[0] as string;
                    var workerValue = values[1] as string;
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        switch (workerValue)
                        {
                            case "Chef":
                                return ChefColor;
                            case "Waiter":
                                return WaiterColor;
                            default:
                                return DefaultColor;
                        }
                    }
                }
                return DefaultColor;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

    }
}