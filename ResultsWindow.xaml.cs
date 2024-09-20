using System.Windows;
using System.Windows.Controls;
using DataManagement;
using DataManagement.Classes;

namespace EsportsTrackerDatabase
{
    /// <summary>
    /// Interaction logic for ResultsWindow.xaml
    /// Window to view the results stored in database
    /// </summary>
    public partial class ResultsWindow : UserControl
    {
        //initialise lists/classes to be used in this window
        Adapter data = new Adapter();
        List<Results> resultList = new List<Results>();
        List<TeamInfo> teamList = new List<TeamInfo>();
        List<Events> eventList = new List<Events>();
        List<Games> gameList = new List<Games>();
        public ResultsWindow()
        {
            InitializeComponent();
            UpdateData();
            //disables buttons untill actions are made
            btnEdit.IsEnabled = false; 
            btnDelete.IsEnabled = false;
            //if there is less than 2 teams or no events or no games disables
            //adding new result
            if(teamList.Count < 2 || eventList.Count == 0 || gameList.Count == 0) 
            {
                btnNew.IsEnabled = false;
            }
        }
        //method to update the data in the data grid and reset lists
        //from database
        private void UpdateData()
        {
            //resets lists from database
            resultList = data.GetAllResultNames();
            AddResultText();
            teamList = data.GetAllTeams();
            eventList = data.GetAllEvents();
            gameList = data.GetAllGames();
            //sets data grid view
            dgvResults.ItemsSource = resultList;
            dgvResults.Items.Refresh();
            SetComboBoxes();
        }
        //method to set text of combo boxes
        private void SetComboBoxes()
        {
            //sets combo box items
            cbResultNo.ItemsSource = resultList;
            cbResultNo.DisplayMemberPath = "ResultId";
            cbTeam1.ItemsSource = teamList;
            cbTeam1.DisplayMemberPath = "TeamName";
            cbEvent.ItemsSource = eventList;
            cbEvent.DisplayMemberPath = "EventName";
            cbGame.ItemsSource = gameList;
            cbGame.DisplayMemberPath = "GameName";
        }
        //reset button method
        //resets datagrid view when clicked
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            //resets datagrid & combo boxes
            UpdateData();
            //sets combo boxes to blank
            cbGame.SelectedItem = null;
            cbTeam1.SelectedItem = null;
            cbEvent.SelectedItem = null;
            btnDelete.IsEnabled = false;
            btnEdit.IsEnabled = false;
        }
        //new result button method
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            //create pop-up for entering data (note no overload
            //We arent passing anything to pop-up window)
            ResultPopup newResult = new ResultPopup();
            //set background opacity to make pop-up stand out more
            Opacity = 0.4;
            //show pop-up
            newResult.ShowDialog();
            //if clicked "save" in pop-up perform sql functions to save
            if (newResult.Success)
            {
                //calls sql function overloaded with data from pop-up
                //{team1}{team2}{results}
                data.AddResultUpdatePoints(newResult.tempTeam1, 
                    newResult.tempTeam2, newResult.saveResult);
                //reset grid & combo boxes
                UpdateData();
            }
            Opacity = 1;
        }
        //delete button method
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //set temporary variable to hold combo box selection
            //(this will also be selected when grid selection is changed)
            Results temp = (Results)cbResultNo.SelectedItem;
            //if temporary value is not null perform delete functions
            if (temp != null)
            {
                //ask user if they want to delete thier selection with pop-up
                MessageBoxResult result = MessageBox.Show
                    ("Are you sure you want to delete?", 
                    $"Deleting {temp.Team1Name} vs {temp.Team2Name}...",
                    MessageBoxButton.YesNo);
                switch (result)
                {
                    //if yes perform delete functions
                    case MessageBoxResult.Yes:
                        teamList = data.GetAllTeams();
                        //set some temporary variables to use for team 1 and 2
                        TeamInfo tempTeam1 = new TeamInfo();
                        TeamInfo tempTeam2 = new TeamInfo();
                        //for loop to check team names against result ids
                        foreach (var team in teamList) 
                        {
                            //if its a draw find team 1 and minus 1 point
                            if (temp.Team1Name.Equals(team.TeamName) && 
                                temp.Result == 0) 
                            {
                                team.Points--;
                                tempTeam1 = team;
                            }
                            //if its a draw find team 2 and minus 1 point
                            if (temp.Team2Name.Equals(team.TeamName) && 
                                temp.Result == 0)
                            {
                                team.Points--;
                                tempTeam2 = team;
                            }
                            //if its team 1 victory find team 1 and
                            //minus 2 points
                            if (temp.Team1Name.Equals(team.TeamName) && 
                                temp.Result == 1) 
                            {
                                team.Points-=2;
                                tempTeam1 = team;
                            }
                            //if its team 2 victory find team 2 and
                            //minus 2 points
                            if (temp.Team2Name.Equals(team.TeamName) && 
                                temp.Result == 2)
                            {
                                team.Points -= 2;
                                tempTeam1 = team;
                            }
                        }
                        //perform sql transaction to delete result and
                        //update points of teams involved
                        data.DeleteResultUpdatePoints(tempTeam1, tempTeam2, 
                            temp.ResultId);
                        //update data grid and combo boxes
                        UpdateData();
                        //reset buttons to disabled until
                        //user makes another action
                        btnDelete.IsEnabled = false;
                        btnEdit.IsEnabled = false;
                        break;
                    //if no dont do anything close message box
                    case MessageBoxResult.No:
                        break;
                }
            }
        }
        //editing button method
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {         
            //set temporary value to hold selected result
            Results temp = (Results)cbResultNo.SelectedItem;
            //create pop-up passing in the selected result
            ResultPopup editResultPopup = new ResultPopup(temp.ResultId);
            //set background opacity to make pop-up stand out more
            Opacity = 0.4;
            //show pop-up
            editResultPopup.ShowDialog();
            //if pop-up retern successful perform update functions
            if (editResultPopup.Success)
            {                    
                //perform sql transaction to update result and team points
                //passing in {team1}{team2}{resultId}
                data.UpdateResultUpdatePoints(editResultPopup.tempTeam1, 
                    editResultPopup.tempTeam2, editResultPopup.saveResult);
                //update datagrid and combo boxes
                UpdateData();
                //reset buttons to disabled until
                //user makes another action
                btnDelete.IsEnabled = false;
                btnEdit.IsEnabled = false;
            }
            //set opacity back to normal after pop-up has closed
            Opacity = 1;
            
        }
        //combo box that shows id of results
        private void cbResultNo_SelectionChanged(object sender, 
            SelectionChangedEventArgs e)
        {
            //turn buttons on if user selected an item
            //other buttons use this selection to function
            btnEdit.IsEnabled = true; 
            btnDelete.IsEnabled = true;
        }
        //method to change 0,1,2 of result value into words
        //to be displayed in datagrid
        //adds these strings to results objects
        private void AddResultText()
        {
            foreach (var result in resultList)
            {
                if (result.Result == 1)
                {
                    //shows team name and win
                    result.WinLose = $"{result.Team1Name} Win";
                }
                else if (result.Result == 2)
                {
                    //shows team name and win
                    result.WinLose = $"{result.Team2Name} Win";
                }
                else
                {
                    //shows draw
                    result.WinLose = "Draw";
                }
            }
        }
        //datagrid selection method
        private void dgvResults_SelectionChanged(object sender, 
            SelectionChangedEventArgs e)
        {
            //temp variable to store grid selection
            Results temp = (Results)dgvResults.SelectedItem;
            //sets combo box to grid selection
            cbResultNo.SelectedItem = temp;
        }
        //combo box "Event" selection method
        private void cbEvent_SelectionChanged(object sender, 
            SelectionChangedEventArgs e)
        {
            //check if selection is null
            //this is because other method set this to null
            //this if statement breaks out of this method if this happens
            if (cbEvent.SelectedItem == null)return;
            //set temporary variable to store selected event
            Events temp = (Events)cbEvent.SelectedItem;
            //perform sql query to get results with passed in event name
            resultList = data.GetResultNamesByEvent(temp.EventName);
            //change grid and combo boxes to show selected changes
            ComboSelectionChanged();
            //set other combo boxes to blank
            cbGame.SelectedItem = null;
            cbTeam1.SelectedItem = null;
        }
        //combo box "Game" selection method
        private void cbGame_SelectionChanged(object sender, 
            SelectionChangedEventArgs e)
        {
            //check if selection is null
            //this is because other method set this to null
            //this if statement breaks out of this method if this happens
            if (cbGame.SelectedItem == null)return;
            //set temporary variable to store selected game
            Games temp = (Games)cbGame.SelectedItem;
            //perform sql query to get results with passed in game name
            resultList = data.GetResultNamesByGame(temp.GameName);
            //change grid and combo boxes to show selected changes
            ComboSelectionChanged();
            //set other combo boxes to blank
            cbEvent.SelectedItem = null;
            cbTeam1.SelectedItem = null;
        }
        //combo box "Team" selection method
        private void cbTeam1_SelectionChanged(object sender, 
            SelectionChangedEventArgs e)
        {
            //check if selection is null
            //this is because other method set this to null
            //this if statement breaks out of this method if this happens
            if (cbTeam1.SelectedItem == null) return;
            //set temporary variable to store selected team
            TeamInfo temp = (TeamInfo)cbTeam1.SelectedItem;
            //perform sql query to get results with passed in team name
            resultList = data.GetResultNamesByName(temp);
            //change grid and combo boxes to show selected changes
            ComboSelectionChanged();
            //set other combo boxes to blank
            cbGame.SelectedItem = null;
            cbEvent.SelectedItem = null;
        }
        //method to show datagrid with selected views
        private void ComboSelectionChanged()
        {
            //datagrid = resultList
            dgvResults.ItemsSource = resultList;
            //change 0,1,2 result to text
            AddResultText();
            //refresh grid
            dgvResults.Items.Refresh();
            //change combo box to datagrid results
            cbResultNo.ItemsSource = resultList;
            cbResultNo.DisplayMemberPath = "ResultId";
        }
    }
}
