using DataManagement.Classes;
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
using System.Windows.Shapes;

namespace EsportsTrackerDatabase
{
    /// <summary>
    /// Interaction logic for GamePopup.xaml
    /// Pop-up window to edit or add new Game to database
    /// </summary>
    public partial class GamePopup : Window
    {
        //saveGame object to store details to pass back to main window
        public Games saveGame = new Games();
        public bool Success = false;
        //list for combo box to use
        List<string> gameTypes = new List<string>() { "Solo", "Team" };
        //initialisation for new game
        public GamePopup()
        {
            InitializeComponent();
            //sets owner to use for initial positioning
            Owner = Application.Current.MainWindow;
            //set combobox items to above list
            cbType.ItemsSource = gameTypes;
        }
        //initialisation for editing game passing in selected object
        public GamePopup(Games tempGame)
        {
            InitializeComponent();
            //sets owner to use for initial positioning
            Owner = Application.Current.MainWindow;
            //sets variables to passed in object variables
            saveGame.GameId = tempGame.GameId;
            txtGame.Text = tempGame.GameName;
            //set combobox items to above list
            cbType.ItemsSource = gameTypes;
            //sets combo box to passed in result
            if (tempGame.GameType.ToLower().Equals("solo"))
            {
                cbType.Text = "Solo";
            }
            else
            {
                cbType.Text = "Team";
            }
        }
        //Let user drag and move the pop-up
        private void Window_MouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            DragMove();
        }
        //save button method
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //check if filled in
            if (!IsFormFilled())
            {
                MessageBox.Show("You Must Fill all Fields!");
                return;
            }
            //if filled in set object variables to text box inputs
            saveGame.GameName = txtGame.Text;
            saveGame.GameType = cbType.Text;
            //pass success back to mainscreen and close pop-up
            Success = true;
            Close();
        }
        //cancel button closes pop-up
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Success = false;
            Close();
        }
        //check if form filled method 
        //if not filled return focus to unfilled box
        private bool IsFormFilled()
        {
            if (string.IsNullOrWhiteSpace(txtGame.Text))
            {
                txtGame.Focus(); return false;
            }
            if (string.IsNullOrWhiteSpace(cbType.Text))
            {
                cbType.Focus(); return false;
            }
            return true;
        }
    }
}
