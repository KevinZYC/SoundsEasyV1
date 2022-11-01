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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SoundsEasyV1
{
    /// <summary>
    /// Interaction logic for InstrumentWindow.xaml
    /// </summary>
    public partial class InstrumentWindow : Window
    {
        List<double> widthRatios = new List<double> { 1, 1, 1, 1.3, 0.7, 1, 0.7 };
        
        public InstrumentWindow()
        {
            InitializeComponent();
            LoadSheets();
            //set min widths
            dataGridInstrument.Loaded += SetMinWidths;
        }
        
        private List<Instrument> CreateList(List<ExpandoObject> myList)
        {
            List<Instrument> ret = new List<Instrument>();

            foreach (var item in myList)
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

        public void LoadSheets()
        {
            var gsh = new GoogleSheetsHelper("fleet-automata-366622-ba1a276c41b4.json", "1POh7lSt7QyI45I_16I3An1iTWSc4PsV0rcYP5ExPKhg");

            var gsp = new GoogleSheetParameters() { RangeColumnStart = 1, RangeRowStart = 1, RangeColumnEnd = 7, RangeRowEnd = 999, FirstRowIsHeaders = true, SheetName = "sheet1" };
            var rowValues = gsh.GetDataFromSheet(gsp);

            
            dataGridInstrument.ItemsSource = CreateList(rowValues);

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

    }
}
