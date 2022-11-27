using System.Collections.Generic;

using GrammerMaterialOrder.MVVM.Models;

namespace GrammerMaterialOrder.Utilities
{
    public static class ProgramState
    {
        public static byte SwitchRadioButton { get; set; }
        public static bool Logout { get; set; }

        public static bool NoveStanovisteAdminRadioButton { get; set; }
        public static bool OpravaStanovisteAdminRadioButton { get; set; }
        public static bool HesloPotvrzeno { get; set; }

        public static List<Employee> employees;
        public static List<Employee> employeesAll;

    }
}
