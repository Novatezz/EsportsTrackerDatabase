using DataManagement;
using DataManagement.Classes;
using System.Diagnostics;
using System.Windows;

namespace EsportsTrackerDatabase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Main window methods - just content control
    /// </summary>
    public partial class MainWindow : Window
    {
    public MainWindow()
        {
            InitializeComponent();
            //set default content to result view
            conMain.Content = new ResultsWindow();
        }

        private void Team_Click(object sender, RoutedEventArgs e)
        {
            //sets view to Team window
            conMain.Content = new TeamWindow();
        }

        private void Event_Click(object sender, RoutedEventArgs e)
        {
            //sets view to event window
            conMain.Content = new EventWindow();
        }

        private void Result_Click(object sender, RoutedEventArgs e)
        {
            //sets view to result window
            conMain.Content = new ResultsWindow();
        }

        private void Game_Click(object sender, RoutedEventArgs e)
        {
            //sets view to games window
            conMain.Content = new GameWindow();
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            //shows help pop-up
            MessageBox.Show("Help Coming Soon...");
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            //shows reports and export window
           conMain.Content = new ReportWindow();
        } 
    }
}