using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (1)
            {
                Console.WriteLine("Hello World");
            }
            Customer gustav = new Customer(199907058631, "Gustav", "Hagland", "hejsan");
            gustav.Accounts.Add(new SalaryAccount("AllKonto"));
            List<Customer> customers = new List<Customer>();
            customers.Add(gustav);
            using (XmlWriter writer = XmlWriter.Create("xml.xml", new XmlWriterSettings()
            {
                Indent = true
            }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("customers");
                foreach(Customer customer in customers)
                {
                    writer.WriteStartElement("customer");
                    writer.WriteElementString("id", customer.PersonalID.ToString());
                    writer.WriteElementString("firstName", customer.FirstName);
                    writer.WriteElementString("lastName", customer.LastName);
                    writer.WriteElementString("passWord", customer.PassWord);
                    writer.WriteStartElement("accounts");
                    foreach (IAccount account in customer.Accounts)
                    {
                        writer.WriteStartElement("account");
                        writer.WriteElementString("type", account.GetType().ToString());
                        writer.WriteElementString("name", account.Name);
                        writer.WriteElementString("balance", account.Balance.ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }

    public class Customer
    {
        public Customer(long personalID, string firstName, string lastName, string passWord)
        {
            Accounts = new List<IAccount>();
            PersonalID = personalID;
            FirstName = firstName;
            LastName = lastName;
            PassWord = passWord;
        }

        public long PersonalID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PassWord { get; set; }
        public List<IAccount> Accounts { get; set; }
    }

    public interface IAccount
    {
        double Balance { get; }
        string Name { get; set; }
        void TransferTo(IAccount target, double amount);
        void WithDraw(double amount);
        void Deposit(double amount);
    }

    public class SalaryAccount : IAccount
    {
        public SalaryAccount(string name)
        {
            Name = name;
        }

        public double Balance { get; private set; }

        public string Name { get; set; }

        public void Deposit(double amount)
        {
            if (amount < 0)
                return;
            Balance += amount;
        }

        public void TransferTo(IAccount target, double amount)
        {
            WithDraw(amount);
            target.Deposit(amount);
        }

        public void WithDraw(double amount)
        {
            if (amount <= Balance)
            {
                Balance -= amount;
            }
        }
    }

    public class SavingsAccount : IAccount
    {
        private DateTime lastWithDrawal;

        public SavingsAccount(string name)
        {
            Name = name;
            lastWithDrawal = new DateTime();
        }

        public double Balance { get; private set; }

        public string Name { get; set; }

        public void Deposit(double amount)
        {
            if (amount < 0)
                return;
            Balance += amount;
        }

        public void TransferTo(IAccount target, double amount)
        {
            if (DateTime.Now - lastWithDrawal > new TimeSpan(7, 0, 0, 0))
            {
                target.Deposit(amount);
                WithDraw(amount);
                lastWithDrawal = DateTime.Now;
            }
        }

        public void WithDraw(double amount)
        {
            if (DateTime.Now - lastWithDrawal > new TimeSpan(7, 0, 0, 0))
            {
                if (Balance - amount > 0)
                {
                    Balance -= amount;
                }
            }
        }
    }
}
