using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoundsEasyV1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    

    public partial class MainWindow : Window
    {
        public static string sheetCode = "";
        static bool sheetCodeValid = false;

        public static ObservableCollection<Instrument> dataSourceInstrument = new ObservableCollection<Instrument>();
        public static ObservableCollection<Student> dataSourceStudent = new ObservableCollection<Student>();

        public MainWindow()
        {
            InitializeComponent();
            string s = txtSheetLink.Text;
            if (s.Length > 38 && s.Substring(0, 38) == "https://docs.google.com/spreadsheets/d")
            {
                sheetCodeValid = true;
                int startIndex = 39;
                int endIndex = s.LastIndexOf("/edit");
                sheetCode = s.Substring(startIndex, endIndex - startIndex);

                Debug.WriteLine(sheetCode);
            } else
            {
                sheetCodeValid = false;
            }
            if (sheetCodeValid)
            {
                loadDataInstrument();
                loadDataStudent();
            }
            
        }

        void txtSheetLink_Changed(object sender, TextChangedEventArgs e)
        {
            
        }

        //what happens when instrument button is clicked
        private void btnInstrumentPage_Click(object sender, RoutedEventArgs e)
        {

            string s = txtSheetLink.Text;
            if (s.Length > 38 && s.Substring(0, 38) == "https://docs.google.com/spreadsheets/d")
            {
                sheetCodeValid = true;
                int startIndex = 39;
                int endIndex = s.LastIndexOf("/edit");
                sheetCode = s.Substring(startIndex, endIndex - startIndex);

                Debug.WriteLine(sheetCode);
            }
            else
            {
                sheetCodeValid = false;
            }
            if (sheetCodeValid)
            {
                InstrumentWindow objInstWindow = new InstrumentWindow();
                objInstWindow.Show();
                objInstWindow.Init(ref objInstWindow);
            }
            
        }

        //what happens when student button is clicked
        private void btnStudentPage_Click(object sender, RoutedEventArgs e)
        {
            string s = txtSheetLink.Text;
            if (s.Length > 38 && s.Substring(0, 38) == "https://docs.google.com/spreadsheets/d")
            {
                sheetCodeValid = true;
                int startIndex = 39;
                int endIndex = s.LastIndexOf("/edit");
                sheetCode = s.Substring(startIndex, endIndex - startIndex);

                Debug.WriteLine(sheetCode);
            }
            else
            {
                sheetCodeValid = false;
            }
            if (sheetCodeValid)
            {
                StudentWindow objStudWindow = new StudentWindow();
                objStudWindow.Show();
                objStudWindow.Init(ref objStudWindow);
            }

                

        }
        //what happens when quick assign button is clicked 
        private void btnQuickAssign_Click(object sender, RoutedEventArgs e)
        {
            QuickAssignWindow objQuickWindow = new QuickAssignWindow();
            objQuickWindow.Show();
        }

        private void btnConfirmLink_Click(object sender, RoutedEventArgs e)
        {
            string s = txtSheetLink.Text;
            if (s.Length > 38 && s.Substring(0, 38) == "https://docs.google.com/spreadsheets/d")
            {
                sheetCodeValid = true;
                int startIndex = 39;
                int endIndex = s.LastIndexOf("/edit");
                sheetCode = s.Substring(startIndex, endIndex - startIndex);

                Debug.WriteLine(sheetCode);
            }
            Debug.WriteLine("inst size " + dataSourceInstrument.Count);
            Debug.WriteLine("stud size " + dataSourceStudent.Count);
        }
        public void loadDataStudent()
        {
            GoogleSheetsHelper gsh = new GoogleSheetsHelper("fleet-automata-366622-ba1a276c41b4.json", MainWindow.sheetCode);

            GoogleSheetParameters gsp = new GoogleSheetParameters() { RangeColumnStart = 1, RangeRowStart = 1, RangeColumnEnd = 5, RangeRowEnd = 999, FirstRowIsHeaders = true, SheetName = "Students" };

            dataSourceStudent.Clear();

            gsh.GetStudentDataFromSheet(gsp, ref dataSourceStudent);
        }

        public void loadDataInstrument()
        {
            GoogleSheetsHelper gsh = new GoogleSheetsHelper("fleet-automata-366622-ba1a276c41b4.json", MainWindow.sheetCode);

            GoogleSheetParameters gsp = new GoogleSheetParameters() { RangeColumnStart = 1, RangeRowStart = 1, RangeColumnEnd = 7, RangeRowEnd = 999, FirstRowIsHeaders = true, SheetName = "Instruments" };

            dataSourceInstrument.Clear();

            gsh.GetInstrumentDataFromSheet(gsp, ref dataSourceInstrument);
        }

        
    }
}
