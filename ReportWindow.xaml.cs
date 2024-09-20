using DataManagement.Classes;
using DataManagement;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace EsportsTrackerDatabase
{
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// Report window for showing different export options and views
    /// </summary>
    public partial class ReportWindow : UserControl
    {
        //set data adapter to run queries
        Adapter data = new Adapter();

        //set filemanager for writing to file
        FileManager writer = new FileManager();

        //set variables for window to use
        List<TeamInfo> teamList = new List<TeamInfo>();
        List<Results> resultList = new List<Results>();        

        //set array for combo box to use
        string[] exportBox = { "Teams By Points", "Results By Event", "Results By Team" };

        public ReportWindow()
        {
            InitializeComponent();

            //set content for combo boxes
            cbExport.ItemsSource = exportBox;
            cbExport.SelectedIndex = 0;
            cbTeam.ItemsSource = teamList;
            cbTeam.DisplayMemberPath = "TeamName";

            UpdateData();
        }
        //update data method
        private void UpdateData()
        {
            //set team combo box disabled - only want it to be enabled when
            //"Results By Team" is selected
            lblName.IsEnabled = false;
            cbTeam.IsEnabled = false;
            cbTeam.SelectedItem = null;
            //refresh grid
            dgvReport.Items.Refresh();
        }
        //export button method
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            //show user a message to confirm export
            //shows them directory being saved to
            MessageBoxResult result = MessageBox.Show
               ($"Are you sure you want to Export the [Current View] to:\n " +
               $"\"{AppDomain.CurrentDomain.BaseDirectory}\"?" +
               $"\nFile Explorer will open after saving.",
               $"Exporting {cbExport.SelectedItem}", MessageBoxButton.YesNo);
            switch (result)
            {
                //if yes do write functions
                case MessageBoxResult.Yes:
                    if ((string)cbExport.SelectedItem == exportBox[0])
                    {
                        //Team Details By Comp Points (Descending)
                        teamList = data.GetAllTeams();
                        //sort list by Team Points
                        teamList.Sort((x, y) => y.Points.CompareTo(x.Points));
                        //pass list to writer class to write to file
                        writer.WriteTeamToFile(teamList);
                    }
                    else if ((string)cbExport.SelectedItem == exportBox[1])
                    {
                        //Team Results By Event
                        resultList = data.GetAllResultNames();
                        //add wich team won or draw
                        AddResultText();
                        //sort list by Name of Event
                        resultList.Sort((x, y) => 
                        y.EventName.CompareTo(x.EventName));
                        //pass list to writer class to write to file
                        writer.WriteResultToFile(resultList, "Event");
                    }
                    else if ((string)cbExport.SelectedItem == exportBox[2])
                    {
                        //Team Results By Team
                        resultList = data.GetAllResultNames();
                        //add wich team won or draw
                        AddResultText();
                        //temporary variable to hold selected team name
                        TeamInfo temp = (TeamInfo)cbTeam.SelectedItem;
                        if (temp != null)
                        {
                            //temporary list to hold results
                            //sort through list and pull out all results
                            //where team1 or team2 = selected team     
                            List<Results> tempList =[
                                .. resultList.Where(t => t.Team1Name.Contains(temp.TeamName) ||
                                    t.Team2Name.Contains(temp.TeamName)),];
                            //pass list to writer class to write to file
                            writer.WriteResultToFile(tempList, "Team");
                        }
                    }

                    //open file explorer to show where file was saved to
                    Process.Start("explorer.exe", AppDomain.CurrentDomain.BaseDirectory);
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }
        //method to add result text to results
        private void AddResultText()
        {
            foreach (var result in resultList)
            {
                if (result.Result == 1)
                {
                    result.WinLose = $"{result.Team1Name} Win";
                }
                else if (result.Result == 2)
                {
                    result.WinLose = $"{result.Team2Name} Win";
                }
                else
                {
                    result.WinLose = "Draw";
                }
            }
        }
        //Export combo box method
        private void cbExport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((string)cbExport.SelectedItem == exportBox[0])
            {
                //Show Team Details By Comp Points (Descending)
                teamList = data.GetAllTeams();
                //sort list by team points
                teamList.Sort((x, y) => y.Points.CompareTo(x.Points));
                //set grid view
                dgvReport.ItemsSource = teamList;
                UpdateData();
            }
            else if ((string)cbExport.SelectedItem == exportBox[1])
            {
                //Show Team Results By Event
                resultList = data.GetAllResultNames();
                AddResultText();
                //sort result list by event name
                resultList.Sort((x, y) => y.EventName.CompareTo(x.EventName));
                //set grid view
                dgvReport.ItemsSource = resultList;
                UpdateData();
            }
            else if ((string)cbExport.SelectedItem == exportBox[2])
            {
                //Turn On Team select combo Box
                lblName.IsEnabled = true;
                cbTeam.IsEnabled = true;
            }
        }
        //combo box Team selection method
        private void cbTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //check if enabled if yes change grid view
            if (cbTeam.IsEnabled)
            {
                //Show Team Results of Selected Team
                resultList = data.GetAllResultNames();
                AddResultText();
                //set temp variable to hold selected item
                TeamInfo temp = (TeamInfo)cbTeam.SelectedItem;
                if (temp != null)
                {
                    //sort grid list filter out all result without selected
                    //team name in either team1 or team2
                    dgvReport.ItemsSource = 
                        resultList.Where(t => t.Team1Name.Contains(temp.TeamName)
                        || t.Team2Name.Contains(temp.TeamName));
                    //refresh grid view
                    dgvReport.Items.Refresh();
                }
            }
        }
    }
}