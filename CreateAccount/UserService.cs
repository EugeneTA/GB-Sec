using CardStorageService.Data;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CreateAccount
{

    internal class UserService
    {
        private const string SecretKey = "jkrDkfyqnnf+!RsfgrWdlfkd";
        private const string _connectionString = "data source=NB-TISHKOV\\SQLEXPRESS;initial catalog=CardStorageDatabase;User Id=CardStorageUser;Password=12345;MultipleActiveResultSets=True;TrustServerCertificate=True;";

        public string CreateAccount(string login, string password, Account account)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password)) return string.Empty;

            if (account != null)
            {
                (account.PasswordSalt, account.PasswordHash) = CreatePasswordHash(password);

                string queryString = $"INSERT INTO accounts (EMail, PasswordSalt, PasswordHash, Locked, FirstName, LastName, SecondName) VALUES (\'{account.EMail}\', \'{account.PasswordSalt}\', \'{account.PasswordHash}\', \'{account.Locked}\', \'{account.FirstName}\', \'{account.LastName}\', \'{account.SecondName}\');";

                using (SqlConnection connection = new SqlConnection(
                    _connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            return "";
        }

        public static (string passwordSalt, string passwordHash) CreatePasswordHash(string password)
        {
            // generate random salt 
            byte[] buffer = new byte[16];
            RandomNumberGenerator secureRandom = new RNGCryptoServiceProvider();
            secureRandom.GetBytes(buffer);

            // create hash 
            string passwordSalt = Convert.ToBase64String(buffer);
            string passwordHash = GetPasswordHash(password, passwordSalt);

            // done
            return (passwordSalt, passwordHash);
        }

        public static string GetPasswordHash(string password, string passwordSalt)
        {
            // build password string
            password = $"{password}~{passwordSalt}~{SecretKey}";
            byte[] buffer = Encoding.UTF8.GetBytes(password);

            // compute hash 
            SHA512 sha512 = new SHA512Managed();
            byte[] passwordHash = sha512.ComputeHash(buffer);

            // done
            return Convert.ToBase64String(passwordHash);
        }
    }
}
