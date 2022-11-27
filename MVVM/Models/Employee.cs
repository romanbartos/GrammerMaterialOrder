using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrammerMaterialOrder.MVVM.Models
{
    [Table("Zamestnanci", Schema = "dbo")]
    public class Employee : INotifyPropertyChanged
    {
        private int _id;

        [Column("ID_Zamestnanci")]
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

        private string _firstName;

        [Column("Jmeno")]
        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        private string _lastName;

        [Column("Prijmeni")]
        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        private bool _isEmployee;

        [Column("JeZamestnanec")]
        public bool IsEmployee
        {
            get
            { 
                return _isEmployee;
            }
            set
            {
                _isEmployee = value;
                OnPropertyChanged(nameof(IsEmployee));
            }
        }

        private int? _personalNumber;

        [Column("PersonalniCislo")]
        public int? PersonalNumber
        {
            get
            {
                return _personalNumber;
            }
            set
            {
                _personalNumber = value;
                OnPropertyChanged(nameof(PersonalNumber));
            }
        }

        private bool _productionOfSeatsWarehouseman;

        [Column("VyrobaSedacekSkladnik")]
        public bool ProductionOfSeatsWarehouseman
        {
            get
            {
                return _productionOfSeatsWarehouseman;
            }
            set
            {
                _productionOfSeatsWarehouseman = value;
                OnPropertyChanged(nameof(ProductionOfSeatsWarehouseman));
            }
        }

        private bool _productionOfSeatsManager;

        [Column("VyrobaSedacekSpravce")]
        public bool ProductionOfSeatsManager
        {
            get
            {
                return _productionOfSeatsManager;
            }
            set
            {
                _productionOfSeatsManager = value;
                OnPropertyChanged(nameof(ProductionOfSeatsManager));
            }
        }


        public string FullName
        {
            get
            {
                return $"{LastName} {FirstName}";
            }
            //set
            //{
            //    _fullName = value;
            //    OnPropertyChanged(nameof(FullName));
            //}
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
