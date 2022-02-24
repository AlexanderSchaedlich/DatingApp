using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]

        public async Task<ActionResult<UserDto>?> Register(RegisterDto registerDto) 
        {
            if (registerDto.Username == null) return null;
            if (registerDto.Password == null) return null;
            if (await UserExists(registerDto.Username) == null) return null; 
            if (await UserExists(registerDto.Username) == true) return BadRequest("Username is taken"); 
            
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users?.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]

        public async Task<ActionResult<UserDto>?> Login(LoginDto loginDto)
        {
            if (_context.Users == null) return null;

            var user = await _context.Users.SingleOrDefaultAsync(user => user.UserName == loginDto.Username);
            
            if (user == null) return Unauthorized("Invalid username");

            if (user.PasswordHash == null) return Problem(); // should be changed
            if (user.PasswordSalt == null) return Problem(); // should be changed
            if (loginDto.Password == null) return Problem(); // should be changed
            
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++) 
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool?> UserExists(string username) 
        {
            if (_context.Users == null)
            {
                BadRequest("No users available");
                return null;
            }

            return await _context.Users.AnyAsync(user => user.UserName == username.ToLower());
            
        }
    }
}