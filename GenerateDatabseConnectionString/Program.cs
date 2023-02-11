using ServiceUtils;
using System.Text;

namespace GenerateDatabaseConnectionString
{
    internal class Program
    {
        static void Main(string[] args)
        {
            byte[] key = { 1, 6, 9, 2, 3, 67, 92 };

            CacheProvider cacheProvider = new CacheProvider(key, null);

            DatabaseConnectionInfo connectionInfo = new DatabaseConnectionInfo();
            Console.WriteLine("Enter Host Name:");
            Console.Write(">");
            connectionInfo.Host = Console.ReadLine();
            Console.WriteLine("Enter Database Name:");
            Console.Write(">");
            connectionInfo.DatabaseName = Console.ReadLine();
            Console.WriteLine("Enter login:");
            Console.Write(">");
            connectionInfo.Login = Console.ReadLine();
            Console.WriteLine("Enter password:");
            Console.Write(">");
            connectionInfo.Password = Console.ReadLine();
            Console.WriteLine();
            File.WriteAllBytes("dbConnection.info", cacheProvider.CacheDatabaseConnection(connectionInfo));
            Console.WriteLine("Data written to file dbConnection.info");
        }
    }
}