using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using GrammerMaterialOrder.MVVM.ViewModels;

namespace GrammerMaterialOrder.MVVM.Views
{
    /// <summary>
    /// Interakční logika pro VlackarView.xaml
    /// </summary>
    public partial class VlackarView : UserControl
    {
        public VlackarView()
        {
            InitializeComponent();
            //var vm = new VlackarViewModel();
            //DataContext = vm;

            //LoadData(this.vlackarView);
        }

        private static void LoadData(UserControl userControl)
        {
            DataGrid dataGrid = (DataGrid)userControl.FindName("zakazkyVlackarDataGrid");
            foreach (var productionOrder in GetAllProductionOrders())
            {
                dataGrid.Items.Add(productionOrder);
            }
        }

        public static IEnumerable<DataOrders> GetAllProductionOrders()
        {
            using var db = new MaterialOrderContext();
            var products = db.Products.ToList();
            var productionOrders = db.ProductionOrders.ToList();

            var query = from productionOrder in productionOrders
                        join product in products on productionOrder.ProductId equals product.Id
                        select new DataOrders() { OrderName = productionOrder.Order, Quantity = productionOrder.Quantity, ProductName = product.Name };

            return query;
        }

        public struct DataOrders
        {
            public string OrderName { get; set; }
            public int Quantity { get; set; }
            public string ProductName { get; set; }
        }
    }
}
