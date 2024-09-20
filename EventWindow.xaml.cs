using DataManagement;
using DataManagement.Classes;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace EsportsTrackerDatabase
{
    /// <summary>
    /// Interaction logic for EventWindow.xaml
    /// Main window for displaying Event Details
    /// </summary>
    public partial class EventWindow : UserControl
    {
        //other classes to be used - Adapter and Events
        Adapter data = new Adapter();
        List<Events> eventList = new List<Events>();
        private List<ResultsId> resultsList;
        private List<TeamInfo> teamList;

        public EventWindow()
        {
            InitializeComponent();
            UpdateData();
        }
        //Method to reset data, grid, buttons and combo boxes
        private void UpdateData()
        {
            eventList = data.GetAllEvents();
            dgvEvents.ItemsSource = eventList;
            dgvEvents.Items.Refresh();
            cbEvents.ItemsSource = eventList;
            cbEvents.DisplayMemberPath = "EventName";
            btnDel.IsEnabled = false;
            btnEdit.IsEnabled = false;
        }
        //Data grid selection change
        private void dgvEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //get temp object from selection
            Events temp = (Events)dgvEvents.SelectedItem;
            if (temp != null)
            {
                //set all text to temp objects fields
                cbEvents.Text = temp.EventName;
                txtLocation.Text = temp.Location;
                txtEventDate.Text = Convert.ToString(temp.Date);
                btnDel.IsEnabled = true;
                btnEdit.IsEnabled = true;
            }
        }
        //Combo box for Event selection changed
        private void cbEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //get temp object from selection
            Events temp = (Events)cbEvents.SelectedItem;
            if (temp != null)
            {
                //set all text to temp objects fields
                cbEvents.Text = temp.EventName;
                txtLocation.Text = temp.Location;
                txtEventDate.Text = Convert.ToString(temp.Date);
                btnDel.IsEnabled = true;
                btnEdit.IsEnabled = true;
            }
        }
        //Edit button method
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //create pop-up and pass in selected item from the combo box selector
            EventPopup eventPopup = new EventPopup((Events)cbEvents.SelectedItem);
            //lower window opacity to make popup stand out
            Opacity = 0.4;
            //show pop-up disable main controls
            eventPopup.ShowDialog();
            //if edit was successful update database
            if (eventPopup.Success)
            {
                data.UpdateEvent(eventPopup.saveEvent);
                UpdateData();
            }
            //turn off buttons and set opacity to normal once exiting the pop-up
            btnDel.IsEnabled = false;
            btnEdit.IsEnabled = false;
            Opacity = 1;
        }
        //new button method
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            //create pop-up with no info
            EventPopup eventPopup = new EventPopup();
            Opacity = 0.4;
            //show pop-up disable main controls
            eventPopup.ShowDialog();
            //if edit was successful update database
            if (eventPopup.Success)
            {
                data.AddNewEvent(eventPopup.saveEvent);
                UpdateData();
            }
            //turn off buttons and set opacity to normal once exiting the pop-up
            btnDel.IsEnabled = false;
            btnEdit.IsEnabled = false;
            Opacity = 1;
        }
        //delete button method
        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            //set temp object to combo box selected item
            Events tempEvent = (Events)cbEvents.SelectedItem;
            //if null dont do anything
            if (tempEvent != null)
            {
                //show warning message box asking if user wants to delete
                MessageBoxResult result = MessageBox.Show
                    ("Are you sure you want to delete?\n" +
                    "\n!!!WARNING THIS WILL DELETE ALL RESULTS CONTANING THIS EVENT!!!",
                    $"Deleting {tempEvent.EventName}...", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        //if yes run sql query passing in deleted item id
                        data.DeleteEvent(tempEvent.EventId);
                        UpdatePointsFromResults();
                        //reset grid and buttons
                        UpdateData();
                        btnDel.IsEnabled = false;
                        btnEdit.IsEnabled = false;
                        break;
                    case MessageBoxResult.No:
                        //if no dont do anything
                        break;
                }
            }
        }
        //method to update team point after deleting event and results together
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
