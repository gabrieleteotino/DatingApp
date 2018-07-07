using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.DTOs
{
    public class PhotoForCreation
    {
        public string Url { get; set; }
        public IFormFile File { get; set; }
        public string  Description { get; set; }
        public DateTime Created { get; }
        public string PublicId { get; set; }

        public PhotoForCreation()
        {
            this.Created = DateTime.Now;
        }
    }
}