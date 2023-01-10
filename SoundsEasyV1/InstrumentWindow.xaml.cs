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
using System.Windows.Controls.Primitives;

namespace SoundsEasyV1
{
    /// <summary>
    /// Interaction logic for InstrumentWindow.xaml
    /// </summary>
    public partial class InstrumentWindow : Window
    {
        List<double> widthRatios = new List<double> { 1, 1, 1, 1.3, 0.7, 1, 0.7 };
        //ObservableCollection<Instrument> dataSourceInstrument = new ObservableCollection<Instrument>();
        ObservableCollection<Instrument> dataSourceInstrumentFiltered = new ObservableCollection<Instrument>();
        ObservableCollection<Student> dataAssignOptions = new ObservableCollection<Student>();
        InstrumentWindow? thisWindow = null;

        bool isLoading = false;

        private GoogleSheetsHelper? gsh = null;
        private GoogleSheetParameters? gsp = null;

        double popupSize = 0.5;

        int selected = -1;

        Student? recipient;

        string insSheetName = "Instruments";


        public InstrumentWindow()
        {
            InitializeComponent();
            //LoadSheets();
            //set min widths
            //dataGridInstrument.Loaded += SetMinWidths;

            //initialize googlesheets helper
            gsh = new GoogleSheetsHelper("fleet-automata-366622-ba1a276c41b4.json", MainWindow.sheetCode);

            //default settings for googlesheets parameters
            gsp = new GoogleSheetParameters() { RangeColumnStart = 1, RangeRowStart = 1, RangeColumnEnd = 7, RangeRowEnd = 999, FirstRowIsHeaders = true, SheetName = insSheetName };

            Debug.WriteLine("started");

            //dataGridInstrument.ItemsSource = dataSourceInstrument;

            //set pop up size
            popupAddInstrument.Height = SystemParameters.PrimaryScreenHeight * popupSize;
            popupAddInstrument.Width = SystemParameters.PrimaryScreenWidth * popupSize;

            popupAssignInstrument.Height = SystemParameters.PrimaryScreenHeight * popupSize;
            popupAssignInstrument.Width = SystemParameters.PrimaryScreenWidth * popupSize;

            //LoadData();


            Loaded += delegate
            {
                // access ActualWidth and ActualHeight here
            };

            LoadDataFilter();

        }

        void popupAddInstrument_Loaded(object sender, RoutedEventArgs e)
        {
            popupAddInstrument.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            popupAddInstrument.Arrange(new Rect(0, 0, popupAddInstrument.DesiredSize.Width, popupAddInstrument.DesiredSize.Height));
            Debug.WriteLine("loaded window");

            
        }

        public void scaleText(TextBox t)
        {
            t.Loaded += new RoutedEventHandler(runScaleText);
        }

        public void scaleText(TextBlock t)
        {
            t.Loaded += new RoutedEventHandler(runScaleText);
        }

        public void runScaleText(object sender, RoutedEventArgs e)
        {
            
            if (sender.GetType() == typeof(TextBox))
            {
                var obj = sender as TextBox;
                
                if(obj.ActualHeight == 0)
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

        public void Init(ref InstrumentWindow windowObj)
        {
            thisWindow = windowObj;
        }

        /* does not work!!!!!!!
        public void scaleTextDimensions(TextBox t)
        {
            t.FontSize = t.ActualHeight;
            Debug.WriteLine("Height" + t.ActualHeight);
        }
        public void scaleTextDimensions(TextBlock t)
        {
            t.FontSize = t.ActualHeight;
            t.FontSize = t.ActualHeight;
            Debug.WriteLine("Height" + t.ActualHeight);
        }
        public void scaleTextDimensions(Button b)
        {
            b.FontSize = b.ActualHeight;
        }
        */

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


        /* unused code for loading data
        //load entire dataset
        public void LoadData()
        {
            
            
            dataSourceInstrument.Clear();
            isLoading = true;
            gsp = new GoogleSheetParameters() { RangeColumnStart = 1, RangeRowStart = 1, RangeColumnEnd = 7, RangeRowEnd = 100, FirstRowIsHeaders = true, SheetName = insSheetName };
            //run data loading in background
            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync();
            

            
        }
        */
        public struct InstrumentOption
        {
            public string Type { get; set; }
        }


        //takes specific items from the overall dataset
        public void LoadDataFilter()
        {
            dataSourceInstrumentFiltered.Clear();
            for (int i = 0; i < MainWindow.dataSourceInstrument.Count; i++)
            {
                if (checkFilter(MainWindow.dataSourceInstrument[i]))
                {
                    dataSourceInstrumentFiltered.Add(MainWindow.dataSourceInstrument[i]);
                }
                
            }
            if(dataSourceInstrumentFiltered.Count == 0)
            {
                string messageBoxText = "No matching entries found, please try again.";
                string caption = "Did you make a typo?";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
            }
            dataGridInstrument.ItemsSource=dataSourceInstrumentFiltered;

            loadInstrumentOptions();


            //dataGridInstrument.ColumnWidth = new DataGridLength(1, DataGridLengthUnitType.Star);
        }

        
        private void loadInstrumentOptions()
        {
            HashSet<string> s = new HashSet<string>();
            foreach (Instrument i in MainWindow.dataSourceInstrument)
            {
                s.Add(i.type);
                Debug.WriteLine(i.type);
            }

            List<InstrumentOption> tempSource = new List<InstrumentOption>();

            foreach (string val in s)
            {
                tempSource.Add(new InstrumentOption { Type = val });
            }

            dataInstrumentOptions.ItemsSource = tempSource;
        }
        /* unused code for loading data
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
            
            gsh.GetInstrumentDataFromSheet(gsp, ref dataSourceInstrument, ref thisWindow, ref worker);

            //LoadSheets();
        }

        private void worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            progressInstrumentLoad.Visibility = Visibility.Hidden;
            progressTextInstrument.Visibility= Visibility.Hidden;
            dataGridInstrument.ItemsSource = dataSourceInstrument;
            isLoading = false;
            LoadDataFilter();

        }

        public void LoadSheets()
        {
            //dataGridInstrument.ItemsSource = dataSourceInstrument;
        }
        */
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
                    if(target.studentID == "none")
                    {
                        btnManageAssign.Content = "Assign to Student";
                    } else
                    {
                        btnManageAssign.Content = "Unassign Instrument";
                    }
                }
                
                
            }
        }

        private void dataInstrumentOptions_Click(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid != null)
            {
                if (dataGrid.SelectedItem is InstrumentOption target)
                {
                    txtAddInstrumentType.Text = target.Type;
                }


            }
        }

        private void dataAssignInstrument_Click(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid != null)
            {
                if (dataGrid.SelectedItem is Student)
                {
                    recipient = dataGrid.SelectedItem as Student;
                }


            }
        }

        private void btnRepairChange_Click(object sender, RoutedEventArgs e)
        {
            if(selected >= 0 && selected <= MainWindow.dataSourceInstrument.Count && dataGridInstrument.SelectedItem != null)
            {
                if(MainWindow.dataSourceInstrument[selected-1].repairStatus == "good")
                {
                    MainWindow.dataSourceInstrument[selected-1].repairStatus = "broken";
                    Debug.WriteLine(dataGridInstrument.SelectedIndex + "  " + selected);
                    dataSourceInstrumentFiltered[dataGridInstrument.SelectedIndex].repairStatus = "broken";
                } else
                {
                    MainWindow.dataSourceInstrument[selected-1].repairStatus = "good";
                    Debug.WriteLine(dataGridInstrument.SelectedIndex + "  " + selected);
                    dataSourceInstrumentFiltered[dataGridInstrument.SelectedIndex].repairStatus = "good";
                }

                updateRowToSheet(selected);
                
            }
        }

        private void btnManageAssign_Click(object sender, RoutedEventArgs e)
        {
            if (selected >= 0 && selected <= MainWindow.dataSourceInstrument.Count && MainWindow.dataSourceInstrument[selected-1].studentID!="none")
            {
                MainWindow.dataSourceInstrument[selected - 1].studentID = "none";
                MainWindow.dataSourceInstrument[selected - 1].grade = -1;

                dataSourceInstrumentFiltered[dataGridInstrument.SelectedIndex].studentID = "none";
                dataSourceInstrumentFiltered[dataGridInstrument.SelectedIndex].grade = -1;

                updateRowToSheet(selected);

            } else if(selected >= 0 && selected <= MainWindow.dataSourceInstrument.Count && MainWindow.dataSourceInstrument[selected - 1].studentID == "none")
            {
                if (!popupAssignInstrument.IsOpen)
                {
                    dataAssignInstrument.ItemsSource = dataAssignOptions;
                    popupAssignInstrument.IsOpen = true;
                }
            }
        }

        private void btnRemoveInstrument_Click(object sender, RoutedEventArgs e)
        {
            if (selected >= 0 && selected <= MainWindow.dataSourceInstrument.Count && dataGridInstrument.SelectedItem!=null)
            {
                MainWindow.dataSourceInstrument.RemoveAt(selected - 1);
                Debug.WriteLine(dataGridInstrument.SelectedIndex + "  " + selected);
                dataSourceInstrumentFiltered.RemoveAt(dataGridInstrument.SelectedIndex);
                gsh.RemoveRow(selected, selected+1);
            } else if(dataGridInstrument.SelectedItem != null)
            {
                Debug.WriteLine(dataGridInstrument.SelectedIndex + "  " + selected);
            }
        }

        private void btnLoadInstruments_Click(object sender, RoutedEventArgs e)
        {
            if(!isLoading)
                LoadDataFilter();
            
        }

        private void btnConfirmAddInstrument_Click(object sender, RoutedEventArgs e)
        {
            if (addInstrumentEntry())
            {
                resetPopup();
            }
        }

        private bool addInstrumentEntry()
        {
            Instrument cur = Instrument.CreateInstrument();
            var curRow = new GoogleSheetRow();

            if (txtAddInstrumentType.Text.Length > 0)
            {
                cur.type = txtAddInstrumentType.Text;
                //add item to GoogleSheetRow
                curRow.Cells.Add(new GoogleSheetCell { CellValue = txtAddInstrumentType.Text });
            } 
            else
            {
                return false;
            }

            if (txtAddInstrumentMake.Text.Length > 0)
            {
                cur.make = txtAddInstrumentMake.Text;
                curRow.Cells.Add(new GoogleSheetCell { CellValue = txtAddInstrumentMake.Text });
            }
            else
            {
                return false;
            }

            if (txtAddInstrumentCase.Text.Length > 0)
            {
                cur.caseNum = txtAddInstrumentCase.Text;
                curRow.Cells.Add(new GoogleSheetCell { CellValue = txtAddInstrumentCase.Text });
            }
            else
            {
                return false;
            }

            if (txtAddInstrumentSerial.Text.Length > 0)
            {
                cur.serialNum = txtAddInstrumentSerial.Text;
                curRow.Cells.Add(new GoogleSheetCell { CellValue = txtAddInstrumentSerial.Text });
            }
            else
            {
                return false;
            }

            if (radioG9.IsChecked == true)
            {
                cur.grade = 9;
            }
            else if (radioG10.IsChecked == true)
            {
                cur.grade = 10;
            }
            else if (radioG11.IsChecked == true)
            {
                cur.grade = 11;
            }
            else if (radioG12.IsChecked == true)
            {
                cur.grade = 12;
            }
            else
            {
                cur.grade = -1;
            }
            curRow.Cells.Add(new GoogleSheetCell { CellValue = cur.grade.ToString() });

            if (txtAddInstrumentStudentID.Text.Length > 0)
            {
                cur.studentID = txtAddInstrumentStudentID.Text;
                curRow.Cells.Add(new GoogleSheetCell { CellValue = txtAddInstrumentStudentID.Text });
            }
            else
            {
                cur.studentID = "none";
                curRow.Cells.Add(new GoogleSheetCell { CellValue = "none" });
            }

           

            cur.id = MainWindow.dataSourceInstrument.Count + 1;

            cur.repairStatus = "good";
            curRow.Cells.Add(new GoogleSheetCell { CellValue = "good" });

            MainWindow.dataSourceInstrument.Add(cur);
            LoadDataFilter();
            
            Debug.WriteLine("cols " + curRow.Cells.Count);

            //the function requires a list of rows
            var rowsToAdd = new List<GoogleSheetRow>() { curRow };
            //add cells request with a built-in construction of a googlesheetparameter
            gsh.AddCells(new GoogleSheetParameters { SheetName = insSheetName, RangeColumnStart = 1, RangeRowStart = MainWindow.dataSourceInstrument.Count + 1 }, rowsToAdd);
            return true;

        }

        private void resetPopup()
        {
            txtAddInstrumentCase.Clear();
            txtAddInstrumentMake.Clear();
            txtAddInstrumentSerial.Clear();
            txtAddInstrumentStudentID.Clear();
            txtAddInstrumentType.Clear();
            popupAddInstrument.IsOpen = false;
        }

        public void addData(Instrument i)
        {
            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                MainWindow.dataSourceInstrument.Add(i);

            });
        }

        
        void updateRowToSheet(int index)
        {
            var obj = MainWindow.dataSourceInstrument[index-1];

            GoogleSheetRow curRow = new GoogleSheetRow();

            curRow.Cells.Add(new GoogleSheetCell { CellValue = obj.type.ToString() });
            curRow.Cells.Add(new GoogleSheetCell { CellValue = obj.make.ToString() });
            curRow.Cells.Add(new GoogleSheetCell { CellValue = obj.caseNum.ToString() });
            curRow.Cells.Add(new GoogleSheetCell { CellValue = obj.serialNum.ToString() });
            curRow.Cells.Add(new GoogleSheetCell { CellValue = obj.grade.ToString() });
            curRow.Cells.Add(new GoogleSheetCell { CellValue = obj.studentID.ToString() });
            curRow.Cells.Add(new GoogleSheetCell { CellValue = obj.repairStatus.ToString() });

            var rowsToAdd = new List<GoogleSheetRow>() { curRow };
            gsh.AddCells(new GoogleSheetParameters { SheetName = insSheetName, RangeColumnStart = 1, RangeRowStart = index + 1 }, rowsToAdd);
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

        //handling the data stuff in the assignment popup
        //when search button is clicked gives a list of students to choose from
        private void btnLoadInsAssign_Click(object sender, RoutedEventArgs e)
        {
            dataAssignOptions.Clear();
            foreach(Student st in MainWindow.dataSourceStudent)
            {
                if (checkAssignFilter(st))
                {
                    dataAssignOptions.Add(st);
                }
            }
        }

        private bool checkAssignFilter(Student s)
        {
            if (s.fname != filterAssignFName.Text && filterAssignFName.Text.Length > 0)
            {
                return false;
            }
            if (s.lname != filterAssignLName.Text && filterAssignLName.Text.Length > 0)
            {
                return false;
            }
            if (s.course != filterAssignCourse.Text && filterAssignCourse.Text.Length > 0)
            {
                return false;
            }
            if (s.grade.ToString() != filterAssignGrade.Text && filterAssignGrade.Text.Length > 0)
            {
                return false;
            }
            if (s.email != filterAssignEmail.Text && filterAssignEmail.Text.Length > 0)
            {
                return false;
            }

            return true;
        }

        //when confirmed assignment, set instrument's owner properties and update row to sheet
        private void btnConfirmAssign_Click(object sender, RoutedEventArgs e)
        {
            if (recipient != null)
            {
                //Debug.WriteLine("not null");
                if (selected >= 0 && selected <= MainWindow.dataSourceInstrument.Count && MainWindow.dataSourceInstrument[selected - 1].studentID == "none")
                {
                    //Debug.WriteLine("ran assigned");
                    MainWindow.dataSourceInstrument[selected - 1].studentID = recipient.email;
                    MainWindow.dataSourceInstrument[selected - 1].grade = recipient.grade;

                    dataSourceInstrumentFiltered[dataGridInstrument.SelectedIndex].studentID = recipient.email;
                    dataSourceInstrumentFiltered[dataGridInstrument.SelectedIndex].grade = recipient.grade;

                    updateRowToSheet(selected);

                }
            }
        }

        //when cancelled, exit the popup
        private void btnCancelAssign_Click(object sender, RoutedEventArgs e)
        {
            popupAssignInstrument.IsOpen = false;
        }

        //handling hint texts
        void hintChangeInsType(object sender, TextChangedEventArgs e)
        {
            hintInsType.Visibility = Visibility.Visible;
            if (txtAddInstrumentType.Text != "")
            {
                hintInsType.Visibility = Visibility.Hidden;
            }
        }

        void hintChangeInsMake(object sender, TextChangedEventArgs e)
        {
            hintInsMake.Visibility = Visibility.Visible;
            if (txtAddInstrumentMake.Text != "")
            {
                hintInsMake.Visibility = Visibility.Hidden;
            }
        }

        void hintChangeInsCase(object sender, TextChangedEventArgs e)
        {
            hintInsCase.Visibility = Visibility.Visible;
            if (txtAddInstrumentCase.Text != "")
            {
                hintInsCase.Visibility = Visibility.Hidden;
            }
        }

        void hintChangeInsSerial(object sender, TextChangedEventArgs e)
        {
            hintInsSerial.Visibility = Visibility.Visible;
            if (txtAddInstrumentSerial.Text != "")
            {
                hintInsSerial.Visibility = Visibility.Hidden;
            }
        }

        void hintChangeInsSID(object sender, TextChangedEventArgs e)
        {
            hintInsSID.Visibility = Visibility.Visible;
            if (txtAddInstrumentStudentID.Text != "")
            {
                hintInsSID.Visibility = Visibility.Hidden;
            }
        }

    }
}
