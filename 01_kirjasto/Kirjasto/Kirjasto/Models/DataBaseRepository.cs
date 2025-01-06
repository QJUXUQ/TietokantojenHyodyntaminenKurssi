using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace Kirjasto.Models
{

    public class DataBaseRepository
    {
        private string _connectionString;
        public DataBaseRepository(string connectionString) 
        {
            _connectionString = connectionString;
        }
        public string IsDbConnectionEstablished() 
        {
            
            using var connection = new SqlConnection(_connectionString);

            try
            {
                connection.Open();
                return "Connection established!";
            }
            catch (SqlException ex) 
            {
                throw;
            }
            catch(Exception ex) 
            {
                throw;
            }
            
        }

        public List<Book> GetAllBooks()
        {
            List<Book> books = new List<Book>();
            using var dbconnection = new SqlConnection(_connectionString);
            dbconnection.Open();
            //using var command = new SqlCommand("SELECT * FROM Book", dbconnection);
            using var cmd = new SqlCommand("SELECT * FROM Book WHERE PublicationYear >YEAR(CURRENT_TIMESTAMP)-5", dbconnection); //haetaan 5 vuoden päästä kirjat
            using var reader = cmd.ExecuteReader();



            while (reader.Read())
            {
                Book book = new Book()
                {
                    Id = Convert.ToInt32(reader["BookId"]),
                    Title = reader["Title"].ToString(),
                    ISBN = reader["ISBN"].ToString(),
                    Published = Convert.ToInt32(reader["PublicationYear"]),
                    Copies = Convert.ToInt32(reader["AvailableCopies"])
                };
                books.Add(book);
            }
            return books;
        }
        public List<Book> GetMaxBooks()
        {
            List<Book> books = new List<Book>();
            using var dbconnection = new SqlConnection(_connectionString);
            dbconnection.Open();
            //using var command = new SqlCommand("SELECT * FROM Book", dbconnection);
            SqlCommand cmd2 = new SqlCommand("SELECT * FROM Book WHERE AvailableCopies = (SELECT MAX(AvailableCopies) FROM Book)", dbconnection);
            using var reader = cmd2.ExecuteReader(); 
            
            while (reader.Read())
            {
                Book book = new Book()
                {
                    Id = Convert.ToInt32(reader["BookId"]),
                    Title = reader["Title"].ToString(),
                    ISBN = reader["ISBN"].ToString(),
                    Published = Convert.ToInt32(reader["PublicationYear"]),
                    Copies = Convert.ToInt32(reader["AvailableCopies"])
                };
                books.Add(book);
            }
            return books;
        }

        public List<Loan> GetAllLoans()
            {
                List<Loan> loans = new List<Loan>();
                using var dbconnection = new SqlConnection(_connectionString);
                dbconnection.Open();
                using var command = new SqlCommand("SELECT * FROM Loan", dbconnection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                Loan loan = new Loan()
                {
                    LoanId = Convert.ToInt32(reader["LoanId"]),
                    BookId = Convert.ToInt32(reader["BookId"]),
                    MemberId = Convert.ToInt32(reader["MemberId"]),
                    LoanDate = Convert.ToDateTime(reader["LoanDate"]),
                    DueDate= Convert.ToDateTime(reader["DueDate"]),
                    ReturnDate= Convert.ToDateTime(reader["ReturnDate"])

                };
                    loans.Add(loan);
                }
                return loans;
            }

            public int GetUserAvg()
            {
            int avgage=0;
                using var dbconnection = new SqlConnection(_connectionString);
                dbconnection.Open();
                using var command = new SqlCommand("SELECT AVG(Age) AS AVGAGE FROM Member", dbconnection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    avgage= Convert.ToInt32(reader["AVGAGE"]);
                }
            return avgage;
        }

        public string[] BookLoan()
        {
            List<User> loans = new List<User>();
            List<Book> books = new List<Book>();
            using var dbconnection = new SqlConnection(_connectionString);
            dbconnection.Open();
            using var command = new SqlCommand("SELECT FirstName, LastName, ISBN FROM Member INNER JOIN Loan ON Member.MemberId = Loan.MemberId INNER JOIN Book ON Loan.BookId = Book.BookId", dbconnection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Book book = new Book()
                {
                    ISBN = reader["ISBN"].ToString()
                }; books.Add(book);
                User user = new User()
                {
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString()
                }; loans.Add(user);
                
                
            }
            string[] booknloan = new string[loans.Count];
            for (int i = 0; i < loans.Count; i++) 
            {
                booknloan[i] = loans[i].FirstName+" " + loans[i].LastName+" " + books[i].ISBN;
            }return booknloan;
        }
            
        






        //User user = new User()
        //{
        //    MemberId = Convert.ToInt32(reader["MemberId"]),
        //    FirstName = reader["FirstName"].ToString(),
        //    LastName = reader["LastName"].ToString(),
        //    Address = reader["Address"].ToString(),
        //    Phone = reader["PhoneNumber"].ToString(),
        //    Email = reader["Email"].ToString(),
        //    Registration = Convert.ToInt32(reader["RegistrationDate"])

        //};

    }
}
