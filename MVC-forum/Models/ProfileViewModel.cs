using MVC_forum.Models.Entities;

namespace MVC_forum.Models
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public ChangePassViewModel ChangePassViewModel { get; set; }
        public int NumberOfArticles => User.Articles.Count;
    }
}
