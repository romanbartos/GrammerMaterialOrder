using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

        public static List<Employee> employees = new();
        public static List<Employee> employeesAll = new();

        // řádky pro DGV, materiál pro přidělení na stanoviště
        public static bool RadkyPridelitMaterialDGV { get; set; }
        // řádky pro DGV, již přidělený materiál na stanoviště
        public static bool RadkyMaterialPridelenDGV { get; set; }
        // id vybraného plánu
        public static int EmployeePlanningId { get; set; }
        public static int StationId { get; set; }

    }
}
