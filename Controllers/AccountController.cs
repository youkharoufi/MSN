using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MSN.Models;
using MSN.Token;

namespace MSN.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;   
            _mapper = mapper;
            
        }

        [HttpPost]
        public async Task<ActionResult<ApplicationUser>> Register(RegisterUser registerUser)
        {
            var user = _mapper.Map<ApplicationUser>(registerUser);


            var newUser = new ApplicationUser
            {
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                Token = ""


            };

            var result = await _userManager.CreateAsync(newUser, registerUser.Password);

            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, newUser.Role);
                return Ok(newUser);
            }
            else
            {
                return BadRequest("Error during user Registration");
            }

        }

        [HttpPost]
        public async Task<ActionResult<ApplicationUser>> Login(LoginUser loginUser)
        {
            var userFromDb = await _userManager.Users.FirstOrDefaultAsync(i=> i.Email == loginUser.Email);

            if (userFromDb == null) return NotFound();


                var result = await _signInManager.CheckPasswordSignInAsync(userFromDb, loginUser.Password, false);

                if (!result.Succeeded) return BadRequest("Invalid Password");

                userFromDb.Token = await _tokenService.GenerateToken(userFromDb);

                return Ok(userFromDb);


        }


    }
}
