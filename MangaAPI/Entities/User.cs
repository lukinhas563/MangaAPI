using MangaAPI.Utils;
using System;

namespace MangaAPI.Entities
{
    public enum UserType { Basic, Premium, Admin };

    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password{ get; set; }
        public UserType UserType { get; set; }
        public bool isDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User(string name, string lastName, string username, string email, string password)
        {
            Id = Guid.NewGuid();
            Name = name;
            LastName = lastName;
            Username = username;
            Email = email;
            Password = password;

            UserType = UserType.Basic;
            isDeleted = false;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string lastName, string username, string email, string password)
        {
            Name = name;
            LastName = lastName;
            Username = username;
            Email = email;
            Password = password;

            UpdatedAt = DateTime.UtcNow;
        }

        public void Upgrade(UserType userType)
        {
            UserType = userType;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            isDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}