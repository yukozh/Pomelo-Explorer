using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Pomelo.Explorer.MySQL
{
    public static class MySqlConnectionExtensions
    {
        public static void EnsureOpened(this MySqlConnection conn)
        {
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }
        }

        public static async Task EnsureOpenedAsync(this MySqlConnection conn, CancellationToken token = default)
        {
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                await conn.OpenAsync(token);
            }
        }
    }
}
