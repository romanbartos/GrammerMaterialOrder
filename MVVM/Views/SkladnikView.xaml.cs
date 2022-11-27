using System;
using System.Windows;
using System.Windows.Controls;

using GrammerMaterialOrder.MVVM.Models;

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
            if (MessageBox.Show("Je opravdu všechno připraveno?", "Příprava materiálu", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // přepíšu slouoec hotovo
                using (MaterialOrderContext db = new())
                {
                    ProdOrderEmployeePlan prodOrderEmployeePlan = new()
                    {
                        Done = true
                    };
                    _ = db.ProdOrdersEmployeePlan.Add(prodOrderEmployeePlan);
                    _ = db.SaveChanges();
                }

            }
        }
    }
}