using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using TablesideOrdering.Areas.Staff.Models;
using TablesideOrdering.Areas.Staff.ViewModels;
using TablesideOrdering.Data;

namespace TablesideOrdering.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "Staff, Admin")]

    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ChatController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public static ChatViewModel ChatViewModel = new ChatViewModel();
        public static string StaffRole;
        public IActionResult Index()
        {
            ChatViewModel chat = new ChatViewModel();
            chat.ChatRoomList = _context.Chats.ToList();

            List<SelectListItem> chatID = new List<SelectListItem>();
            foreach (var id in chat.ChatRoomList)
            {
                chatID.Add(new SelectListItem { Text = id.ChatRoomID, Value = id.ChatRoomID });
            }

            ViewBag.ChatTableLists = chatID;
            return View(chat);
        }

        public async Task<IActionResult> ChatRoom(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            foreach (var r in role)
            {
                ViewBag.RoleForRoom = r;
                StaffRole = r;
            }

            var chatRoom = _context.Chats.FirstOrDefault(x => x.ChatRoomID == id);
            List<ChatHistory> history = new List<ChatHistory>();

            if (ChatViewModel.ChatHistory != null)
            {
                foreach (var his in ChatViewModel.ChatHistory)
                {
                    if (his.ChatRoomId == id)
                    {
                        history.Add(his);
                    }

                }
            }

            ChatViewModel chat = new ChatViewModel();
            chat.ChatRoom = chatRoom;
            chat.ChatHistory = history;

            return View(chat);
        }
    }
}
