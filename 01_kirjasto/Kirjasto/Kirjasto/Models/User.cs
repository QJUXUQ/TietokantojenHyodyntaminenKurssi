using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace Kirjasto.Models
{
    public class User
    {
        //public User(SqlDataReader reader) 
        //{
        //MemberId = Convert.ToInt32(reader["MemberId"]);
        //FirstName = reader["FirstName"].ToString();
        //LastName = reader["LastName"].ToString();
        //Address = reader["Address"].ToString();
        //Phone = reader["PhoneNumber"].ToString();
        //Email = reader["Email"].ToString();
        //Registration = Convert.ToInt32(reader["RegistrationDate"]);
        //Age = Convert.ToInt32(reader["Age"]);
        //}

        public int MemberId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Registration { get; set; }
    }
}
