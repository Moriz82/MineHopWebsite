using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MineHopWebsite.Pages.Forums
{
    public class Discussion : PageModel
    {
        public void OnGet()
        {
            
        }
        public void OnPostCreateThread(int sessionCount)
        {
            HttpContext.Response.Redirect("CreateThread");
        }

        public void OnPostRemoveReply(int sessionCount)
        {
            Forum forum = Program.SelctedForums[HttpContext.Request.Cookies["username"]];
            forum.Replies.RemoveAt(sessionCount);
            Program.UpdateForms(); 
            HttpContext.Response.Redirect("Discussion");
        }
    }
}