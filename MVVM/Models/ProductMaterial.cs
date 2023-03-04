using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrammerMaterialOrder.MVVM.Models
{
    [Table("VyrobekMaterial", Schema = "dbo")]

    public class ProductMaterial
    {
        private int _id;

        [Column("VyrobekMaterialID")]
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

        private int _productId;

        //[ForeignKey("VyrobekId")]
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
        //public Product Product { get; set; }

        private int _materialId;

        //[ForeignKey("MaterialId")]
        [Column("MaterialID")]
        public int MaterialId
        {
            get
            {
                return _materialId;
            }
            set
            {
                _materialId = value;
                OnPropertyChanged(nameof(MaterialId));
            }
        }
        //public Material Material { get; set; }

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
