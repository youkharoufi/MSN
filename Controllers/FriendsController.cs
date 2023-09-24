using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSN.Claims;
using MSN.Data;
using MSN.Models;
using SQLitePCL;

namespace MSN.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public FriendsController(UserManager<ApplicationUser> userManager, IMapper mapper, DataContext context)
        {
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
            
        }

        [HttpPost("add-friend/{myUsername}/{username}")]
        public async Task<ActionResult<ApplicationUser>> AddFriend(string myUsername, string username)
        {


            var connectedUser = await _userManager.Users.Include(o=>o.Friends).FirstOrDefaultAsync(o=>o.UserName == myUsername);

            var newFriend = await _userManager.FindByNameAsync(username);

            if (newFriend == null) return NotFound();

            connectedUser?.Friends?.Add(newFriend);
            newFriend?.Friends?.Add(connectedUser);

            await _userManager.UpdateAsync(connectedUser);
            await _userManager.UpdateAsync(newFriend);

            return Ok(connectedUser);
        }

        [HttpGet("all-friends")]
        public async Task<ActionResult<List<ApplicationUser>>> GetAllFriends()
        {
            var connectedUserId = User.GetUserId();

            var connectedUser = await _userManager.Users.Include(p=>p.Friends).FirstOrDefaultAsync(o => o.Id == connectedUserId);

            var allFriends = connectedUser?.Friends;

            return Ok(allFriends);
        }

        [HttpPost("send-friend-request/{currentUserName}/{receivingUserName}")]
        public async Task<ActionResult<ApplicationUser>> SendFriendRequest(string currentUserName, string receivingUserName)
        {
            var userReceiving = await _userManager.Users.Include(e => e.FriendRequests).FirstOrDefaultAsync(w => w.UserName == receivingUserName);

            if (userReceiving == null) return NotFound("No such user found");

            var currentUser = await _userManager.FindByNameAsync(currentUserName);

            if (currentUser == null) return NotFound("No such user found");

            if (userReceiving.FriendRequests.Any(fr => fr.UserName == receivingUserName))
            {
                return BadRequest("Friend request already sent");
            }

            var friendRequest = new FriendRequest
            {
                Id = Guid.NewGuid().ToString(),
                UserName = currentUserName,
            };

            userReceiving.FriendRequests.Add(friendRequest);

            var result = await _userManager.UpdateAsync(userReceiving);

            if(!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(userReceiving);

        }

        [HttpPost("accept-friend-request/{currentUserName}/{senderUsername}")]
        public async Task<ActionResult<ApplicationUser>> AcceptFriendRequest(string currentUserName, string senderUsername)
        {
            var newFriend = await _userManager.Users.Include(l => l.FriendRequests).FirstOrDefaultAsync(j => j.UserName == senderUsername);

            if (newFriend == null) return NotFound("No such user found");

            var currentUser = await _userManager.Users.Include(j => j.FriendRequests).FirstOrDefaultAsync(k => k.UserName == currentUserName);

            if (currentUser == null) return NotFound("No such user found");

            var friendRequest = currentUser.FriendRequests.Where(u => u.UserName == senderUsername).FirstOrDefault();

            currentUser.FriendRequests.Remove(friendRequest);

            currentUser.Friends.Add(newFriend);

            newFriend.Friends.Add(currentUser);

            await _userManager.UpdateAsync(currentUser);
            await _userManager.UpdateAsync(newFriend);

            return Ok(currentUser);


        }

        [HttpPost("deny-friend-request/{currentUserName}/{senderUserName}")]
        public async Task<ActionResult<ApplicationUser>> DenyFriendRequest(string currentUserName, string senderUserName)
        {
            var newFriend = await _userManager.FindByNameAsync(senderUserName);

            if (newFriend == null) return NotFound("No such user found");

            var currentUser = await _userManager.Users.Include(f => f.FriendRequests).FirstOrDefaultAsync(m => m.UserName == currentUserName);

            if (currentUser == null) return NotFound("No such user found");

            var fr = currentUser.FriendRequests.Where(o => o.UserName == senderUserName).FirstOrDefault();

            currentUser.FriendRequests.Remove(fr);

            await _userManager.UpdateAsync(currentUser);

            return Ok(currentUser);


        }

        [HttpGet("get-friend-requests-count/{userName}")]
        public async Task<ActionResult<int>> getCount(string userName)
        {
            var user = await _userManager.Users.Include(o => o.FriendRequests).FirstOrDefaultAsync(s => s.UserName == userName);

            if (user == null) return NotFound("No such user");

            var count = user.FriendRequests?.Count();

            if (count == null)
            {
                return 0;
            }

            return count;
        }

        [HttpGet("get-friend-request-by-sender-username/{userName}/{senderUsername}")]
        public async Task<ActionResult<FriendRequest>> getFriendRequest(string userName, string senderUsername)
        {
            var user = await _userManager.Users.Include(o => o.FriendRequests).FirstOrDefaultAsync(s => s.UserName == userName);

            if (user == null) return NotFound("No such user");

            var friendRequest = user.FriendRequests.Where(o =>  o.UserName == senderUsername).FirstOrDefault();

            if (friendRequest == null) return NotFound("No such friend request");

            return friendRequest;
        }

        [HttpGet("get-all-friend-requests/{currentUsername}")]
        public async Task<ActionResult<List<FriendRequest>>> getAllFriendRequests(string currentUsername)
        {

            var currentUser = await _userManager.Users.Include(f => f.FriendRequests).FirstOrDefaultAsync(m => m.UserName == currentUsername);

            if (currentUser == null) return NotFound("No such user found");

            var fr = currentUser.FriendRequests.ToList();

            return Ok(fr);
        }

        [HttpGet("get-all-users-based-on-user-username/{currentUsername}")]
        public async Task<ActionResult<List<ApplicationUser>>> getAllUsersBasedOnFriendRequests(string currentUsername)
        {
            var user = await _userManager.Users.Include(o => o.FriendRequests).FirstOrDefaultAsync(l => l.UserName == currentUsername);

            if (user == null) return NotFound("No such user");

            var userList = new List<ApplicationUser>();

            foreach (var fr in user.FriendRequests)
            {
                var getUser = await _userManager.FindByNameAsync(fr.UserName);
                if (getUser != null) userList.Add(getUser);
            }

            return Ok(userList);
        }


        [HttpGet("get-all-user-friends/{currentUsername}")]
        public async Task<ActionResult<List<ApplicationUser>>> getAllUsersFriends(string currentUsername)
        {
            var currentUser = await _userManager.Users.Include(n => n.Friends).FirstOrDefaultAsync(o => o.UserName == currentUsername);

            if (currentUser == null) return NotFound("No such User");

            var allFriends = new List<ApplicationUser>();

            allFriends = currentUser.Friends;

            return Ok(allFriends);
        }

    }
}
