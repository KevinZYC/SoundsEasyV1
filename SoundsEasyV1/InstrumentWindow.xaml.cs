﻿using Google.Apis.Auth.OAuth2;
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

        double popupSize = 0.5;

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

            //dataGridInstrument.ItemsSource = dataSource;

            //set pop up size
            popupAddInstrument.Height = SystemParameters.PrimaryScreenHeight * popupSize;
            popupAddInstrument.Width = SystemParameters.PrimaryScreenWidth * popupSize;

            LoadData();

            scaleTextDimensions(txtAddInstrumentType);
            scaleTextDimensions(txtAddInstrumentMake);
            scaleTextDimensions(txtAddInstrumentCase);
            scaleTextDimensions(txtAddInstrumentSerial);
            scaleTextDimensions(txtAddInstrumentStudentID);

            scaleTextDimensions(hintInsType);
            scaleTextDimensions(hintInsMake);
            scaleTextDimensions(hintInsCase);
            scaleTextDimensions(hintInsSerial);
            scaleTextDimensions(hintInsSID);

        }
        
        public void Init(ref InstrumentWindow windowObj)
        {
            thisWindow = windowObj;
        }

        public void scaleTextDimensions(TextBox t)
        {
            t.FontSize = SystemParameters.PrimaryScreenHeight * popupSize / 25;
            Thickness margin = t.Margin;
            margin.Top = SystemParameters.PrimaryScreenHeight * popupSize / 50;
            margin.Bottom = SystemParameters.PrimaryScreenHeight * popupSize / 50;
            margin.Left = SystemParameters.PrimaryScreenHeight * popupSize / 50;
            margin.Right = SystemParameters.PrimaryScreenHeight * popupSize / 50;
            t.Margin = margin;
        }
        public void scaleTextDimensions(TextBlock t)
        {

        }
        public void scaleTextDimensions(Button b)
        {

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
            gsp = new GoogleSheetParameters() { RangeColumnStart = 1, RangeRowStart = 1, RangeColumnEnd = 7, RangeRowEnd = 100, FirstRowIsHeaders = true, SheetName = "Sheet1" };
            //run data loading in background
            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync();
            

            
        }

        public struct InstrumentOption
        {
            public string Type { get; set; }
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

            loadInstrumentOptions();


            //dataGridInstrument.ColumnWidth = new DataGridLength(1, DataGridLengthUnitType.Star);
        }

        private void loadInstrumentOptions()
        {
            HashSet<string> s = new HashSet<string>();
            foreach (Instrument i in dataSourceFiltered)
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
            LoadDataFilter();
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

        private void btnRepairChange_Click(object sender, RoutedEventArgs e)
        {
            if(selected >= 0 && selected < dataSource.Count && dataGridInstrument.SelectedItem != null)
            {
                if(dataSource[selected-1].repairStatus == "good")
                {
                    dataSource[selected-1].repairStatus = "broken";
                    Debug.WriteLine(dataGridInstrument.SelectedIndex + "  " + selected);
                    dataSourceFiltered[dataGridInstrument.SelectedIndex].repairStatus = "broken";
                } else
                {
                    dataSource[selected-1].repairStatus = "good";
                    Debug.WriteLine(dataGridInstrument.SelectedIndex + "  " + selected);
                    dataSourceFiltered[dataGridInstrument.SelectedIndex].repairStatus = "good";
                }
                
            }
        }

        private void btnRemoveInstrument_Click(object sender, RoutedEventArgs e)
        {
            if (selected >= 0 && selected <= dataSource.Count && dataGridInstrument.SelectedItem!=null)
            {
                dataSource.RemoveAt(selected - 1);
                Debug.WriteLine(dataGridInstrument.SelectedIndex + "  " + selected);
                dataSourceFiltered.RemoveAt(dataGridInstrument.SelectedIndex);
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

            if (txtAddInstrumentStudentID.Text.Length > 0)
            {
                cur.studentID = txtAddInstrumentStudentID.Text;
                curRow.Cells.Add(new GoogleSheetCell { CellValue = txtAddInstrumentStudentID.Text });
            }
            else
            {
                cur.studentID = "none";
            }

            if (radioG9.IsChecked == true)
            {
                cur.grade = 9;
            } else if(radioG10.IsChecked == true)
            {
                cur.grade = 10;
            } else if(radioG11.IsChecked == true)
            {
                cur.grade = 11;
            } else if(radioG12.IsChecked == true)
            {
                cur.grade = 12;
            } else
            {
                cur.grade = -1;
            }
            curRow.Cells.Add(new GoogleSheetCell { CellValue = cur.grade.ToString() });

            cur.id = dataSource.Count + 1;

            cur.repairStatus = "good";
            curRow.Cells.Add(new GoogleSheetCell { CellValue = "good" });

            dataSource.Add(cur);
            LoadDataFilter();
            
            Debug.WriteLine("cols " + curRow.Cells.Count);

            //the function requires a list of rows
            var rowsToAdd = new List<GoogleSheetRow>() { curRow };
            //add cells request with a built-in construction of a googlesheetparameter
            gsh.AddCells(new GoogleSheetParameters { SheetName = "Sheet1", RangeColumnStart = 1, RangeRowStart = dataSource.Count + 1 }, rowsToAdd);
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
