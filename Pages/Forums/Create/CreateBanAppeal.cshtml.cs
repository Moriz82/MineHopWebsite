using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MineHopWebsite.Pages.Forums.Create
{
    public class CreateBanAppeal : PageModel
    {
        public void OnGet()
        {
            
        }
        [Required(ErrorMessage ="Please Enter Title...")]  
        [Display(Name = "Title")]  
        public string Title { get; set; }  
  
        [Required(ErrorMessage ="Please Enter A Body...")]
        [Display(Name ="Body")]  
        public string Body { get; set; }

        public void OnPostCreateReply(int sessionCount)
        {
            Title = String.Format("{0}", Request.Form["TitleInput"]);
            Body = String.Format("{0}", Request.Form["BodyInput"]);
            
            if (Title.Length < 8)
            {
                ViewData["msg"] = "Title must me at least 8 characters";
                return;
            }

            if (Body.Length < 32)
            {
                ViewData["msg"] = "Body must me at least 32 characters";
                return;
            }
            
            string username = HttpContext.Request.Cookies["username"];
            Forum fourm = new Forum(Forum.ForumType.BanAppeal, Title, username, Body);
            Program.Forums.Add(fourm);
            Program.UpdateForms();
            Program.SelctedForums.Remove(username);
            Program.SelctedForums.Add(username, fourm);
            HttpContext.Response.Redirect("/Forums/BanAppeal");
        }
    }
}