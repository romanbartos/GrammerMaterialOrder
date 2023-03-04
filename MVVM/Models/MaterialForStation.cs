using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrammerMaterialOrder.MVVM.Models
{
    [Table("MaterialNaStanoviste", Schema = "dbo")]
    public class MaterialForStation
    {
        private int _id;

        [Column("MaterialNaStanovisteID")]
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

        private int _materialId;

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

        //[Column("ObjednavkaID")]
        //public int OrderId { get; set; }

        private int _stationId;

        [Column("StanovisteID")]
        public int StationId
        {
            get
            {
                return _stationId;
            }
            set
            {
                _stationId = value;
                OnPropertyChanged(nameof(StationId));
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
