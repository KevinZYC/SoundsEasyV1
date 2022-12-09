using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Runtime.ConstrainedExecution;

namespace SoundsEasyV1
{
    /// <summary>
    /// Interaction logic for StudentWindow.xaml
    /// </summary>
    public partial class StudentWindow : Window
    {
        public ObservableCollection<Student> dataSource = new ObservableCollection<Student>();
        public ObservableCollection<Student> dataSourceFiltered = new ObservableCollection<Student>();
        

        StudentWindow? thisWindow = null;
        int selected = -1;

        bool isLoading = false;

        private GoogleSheetsHelper? gsh = null;
        private GoogleSheetParameters? gsp = null;


        public StudentWindow()
        {
            InitializeComponent();

            gsh = new GoogleSheetsHelper("fleet-automata-366622-ba1a276c41b4.json", "1NolOQnhFOmPTaXXp7G1tNkrl8is3gVe9X1AIjgEKg7c");

            gsp = new GoogleSheetParameters() { RangeColumnStart = 1, RangeRowStart = 1, RangeColumnEnd = 5, RangeRowEnd = 100, FirstRowIsHeaders = true, SheetName = "sheet1" };

            LoadData();
        }

        public void Init(ref StudentWindow windowObj)
        {
            thisWindow = windowObj;
        }

        public void LoadData()
        {

            //clear data source list
            dataSource.Clear();
            isLoading = true;

            //set pop up size
            popupAddStudent.Height = SystemParameters.PrimaryScreenHeight * 0.5;
            popupAddStudent.Width = SystemParameters.PrimaryScreenWidth * 0.5;

            //set parameters for pull request
            gsp = new GoogleSheetParameters() { RangeColumnStart = 1, RangeRowStart = 1, RangeColumnEnd = 5, RangeRowEnd = 100, FirstRowIsHeaders = true, SheetName = "sheet1" };
            //run data loading in background
            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync();


            

        }

        public void LoadDataFilter()
        {
            dataSourceFiltered.Clear();
            for (int i = 0; i < dataSource.Count; i++)
            {
                if (checkFilter(dataSource[i]))
                {
                    dataSourceFiltered.Add(dataSource[i]);
                }

            }
            dataGridStudent.ItemsSource = dataSourceFiltered;

        }

        private bool checkFilter(Student s)
        {
            if (s.fname != txtStudentFName.Text && txtStudentFName.Text.Length > 0)
            {
                return false;
            }
            if (s.lname != txtStudentLName.Text && txtStudentLName.Text.Length > 0)
            {
                return false;
            }
            if (s.course != txtStudentCourse.Text && txtStudentCourse.Text.Length > 0)
            {
                return false;
            }
            if (s.grade.ToString() != txtStudentGrade.Text && txtStudentGrade.Text.Length > 0)
            {
                return false;
            }
            if (s.fname != txtStudentEmail.Text && txtStudentEmail.Text.Length > 0)
            {
                return false;
            }

            return true;
        }


        //copied files from InstrumentWindow.xaml.cs, changed to apply to Student Window from here on
        private void worker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            progressStudentLoad.Visibility = Visibility.Visible;
            progressStudentLoad.Value = e.ProgressPercentage;
            progressStudentLoad.Visibility = Visibility.Visible;
            progressTextStudent.Text = (string)e.UserState;

        }

        private void worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;

            gsh.GetStudentDataFromSheet(gsp, ref dataSource, ref thisWindow, ref worker);

            //LoadSheets();
        }

        private void worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            progressStudentLoad.Visibility = Visibility.Hidden;
            progressTextStudent.Visibility = Visibility.Hidden;
            dataGridStudent.ItemsSource = dataSource;
            isLoading = false;

            LoadDataFilter();
        }

        public void LoadSheets()
        {
            //dataGridStudent.ItemsSource = dataSource;
        }

        //when a row in the data is selected:
        private void StudentRow_Click(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid != null)
            {
                Student target = dataGrid.SelectedItem as Student;
                if (target != null)
                {
                    selected = target.id;
                    Debug.WriteLine(selected);
                }

            }
        }

        private void dataClassOptions_Click(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnLoadStudents_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoading)
                LoadDataFilter();

        }
        
        private void popupCloserS_click(object sender, RoutedEventArgs e)
        {
            btnAddStudent.IsChecked = false;

        }

        public void addData(Student i)
        {
            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                dataSource.Add(i);
                Debug.WriteLine(dataSource.Count);

            });
        }

        private bool addStudentEntry()
        {
             Student stud = Student.CreateStudent();
             var studRow = new GoogleSheetRow();

             if (txtAddFirstName.Text.Length > 0)
             {
                stud.fname = txtAddFirstName.Text;
                //adds item to GoogleSheetRow
                studRow.Cells.Add(new GoogleSheetCell { CellValue = txtAddFirstName.Text });
    
              }
            else
            {
                return false;
            }
            if (txtAddLastName.Text.Length > 0)
            {
                stud.lname = txtAddLastName.Text;
                studRow.Cells.Add(new GoogleSheetCell { CellValue = txtAddLastName.Text });
            }
            else
            {
                return false;
            }
            if (txtAddEmail.Text.Length > 0)
            {
                stud.email = txtAddEmail.Text;
                studRow.Cells.Add(new GoogleSheetCell { CellValue = txtAddEmail.Text });
            }
            else
            {
                return false;
            }
            if (radioG9.IsChecked == true)
            {
                stud.grade = 9;
            }
            else if (radioG10.IsChecked == true)
            {
                stud.grade = 10;
            }
            else if (radioG11.IsChecked == true)
            {
                stud.grade = 11;
            }
            else if (radioG12.IsChecked == true)
            {
                stud.grade = 12;
            }
            else
            {
                stud.grade = -1;
            }
            studRow.Cells.Add(new GoogleSheetCell { CellValue = stud.grade.ToString() });

            stud.id = dataSource.Count + 1;

            studRow.Cells.Add(new GoogleSheetCell { CellValue = "good" });

            dataSource.Add(stud);
            LoadDataFilter();

            Debug.WriteLine("cols " + studRow.Cells.Count);

            //the function requires a list of rows
            var rowsToAdd = new List<GoogleSheetRow>() { studRow };
            //add cells request with a built-in construction of a googlesheetparameter
            gsh.AddCells(new GoogleSheetParameters { SheetName = "Sheet1", RangeColumnStart = 1, RangeRowStart = dataSource.Count + 1 }, rowsToAdd);
            return true;

        }
        private void resetPopup()
        {
            txtAddFirstName.Clear();
            txtAddLastName.Clear();
            txtAddEmail.Clear();
            popupAddStudent.IsOpen = false;
        }


        //handling hint texts
        void hintChangeFirstName(object sender, TextChangedEventArgs e)
        {
            hintFirstName.Visibility = Visibility.Visible;
            if (txtAddFirstName.Text != "")
            {
                hintFirstName.Visibility= Visibility.Hidden;
            }
        }

        void hintChangeLastName(object sender, TextChangedEventArgs e)
        {
            hintFirstName.Visibility = Visibility.Visible;
            if (txtAddFirstName.Text != "")
            {
                hintFirstName.Visibility = Visibility.Hidden;
            }
        }

        void hintChangeEmail(object sender, TextChangedEventArgs e)
        {
            hintEmail.Visibility = Visibility.Visible;
            if (txtAddEmail.Text != "")
            {
                hintEmail.Visibility = Visibility.Hidden;
            }
        }
    }
        }
   
