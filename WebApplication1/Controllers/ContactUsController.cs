using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services.Iservices;
using WebApplication1.Utility;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IContactUsService _contactUsService;

        public ContactUsController(UserManager<ApplicationUser> userManager, IContactUsService contactUsService)
        {
            this.userManager = userManager;
            this._contactUsService = contactUsService;
        }

        // ✅ Users can submit a message
        [HttpPost("Create")]
        [Authorize]
        public async Task<IActionResult> CreateContactUs([FromBody] ContactUS contactUs)
        {
            
            var user = await userManager.GetUserAsync(User);
            contactUs.UserID = user.Id;
            contactUs.Name = user.UserName ?? "Unknown User";
            contactUs.Email = user.Email ?? "no-email@example.com";
            contactUs.Status = true;

            await _contactUsService.CreateContactUs(contactUs);

            return Ok(new { message = "Your message has been sent successfully." });
        }

        // ✅ Admin can view all contact messages
        [HttpGet("AdminMessages")]
        [Authorize(Roles = SD.adminRole)]
        public async Task<IActionResult> GetContactMessages()
        {
            var messages = await _contactUsService.GetAllMessages();
            return Ok(messages);
        }

        // ✅ Users can view their own messages
        [HttpGet("MyMessages")]
        [Authorize]
        public async Task<IActionResult> GetMyContactMessages()
        {
            var userId = userManager.GetUserId(User);
            var messages = await _contactUsService.GetUserMessages(userId);
            return Ok(messages);
        }

        // ✅ Users can update their messages
        [HttpPut("Update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateContactMessage(int id, [FromBody] ContactUS updatedContactUs)
        {

            var user = await userManager.GetUserAsync(User);
            
            await _contactUsService.UpdateContactUs(id, updatedContactUs);

            return Ok(new { message = "Message updated successfully." });
        }

        // ✅ Admin can view a specific contact message
        [HttpGet("Message/{id}")]
        [Authorize]
        public async Task<IActionResult> GetContactMessage(int id)
        {
            var message = await _contactUsService.GetMessageById(id);
            if (message == null)
                return NotFound(new { message = "Message not found." });

            return Ok(message);
        }

        // ✅ Admin can reply to a message
        [HttpPut("Reply/{id}")]
        [Authorize(Roles = SD.adminRole)]
        public async Task<IActionResult> ReplyToMessage(int id, [FromBody] ContactUS contactUs)
        {
            var contactMessage = await _contactUsService.GetMessageById(id);
            
            var adminUser = await userManager.GetUserAsync(User);
            

            await _contactUsService.ReplyToMessage(id, contactUs.Reply);

            return Ok(new { message = "Reply sent successfully." });
        }

        // ✅ Admin can delete a message
        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteContactMessage(int id)
        {
            var message = await _contactUsService.GetMessageById(id);
            if (message == null)
                return NotFound(new { message = "Message not found." });

            await _contactUsService.DeleteMessage(id);

            return Ok(new { message = "Message deleted successfully." });
        }
    }
}
