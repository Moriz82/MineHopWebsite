using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace MineHopWebsite.Pages
{
    public class Login : PageModel
    {
        [Required(ErrorMessage ="Please Enter Username..")]  
        [Display(Name = "Username")]  
        public string Username { get; set; }  
  
        [Required(ErrorMessage ="Please Enter Password...")]  
        [DataType(DataType.Password)]  
        [Display(Name ="Password")]  
        public string Password { get; set; }
        
        [Required(ErrorMessage ="Please Enter TwoFaCode...")]
        [Display(Name ="TwoFaCode")]  
        public string TwoFaCode { get; set; }

        public void OnGet()
        {
        }

        public void OnPostLoginButton(int sessionCount)
        {
            Dictionary<string, string> Users = new();
            Dictionary<string, string> Emails = new();
            Username = String.Format("{0}", Request.Form["userInput"]);
            Password = String.Format("{0}", Request.Form["passInput"]);
            TwoFaCode = null;
            if (Request.Form["TwoFaCodeInput"].Count > 0)
            {
                TwoFaCode = String.Format("{0}", Request.Form["TwoFaCodeInput"]);
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
                    string username = dr.GetString(dr.GetOrdinal("Username"));
                    string password = dr.GetString(dr.GetOrdinal("Password"));
                    string email = dr.GetString(dr.GetOrdinal("Email"));
                    Users.Add(username, password);
                    Emails.Add(Program.XOR(username), Program.XOR(email));
                }
                dr.Close();
            }
            finally
            {
                sqlConnection.Close();
            }
            
            foreach (KeyValuePair<string, string> user in Users)
            {
                if (Program.XOR(user.Key).Equals(Username) && Program.XOR(user.Value).Equals(Password))
                {
                    if (!Program.TwoFaCodes.ContainsKey(Username))
                    {
                        //Program.TwoFaCodes.Add(Username, "");
                        SendTwoFaEmail(Username, Emails[Username]);
                        ViewData["msg"] = "Check your email for your TwoFaCode!";
                        return;
                    }
                    if (TwoFaCode != null && Program.TwoFaCodes.ContainsKey(Username))
                    {
                        if (Program.TwoFaCodes[Username].Equals(TwoFaCode))
                        {
                            Program.TwoFaCodes.Remove(Username);
                            HttpContext.Response.Cookies.Append("username", Program.XOR(Username));
                            ViewData["msg"] = "Successfully Logged in as "+Username+"!";
                            Response.Headers.Add("REFRESH", "1;URL=Index");
                            return;   
                        }
                    }
                }
            }
            ViewData["msg"] = "Invalid username or password!";
        }

        public static string GenerateTwoFaCode()
        {
            int length = 10;
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }

        public static void SendTwoFaEmail(string username, string email)
        {
            string twoFaCode = GenerateTwoFaCode();
            Program.TwoFaCodes.Add(username, twoFaCode);
            
            string from = "minehop2fa@gmail.com";
            MailMessage message = new MailMessage(from, email);  
  
            string mailbody = "Two Factor Code: "+twoFaCode;  
            message.Subject = "Two Factor Code For MineHop!";  
            message.Body = mailbody;  
            message.BodyEncoding = Encoding.UTF8;  
            message.IsBodyHtml = true;  
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            System.Net.NetworkCredential basicCredential1 = new  
                System.Net.NetworkCredential("minehop2fa@gmail.com", "6w7Qy9FO8Gnb4sjv");  
            client.EnableSsl = true;  
            client.UseDefaultCredentials = false;  
            client.Credentials = basicCredential1;  
            try   
            {  
                client.Send(message);  
            }
            catch (Exception ex)   
            {
                throw;
            }  
        }
    }
}