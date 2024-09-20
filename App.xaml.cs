using DataManagement;
using System.Windows;

namespace EsportsTrackerDatabase
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //Create oour class for database building
            DbBuilder builder = new DbBuilder();
            //Tell the builder to build the database. This will do nothing if it already exists.
            builder.CreateDatabase();
            //Check if the database has any tables yet.
            if (builder.DoTablesExist() == false)
            {
                //If not, trigger the building of the database tables
                builder.BuildDatabaseTables();
            }
        }
    }

}
