using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrammerMaterialOrder.MVVM.Models
{
    [Table("ObjednavkaPlanZam", Schema = "dbo")]
    public class ProdOrderEmployeePlan
    {
        private int _id;

        [Column("ObjednavkaPlanZamID")]
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

        private int _productionOrderId;

        [Column("ObjednavkaID")]
        public int ProductionOrderId
        {
            get
            {
                return _productionOrderId;
            }
            set
            {
                _productionOrderId = value;
                OnPropertyChanged(nameof(ProductionOrderId));
            }
        }

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

        private int? _employeePlanningId;

        [Column("PlanovaniZamestnancuID")]
        public int? EmployeePlanningId
        {
            get
            {
                return _employeePlanningId;
            }
            set
            {
                _employeePlanningId = value;
                OnPropertyChanged(nameof(EmployeePlanningId));
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

        private string _note;

        [Column("Poznamka")]
        public string Note
        {
            get
            {
                return _note;
            }
            set
            {
                _note = value;
                OnPropertyChanged(nameof(Note));
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
