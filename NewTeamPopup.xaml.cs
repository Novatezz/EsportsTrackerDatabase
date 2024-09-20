using DataManagement.Classes;
using System.Windows;
using System.Windows.Input;

namespace EsportsTrackerDatabase
{
    /// <summary>
    /// Interaction logic for NewTeamPopup.xaml
    /// Team Info pop-up for entering new teams or editing teams
    /// </summary>
    public partial class NewTeamPopup : Window
    {
        //set variable for window to use
        public TeamInfo saveTeam  = new TeamInfo();
        public bool Success = false;
        //constructor for entering a new team (note no overloads)
        public NewTeamPopup()
        {
            InitializeComponent();
            //sets owner for pop-up positioning
            Owner = Application.Current.MainWindow;
        }
        //constructor for editing a teams info (passing in teamInfo object)
        public NewTeamPopup(TeamInfo updateTeam)
        {
            InitializeComponent();
            //sets owner for pop-up positioning
            Owner = Application.Current.MainWindow;
            //set text boxes to passed in teamInfo
            txtTeamName.Text = updateTeam.TeamName;
            txtContactName.Text = updateTeam.ContactName;
            txtContactPhone.Text = updateTeam.ContactPhone;
            txtContactEmail.Text = updateTeam.ContactEmail;
        }
        //cancel button method
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //close pop-up
            Close();
        }
        //lets user drag pop-up around
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        //save button method
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //check if form is filled properly if not display warning
            if (!IsFormFilled())
            {
                MessageBox.Show("You Must Fill all Fields!");
                return;
            }
            //set saveTeam values to entered values
            saveTeam = new TeamInfo();
            saveTeam.TeamName = txtTeamName.Text;
            saveTeam.ContactName = txtContactName.Text;
            saveTeam.ContactPhone = txtContactPhone.Text;
            saveTeam.ContactEmail = txtContactEmail.Text;
            saveTeam.Points = 0;
            //success = true means sql will run when returning to main screen
            Success = true;
            //close pop-up
            Close();            
        }
        //form validation method
        //checks each text box to make sure it is filled
        //if not returns false and sets focus on the first empty field
        private bool IsFormFilled()
        {
            if (string.IsNullOrWhiteSpace(txtTeamName.Text)) 
            {
                txtTeamName.Focus(); return false;
            }
            if (string.IsNullOrWhiteSpace(txtContactName.Text)) 
            {
                txtContactName.Focus(); return false;
            }
            if (string.IsNullOrWhiteSpace(txtContactPhone.Text)) 
            {
                txtContactPhone.Focus(); return false;
            }
            if (string.IsNullOrWhiteSpace(txtContactEmail.Text)) 
            {
                txtContactEmail.Focus(); return false;
            }
            return true;
        }
    }
}
