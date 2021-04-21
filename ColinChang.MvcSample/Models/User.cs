using System.Collections.Generic;

namespace ColinChang.MvcSample.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public IEnumerable<Role> Roles { get; set; }

        public User()
        {
        }

        public User(string username, string password, IEnumerable<Role> roles)
        {
            Username = username;
            Password = password;
            Roles = roles;
        }
    }

    public class Role
    {
        public string Name { get; set; }

        public Role()
        {
        }

        public Role(string name) => Name = name;
    }
}