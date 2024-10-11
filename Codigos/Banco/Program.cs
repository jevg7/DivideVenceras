using System;
using System.IO;

namespace bank
{
    public struct Account
    {
        public string Names;
        public string LastNames;
        public string Cedula;
        public string Email;
        public int Phone;
        public string UserName;
        public string Password;
        public int AccountNumber;
        public decimal Balance;

        public Account(string names, string lastnames, string cedula, string email, int phone,
        string username, string password, int accountnumber, decimal balance)
        {
            Names = names;
            LastNames = lastnames;
            Cedula = cedula;
            Email = email;
            Phone = phone;
            UserName = username;
            Password = password;
            AccountNumber = accountnumber;
            Balance = balance;
        }
    }

    public class Bank
    {
        private const string BankData = "BankData.txt";
        private static Account loggedInAccount;
        private static Account[] accounts;

        public static void Main()
        {
            int option;
            do
            {
                Console.WriteLine("Bienvenido al banco");
                Console.WriteLine("1. Crear cuenta");
                Console.WriteLine("2. Iniciar sesión");
                Console.WriteLine("3. Salir");
                Console.Write("Elige una opción: ");
                option = int.Parse(Console.ReadLine());
                Console.Clear();

                switch (option)
                {
                    case 1:
                        CreateAccount();
                        break;
                    case 2:
                        LoadAccounts();
                        Login();
                        break;
                    case 3:
                        Console.WriteLine("Gracias por utilizar el banco. Adiós!");
                        break;
                    default:
                        Console.WriteLine("Opción inválida, por favor intenta de nuevo.");
                        break;
                }
            } while (option != 3);  
        }

        public static void CreateAccount()
        {
            Console.Write("Ingresa tus nombres: ");
            string names = Console.ReadLine();
            Console.Write("Ingresa tus apellidos: ");
            string lastnames = Console.ReadLine();
            Console.Write("Ingresa tu cédula: ");
            string cedula = Console.ReadLine();
            Console.Write("Ingresa tu correo electrónico: ");
            string email = Console.ReadLine();
            Console.Write("Ingresa tu teléfono: ");
            int phone = int.Parse(Console.ReadLine());
            Console.Write("Ingresa tu nombre de usuario: ");
            string username = Console.ReadLine();
            Console.Write("Ingresa tu contraseña: ");
            string password = Console.ReadLine();
            Random random = new Random();
            int accountNumber = random.Next(100000, 999999);

            using (BinaryWriter writer = new BinaryWriter(File.Open(BankData, FileMode.Append)))
            {
                writer.Write(names);
                writer.Write(lastnames);
                writer.Write(cedula);
                writer.Write(email);
                writer.Write(phone);
                writer.Write(username);
                writer.Write(password);
                writer.Write(accountNumber);
                writer.Write(0m);  
            }

            Console.WriteLine("Cuenta creada exitosamente");
            Console.WriteLine($"Tu número de cuenta es: {accountNumber}");
            Console.WriteLine("Presiona cualquier tecla para volver al menú...");
            Console.ReadKey();
            Console.Clear();
        }

        public static void LoadAccounts()
        {
            if (File.Exists(BankData))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(BankData, FileMode.Open)))
                {
                    var accountList = new System.Collections.Generic.List<Account>();
                    try
                    {
                        while (true)
                        {
                            Account account = new Account
                            {
                                Names = reader.ReadString(),
                                LastNames = reader.ReadString(),
                                Cedula = reader.ReadString(),
                                Email = reader.ReadString(),
                                Phone = reader.ReadInt32(),
                                UserName = reader.ReadString(),
                                Password = reader.ReadString(),
                                AccountNumber = reader.ReadInt32(),
                                Balance = reader.ReadDecimal()
                            };
                            accountList.Add(account);
                        }
                    }
                    catch (EndOfStreamException)
                    {
                        accounts = accountList.ToArray();
                    }
                }
            }
        }

        public static void Login()
        {
            Console.Write("Ingresa tu nombre de usuario: ");
            string tempUsername = Console.ReadLine();
            Console.Write("Ingresa tu contraseña: ");
            string tempPassword = Console.ReadLine();
            Console.Clear();

            bool foundAccount = false;
            foreach (var account in accounts)
            {
                if (account.UserName == tempUsername && account.Password == tempPassword)
                {
                    loggedInAccount = account;
                    foundAccount = true;
                    
                    break;
                }
            }

            if (!foundAccount)
            {
                Console.WriteLine("Nombre de usuario o contraseña incorrectos.");
                return;
            }

            int tempOpt;
            do
            {
                Console.WriteLine($"Bienvenido, {loggedInAccount.Names} {loggedInAccount.LastNames}");
                Console.WriteLine($"Numero de cuenta: {loggedInAccount.AccountNumber}");
                Console.WriteLine("1. Depósito");
                Console.WriteLine("2. Retiro");
                Console.WriteLine("3. Consultar saldo");
                Console.WriteLine("4. Cerrar sesión");
                Console.Write("Elige una opción: ");
                tempOpt = int.Parse(Console.ReadLine());
                Console.Clear();

                switch (tempOpt)
                {
                    case 1:
                        Deposit();
                        break;
                    case 2:
                        Withdraw();
                        break;
                    case 3:
                        Balance();
                        break;
                    case 4:
                        Console.WriteLine("Sesión cerrada.");
                        break;
                    default:
                        Console.WriteLine("Opción inválida, por favor intenta de nuevo.");
                        break;
                }
            } while (tempOpt != 4);
        }

        public static void Deposit()
        {
            Console.WriteLine("1.Depósito propio");
            Console.WriteLine("2.Deposito a tercero");
            Console.Write("Elige una opción: ");
            int option = int.Parse(Console.ReadLine());

            if (option == 1)
            {
                Console.WriteLine("Ingresa la cantidad a depositar: ");
                decimal amount = decimal.Parse(Console.ReadLine());
                loggedInAccount.Balance += amount;
                UpdateAccount(loggedInAccount);
                Console.WriteLine("Depósito exitoso. Presiona cualquier tecla para volver al menú...");
                Console.ReadKey();
                Console.Clear();
            }

            else if (option == 2)
            {
                Console.WriteLine("Ingresa el número de cuenta a la que deseas depositar: ");
                int accountNumber = int.Parse(Console.ReadLine());
                Console.WriteLine("Ingresa la cantidad a depositar: ");
                decimal amount = decimal.Parse(Console.ReadLine());
                bool foundAccount = false;
                foreach (var account in accounts)
                {
                    if (account.AccountNumber == accountNumber)
                    {
                        var updatedAccount = account;
                        updatedAccount.Balance += amount;
                        UpdateAccount(updatedAccount);
                        foundAccount = true;
                        Console.WriteLine("Depósito exitoso. Presiona cualquier tecla para volver al menú...");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Opción inválida, por favor intenta de nuevo.");
            }
        }

        public static void Withdraw()
        {
            Console.WriteLine("Ingresa la cantidad a retirar: ");
            decimal amount = decimal.Parse(Console.ReadLine());
            if (loggedInAccount.Balance >= amount)
            {
                loggedInAccount.Balance -= amount;
                UpdateAccount(loggedInAccount);
                Console.WriteLine("Retiro exitoso. Presiona cualquier tecla para volver al menú...");
            }
            else
            {
                Console.WriteLine("Saldo insuficiente. Presiona cualquier tecla para volver al menú...");
            }
            Console.ReadKey();
            Console.Clear();
        }

        public static void Balance()
        {
            Console.WriteLine($"Tu saldo actual es: {loggedInAccount.Balance}");
            Console.WriteLine("Presiona cualquier tecla para volver al menú...");
            Console.ReadKey();
            Console.Clear();
        }

        public static void UpdateAccount(Account accountToUpdate)
        {
            var updatedAccounts = new System.Collections.Generic.List<Account>();
            foreach (var account in accounts)
            {
                if (account.AccountNumber == accountToUpdate.AccountNumber)
                {
                    updatedAccounts.Add(accountToUpdate);
                }
                else
                {
                    updatedAccounts.Add(account);
                }
            }

            accounts = updatedAccounts.ToArray();

            using (BinaryWriter writer = new BinaryWriter(File.Open(BankData, FileMode.Create)))
            {
                foreach (var account in accounts)
                {
                    writer.Write(account.Names);
                    writer.Write(account.LastNames);
                    writer.Write(account.Cedula);
                    writer.Write(account.Email);
                    writer.Write(account.Phone);
                    writer.Write(account.UserName);
                    writer.Write(account.Password);
                    writer.Write(account.AccountNumber);
                    writer.Write(account.Balance);
                }
            }
        }
    }
}
