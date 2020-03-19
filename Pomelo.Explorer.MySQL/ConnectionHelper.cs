using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Pomelo.Explorer.MySQL
{
    public static class ConnectionHelper
    {
        public static Dictionary<string, MySqlConnection> Connections = new Dictionary<string, MySqlConnection>();
    }
}
