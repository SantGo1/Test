using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sat.Recruitment.Api.Data;
using Sat.Recruitment.Api.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sat.Recruitment.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public partial class UsersController : ControllerBase
    {
        private readonly List<User> _users = new List<User>();
        private IUserRepository _userRepository;

        public UsersController()
        {
            _userRepository = new UserRepository();
        }

        [HttpPost]
        [Route("/create-user")]
        public async Task<Result> CreateUser(string name, string email, string address, string phone, string userType, string money)
        {
            var newUser = new User
            {
                Name = name,
                Email = email,
                Address = address,
                Phone = phone,
                UserType = userType,
                Money = decimal.Parse(money)
            };

            var errors = "";

            ValidateErrors(newUser, ref errors);

            if (string.IsNullOrEmpty(errors))
                return new Result()
                {
                    IsSuccess = false,
                    Errors = errors
                };

            CalculateUserMoney(newUser);

            GetUserList();

            Result result = null;
            try
            {
                if (!IsDuplicated(newUser))
                {
                    Debug.WriteLine("User Created");

                    result = new Result()
                    {
                        IsSuccess = true,
                        Errors = "User Created"
                    };
                }
                else
                {
                    Debug.WriteLine("The user is duplicated");

                    result = new Result()
                    {
                        IsSuccess = false,
                        Errors = "The user is duplicated"
                    };
                }
            }
            catch
            {
                Debug.WriteLine("There was an error while creating the user");
                result = new Result()
                {
                    IsSuccess = false,
                    Errors = "There was an error while creating the user"
                };
            }
            return result;
        }

        private bool IsDuplicated(User newUser)
        {
            foreach (var user in _users)
            {
                if ((user.Email == newUser.Email || user.Phone == newUser.Phone) ||
                    (user.Name == newUser.Name && user.Address == newUser.Address))
                {
                    return true;
                }
            }
            return false;
        }

        private User CalculateUserMoney(User newUser)
        {
            switch (newUser.UserType)
            {
                case "Normal":
                    if (newUser.Money > 100)
                    {
                        var percentage = Convert.ToDecimal(0.12);
                        //If new user is normal and has more than USD100
                        var gift = newUser.Money * percentage;
                        newUser.Money = newUser.Money + gift;
                    }
                    else if (newUser.Money < 100 && newUser.Money > 10)
                    {
                        var percentage = Convert.ToDecimal(0.8);
                        var gift = newUser.Money * percentage;
                        newUser.Money = newUser.Money + gift;
                    }
                    break;
                case "SuperUser":
                    if (newUser.Money > 100)
                    {
                        var percentage = Convert.ToDecimal(0.20);
                        var gift = newUser.Money * percentage;
                        newUser.Money = newUser.Money + gift;
                    }
                    break;
                case "Premium":
                    if (newUser.Money > 100)
                    {
                        var gift = newUser.Money * 2;
                        newUser.Money = newUser.Money + gift;
                    }
                    break;
                default:
                    break;
            }
            return newUser;
        }

        //Validate errors
        private void ValidateErrors(User newUser, ref string errors)
        {
            if (string.IsNullOrEmpty(newUser.Name))
                //Validate if Name is null
                errors = "The name is required";
            if (string.IsNullOrEmpty(newUser.Email))
                //Validate if Email is null
                errors = errors + " The email is required";
            if (string.IsNullOrEmpty(newUser.Address))
                //Validate if Address is null
                errors = errors + " The address is required";
            if (string.IsNullOrEmpty(newUser.Phone))
                //Validate if Phone is null
                errors = errors + " The phone is required";
        }

        private void GetUserList()
        {
            _users.Clear();
            _users.AddRange(_userRepository.GetAllUsers());
        }
    }
}
