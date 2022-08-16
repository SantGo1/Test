using Sat.Recruitment.Api.Models;
using System.Collections.Generic;
using System.IO;

namespace Sat.Recruitment.Api.Data
{
    public class UserRepository : IUserRepository
    {
        public void DeleteUserByName(string name)
        {
            throw new System.NotImplementedException();
        }

        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            var reader = ReadUsersFromFile();

            while (reader.Peek() >= 0)
            {
                var line = reader.ReadLineAsync().Result;
                var user = new User
                {
                    Name = line.Split(',')[0].ToString(),
                    Email = line.Split(',')[1].ToString(),
                    Phone = line.Split(',')[2].ToString(),
                    Address = line.Split(',')[3].ToString(),
                    UserType = line.Split(',')[4].ToString(),
                    Money = decimal.Parse(line.Split(',')[5].ToString()),
                };
                users.Add(user);
            }
            reader.Close();
            return users;
        }

        public User GetUserByName(string name)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateUserByName(string name)
        {
            throw new System.NotImplementedException();
        }
        private StreamReader ReadUsersFromFile()
        {
            var path = Directory.GetCurrentDirectory() + "/Files/Users.txt";

            FileStream fileStream = new FileStream(path, FileMode.Open);

            StreamReader reader = new StreamReader(fileStream);
            return reader;
        }
    }
}
