using System;

namespace DatingApp.API.DTOs
{
    public class MessageForList
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public string Content { get; set; }
        public DateTime SentDate { get; set; }
    }
}