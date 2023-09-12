using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MSN.Data;
using MSN.Messages;
using MSN.Models;
using System.Text.RegularExpressions;
using MSN.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace MSN.SignalR
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MessageHub : Hub
    {

        private readonly IMessageRepository _messageRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageHub(IMessageRepository messageRepository, UserManager<ApplicationUser> userManager,
                          DataContext context, IMapper mapper)
        {

            _messageRepository = messageRepository;
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"];
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await addToGroup(groupName);


            var messages = await _messageRepository.MessageThread(Context.User.GetUsername(), otherUser);

            await Clients.Group(groupName).SendAsync("ReceivedMessageThread", messages);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveFromMessageGroup();
            await base.OnDisconnectedAsync(exception);

        }

        public async Task SendMessage(ChatMessageSent messageSent)
        {
            var meUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == messageSent.SenderUsername);

            var targetUser = await _userManager.Users.FirstOrDefaultAsync(r => r.Id == messageSent.targetId);

            var newMessage = new ChatMessage
            {

                SenderId = meUser.Id,
                SenderUsername = meUser.UserName,
                Sender = meUser,

                TargetId = messageSent.targetId,
                TargetUsername = targetUser.UserName,
                Target = targetUser,

                Content = messageSent.Content,
                MessageSent = DateTime.UtcNow
            };

            var groupName = GetGroupName(meUser.UserName, targetUser.UserName);
            var group = await _messageRepository.GetMessageGroup(groupName);

            //if (group.Connections.Any(x => x.Username == targetUser.UserName))
            //{
            //    newMessage.DateRead = DateTime.UtcNow;
            //}
            //else
            //{
            //    var connections = await PresenceTracker.GetConnectionsForUser(targetUser.UserName);
            //    if (connections != null)
            //    {
            //        await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new { username = meUser.UserName });
            //    }
            //}

            _context.Messages.Add(newMessage);

            if (await _context.SaveChangesAsync() > 0)
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<ChatMessageDto>(messageSent));
            };
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        private async Task<bool> addToGroup(String groupName)
        {
            var group = await _messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if (group == null)
            {
                group = new Models.Group(groupName);
                _messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            return await _messageRepository.SaveAllAsync();
        }

        private async Task RemoveFromMessageGroup()
        {
            var connection = await _messageRepository.GetConnection(Context.ConnectionId);
            _messageRepository.RemoveConnection(connection);
            await _messageRepository.SaveAllAsync();
        }
    }
}
