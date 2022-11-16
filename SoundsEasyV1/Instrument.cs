using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

//https://stackoverflow.com/questions/40054539/datagrid-itemsource-binding-not-updating-and-binding-in-wpf-not-working this source for notify data changed
namespace SoundsEasyV1
{
    public class Instrument : INotifyPropertyChanged
    {


        /*
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
        }*/
        //standard declaration for INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /*
        public int id = 0;
        public string type = "";
        public string make = "";
        public string caseNum = "";
        public string serialNum = "";
        public int grade = 0;
        public string studentID = "";
        public string repairStatus = "";
        */

        private Instrument()
        {
            //variables that DONT update should be put here

            /*
            id = 0;
            type = "";
            make = "";
            caseNum = "";
            serialNum = "";
            grade = 0;
            studentID = "";
            repairStatus = "";
            */
        }
        //public constructor
        public static Instrument CreateInstrument()
        {
            return new Instrument();
        }

        //get and set for each variable
        public int id
        {
            get
            {
                return this.id;
            }
            set
            {
                if(value != this.id)
                {
                    this.id = id;
                    NotifyPropertyChanged();
                }
            }
        }

        public string type
        {
            get
            {
                return this.type;
            }
            set
            {
                if (value != this.type)
                {
                    this.type = type;
                    NotifyPropertyChanged();
                }
            }
        }

        public string make
        {
            get
            {
                return this.make;
            }
            set
            {
                if (value != this.make)
                {
                    this.make = make;
                    NotifyPropertyChanged();
                }
            }
        }

        public string caseNum
        {
            get
            {
                return this.caseNum;
            }
            set
            {
                if (value != this.caseNum)
                {
                    this.caseNum = caseNum;
                    NotifyPropertyChanged();
                }
            }
        }

        public string serialNum
        {
            get
            {
                return this.serialNum;
            }
            set
            {
                if (value != this.serialNum)
                {
                    this.serialNum = serialNum;
                    NotifyPropertyChanged();
                }
            }
        }

        public int grade
        {
            get
            {
                return this.grade;
            }
            set
            {
                if (value != this.grade)
                {
                    this.grade = grade;
                    NotifyPropertyChanged();
                }
            }
        }

        public string studentID
        {
            get
            {
                return this.studentID;
            }
            set
            {
                if (value != this.studentID)
                {
                    this.studentID = studentID;
                    NotifyPropertyChanged();
                }
            }
        }

        public string repairStatus
        {
            get
            {
                return this.repairStatus;
            }
            set
            {
                if (value != this.repairStatus)
                {
                    this.repairStatus = repairStatus;
                    NotifyPropertyChanged();
                }
            }
        }


    }
}
