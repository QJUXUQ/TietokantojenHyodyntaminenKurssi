using Kirjasto.Models;
using System.Data.SqlClient;
using System.Security.Cryptography;


namespace Kirjasto
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            
            var dataRepo = new DataBaseRepository(@"Data Source="+@"(localdb)\MSSQLLocalDB"+";Initial Catalog=KirjastoData");
            //string result = dataRepo.IsDbConnectionEstablished();
            //Console.WriteLine(result);
            //var booklist = dataRepo.GetAllBooks();
            //for(int i = 0;i<booklist.Count; i++) 
            //{
            //    Console.WriteLine(booklist[i].Title);
            //}
            //SqlCommand cmd = new SqlCommand("SELECT TOP(5) * FROM Book ORDER BY PublicationYear desc"); //kirjat, jotka on julkaistu viiden vuoden sisällä
            //SELECT AVG(Age) FROM Member
            //SELECT Title FROM Book WHERE AvailableCopies = (SELECT MAX(AvailableCopies) FROM Book) // eniten tarjolla kirjastossa

            List<Book> books = dataRepo.GetAllBooks();
            foreach(Book book in books) 
            {
                Console.WriteLine(book.Title);
            }
            List<Book> books2 = dataRepo.GetMaxBooks();
            foreach (Book book in books2)
            {
                Console.WriteLine(book.Title+" Eniten tarjolla");
            }

            Console.WriteLine(dataRepo.GetUserAvg());

            var bookloan = dataRepo.BookLoan();
            for (int i=0; i<bookloan.Length;i++) 
            {
                Console.WriteLine(bookloan[i]);
            }
            




        }
    }
}
