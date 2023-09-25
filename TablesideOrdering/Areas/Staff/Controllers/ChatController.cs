using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using TablesideOrdering.Areas.Staff.Models;
using TablesideOrdering.Areas.Staff.ViewModels;
using TablesideOrdering.Areas.StoreOwner.Models;
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
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            foreach (var r in role)
            {
                StaffRole = r;
            }

            ChatViewModel chat = new ChatViewModel();
            chat.StaffRole = StaffRole;
            chat.ChatRoomList = ChatRoomList();
            List<SelectListItem> chatID = new List<SelectListItem>();
            foreach (var id in chat.ChatRoomList)
            {
                chatID.Add(new SelectListItem { Text = id.ChatRoomID, Value = id.ChatRoomID });
            }
            ViewBag.ChatTableLists = chatID;
            return View(chat);
        }

        public IActionResult ChatRoom(string id)
        {
            var chatRoom = _context.Chat.FirstOrDefault(x => x.ChatRoomID == id);
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
            chat.ChatRoomList = ChatRoomList();
            chat.ChatRoom = chatRoom;
            chat.ChatHistory = history;
            chat.StaffRole = StaffRole;

            return View(chat);
        }

        public List<Chat> ChatRoomList()
        {
            List<Chat> chat = _context.Chat.ToList();
            return chat;
        }

    }
}
