using DataManagement;
using DataManagement.Classes;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace EsportsTrackerDatabase
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// Main window for displaying Game information
    /// </summary>
    public partial class GameWindow : UserControl
    {
        //sets classes to be used in window
        Adapter data = new Adapter();
        List<Games> gameList = new List<Games>();
        private List<ResultsId> resultsList;
        private List<TeamInfo> teamList;

        public GameWindow()
        {
            InitializeComponent();
            UpdateData();
        }
        //Update data method - resets lists, datagrid, combo box and buttons
        private void UpdateData()
        {
            gameList = data.GetAllGames();
            //datagrid set list and refresh
            dgvGame.ItemsSource = gameList;
            dgvGame.Items.Refresh();
            //combobox set list and refence to show
            cbGameName.ItemsSource = gameList;
            cbGameName.DisplayMemberPath = "GameName";
            //disable buttons
            btnDel.IsEnabled = false;
            btnEdit.IsEnabled = false;
        }
        //edit method
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //create gamePopup passing in selected object to edit
            GamePopup gamePopup = new GamePopup((Games)cbGameName.SelectedItem);
            //set opacity of screen to make popup stand out
            Opacity = 0.4;
            //show popup
            gamePopup.ShowDialog();
            //if success update database
            if (gamePopup.Success)
            {
                data.UpdateGame(gamePopup.saveGame);
                UpdateData();
            }
            //set opacity back to normal
            Opacity = 1;
        }
        //new method
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            //create gamepopup with no info
            GamePopup gamePopup = new GamePopup();
            //set opacity of screen to make popup stand out
            Opacity = 0.4;
            //show popup
            gamePopup.ShowDialog();
            //if success insert new game into database
            if (gamePopup.Success)
            {
                data.AddNewGame(gamePopup.saveGame);
                UpdateData();
            }
            //set opacity back to normal
            Opacity = 1;
        }
        //delete button method
        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            //sets temp variable to hold combo box selection
            Games gameTemp = (Games)cbGameName.SelectedItem;
            if (gameTemp != null)
            {
                //show warning message for user to confirm delete
                MessageBoxResult result = MessageBox.Show
                    ("Are you sure you want to delete?\n" +
                    "\n!!!WARNING THIS WILL DELETE ALL RESULTS CONTANING THIS GAME!!!",
                    $"Deleting {gameTemp.GameName}...", MessageBoxButton.YesNo);
                switch (result)
                {
                    //if yes run delete sql command
                    case MessageBoxResult.Yes:
                        data.DeleteGame(gameTemp.GameId);
                        UpdatePointsFromResults();
                        UpdateData();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }
        //method to update points after deleting games and results together
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
        //combo box selection method
        private void cbGameName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //sets temp variable to hold combo box selection
            Games games = (Games)cbGameName.SelectedItem;
            if(games != null)
            {
                //set text of game type to matching type
                cbGameType.Text = games.GameType;
                //enable edit and delete buttons
                btnDel.IsEnabled = true;
                btnEdit.IsEnabled = true;
            }
        }
        private void dgvGame_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //sets temp variable to hold grid selection
            Games games = (Games)dgvGame.SelectedItem;
            if (games != null)
            {
                //set combo boxes matching selection
                cbGameName.Text = games.GameName;
                cbGameType.Text = games.GameType;
                //enable edit and delete buttons
                btnDel.IsEnabled = true;
                btnEdit.IsEnabled = true;
            }
        }
    }
}
