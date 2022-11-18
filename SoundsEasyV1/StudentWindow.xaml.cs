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

namespace SoundsEasyV1
{
    /// <summary>
    /// Interaction logic for StudentWindow.xaml
    /// </summary>
    public partial class StudentWindow : Window
    {
        public ObservableCollection<Student> dataSource = new ObservableCollection<Student>();
        

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

            dataGridStudent.ItemsSource = dataSource;
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

        private void btnLoadStudents_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoading)
                LoadData();

        }

        public void addData(Student i)
        {
            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                dataSource.Add(i);
                Debug.WriteLine(dataSource.Count);

            });
        }
    }

    

}

