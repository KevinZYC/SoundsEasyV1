using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Dynamic;

using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace SoundsEasyV1
{
    /// <summary>
    /// Interaction logic for InstrumentWindow.xaml
    /// </summary>
    public partial class InstrumentWindow : Window
    {
        List<double> widthRatios = new List<double> { 1, 1, 1, 1.3, 0.7, 1, 0.7 };
        ObservableCollection<Instrument> dataSource = new ObservableCollection<Instrument>();
        ObservableCollection<Instrument> dataSourceFiltered = new ObservableCollection<Instrument>();
        InstrumentWindow? thisWindow = null;

        bool isLoading = false;

        private GoogleSheetsHelper? gsh = null;
        private GoogleSheetParameters? gsp = null;

        int selected = -1;


        public InstrumentWindow()
        {
            InitializeComponent();
            //LoadSheets();
            //set min widths
            //dataGridInstrument.Loaded += SetMinWidths;

            //initialize googlesheets helper
            gsh = new GoogleSheetsHelper("fleet-automata-366622-ba1a276c41b4.json", "1POh7lSt7QyI45I_16I3An1iTWSc4PsV0rcYP5ExPKhg");

            //default settings for googlesheets parameters
            gsp = new GoogleSheetParameters() { RangeColumnStart = 1, RangeRowStart = 1, RangeColumnEnd = 7, RangeRowEnd = 100, FirstRowIsHeaders = true, SheetName = "sheet1" };

            Debug.WriteLine("started");

            dataGridInstrument.ItemsSource = dataSource;

            LoadData();
        }
        
        public void Init(ref InstrumentWindow windowObj)
        {
            thisWindow = windowObj;
        }

        /* currently unused
        private ObservableCollection<Instrument> CreateObservableCollection(ObservableCollection<ExpandoObject> myObservableCollection)
        {
            ObservableCollection<Instrument> ret = new ObservableCollection<Instrument>();

            foreach (var item in myObservableCollection)
            {
                var dict = (IDictionary<string, object>)item;
                var type = dict["Type"] as string;
                var make = dict["Make"] as string;
                var caseN = dict["Case Number"] as string;
                var serial = dict["Serial Number"] as string;

                var grade = -1;
                try
                {
                    grade = Int32.Parse(dict["Grade"] as string);
                } catch (InvalidCastException e)
                {
                    break; //when reaching an item with invalid grade, end data fetching
                }

                var sID = dict["Student ID"] as string;
                var repair = dict["Repair Status"] as string;

                ret.Add(new Instrument(type, make, caseN, serial, grade, sID, repair));
            }

            return ret;
        }
        */

        //load entire dataset
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

        //takes specific items from the overall dataset
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
            dataGridInstrument.ItemsSource=dataSourceFiltered;

        }

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
            progressTextInstrument.Visibility= Visibility.Hidden;
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
            if(dataGrid != null)
            {
                Instrument target = dataGrid.SelectedItem as Instrument;
                if(target != null)
                {
                    selected = target.id;
                    Debug.WriteLine(selected);
                }
                
                
            }
        }

        private void btnRepairChange_Click(object sender, RoutedEventArgs e)
        {
            if(selected >= 0 && selected < dataSource.Count)
            {
                dataSource[selected].repairStatus = "broken";
                Debug.WriteLine(dataGridInstrument.SelectedIndex + "  " + dataSourceFiltered.Count);
                dataSourceFiltered[dataGridInstrument.SelectedIndex].repairStatus = "broken";
            }
        }

        private void btnLoadInstruments_Click(object sender, RoutedEventArgs e)
        {
            if(!isLoading)
                LoadDataFilter();
            
        }

        public void addData(Instrument i)
        {
            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                dataSource.Add(i);

            });
        }

        //pulls the filter text box's contents and check if an instrument fits the filter
        private bool checkFilter(Instrument i)
        {
            if(i.type != txtInstrumentType.Text && txtInstrumentType.Text.Length > 0)
            {
                return false;
            }
            if (i.make != txtInstrumentMake.Text && txtInstrumentMake.Text.Length > 0)
            {
                return false;
            }
            if (i.caseNum != txtInstrumentCase.Text && txtInstrumentCase.Text.Length > 0)
            {
                return false;
            }
            if (i.serialNum != txtInstrumentSerial.Text && txtInstrumentSerial.Text.Length > 0)
            {
                return false;
            }
            if (i.grade.ToString() != txtInstrumentGrade.Text && txtInstrumentGrade.Text.Length > 0)
            {
                return false;
            }
            if (i.studentID != txtInstrumentStudent.Text && txtInstrumentStudent.Text.Length > 0)
            {
                return false;
            }
            if (i.repairStatus != txtInstrumentRepair.Text && txtInstrumentRepair.Text.Length > 0)
            {
                return false;
            }

            return true;
        }

    }
}
