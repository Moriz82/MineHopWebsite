using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MineHopWebsite.Pages.Forums
{
    public class BanAppeals : PageModel
    {
        public void OnGet()
        {
            
        }
        public void OnPostCreateForm(string sessionCount)
        {
            Response.Redirect("Create/CreateBanAppeal");
        }
        public void OnPostSelect(string sessionCount)
        {
            
            string[] s = sessionCount.Split("~#$^&");
            string title = s[0];
            string name = s[1];
            Forum forum = null;
            foreach (Forum f in Program.BanApealForums)
            {
                if (f.Title.Equals(title))
                {
                    forum = f;
                    break;
                }
            }
            Program.SelctedForums.Remove(name);
            Program.SelctedForums.Add(name, forum);
            Response.Redirect("BanAppeal");
        }
        public void OnPostRemoveReply(int sessionCount)
        {
            Program.Forums.Remove(Program.BanApealForums[sessionCount]);
            Program.UpdateForms(); 
            HttpContext.Response.Redirect("BanAppeals");
        }
    }
}