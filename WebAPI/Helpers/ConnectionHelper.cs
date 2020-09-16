using System.Configuration;

namespace WebAPI.Helpers
{
    public class ConnectionHelper
    {
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["apiDatabase"].ConnectionString;
        }
    }
}