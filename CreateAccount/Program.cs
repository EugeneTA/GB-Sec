using CardStorageService.Data;

namespace CreateAccount
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Account account = new Account();
            Console.WriteLine("Enter user email:");
            Console.Write("> ");
            account.EMail = Console.ReadLine();

            Console.WriteLine("Enter password:");
            Console.Write("> ");
            string password = Console.ReadLine();

            Console.WriteLine("Enter name:");
            Console.Write("> ");
            account.FirstName = Console.ReadLine();

            Console.WriteLine("Enter surname:");
            Console.Write("> ");
            account.SecondName = Console.ReadLine();

            Console.WriteLine("Enter patronymic:");
            Console.Write("> ");
            account.LastName = Console.ReadLine();

            account.Locked = false;

            UserService userService = new UserService();
            userService.CreateAccount(account.EMail, password, account); 

            Console.ReadKey();
        }
    }
}