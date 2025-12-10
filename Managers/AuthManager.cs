using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces.IManagers;
using Microsoft.AspNetCore.Identity;
using DataAccess.Entities;
using Models.DTOs;
using Interfaces.IRepository;
namespace Managers
{
    public class AuthManager:IAuthManager
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IGenericRepository<Customer> _repo;
        public AuthManager(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IGenericRepository<Customer> repo)
        {
            
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _repo = repo;
        }
        public async Task<Result> Register(Register dto)
        {
            var user= await _userManager.FindByEmailAsync(dto.Email);
            if(user!=null)
            {
                return new Result
                {
                    Success=false,
                    Message="User with this email already exists"
                };
            }
            var newuser=new AppUser
            {
                Email=dto.Email,
                UserName=dto.UserName,
                FullName=dto.FullName,
                Age=dto.Age,
                
            };

            var createUser= await _userManager.CreateAsync(newuser,dto.Password);
            if(!createUser.Succeeded)
            {
                
                return new Result
                {
                    Success=false,
                    Message= "Failed to create user"
                };
            }
            if(!await _roleManager.RoleExistsAsync(dto.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(dto.Role));
            }
            var addToRole= await _userManager.AddToRoleAsync(newuser,dto.Role);
            if (!addToRole.Succeeded)
            {
                return new Result
                {
                    Success=false,
                    Message="Failed to assign role to user"
                };
            }
            var customer = new Customer
            {
                FullName = dto.FullName,
                UserId = newuser.Id
            };
            await _repo.AddAsync(customer);
            return new Result
            {
                Success = true,
                Message = "User registered successfully"
            };

        }
        public async Task<Result<AuthResponse>> Login(Login dto)
        {
            var user= await _userManager.FindByEmailAsync(dto.Email);
            if(user==null)
            {
                return new Result<AuthResponse>
                {
                    Success=false,
                    Message="Invalid email or password"
                };
            }
            var signInResult= await _userManager.CheckPasswordAsync(user,dto.Password);
            if(!signInResult)
            {
                return new Result<AuthResponse>
                {
                    Success=false,
                    Message="Invalid email or password"
                };
            }
            var roles = await _userManager.GetRolesAsync(user);
            return new Result<AuthResponse>
            {
                Success=true,
                Message="Login successful",
                Data=new AuthResponse
                {
                    UserName=user.UserName,
                    Email =user.Email,
                    Roles=roles.ToList(),
                    Age=user.Age
                }
            };
        }
    }
}
