using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

//https://stackoverflow.com/questions/40054539/datagrid-itemsource-binding-not-updating-and-binding-in-wpf-not-working this source for notify data changed
namespace SoundsEasyV1
{
    public class Instrument
    {
        public int id { get; set; }
        public string type { get; set; }
        public string make { get; set; }
        public string caseNum { get; set; }
        public string serialNum { get; set; }
        public int grade { get; set; }
        public string studentID { get; set; }
        public string repairStatus { get; set; }

        
        public Instrument(int id, string type, string make, string caseNum, string serialNum, int grade, string studentID, string repairStatus)
        {
            this.id = id;
            this.type = type;
            this.make = make;
            this.caseNum = caseNum;
            this.serialNum = serialNum;
            this.grade = grade;
            this.studentID = studentID;
            this.repairStatus = repairStatus;
        }
    }
}
