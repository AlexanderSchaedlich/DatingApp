using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    // api/users
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>?>?> GetUsers() 
        {
            if (_context.Users != null) 
            {
                return await _context.Users.ToListAsync();
            }
            else
            {
                return null;
            }
        }
        
        // api/users/3
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser?>?> GetUser(int id) 
        {
            if (_context.Users != null) 
            {
                return await _context.Users.FindAsync(id);
            }
            else
            {
                return null;
            }
            
        }
    }
}