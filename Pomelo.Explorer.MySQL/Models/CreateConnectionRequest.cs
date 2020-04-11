namespace Pomelo.Explorer.MySQL.Models
{
    public class CreateConnectionRequest
    {
        public string Address { get; set; }

        public uint Port { get; set; }

        public SslOption Ssl { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string InstanceId { get; set; }
    }
}
