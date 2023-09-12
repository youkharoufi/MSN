using MSN.Models;
using System.Text.RegularExpressions;

namespace MSN.Messages
{
    public interface IMessageRepository
    {

        void AddMessage(ChatMessage message);
        void DeleteMessage(ChatMessage message);

        Task<bool> SaveAllAsync();

        Task<List<ChatMessage>> MessageThread(string username, string otherUsername);

        void AddGroup(Models.Group group);

        void RemoveConnection(Connection connection);

        Task<Connection> GetConnection(string connectionId);

        Task<Models.Group> GetMessageGroup(string groupName);

    }
}
