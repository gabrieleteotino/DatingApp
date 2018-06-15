using System;

namespace DatingApp.API.DTOs
{
    public class Username : IEquatable<Username>, IEquatable<string>
    {
        public string Value { get; }

        public Username(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new Exception("Username is invalid.");
            
            // Convert username to lowercase to avoid multiple user with similar names like "John" and "john"
            // Use invariant to avoid conflicts for users from different cultures
            this.Value = username.ToLowerInvariant();
        }

        public static implicit operator string(Username value) => value.Value;

        public static implicit operator Username(string value) => new Username(value);

        public override bool Equals(object obj)
        {
            var other = obj as Username;

            return other != null ? Equals(other) : Equals(obj as string);
        }

        public bool Equals(Username other)
        {
            return other != null && Value == other.Value;
        }

        public bool Equals(string other) => Value == other;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;

        public static bool operator ==(Username a, Username b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (((object)a == null) || ((object)b == null)) return false;

            return a.Value == b.Value;
        }

        public static bool operator !=(Username a, Username b) => !(a == b);
    }
}