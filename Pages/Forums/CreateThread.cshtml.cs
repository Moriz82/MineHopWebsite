using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MineHopWebsite.Pages.Forums
{
    public class CreateThread : PageModel
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

            if (Body.Length < 8)
            {
                ViewData["msg"] = "Body must me at least 8 characters";
                return;
            }
            
            string username = Program.XOR(HttpContext.Request.Cookies["username"]);
            Forum forum = Program.SelctedForums[username];
            Forum reply = new Forum(Forum.ForumType.Reply, Title, username, Body);
            forum.Replies.Add(reply);
            Program.UpdateForms();
            HttpContext.Response.Redirect("Announcement");
        }
    }
}