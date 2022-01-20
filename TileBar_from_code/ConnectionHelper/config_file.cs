using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileBar_from_code.ConnectionHelper
{
    class config_file
    {
        #region properties
        public string MSSQLServerName { get; set; }
        public string MSSQLServerUserName { get; set; }
        public string MSSQLServerPassword { get; set; }
        public string MSSQLServerDbName { get; set; }
        #endregion properties
        public config_file()
        {

        }
    }
}
