 
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;  
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;  

namespace MineHopWebsite.Pages
{
    public class User : PageModel
    {
        [Required(ErrorMessage ="Please Enter a Username..")]  
        [Display(Name = "Username")]  
        public string Username { get; set; }  
  
        [Required(ErrorMessage ="Please Enter a Password...")]  
        [DataType(DataType.Password)]  
        [Display(Name ="Password")]  
        public string Password { get; set; }
        
        [Required(ErrorMessage ="Please Enter a Email..")]  
        [Display(Name = "Email")]  
        public string Email{ get; set; } 
        
        [Required(ErrorMessage ="Please Enter a Confirmation Code..")]  
        [Display(Name = "Confirmation Code")]  
        public string ConfirmationCode{ get; set; } 

        public void OnGet()
        {
        }

        public void OnPostRegisterButton(int sessionCount)
        {
            Dictionary<string, string> Users = new Dictionary<string, string>();
            Username = String.Format("{0}", Request.Form["userInput"]);
            Password = String.Format("{0}", Request.Form["passInput"]); 
            Email = String.Format("{0}", Request.Form["emailInput"]);
            
            if (Username.Length < 3)
            {
                ViewData["msg"] = "Username must me at least 3 characters!";
                return;
            }

            if (Password.Length < 8)
            {
                ViewData["msg"] = "Password must me at least 8 characters!";
                return;
            }

            if (!Email.Contains('@') || !Email.Contains('.'))
            {
                ViewData["msg"] = "Please enter a valid email address!";
                return;
            }
            
            SqlConnection sqlConnection = new SqlConnection("Data Source=SQL5108.site4now.net;Initial Catalog=db_a79d4e_minehopadmin;User Id=db_a79d4e_minehopadmin_admin;Password=ggk9p6tpeir69");
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = "select * from Logins";
            SqlDataReader dr;
            try
            {
                sqlConnection.Open();
                dr = sqlCommand.ExecuteReader();
                while (dr.Read())
                {
                    Users.Add(
                        dr.GetString(dr.GetOrdinal("Username")),
                        dr.GetString(dr.GetOrdinal("Password"))
                    );
                    
                }
                dr.Close();
            }
            finally
            {
                sqlConnection.Close();
            }

            bool isFound = false;

            foreach (KeyValuePair<string, string> user in Users)
            {
                if (user.Key.Equals(Username))
                {
                    isFound = true;
                    ViewData["msg"] = "Username already in use";
                }
            }
            if (!isFound)
            {
                string key = Login.GenerateTwoFaCode();
                
                Login.SendTwoFaEmail(key, Email);
                HttpContext.Response.Cookies.Append("stuff", key);
                Program.LoginMakers.Add(key, new LoginMaker(Username, Password, Email));
                Response.Redirect("Register");
            }
        }

        public void OnPostRegister(int sessionCount)
        {
            string stuff = HttpContext.Request.Cookies["stuff"];
            ConfirmationCode = String.Format("{0}", Request.Form["ConfirmationCode"]); 

            if (!Program.TwoFaCodes[stuff].Equals(ConfirmationCode))
            {
                ViewData["msg"] = "Confirmation Code is incorrect!";
                return;
            }

            LoginMaker maker = Program.LoginMakers[stuff];

            Username = maker.Username;
            Password = maker.Password;
            Email = maker.Email;
            
            Username = Program.XOR(Username);
            Password = Program.XOR(Password);
            Email = Program.XOR(Email);
                
            SqlConnection sqlConn = new SqlConnection("Data Source=SQL5108.site4now.net;Initial Catalog=db_a79d4e_minehopadmin;User Id=db_a79d4e_minehopadmin_admin;Password=ggk9p6tpeir69");
            sqlConn.Open();
            SqlCommand sqlCom = new SqlCommand();
            sqlCom.Connection = sqlConn;
            sqlCom.Parameters.AddWithValue("@username", Username);
            sqlCom.Parameters.AddWithValue("@password", Password);
            sqlCom.Parameters.AddWithValue("@email", Email);
            sqlCom.CommandText = "INSERT INTO Logins (Username, Password, Email) VALUES (@username, @password, @email)";
            sqlCom.ExecuteNonQuery();
                
            Username = Program.XOR(Username);
            Password = Program.XOR(Password);
            Email = Program.XOR(Email);
            
            HttpContext.Response.Cookies.Append("stuff", "", new CookieOptions() {
                Expires = DateTime.Now.AddDays(-1)
            });

            ViewData["msg"] = "Successfully Registered!";
            Response.Headers.Add("REFRESH", "1;URL=Login");
        }
    }

    public class LoginMaker
    {
        public string Username, Password, Email;

        public LoginMaker(string username, string password, string email)
        {
            Username = username;
            Password = password;
            Email = email;
        }
    }

    public class ApplicationUser : DbContext
    {
        public ApplicationUser(DbContextOptions<ApplicationUser> options) : base(options)
        {
        }

        public DbSet<User> Logins { get; set; }
    }
}