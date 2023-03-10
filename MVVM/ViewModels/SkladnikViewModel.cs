using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using System;

using GrammerMaterialOrder.MVVM.Models;
using GrammerMaterialOrder.Utilities;
using NPOI.SS.Formula.Functions;

namespace GrammerMaterialOrder.MVVM.ViewModels
{
    public class SkladnikViewModel : INotifyPropertyChanged
    {
        public SkladnikViewModel()
        {
            IEnumerable<DataOrdersWPF> productionOrders = GetAllProductionOrders();
            _productionOrderEntries = new ObservableCollection<DataOrdersWPF>(productionOrders);
        }

        private static ObservableCollection<DataOrdersWPF> _productionOrderEntries;

        public ObservableCollection<DataOrdersWPF> ProductionOrderEntries
        {
            get { return _productionOrderEntries; }
            set
            {
                if (_productionOrderEntries == value) return;
                _productionOrderEntries = value;
                OnPropertyChanged(nameof(ProductionOrderEntries));
            }
        }

        public static void UpdateProductionOrder(DataOrdersWPF zakazka)
        {
            //_productionOrderEntries.Remove(zakazka);

            var found = _productionOrderEntries.FirstOrDefault(x => x.ProductionOrderId == zakazka.ProductionOrderId);
            //found.Quantity = 44;
            found.OrderQuantity = 44;
            //found.ProductionOrder.Quantity = 44;
        }

        private static List<DataOrdersWPF> GetAllProductionOrders()
        {
            using var db = new MaterialOrderContext();
            var products = db.Products.ToList();
            var productionOrders = db.ProductionOrders.ToList();
            var prodOrderEmplPlan = db.ProdOrdersEmployeePlan.ToList();

            var query = db.ProductionOrders
                .Join(db.Products, prodorder => prodorder.ProductId, product => product.Id, (prodorder, product) => new { prodorder, product })
                .Join(db.ProdOrdersEmployeePlan, pop => pop.prodorder.Id, poep => poep.ProductionOrderId, (pop, poep) => new { pop, poep })
                .Select (m => new DataOrders()
                {
                    ProductionOrderEmployeePlanId = m.poep.Id,
                    ProductionOrderId = m.poep.ProductionOrderId,
                    StationId = m.poep.StationId,
                    EmployeePlanningId = m.poep.EmployeePlanningId,
                    ProductionOrder = m.pop.prodorder,
                    Product = m.pop.product,
                    Done = m.poep.Done,
                    Note = m.poep.Note,
                    ProdOEP = m.poep
                }).ToList();

            //var query = m.ToList().GroupBy(x => x.ProductionOrderId).Select(g => g.First());

            //var query = from productionOrder in productionOrders
            //            join product in products on productionOrder.ProductId equals product.Id
            //            join prodOEP in prodOrderEmplPlan on productionOrder.Id equals prodOEP.ProductionOrderId
            //            where prodOEP.StationId == ProgramState.StationId && prodOEP.Done is false
            //            select new DataOrders() { ProductionOrderEmployeePlanId = prodOEP.Id, 
            //                ProductionOrderId = prodOEP.ProductionOrderId, 
            //                StationId = prodOEP.StationId, 
            //                EmployeePlanningId = prodOEP.EmployeePlanningId, 
            //                ProductionOrder = productionOrder, 
            //                Product = product, 
            //                Done = prodOEP.Done, 
            //                Note = prodOEP.Note,
            //                ProdOEP = prodOEP };



            // tady bude převod do struktury, kde budou všechna stanoviště v jednom záznamu
            List<DataOrdersWPF> listWPF = new();
            int j = 0;
            for (int i = 0; i < query.Count(); i++)
            {
                var a = new DataOrdersWPF {
                    ProductionOrderEmployeePlanId = query[i].ProductionOrderEmployeePlanId,
                    ProductionOrderId = query[i].ProductionOrderId,
                    StationId = query[i].StationId,
                    OrderName = query[i].ProductionOrder.Order,
                    OrderQuantity = query[i].ProductionOrder.Quantity,
                    ProductName = query[i].Product.Name,
                    EmplPlanNote = query[i].ProdOEP.Note,
                    EmployeePlanningId1 = query[i].EmployeePlanningId,
                    EmplPlanDoneSt1 = query[i].ProdOEP.Done
                };
                i++;
                a.EmplPlanDoneSt2 = query[i].Done;
                i++;
                a.EmplPlanDoneSt3 = query[i].Done;

                listWPF.Add(a);
            }

            if (listWPF.Any())
                return listWPF;
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
            public ProdOrderEmployeePlan ProdOEP { get; set; }
        }

        public struct DataOrdersWPF
        {
            public int ProductionOrderEmployeePlanId { get; set; }
            public int ProductionOrderId { get; set; }
            public int StationId { get; set; }
            public int? EmployeePlanningId { get; set; }
            public string OrderName { get; set; }
            public int OrderQuantity { get; set; }
            public string ProductName { get; set; }
            public string EmplPlanNote { get; set; }
            public int? EmployeePlanningId1 { get; set; }
            public int? EmployeePlanningId2 { get; set; }
            public int? EmployeePlanningId3 { get; set; }
            public bool EmplPlanDoneSt1 { get; set; }
            public bool EmplPlanDoneSt2 { get; set; }
            public bool EmplPlanDoneSt3 { get; set; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
    }
}
