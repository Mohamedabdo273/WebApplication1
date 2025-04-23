using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;
using WebApplication1.Repository.IRepository;
using WebApplication1.Services.Iservices;
using System.Security.Claims;

namespace WebApplication1.Services
{
    public class ContactUsService : IContactUsService
    {
        private readonly IContactUs _contactUs;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContactUsService(IContactUs contactUs, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _contactUs = contactUs;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task CreateContactUs(ContactUS contactUs)
        {
            if (string.IsNullOrWhiteSpace(contactUs.Subject) || string.IsNullOrWhiteSpace(contactUs.Message))
                throw new ArgumentException("Subject and Message are required.");

            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var appUser = await _userManager.GetUserAsync(user);
            if (appUser == null)
                throw new UnauthorizedAccessException("User not found.");

            contactUs.UserID = appUser.Id;
            contactUs.Name = appUser.UserName ?? "Unknown User";
            contactUs.Email = appUser.Email ?? "no-email@example.com";
            contactUs.Status = true;

            _contactUs.Create(contactUs);
            _contactUs.Commit();
        }

        public async Task<IEnumerable<ContactUS>> GetAllMessages()
        {
            return _contactUs.Get();
        }

        public async Task<IEnumerable<ContactUS>> GetUserMessages(string userId)
        {
            return _contactUs.Get(expression: e => e.UserID == userId);
        }

        public async Task<ContactUS> GetMessageById(int id)
        {
            return _contactUs.GetOne(expression: e => e.Id == id);
        }

        public async Task UpdateContactUs(int id, ContactUS updatedContactUs)
        {
            var existing = _contactUs.GetOne(expression: e => e.Id == id && e.UserID == updatedContactUs.UserID);
            if (existing == null)
                throw new KeyNotFoundException("Message not found or unauthorized.");

            existing.Subject = updatedContactUs.Subject;
            existing.Message = updatedContactUs.Message;
            existing.Status = true;

            _contactUs.Edit(existing);
            _contactUs.Commit();
        }

        public async Task ReplyToMessage(int id, string reply)
        {
            var message = _contactUs.GetOne(expression: e => e.Id == id);
            if (message == null)
                throw new KeyNotFoundException("Message not found.");

            message.Reply = reply;
            message.Status = false;

            _contactUs.Edit(message);
            _contactUs.Commit();
        }

        public async Task DeleteMessage(int id)
        {
            var message = _contactUs.GetOne(expression: e => e.Id == id);
            if (message == null)
                throw new KeyNotFoundException("Message not found.");

            _contactUs.Delete(message);
            _contactUs.Commit();
        }
    }
}
