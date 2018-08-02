namespace DatingApp.API.DTOs
{
    public class UserParams
    {
        private const int MaxPageSize = 50;
        public static readonly int MinAgeDefault = 18;
        public static readonly int MaxAgeDefault = 99;
        public int PageNumber { get; set; } = 1;

        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }
        public int UserId { get; set; }
        public string Gender { get; set; }
        public int MinAge { get; set; } = MinAgeDefault;
        public int MaxAge { get; set; } = MaxAgeDefault;

        public string OrderBy { get; set; }
        public bool Likees { get; set; } = false;
        public bool Likers { get; set; } = false;
    }
}