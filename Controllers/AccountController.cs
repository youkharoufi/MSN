using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MSN.Claims;
using MSN.Data;
using MSN.Models;
using MSN.Services;
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
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly DataContext _context;
        private readonly IEmailSender _emailSender;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService, IMapper mapper, IWebHostEnvironment hostEnvironment, DataContext context,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;   
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
            _context = context;
            _emailSender = emailSender;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApplicationUser>> Register([FromForm]RegisterUser registerUser)
        {

            if(registerUser.File == null) {

                return NotFound("No file uploaded");
            }


                string wwwRootPath = _hostEnvironment.WebRootPath;

                MemoryStream memoryStream = new MemoryStream();
                registerUser.File.OpenReadStream().CopyTo(memoryStream);
                string photoUrl = Convert.ToBase64String(memoryStream.ToArray());

                string filename = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\users");
                var extension = Path.GetExtension(registerUser.File.FileName);


                Uri domain = new Uri(Request.GetDisplayUrl());



                using (var fileStreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
                {
                    registerUser.File.CopyTo(fileStreams);
                }

                photoUrl = domain.Scheme + "://" + domain.Host + (domain.IsDefaultPort ? "" : ":" + domain.Port) + "/images/users/" + filename + extension;


            


            var newUser = new ApplicationUser
            {
                UserName = registerUser.UserName,
                Email = registerUser.Email,
                Role = registerUser.Role,
                Token = "",
                PhotoUrl = photoUrl,


            };

            var result = await _userManager.CreateAsync(newUser, registerUser.Password);

            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, newUser.Role);
             
                var tof = new Photo
                {
                    Url = photoUrl,
                    UserId = newUser.Id,
                };



                _context.Photos.Add(tof);
                await _context.SaveChangesAsync();

                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                var message = new Message(new string[] { newUser.Email }, "Confirmation email link", registerUser.Link + emailConfirmationToken);

                await _emailSender.SendEmailAsync(message);

                var result2 = await _signInManager.CheckPasswordSignInAsync(newUser, registerUser.Password, false);

                if (!result2.Succeeded) return BadRequest("Invalid Password");

                newUser.Token = await _tokenService.GenerateToken(newUser);

                return Ok(newUser);
            }
            else
            {
              
              return BadRequest(result.Errors.Select(e => e.Description));
   
            }

        }


        [HttpPost("login")]
        public async Task<ActionResult<ApplicationUser>> Login(LoginUser loginUser)
        {

                var userFromDb = await _userManager.Users.Include(o => o.Friends).Include(r => r.FriendRequests).FirstOrDefaultAsync(i => i.UserName == loginUser.UserName);

            if (userFromDb.FriendRequests != null)
            {
                userFromDb = await _userManager.Users.Include(o => o.Friends).Include(r => r.FriendRequests).FirstOrDefaultAsync(i => i.UserName == loginUser.UserName);

                if (userFromDb == null) return NotFound("No such user is registered");

                var resultA = await _signInManager.CheckPasswordSignInAsync(userFromDb, loginUser.Password, false);

                if (!resultA.Succeeded) return BadRequest("Invalid Password");

                userFromDb.Token = await _tokenService.GenerateToken(userFromDb);

                return Ok(userFromDb);
            }

            if (userFromDb == null) return NotFound("No such user is registered");

            var resultB = await _signInManager.CheckPasswordSignInAsync(userFromDb, loginUser.Password, false);

            if (!resultB.Succeeded) return BadRequest("Invalid Password");

            userFromDb.Token = await _tokenService.GenerateToken(userFromDb);

            return Ok(userFromDb);





        }

        [HttpPost("email-confirmation")]
        public async Task<ActionResult<ApplicationUser>> EmailConfirmation(ConfirmationEmailModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return NotFound("No such user registered in the app");

            var code = model.Token.Replace(" ", "+");

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded) return Ok(user);

            return BadRequest("Cannot confirm email");
        }


        [HttpGet("all-users")]
        public async Task<ActionResult<List<ApplicationUser>>> GetAllUsers()
        {
            var users = await _userManager.Users.Include(i=>i.Friends).ToListAsync();

            return Ok(users);
        }


        [HttpGet("current-user/{username}")]
        public async Task<ActionResult<ApplicationUser>> GetCurrentUser(string username)
        {
            //var userName = User.GetUsername();

            //if (userName == null) return NotFound("claims not working");

            //var user = await _userManager.FindByNameAsync(userName);

            //var user = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByNameAsync(username);

            if(user == null)
            {
                user = await _userManager.Users.FirstOrDefaultAsync(p=>p.Email == username);

                if (user == null) return NotFound("No shuch user");
            }

        

            return Ok(user);
        }

        [HttpGet("get-user-by-username/{userName}")]
        public ActionResult<ApplicationUser> GetUserByUserName(string userName)
        {
            var user = _userManager.Users.Include(l=>l.Friends).Include(i=>i.FriendRequests).FirstOrDefault(e=>e.UserName == userName);

            if (user == null) return NotFound("Bruh");

            return Ok(user);
        }

    }
}
