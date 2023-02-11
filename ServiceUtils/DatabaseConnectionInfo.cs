using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceUtils
{
    public class DatabaseConnectionInfo
    {
        public string Host { get; set; }
        public string DatabaseName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            return $"data source={Host};initial catalog={DatabaseName};User Id={Login};Password={Password};MultipleActiveResultSets=True;TrustServerCertificate=True;App=EntityFramework";
        }
    }
}
