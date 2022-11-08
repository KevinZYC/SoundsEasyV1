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

namespace SoundsEasyV1
{
    /// <summary>
    /// Interaction logic for StudentWindow.xaml
    /// </summary>
    public partial class StudentWindow : Window
    {
        public ObservableCollection<Student> dataSource = new ObservableCollection<Student>();
        Student? thisWindow = null;

        bool isLoading = false;

        private GoogleSheetsHelper? gsh = null;
        private GoogleSheetParameters? gsp = null;


        public StudentWindow()
        {
            InitializeComponent();

            gsh = new GoogleSheetsHelper("fleet-automata-366622-ba1a276c41b4.json", "1NolOQnhFOmPTaXXp7G1tNkrl8is3gVe9X1AIjgEKg7c");

            gsp = new GoogleSheetParameters() { RangeColumnStart = 1, RangeRowStart = 1, RangeColumnEnd = 7, RangeRowEnd = 100, FirstRowIsHeaders = true, SheetName = "sheet1" };

            dataGridStudent.ItemsSource = dataSource;
        }
    }

    public void Init(ref StudentWindow windowObj)
    {
        thisWindow = windowObj;
    }

    public void LoadData()
    {


        dataSource.Clear();
        isLoading = true;
        gsp = new GoogleSheetParameters() { RangeColumnStart = 1, RangeRowStart = 1, RangeColumnEnd = 7, RangeRowEnd = 100, FirstRowIsHeaders = true, SheetName = "sheet1" };
        //run data loading in background
        BackgroundWorker worker = new BackgroundWorker();
        worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        worker.WorkerReportsProgress = true;
        worker.DoWork += worker_DoWork;
        worker.ProgressChanged += worker_ProgressChanged;
        worker.RunWorkerAsync();



    }
    //copied files from InstrumentWindow.xaml.cs, need to change to apply to Student Window from here on
    private void worker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
    {
        progressInstrumentLoad.Visibility = Visibility.Visible;
        progressInstrumentLoad.Value = e.ProgressPercentage;
        progressTextInstrument.Visibility = Visibility.Visible;
        progressTextInstrument.Text = (string)e.UserState;

    }

    private void worker_DoWork(object? sender, DoWorkEventArgs e)
    {
        var worker = sender as BackgroundWorker;

        gsh.GetInstrumentDataFromSheet(gsp, ref dataSource, ref thisWindow, ref worker);

        //LoadSheets();
    }

    private void worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        progressInstrumentLoad.Visibility = Visibility.Hidden;
        progressTextInstrument.Visibility = Visibility.Hidden;
        dataGridInstrument.ItemsSource = dataSource;
        isLoading = false;
    }

    public void LoadSheets()
    {
        //dataGridInstrument.ItemsSource = dataSource;
    }

    //function to pass into xaml to distribute widths
    public void SetMinWidths(object source, EventArgs e)
    {
        int i = 0;
        foreach (var column in dataGridInstrument.Columns)
        {
            column.MinWidth = column.ActualWidth;
            column.Width = new DataGridLength(widthRatios[i], DataGridLengthUnitType.Star);
            i++;
        }
    }

    //when a row in the data is selected:
    private void InstrumentRow_Click(object sender, MouseButtonEventArgs e)
    {
        var dataGrid = sender as DataGrid;
        if (dataGrid != null)
        {
            int index = dataGrid.SelectedIndex;
            Debug.WriteLine(index);

        }
    }

    private void btnLoadInstruments_Click(object sender, RoutedEventArgs e)
    {
        if (!isLoading)
            LoadData();

    }

    public void addData(Instrument i)
    {
        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
        {
            dataSource.Add(i);

        });
    }

}
