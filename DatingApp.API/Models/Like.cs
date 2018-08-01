namespace DatingApp.API.Models
{
    public class Like
    {
        public int FromId { get; set; }
        public User From { get; set; }
        public int ToId { get; set; }
        public User To { get; set; }
    }
}