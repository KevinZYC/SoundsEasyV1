using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Diagnostics;

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

        
        private int Id = 0;
        private string Type = "";
        private string Make = "";
        private string CaseNum = "";
        private string SerialNum = "";
        private int Grade = 0;
        private string StudentID = "";
        private string RepairStatus = "";


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

        //variable themselves are capitalized, but the name of the get/set is lower case
        public int id
        {
            get
            {
                return this.Id;
            }
            set
            {
                if(value != this.Id)
                {
                    Debug.WriteLine("id");
                    Debug.WriteLine(this.Id);
                    this.Id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string type
        {
            get
            {
                return this.Type;
            }
            set
            {
                if (value != this.Type)
                {
                    Debug.WriteLine("id");
                    this.Type = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string make
        {
            get
            {
                return this.Make;
            }
            set
            {
                if (value != this.Make)
                {
                    this.Make = value;
                    Debug.WriteLine("id");
                    NotifyPropertyChanged();
                }
            }
        }

        public string caseNum
        {
            get
            {
                return this.CaseNum;
            }
            set
            {
                if (value != this.CaseNum)
                {
                    this.CaseNum = value;
                    Debug.WriteLine("id");
                    NotifyPropertyChanged();
                }
            }
        }

        public string serialNum
        {
            get
            {
                return this.SerialNum;
            }
            set
            {
                if (value != this.SerialNum)
                {
                    Debug.WriteLine(this.SerialNum);
                    this.SerialNum = value;
                    
                    NotifyPropertyChanged();
                }
            }
        }

        public int grade
        {
            get
            {
                return this.Grade;
            }
            set
            {
                if (value != this.Grade)
                {
                    this.Grade = value;
                    Debug.WriteLine("id");
                    NotifyPropertyChanged();
                }
            }
        }

        public string studentID
        {
            get
            {
                return this.StudentID;
            }
            set
            {
                if (value != this.StudentID)
                {
                    this.StudentID = value;
                    Debug.WriteLine("id");
                    NotifyPropertyChanged();
                }
            }
        }

        public string repairStatus
        {
            get
            {
                return this.RepairStatus;
            }
            set
            {
                if (value != this.RepairStatus)
                {
                    this.RepairStatus = value;
                    Debug.WriteLine("id");
                    NotifyPropertyChanged();
                }
            }
        }


    }
}
