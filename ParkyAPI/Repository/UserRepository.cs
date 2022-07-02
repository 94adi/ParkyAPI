using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ParkyAPI.Data;
using ParkyAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ParkyAPI.Repository.IRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly AppSettings _appSettings;

        public UserRepository(ApplicationDbContext db, IOptions<AppSettings> appSettings)
        {
            _db = db;
            _appSettings = appSettings.Value;
        }

        public User Authenticate(string username, string password)
        {
            var user = _db.Users.SingleOrDefault(x => x.Username == username && x.Password == password);

            if(user == null)
            {
                return null;
            }

            //more info: https://jwt.io/introduction/
            //used to create/assign the token
            var tokenHandler = new JwtSecurityTokenHandler();
            //secret key for signing
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            //stores token info
            var tokennDescriptor = new SecurityTokenDescriptor
            {
                //statement about the user
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                //available for 1 day
                Expires = DateTime.UtcNow.AddDays(1),
                //algo + private key used to sign JWT
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            //create token
            var token = tokenHandler.CreateToken(tokennDescriptor);
            //assign to user obj so it can be used in further requests to authenticate + authorize
            user.Token = tokenHandler.WriteToken(token);
            //don't send user password in the response obj
            user.Password = "";
            //pass authenticated user with its jwt
            return user;
        }

        public bool IsUniqueUser(string username)
        {
            var user = _db.Users.SingleOrDefault(x => x.Username == username );

            if (user == null)
                return true;

            return false;
        }

        public User Register(string username, string password)
        {
            User user = new User();

            user.Username = username;
            user.Password = password;
            user.Role = "Admin"; 

            _db.Users.Add(user);
            _db.SaveChanges();
            user.Password = "";
            return user;
        }
    }
}
