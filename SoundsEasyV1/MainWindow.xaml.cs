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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoundsEasyV1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        

        //what happens when instrument button is clicked
        private void btnInstrumentPage_Click(object sender, RoutedEventArgs e)
        {
            InstrumentWindow objInstWindow = new InstrumentWindow();
            objInstWindow.Show();
            objInstWindow.Init(ref objInstWindow);
        }

        //what happens when student button is clicked
        private void btnStudentPage_Click(object sender, RoutedEventArgs e)
        {
            StudentWindow objStudWindow = new StudentWindow();
            objStudWindow.Show();

        }
        //what happens when quick assign button is clicked 
        private void btnQuickAssign_Click(object sender, RoutedEventArgs e)
        {
            QuickAssignWindow objQuickWindow = new QuickAssignWindow();
            objQuickWindow.Show();
        }

    }
}
