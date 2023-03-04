using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrammerMaterialOrder.MVVM.Models
{
    [Table("Objednavka", Schema = "dbo")]
    public class ProductionOrder
    {
        private int _id;

        [Column("ObjednavkaID")]
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private string _order;

        [Column("Zakazka")]
        public string Order
        {
            get
            {
                return _order;
            }
            set
            {
                _order = value;
                OnPropertyChanged(nameof(Order));
            }
        }

        private int _quantity;

        [Column("Pocet")]
        public int Quantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }

        private int _productId;

        [Column("VyrobekID")]
        public int ProductId
        {
            get
            {
                return _productId;
            }
            set
            {
                _productId = value;
                OnPropertyChanged(nameof(ProductId));
            }
        }

        private bool _done;

        [Column("Hotovo")]
        public bool Done
        {
            get
            {
                return _done;
            }
            set
            {
                _done = value;
                OnPropertyChanged(nameof(Done));
            }
        }

        private byte _stateObject;

        [Column("StavObjektu")]
        public byte StateObject
        {
            get
            {
                return _stateObject;
            }
            set
            {
                _stateObject = value;
                OnPropertyChanged(nameof(StateObject));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}