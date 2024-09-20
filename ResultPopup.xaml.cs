using DataManagement;
using DataManagement.Classes;
using System.Windows;
using System.Windows.Input;

namespace EsportsTrackerDatabase
{
    /// <summary>
    /// Interaction logic for ResultPopup.xaml
    /// Pop-up for adding result or editing results
    /// </summary>
    public partial class ResultPopup : Window
    {
        //set data adapter
        Adapter data = new Adapter();
        //set lists to be used in this window
        List<TeamInfo> teamList = new List<TeamInfo>();
        List<Events> eventList = new List<Events>();
        List<Games> gameList = new List<Games>();
        List<ResultsId> resultsId = new List<ResultsId>();
        //set temporary team variables to be used in this window
        public TeamInfo tempTeam1 = new TeamInfo();
        public TeamInfo tempTeam2 = new TeamInfo();
        //set temporary team variables for teaminfo in this window
        //to be passed in when editing
        public TeamInfo loadedTeam1 = new TeamInfo();
        public TeamInfo loadedTeam2 = new TeamInfo();
        //set temporary result variable to store the result of this window
        public ResultsId saveResult;
        //set temporary result variable to be passed in when editing
        public ResultsId loadedResult = new ResultsId();
        //set success variable to show pass or fail
        //when saving and passing back to main window
        public bool Success = false;
        //set edit mode variable
        public bool editMode = false;
        public int tempResult;
        public int tempId;

        //constructor for new result (note no overload)
        public ResultPopup()
        {
            InitializeComponent();
            //sets owner of pop-up to set position of window
            Owner = Application.Current.MainWindow;
            //set combo boxes
            SetBoxes();
            //set save button to false
            btnSave.IsEnabled = false;
        }

        //contsructor to edit results (passing in result Id from main window)
        public ResultPopup(int temp)
        {
            //set editing mode to true
            //this lets the cancel button revert point changes
            editMode = true;
            InitializeComponent();
            //sets owner of pop-up to set position of window
            Owner = Application.Current.MainWindow;
            //set combo boxes
            SetBoxes();
            //enables save button on startup in edit mode
            btnSave.IsEnabled = true;
            //set id for sql command to use
            tempId = temp;
            //loop through result list to find matching result 
            //matching passed in id
            foreach (var result in resultsId)
            {
                if (result.ResultId == temp)
                {
                    //store in temporary variable
                    loadedResult = (ResultsId)result;
                    tempResult = result.Result;
                }
            }
            //loop through team list to find matching teams
            //matching ids from loadedResult
            foreach (var team in teamList)
            {
                if (loadedResult.Team1Id.Equals(team.TeamId.ToString()))
                {                    
                    //store team1
                    loadedTeam1 = (TeamInfo)team;
                }
                if (loadedResult.Team2Id.Equals(team.TeamId.ToString()))
                {
                    //store team2
                    loadedTeam2 = (TeamInfo)team;
                }
            }
            //check result and adjust points
            if (loadedResult.Result == 0)
            {
                //set radio button
                rbtnDraw.IsChecked = true;
                //on draw minus 1 point from each team
                loadedTeam1.Points--;
                loadedTeam2.Points--;
            }
            else if (loadedResult.Result == 1)
            {
                //set radio button
                rbtnTeam1.IsChecked = true;
                //on team 1 win minus 2 points from team1
                loadedTeam1.Points -= 2;
            }
            else if (loadedResult.Result == 2)
            {
                //set radio button
                rbtnTeam2.IsChecked = true;
                //on team 2 win minus 2 points from team2
                loadedTeam2.Points -= 2;
            }
            //perform sql methods to update points from team 1 and 2
            data.UpdateTeamScore(loadedTeam1);
            data.UpdateTeamScore(loadedTeam2);
            //reset combo boxes
            SetBoxes();
            //loop through team list to find matching Teams
            //matching ids from loadedResult
            //this one is to set combo box selection
            foreach (var team in teamList)
            {
                if (loadedResult.Team1Id.Equals(team.TeamId.ToString()))
                {
                    //set combo box to team 1
                    cbTeam1.SelectedItem = (TeamInfo)team;
                }
                if (loadedResult.Team2Id.Equals(team.TeamId.ToString()))
                {
                    //set combo box to team 2
                    cbTeam2.SelectedItem = (TeamInfo)team;
                }
            }
            //loop through event list to find matching event
            //matching ids from loadedResult
            foreach (var tempEvent in eventList)
            {
                if (loadedResult.EventId.Equals(tempEvent.EventId.ToString()))
                {
                    //set combo box to matching event
                    cbEvent.SelectedItem = (Events)tempEvent;
                }
            }
            //loop through game list to find matching game
            //matching ids from loadedResult
            foreach (var game in gameList)
            {
                if (loadedResult.GameId.Equals(game.GameId.ToString()))
                {
                    //set combo box to matching game
                    cbGame.SelectedItem = (Games)game;
                }
            }            
        }
        //method to set combo boxes and buttons when entering pop-up
        private void SetBoxes()
        {
            //run query to set team list / results list
            teamList = data.GetAllTeams();
            resultsId = data.GetAllResultIds();
            //set team 1 combo box
            cbTeam1.ItemsSource = teamList;
            cbTeam1.DisplayMemberPath = "TeamName";
            cbTeam2.ItemsSource = teamList;
            //set team 2 combo box
            cbTeam2.DisplayMemberPath = "TeamName";
            //run query to set event list
            eventList = data.GetAllEvents();
            //set event combo box
            cbEvent.ItemsSource = eventList;
            cbEvent.DisplayMemberPath = "EventName";
            //run query to set game list
            gameList = data.GetAllGames();
            //set game combo box
            cbGame.ItemsSource = gameList;
            cbGame.DisplayMemberPath = "GameName";
        }
        //lets user move window around
        private void Window_MouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            DragMove();
        }
        //cancel click method
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //check if in edit mode if yes revert points back
            if (editMode)
            {
                //if result was draw add a point to each team
                if (loadedResult.Result == 0)
                {
                    loadedTeam2.Points++;
                    loadedTeam1.Points++;
                }
                //if result was team 1 win add 2 points to team 1
                else if (loadedResult.Result == 1)
                {
                    loadedTeam1.Points += 2;
                }
                //if result was team 2 win add 2 points to team 2
                else if (loadedResult.Result == 2)
                {
                    loadedTeam2.Points += 2;
                }
                //perform sql methods to update points from team 1 and 2
                data.UpdateTeamScore(loadedTeam1);
                data.UpdateTeamScore(loadedTeam2);
            }
            //close pop-up window
            Close();
        }
        //save button method
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //check if selected teams match
            //show warning message if they do
            if (cbTeam1.Text == cbTeam2.Text)
            {
                MessageBox.Show(
                    "You Cannot Have a Team Against Itself!\n" +
                    "\nPlease Choose 2 different Teams.");
                return;
            }
            //check if all boxes are filled out 
            //show warning message if they do
            if (!IsFormFilled())
            {
                MessageBox.Show(
                    "You Must Fill all Fields Correctly!\n" +
                    "\nPlease Check Your Entries Exist First.");
                return;
            }            
            //set temp teams to selected items in combo boxes
            tempTeam1 = (TeamInfo)cbTeam1.SelectedItem;
            tempTeam2 = (TeamInfo)cbTeam2.SelectedItem;
            //set temp event to selected item in combo box
            Events tempEvent = (Events)cbEvent.SelectedItem;
            //set temp game to selected item in combo box
            Games game = (Games)cbGame.SelectedItem;            
            if (rbtnTeam1.IsChecked == true)
            {
                //team 1 win
                tempResult = 1;
                tempTeam1.Points += 2;
            }
            else if (rbtnTeam2.IsChecked == true)
            {
                //team 2 win
                tempResult = 2;
                tempTeam2.Points += 2;
            }
            else if (rbtnDraw.IsChecked == true)
            {
                //draw
                tempResult = 0;
                tempTeam2.Points++;
                tempTeam1.Points++;
            }
            //set all values to saveResult with overloads
            saveResult = new ResultsId
                (Convert.ToString(tempTeam1.TeamId),
                Convert.ToString(tempTeam2.TeamId), 
                tempResult, 
                Convert.ToString(tempEvent.EventId), 
                Convert.ToString(game.GameId));
            //if editing set id of saveResult for update sql command to use
            if (editMode) saveResult.ResultId = tempId;
            //set success to true
            //will perform sql transaction on return to main window
            Success = true;
            //close pop-up
            Close();
        }
        //radio button methods - turns on save button when one is checked
        private void rbtnTeam1_Checked(object sender, RoutedEventArgs e)
        {
            btnSave.IsEnabled = true;
        }
        private void rbtnDraw_Checked(object sender, RoutedEventArgs e)
        {
            btnSave.IsEnabled = true;
        }
        private void rbtnTeam2_Checked(object sender, RoutedEventArgs e)
        {
            btnSave.IsEnabled = true;
        }
        //Method to check all fields are filled correctly
        //returns false if any are and sets focus to
        //the first box that was empty or not an existing team/event/game
        private bool IsFormFilled()
        {
            if (string.IsNullOrWhiteSpace(cbTeam1.Text) ||
                !TeamCheck(cbTeam1.Text))
            {
                cbTeam1.Focus(); return false;
            }
            if (string.IsNullOrWhiteSpace(cbTeam2.Text) ||
                !TeamCheck(cbTeam2.Text))
            {
                cbTeam2.Focus(); return false;
            }
            if (string.IsNullOrWhiteSpace(cbEvent.Text) ||
                !EventCheck(cbEvent.Text))
            {
                cbEvent.Focus(); return false;
            }
            if (string.IsNullOrWhiteSpace(cbGame.Text) ||
                !GameCheck(cbGame.Text))
            {
                cbGame.Focus(); return false;
            }
            //return true if all pass checks
            return true;
        }
        //method to check if team name is valid
        //loops through team list
        private bool TeamCheck(string teamName)
        {
            teamList = data.GetAllTeams();
            foreach (TeamInfo team in teamList)
            {
                if (team.TeamName == teamName) { return true; }
            }
            return false;
        }
        //method to check if event name is valid
        //loops through event list
        private bool EventCheck(string eventName)
        {
            eventList = data.GetAllEvents();
            foreach (Events temp in eventList)
            {
                if (temp.EventName == eventName) { return true; }
            }
            return false;
        }
        //method to check if game name is valid
        //loops through game list
        private bool GameCheck(string gameName)
        {
            gameList = data.GetAllGames();
            foreach (Games temp in gameList)
            {
                if (temp.GameName == gameName) { return true; }
            }
            return false;
        }
    }
}
