using System.Windows;
using System.Windows.Controls;
using DataManagement;
using DataManagement.Classes;


namespace EsportsTrackerDatabase
{
    /// <summary>
    /// Interaction logic for TeamWindow.xaml
    /// Window to show team info and details
    /// </summary>

    public partial class TeamWindow : UserControl
    {
        //set data adapter to run sql queries
        Adapter data = new Adapter();
        //set variable to hold list of teams
        List<TeamInfo> teamList = new List<TeamInfo>();
        List<ResultsId> resultsList = new List<ResultsId>();
        public TeamWindow()
        {
            InitializeComponent();
            UpdateData();
            //disable edit and delete buttons on startup
            btnDel.IsEnabled = false;
            btnEdit.IsEnabled = false;
        }
        //update method used to reset grid and set combo boxes
        private void UpdateData()
        {
            //get teams
            teamList = data.GetAllTeams();
            //set grid to list of teams
            dgvTeam.ItemsSource = teamList;
            //refresh grid
            dgvTeam.Items.Refresh();
            //set combo box
            cbTeamName.ItemsSource = teamList;
            cbTeamName.DisplayMemberPath = "TeamName";
        }
        //data grid selection method
        private void dgvTeam_SelectionChanged(object sender,
            SelectionChangedEventArgs e)
        {
            //set temporary variable to hold grid selection
            TeamInfo team = (TeamInfo)dgvTeam.SelectedItem;
            if (team != null)
            {
                //set text boxes to team details of grid selection
                cbTeamName.Text = team.TeamName;
                txtContactName.Text = team.ContactName;
                txtPhoneNum.Text = team.ContactPhone;
                txtEmail.Text = team.ContactEmail;
                txtPoints.Text = Convert.ToString(team.Points);

                //enable edit and delete buttons
                btnDel.IsEnabled = true;
                btnEdit.IsEnabled = true;
            }
        }
        //combo box selection method
        private void cbTeamName_SelectionChanged(object sender,
            SelectionChangedEventArgs e)
        {
            //set temporary variable to hold combo box selection
            TeamInfo team = (TeamInfo)cbTeamName.SelectedItem;
            if (team != null)
            {
                //set text boxes to team details of combo box selection
                cbTeamName.Text = team.TeamName;
                txtContactName.Text = team.ContactName;
                txtPhoneNum.Text = team.ContactPhone;
                txtEmail.Text = team.ContactEmail;
                txtPoints.Text = Convert.ToString(team.Points);

                //enable edit and delete buttons
                btnDel.IsEnabled = true;
                btnEdit.IsEnabled = true;
            }
        }
        //edit button method
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //create pop-up passing in combo box selection
            //note: updates with grid selection too
            NewTeamPopup newTeamPopup =
                new NewTeamPopup((TeamInfo)cbTeamName.SelectedItem);
            //set opacity down to make pop-up stand out
            Opacity = 0.4;
            //show pop-up
            newTeamPopup.ShowDialog();
            if (newTeamPopup.Success)
            {
                //if data entry was successful run sql with that data
                data.UpdateTeam(newTeamPopup.saveTeam);
                //reset grid
                UpdateData();
            }
            //set opacity back to normal
            Opacity = 1;
        }
        //new button method
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            //create pop-up (note no overloads)
            NewTeamPopup newTeamPopup = new NewTeamPopup();
            //set opacity down to make pop-up stand out
            Opacity = 0.4;
            //show pop-up
            newTeamPopup.ShowDialog();
            if (newTeamPopup.Success)
            {
                //if data entry was successful run sql with that data
                data.AddNewTeam(newTeamPopup.saveTeam);
                //reset grid
                UpdateData();
            }
            //set opacity back to normal
            Opacity = 1;
        }
        //delete button method
        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            //temporary variable to hold selection
            TeamInfo team = (TeamInfo)cbTeamName.SelectedItem;
            if (team != null)
            {
                //show warning message 
                MessageBoxResult result = MessageBox.Show
                    ("Are you sure you want to delete?\n" +
                    "\n!!!WARNING THIS WILL DELETE ALL RESULTS " +
                    "CONTANING THIS TEAM!!!",
                    $"Deleting {team.TeamName}...", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        //if yes run sql deleting team and related results
                        data.DeleteTeam(team.TeamId);
                        //re-calculate points
                        UpdatePointsFromResults();
                        //reset grid
                        UpdateData();
                        //disable edit and delete buttons
                        btnDel.IsEnabled = false;
                        btnEdit.IsEnabled = false;
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }
        //method to recalculate points from results (used when deleting teams)
        private void UpdatePointsFromResults()
        {
            //read lists in
            resultsList = data.GetAllResultIds();
            teamList = data.GetAllTeams();
            //set all teams points to 0
            foreach (var team in teamList)
            {
                team.Points = 0;
            }
            //loops through each result
            foreach (var result in resultsList)
            {
                //loops through each team
                foreach (var team in teamList)
                {
                    //if team = team1
                    if (result.Team1Id.Equals(team.TeamId.ToString()))
                    {
                        //if result was a draw add 1 point to team 1
                        if (result.Result == 0)
                        {
                            team.Points++;
                        }//if result was a win add 2 points to team 1
                        else if (result.Result == 1)
                        {
                            team.Points += 2;
                        }
                    }
                    //if team = team2
                    if (result.Team2Id.Equals(team.TeamId.ToString()))
                    {
                        //if result was a draw add 1 point to team 2
                        if (result.Result == 0)
                        {
                            team.Points++;
                        }//if result was a win add 2 points to team 2
                        else if (result.Result == 2)
                        {
                            team.Points += 2;
                        }
                    }
                    //run query updating point for current team
                    data.UpdateTeamScore(team);
                }
            }
        }
    }
}
