using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sat.Recruitment.Api.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sat.Recruitment.Api.Controllers
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string Errors { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public partial class UsersController : ControllerBase
    {

        private readonly List<User> _users = new List<User>();
        public UsersController()
        {
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
                    else if (newUser.Money < 100 && newUser.Money >10)
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

            //if (newUser.UserType == "Normal")
            //{
            //    if (decimal.Parse(money) > 100)
            //    {
            //        var percentage = Convert.ToDecimal(0.12);
            //        //If new user is normal and has more than USD100
            //        var gif = decimal.Parse(money) * percentage;
            //        newUser.Money = newUser.Money + gif;
            //    }
            //    if (decimal.Parse(money) < 100)
            //    {
            //        if (decimal.Parse(money) > 10)
            //        {
            //            var percentage = Convert.ToDecimal(0.8);
            //            var gif = decimal.Parse(money) * percentage;
            //            newUser.Money = newUser.Money + gif;
            //        }
            //    }
            //}
            //if (newUser.UserType == "SuperUser")
            //{
            //    if (decimal.Parse(money) > 100)
            //    {
            //        var percentage = Convert.ToDecimal(0.20);
            //        var gif = decimal.Parse(money) * percentage;
            //        newUser.Money = newUser.Money + gif;
            //    }
            //}
            //if (newUser.UserType == "Premium")
            //{
            //    if (decimal.Parse(money) > 100)
            //    {
            //        var gif = decimal.Parse(money) * 2;
            //        newUser.Money = newUser.Money + gif;
            //    }
            //}

            ////Normalize email
            //var aux = newUser.Email.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

            //var atIndex = aux[0].IndexOf("+", StringComparison.Ordinal);

            //aux[0] = atIndex < 0 ? aux[0].Replace(".", "") : aux[0].Replace(".", "").Remove(atIndex);

            //newUser.Email = string.Join("@", new string[] { aux[0], aux[1] });

            //mover esto a una clase de repository con dependecy injection si es posible
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
                _users.Add(user);
            }
            reader.Close();

            Result result = null;
            try
            {
                var isDuplicated = false;
                foreach (var user in _users)
                {
                    if ((user.Email == newUser.Email|| user.Phone == newUser.Phone) || 
                        (user.Name == newUser.Name && user.Address == newUser.Address))
                    {
                        isDuplicated = true;
                        throw new Exception("User is duplicated");
                    //}
                    //else if (user.Name == newUser.Name)
                    //{
                    //    if (user.Address == newUser.Address)
                    //    {
                    //        isDuplicated = true;
                    //        throw new Exception("User is duplicated");
                    //    }

                    //}
                }

                if (!isDuplicated)
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
    }
}
