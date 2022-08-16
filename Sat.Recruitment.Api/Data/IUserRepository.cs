using Sat.Recruitment.Api.Models;
using System.Collections.Generic;

namespace Sat.Recruitment.Api.Data
{
    public interface IUserRepository
    {
        User GetUserByName(string name);
        List<User> GetAllUsers();
        void DeleteUserByName(string name);
        void UpdateUserByName(string name);
    }
}
