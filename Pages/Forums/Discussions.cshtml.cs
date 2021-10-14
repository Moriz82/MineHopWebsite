using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MineHopWebsite.Pages.Forums
{
    public class Discussions : PageModel
    {
        public void OnGet()
        {
            
        }
        public void OnPostCreateForm(string sessionCount)
        {
            Response.Redirect("Create/CreateDiscussion");
        }
        public void OnPostSelect(string sessionCount)
        {
            
            string[] s = sessionCount.Split("~#$^&");
            string title = s[0];
            string name = s[1];
            Forum forum = null;
            foreach (Forum f in Program.DiscussionForums)
            {
                if (f.Title.Equals(title))
                {
                    forum = f;
                    break;
                }
            }
            Program.SelctedForums.Remove(name);
            Program.SelctedForums.Add(name, forum);
            Response.Redirect("Discussion");
        }
        public void OnPostRemoveReply(int sessionCount)
        {
            Program.Forums.Remove(Program.DiscussionForums[sessionCount]);
            Program.UpdateForms(); 
            HttpContext.Response.Redirect("Discussions");
        }
    }
}