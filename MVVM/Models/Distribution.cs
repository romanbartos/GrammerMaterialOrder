using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrammerMaterialOrder.MVVM.Models
{
    [Table("Distribution", Schema = "dbo")]
    public class Distribution
    {
        private int _id;
        //[Key]
        [Column("RozdeleniID")]
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

        private string _rozdeleni;

        [Column("Nazev")]
        public string Name
        {
            get
            {
                return _rozdeleni;
            }
            set
            {
                _rozdeleni = value;
                OnPropertyChanged(nameof(Name));
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

        //public List<ProductMaterial> ProductsMaterials { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
