using DataManagement.Classes;
using System.Windows;
using System.Windows.Input;

namespace EsportsTrackerDatabase
{
    /// <summary>
    /// Interaction logic for EventPopup.xaml
    /// Shows a new window to edit or create a new Event 
    /// </summary>
    public partial class EventPopup : Window
    {    
        //set variable to hold event details to be saved and passed back to main program
        public Events saveEvent = new Events();
        //set variable to pass back a successful entry
        public bool Success = false;
        //standard initailise for new entry
        public EventPopup()
        {
            InitializeComponent();
            //set owner to position window in the middle of application
            this.Owner = Application.Current.MainWindow;
        }
        //initialisation for editing, passing in selected Event
        public EventPopup(Events editEvent)
        {
            InitializeComponent();
            //set owner to position window in the middle of application
            this.Owner = Application.Current.MainWindow;
            saveEvent.EventId = editEvent.EventId;
            txtEvent.Text = editEvent.EventName;
            txtLocation.Text = editEvent.Location;
            pDate.Text = editEvent.Date.ToString();
        }
        //sets movement of window to let user drag around
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        //save data function

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //check form is filled out if not displays error and exits function
            if (!IsFormFilled()) 
            {
                MessageBox.Show("You Must Fill all Fields!");
                return;
            }
            //sets object details to entered details
            saveEvent.EventName = txtEvent.Text;
            saveEvent.Location = txtLocation.Text;
            saveEvent.Date = (DateTime)pDate.SelectedDate;
            //sets success to true and passes object back to parent to be saved to database
            Success = true;
            Close();
        }
        //cancel btn - closes window with success = false
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Success = false;
            Close();
        }
        //check form is filled function - checks each box, if not filled resets focus to that box
        private bool IsFormFilled()
        {
            if (string.IsNullOrWhiteSpace(txtEvent.Text))
            {
                txtEvent.Focus(); return false;
            }
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                txtLocation.Focus(); return false;
            }
            if (string.IsNullOrWhiteSpace(pDate.Text))
            {
                pDate.Focus(); return false;
            }            
            return true;
        }
    }
}
