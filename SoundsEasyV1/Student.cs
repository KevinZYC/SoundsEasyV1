using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SoundsEasyV1
{
    public class Student : INotifyPropertyChanged
    {
        //Student class that implements the INotifyPropertyChanged interface
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int Id = 0;
        private string FName = "";
        private string LName = "";
        private string Course = "";
        private int Grade = 0;
        private string Email = "";
       
        private Student()
        {
            
        }
        //public constructor
        public static Student CreateStudent()
        {
            return new Student();
        }

        //get and set for each variable 
        //variables capitalized, get/set lowercase
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
                    this.Id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string fname
        {
            get
            {
                return this.FName;
            }
            set
            {
                if(value != this.FName){
                    this.FName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string lname
        {
            get
            {
                return this.LName;
            }
            set
            {
                if(value != this.LName)
                {
                    this.LName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string course
        {
            get
            {
                return this.Course;
            }
            set
            {
                if(value != this.Course)
                {
                    this.Course = value;
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
                if(value!= this.Grade)
                {
                    this.Grade = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string email
        {
            get
            {
                return this.Email;
            }
            set
            {
                if(value != this.Email)
                {
                    this.Email = value;
                    NotifyPropertyChanged();
                }
            }
        }

    }
}
