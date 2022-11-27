using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrammerMaterialOrder.MVVM.Models
{
    [Table("PlanovaniZamestnancu", Schema = "dbo")]
    public class EmployeePlanning
    {
        private int _id;

        [Column("PlanovaniZamestnancuID")]
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

        private int _employeeId;

        [Column("ZamestnanecID")]
        public int EmployeeId
        {
            get
            {
                return _employeeId;
            }
            set
            {
                _employeeId = value;
                OnPropertyChanged(nameof(EmployeeId));
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

        private DateTime _timeStampFrom;

        [Column("CasovaZnackaOd")]
        public DateTime TimeStampFrom
        {
            get
            {
                return _timeStampFrom;
            }
            set
            {
                _timeStampFrom = value;
                OnPropertyChanged(nameof(TimeStampFrom));
            }
        }

        private DateTime _timeStampTo;

        [Column("CasovaZnackaDo")]
        public DateTime TimeStampTo
        {
            get
            {
                return _timeStampTo;
            }
            set
            {
                _timeStampTo = value;
                OnPropertyChanged(nameof(TimeStampTo));
            }
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
