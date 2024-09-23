using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services
{
    public class UserServices : IUserServices
    {
        private readonly DotnetDbContext _dbContext;

        public UserServices(DotnetDbContext context) 
        {
            _dbContext = context;
        }

        public async Task<List<ResUserDto>> GetAllUser()
        {
            return await _dbContext.Users
                .Where(user => user.Role != "admin")
                .Select(user => new ResUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Password = user.Password,
                    Balance = user.Balance,
                }).ToListAsync();
        }

        public async Task<ResUserDto> GetUserById(string id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null) return null;

            return new ResUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Balance = user.Balance
            };
        }

        public async Task<string> Register(ReqRegisterUserDto register)
        {
            var isAnyEmail = await _dbContext.Users.SingleOrDefaultAsync(e => e.Email == register.Email);
            if (isAnyEmail != null)
            {
                throw new Exception("Email already used");
            }

            var newUser = new User
            {
                Name = register.Name,
                Email = register.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                Role = register.Role,
                Balance = register.Balance,
            };

            await _dbContext.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();

            return newUser.Name;

        }
    }
}
