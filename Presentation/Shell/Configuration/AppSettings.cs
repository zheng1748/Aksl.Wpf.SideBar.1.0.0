using System;
using System.IO;


namespace Aksl.Modules.Shell.Configuration
{
    public class AppSettings
    {
        #region Constructors
        public AppSettings()
        {
        }
        #endregion

        #region Properties
        public string MenuFilePath { get; set; }

        public string DatabasePath { get; set; }

        public string DatabaseName { get; set; }

        //public  string DatabaseFileName => Path.Combine(DatabasePath, DatabaseName);

        //public  string SQLiteConnectionString => $"Data Source={DatabaseFileName}";
        #endregion
    }
}
