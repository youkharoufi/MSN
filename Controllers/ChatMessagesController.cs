using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSN.Claims;
using MSN.Data;
using MSN.Messages;
using MSN.Models;

namespace MSN.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ChatMessagesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DataContext _context;
        private readonly IMessageRepository _messageRepository;
        private IMapper _mapper;
        public ChatMessagesController(UserManager<ApplicationUser> userManager, DataContext context, IMessageRepository messageRepository, IMapper mapper)
        {
            _userManager = userManager;
            _context = context;
            _messageRepository = messageRepository;
            _mapper = mapper;

        }

        [HttpPost("createMessage")]
        public async Task<ActionResult<ChatMessage>> CreateMessage([FromBody]ChatMessageSent message)
        {

            var meUseriD = User.GetUserId();

            var meUser = await _userManager.FindByIdAsync(meUseriD);

            var targetUser = await _userManager.Users.FirstOrDefaultAsync(r => r.Id == message.targetId);

            var newMessage = new ChatMessage
            {

                SenderId = meUser.Id,
                SenderUsername = meUser.UserName,
                Sender = meUser,

                TargetId = message.targetId,
                TargetUsername = targetUser.UserName,
                Target = targetUser,

                Content = message.Content
            };


            _context.Messages.Add(newMessage);

            await _context.SaveChangesAsync();

            return Ok();

        }


        [HttpGet("inbox/{id}")]
        public async Task<List<ChatMessage>> Inbox()
        {
            var userId = User.GetUserId();

            var user = await _userManager.FindByIdAsync(userId);

            var messages = await _context.Messages.Where(m => m.Target == user).OrderBy(e => e.Sender).ToListAsync();

            return messages;
        }

        [HttpGet("outbox/{id}")]
        public async Task<List<ChatMessage>> Outbox()
        {
            var userId = User.GetUserId();

            var user = await _userManager.FindByIdAsync(userId);

            var messages = await _context.Messages.Where(m => m.Sender == user).OrderBy(e => e.Target).ToListAsync();

            return messages;
        }

        [HttpGet("thread/{mes.username}/{mes.otherUsername}")]
        public async Task<ActionResult<List<ChatMessage>>> MessagesThread(MessageThread mes)
        {
            return Ok(await _messageRepository.MessageThread(mes.currentUsername, mes.otherUsername));


        }


        [HttpGet("unread-messages-count")]
        public async Task<int> GetUnreadMessagesCount()
        {
            var userId = User.GetUserId();

            List<ChatMessage> unreadMessages = await _context.Messages.Where(o => o.TargetId == userId && o.DateRead == null).ToListAsync();

            var messagesCount = unreadMessages.Count();

            return messagesCount;
        }

    }
}