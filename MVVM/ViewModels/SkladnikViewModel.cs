using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

using GrammerMaterialOrder.MVVM.Models;
using GrammerMaterialOrder.Utilities;

namespace GrammerMaterialOrder.MVVM.ViewModels
{
    public class SkladnikViewModel : INotifyPropertyChanged
    {
        public SkladnikViewModel()
        {
            IEnumerable<DataOrders> productionOrders = GetAllProductionOrders();
            _productionOrderEntries = new ObservableCollection<DataOrders>(productionOrders);
        }

        private ObservableCollection<DataOrders> _productionOrderEntries;

        public ObservableCollection<DataOrders> ProductionOrderEntries
        {
            get { return _productionOrderEntries; }
            set
            {
                if (_productionOrderEntries == value) return;
                _productionOrderEntries = value;
                OnPropertyChanged(nameof(ProductionOrderEntries));
            }
        }

        private static List<DataOrders> GetAllProductionOrders()
        {
            using var db = new MaterialOrderContext();
            var products = db.Products.ToList();
            var productionOrders = db.ProductionOrders.ToList();
            var prodOrderEmplPlan = db.ProdOrdersEmployeePlan.ToList();

            var query = from productionOrder in productionOrders
                        join product in products on productionOrder.ProductId equals product.Id
                        join prodOEP in prodOrderEmplPlan on productionOrder.Id equals prodOEP.ProductionOrderId
                        where prodOEP.StationId == ProgramState.StationId && prodOEP.Done is false
                        select new DataOrders() { ProductionOrderEmployeePlanId = prodOEP.Id, 
                            ProductionOrderId = prodOEP.ProductionOrderId, 
                            StationId = prodOEP.StationId, 
                            EmployeePlanningId = prodOEP.EmployeePlanningId, 
                            ProductionOrder = productionOrder, 
                            Product = product, 
                            Done = prodOEP.Done, 
                            Note = prodOEP.Note };
            
            if (query.Any())
                return query.ToList();
            else
                return null;
        }

        public struct DataOrders
        {
            public int ProductionOrderEmployeePlanId { get; set; }
            public int ProductionOrderId { get; set; }
            public int StationId { get; set; }
            public int? EmployeePlanningId { get; set; }
            public ProductionOrder ProductionOrder { get; set; }
            public Product Product { get; set; }
            public bool Done { get; set; }
            public string Note { get; set; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
