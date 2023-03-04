using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrammerMaterialOrder.MVVM.Models
{
    [Table("Vyrobek", Schema = "dbo")]
    public class Product
    {
        private int _id;

        //[Key]
        [Column("VyrobekID")]
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

        private string _name;

        [Column("Nazev")]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
