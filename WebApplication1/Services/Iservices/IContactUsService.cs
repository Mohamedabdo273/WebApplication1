using WebApplication1.Models;

namespace WebApplication1.Services.Iservices
{
    public interface IContactUsService
    {
        Task CreateContactUs(ContactUS contactUs);
        Task<IEnumerable<ContactUS>> GetAllMessages();
        Task<IEnumerable<ContactUS>> GetUserMessages(string userId);
        Task<ContactUS> GetMessageById(int id);
        Task UpdateContactUs(int id, ContactUS updatedContactUs);
        Task ReplyToMessage(int id, string reply);
        Task DeleteMessage(int id);
    }
}
