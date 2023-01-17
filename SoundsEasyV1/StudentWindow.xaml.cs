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
        //public ObservableCollection<Student> dataSourceStudent = new ObservableCollection<Student>();
        public ObservableCollection<Student> dataSourceStudentFiltered = new ObservableCollection<Student>();


        StudentWindow? thisWindow = null;
        int selected = -1;

        double popupSize = 0.5;

        bool isLoading = false;

        bool isAddingIns = false;

        private GoogleSheetsHelper? gsh = null;
        private GoogleSheetParameters? gsp = null;

        public static string studSheetName = "Students";

        public StudentWindow()
        {
            InitializeComponent();

            gsh = new GoogleSheetsHelper("fleet-automata-366622-ba1a276c41b4.json", MainWindow.sheetCode);

            gsp = new GoogleSheetParameters() { RangeColumnStart = 1, RangeRowStart = 1, RangeColumnEnd = 5, RangeRowEnd = 100, FirstRowIsHeaders = true, SheetName = studSheetName };

            popupAddStudent.Height = SystemParameters.PrimaryScreenHeight * popupSize;
            popupAddStudent.Width = SystemParameters.PrimaryScreenWidth * popupSize;

            popupAddStudent.Height = SystemParameters.PrimaryScreenHeight * popupSize;
            popupAddStudent.Height = SystemParameters.PrimaryScreenHeight * popupSize;

            Loaded += delegate
            {
                
            };

            LoadDataFilter();
            //LoadData();
        }

        public void Init(ref StudentWindow windowObj)
        {
            thisWindow = windowObj;
        }

        public void runScaleText(object sender, RoutedEventArgs e)
        {

            if (sender.GetType() == typeof(TextBox))
            {
                var obj = sender as TextBox;

                if (obj.ActualHeight == 0)
                {
                    Debug.WriteLine("  failed run scale text     ");
                    return;
                }

                obj.FontSize = obj.ActualHeight / 1.75;
                Debug.WriteLine("scale text" + obj.ActualHeight);

            }
            if (sender.GetType() == typeof(TextBlock))
            {

                var obj = sender as TextBlock;
                if (obj.ActualHeight == 0)
                {
                    Debug.WriteLine("  failed run scale text     ");
                    return;
                }
                obj.FontSize = obj.ActualHeight / 1.75;
                Debug.WriteLine("scale text" + obj.ActualHeight);
            }
            if (sender.GetType() == typeof(Button))
            {

                var obj = sender as Button;
                if (obj.ActualHeight == 0)
                {
                    Debug.WriteLine("  failed run scale text     ");
                    return;
                }
                obj.FontSize = obj.ActualHeight / 3.5;
                Debug.WriteLine("scale text" + obj.ActualHeight);
            }
            if (sender.GetType() == typeof(ToggleButton))
            {

                var obj = sender as ToggleButton;
                if (obj.ActualHeight == 0)
                {
                    Debug.WriteLine("  failed run scale text     ");
                    return;
                }
                obj.FontSize = obj.ActualHeight / 3.5;
                Debug.WriteLine("scale text" + obj.ActualHeight);
            }
        }


        /*
        public void LoadData()
        {

            //clear data source list
            MainWindow.dataSourceStudent.Clear();
            isLoading = true;

            //set pop up size
            popupAddStudent.Height = SystemParameters.PrimaryScreenHeight * 0.5;
            popupAddStudent.Width = SystemParameters.PrimaryScreenWidth * 0.5;

            //set parameters for pull request
            gsp = new GoogleSheetParameters() { RangeColumnStart = 1, RangeRowStart = 1, RangeColumnEnd = 5, RangeRowEnd = 100, FirstRowIsHeaders = true, SheetName = studSheetName };
            //run data loading in background
            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync();


            

        }*/

        public struct StudentOption
        {
            public string Course { get; set; }

        }

        public void LoadDataFilter()
        {
            dataSourceStudentFiltered.Clear();
            for (int i = 0; i < MainWindow.dataSourceStudent.Count; i++)
            {
                if (checkFilter(MainWindow.dataSourceStudent[i]))
                {
                    dataSourceStudentFiltered.Add(MainWindow.dataSourceStudent[i]);
                }
                if (dataSourceStudentFiltered.Count == 0)
                {
                    string messageBoxText = "No matching entries found, please try again.";
                    string caption = "Did you make a typo?";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBoxResult result;

                    result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                }
            }
            dataGridStudent.ItemsSource = dataSourceStudentFiltered;

            loadStudentOptions();

        }

        private void loadStudentOptions()
        {
            HashSet<string> s = new HashSet<string>();
            foreach(Student stu in MainWindow.dataSourceStudent)
            {
                s.Add(stu.course);
                Debug.WriteLine(stu.course);
            }

            List<StudentOption> tempSource = new List<StudentOption>();

            foreach(string val in s)
            {
                tempSource.Add(new StudentOption { Course = val });
            }

            dataClassOptions.ItemsSource = tempSource;
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

        /* code no longer necessary
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

            gsh.GetStudentDataFromSheet(gsp, ref MainWindow.dataSourceStudent, ref thisWindow, ref worker);

            //LoadSheets();
        }

        private void worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            progressStudentLoad.Visibility = Visibility.Hidden;
            progressTextStudent.Visibility = Visibility.Hidden;
            dataGridStudent.ItemsSource = MainWindow.dataSourceStudent;
            isLoading = false;

            LoadDataFilter();
        }
        
        public void LoadSheets()
        {
            //dataGridStudent.ItemsSource = dataSourceStudent;
        }
        */
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

        private void addStudentButton_Click(object sender, RoutedEventArgs e)
        {
            if (addStudentEntry())
            {
                resetPopup();
            }
        }

        public void addData(Student i)
        {
            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                MainWindow.dataSourceStudent.Add(i);
                Debug.WriteLine(MainWindow.dataSourceStudent.Count);

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
            
            if (dataClassOptions.SelectedItem is StudentOption current)
            {
                stud.course = current.Course;
                studRow.Cells.Add(new GoogleSheetCell { CellValue = stud.course });
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

            if (txtAddEmail.Text.Length > 0)
            {
                stud.email = txtAddEmail.Text;
                studRow.Cells.Add(new GoogleSheetCell { CellValue = txtAddEmail.Text });
            }
            else
            {
                return false;
            }
            stud.id = MainWindow.dataSourceStudent.Count + 1;

            MainWindow.dataSourceStudent.Add(stud);
            LoadDataFilter();

            Debug.WriteLine("cols " + studRow.Cells.Count);

            //the function requires a list of rows
            var rowsToAdd = new List<GoogleSheetRow>() { studRow };
            //add cells request with a built-in construction of a googlesheetparameter
            gsh.AddCells(new GoogleSheetParameters { SheetName = studSheetName, RangeColumnStart = 1, RangeRowStart = MainWindow.dataSourceStudent.Count + 1 }, rowsToAdd);
            return true;

        }


        private void resetPopup()
        {
            txtAddFirstName.Clear();
            txtAddLastName.Clear();
            txtAddEmail.Clear();
            popupAddStudent.IsOpen = false;
        }

        private void btnViewStudAssign_Click(object sender, RoutedEventArgs e)
        {
            if (selected<0)
            {
                return;
            }

            popupOwnedInstruments.Width = SystemParameters.PrimaryScreenWidth * 0.5;
            popupOwnedInstruments.Height = SystemParameters.PrimaryScreenHeight * 0.5;

            if (popupOwnedInstruments.IsOpen)
            {
                popupOwnedInstruments.IsOpen = false;
                dataGridStudent.IsHitTestVisible = true;
            } else
            {
                popupOwnedInstruments.IsOpen = true;
                dataGridStudent.IsHitTestVisible = false;
            }

            Student current = MainWindow.dataSourceStudent[selected - 1];
            loadOwnedInstruments(current);
            
            txtOwnedInsTitle.Text = "Instruments assigned to " + current.fname.ToUpper() + " " + current.lname.ToUpper();
            isAddingIns = false; // bool to check if the popup is in addition mode
        }

        private void loadOwnedInstruments(Student current)
        {
            txtOwnedInsTitle.Text = "Instruments assigned to " + current.fname.ToUpper() + " " + current.lname.ToUpper();

            List<Instrument> insAssignToStud = new List<Instrument>();
            foreach (var instrument in MainWindow.dataSourceInstrument)
            {

                if (current.email.ToUpper() == instrument.studentID.ToUpper())
                {
                    insAssignToStud.Add(instrument);
                }
            }
            dataOwnedInstruments.ItemsSource = insAssignToStud;

            btnAddOwnedIns.Content = "Assign New Instrument";
            btnRemoveOwnedIns.Content = "Unassign Instrument";
        }

        void updateInstrumentToSheet(int index)
        {
            Debug.WriteLine("run update ins");
            
            var obj = MainWindow.dataSourceInstrument[index - 1];

            GoogleSheetRow curRow = new GoogleSheetRow();

            curRow.Cells.Add(new GoogleSheetCell { CellValue = obj.type.ToString() });
            curRow.Cells.Add(new GoogleSheetCell { CellValue = obj.make.ToString() });
            curRow.Cells.Add(new GoogleSheetCell { CellValue = obj.caseNum.ToString() });
            curRow.Cells.Add(new GoogleSheetCell { CellValue = obj.serialNum.ToString() });
            curRow.Cells.Add(new GoogleSheetCell { CellValue = obj.grade.ToString() });
            curRow.Cells.Add(new GoogleSheetCell { CellValue = obj.studentID.ToString() });
            curRow.Cells.Add(new GoogleSheetCell { CellValue = obj.repairStatus.ToString() });

            var rowsToAdd = new List<GoogleSheetRow>() { curRow };
            gsh.AddCells(new GoogleSheetParameters { SheetName = InstrumentWindow.insSheetName, RangeColumnStart = 1, RangeRowStart = index + 1 }, rowsToAdd);
        }

        private void loadAvailableInstruments()
        {
            txtOwnedInsTitle.Text = "Adding New Instrument";

            List<Instrument> insAvailable = new List<Instrument>();
            foreach (var instrument in MainWindow.dataSourceInstrument)
            {

                if (instrument.studentID == null || instrument.studentID.ToUpper() == "NONE")
                {
                    insAvailable.Add(instrument);
                }
            }
            dataOwnedInstruments.ItemsSource = insAvailable;

            btnAddOwnedIns.Content = "Confirm Assignment";
            btnRemoveOwnedIns.Content = "Cancel";
        }

        private void btnAddOwnedIns_Click(object sender, RoutedEventArgs e)
        {
            if (!isAddingIns)
            {
                
                loadAvailableInstruments();
                isAddingIns = true;
            } else if(dataOwnedInstruments.SelectedItem is Instrument insToAdd)
            {
                Student current = MainWindow.dataSourceStudent[selected - 1];

                MainWindow.dataSourceInstrument[insToAdd.id - 1].grade = current.grade;
                MainWindow.dataSourceInstrument[insToAdd.id - 1].studentID = current.email;

                updateInstrumentToSheet(insToAdd.id);

                isAddingIns=false;

                loadOwnedInstruments(current);
            }
            
        }

        private void btnRemoveOwnedIns_Click(object sender, RoutedEventArgs e)
        {
            Student current = MainWindow.dataSourceStudent[selected - 1];

            if (isAddingIns)
            {
                loadOwnedInstruments(current);
                isAddingIns = false;
            } else if (dataOwnedInstruments.SelectedItem is Instrument insToAdd)
            {
                MainWindow.dataSourceInstrument[insToAdd.id - 1].grade = -1;
                MainWindow.dataSourceInstrument[insToAdd.id - 1].studentID = "none";

                updateInstrumentToSheet(insToAdd.id);

                loadOwnedInstruments(current);
            }

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
            hintLastName.Visibility = Visibility.Visible;
            if (txtAddLastName.Text != "")
            {
                hintLastName.Visibility = Visibility.Hidden;
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
   
