using System;

namespace DatingApp.API.Helpers
{
    public static class DataTimeExtensions
    {
        public static int CalculateAge(this DateTime birthDate)
        {
            var age = DateTime.Today.Year - birthDate.Year;
            // Correct if the birthday for this year is in the future
            if(birthDate.AddYears(age) > DateTime.Today) age--;

            return age;
        }
    }
}