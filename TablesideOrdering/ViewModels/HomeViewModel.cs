using System.ComponentModel.DataAnnotations;
using TablesideOrdering.Areas.Staff.Models;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Areas.StoreOwner.ViewModels;
using TablesideOrdering.Models;
using TablesideOrdering.Models.Momo;
using TablesideOrdering.Models.VNPay;

namespace TablesideOrdering.ViewModels
{
    public class HomeViewModel
    {
        //Variable for Store Owner Management
        public List<Category> Category { get; set; }
        public List<TopFood> TopProduct { get; set; }
        public List<ProductSize> ProductSizes { get; set; }
        public List<Orders> Orders { get; set; }
        public IQueryable<ProductSizePriceViewModel> Product { get; set; }

        
        public ReservationViewModel Reservation { get; set; }
        public Reservation Reser { get; set; }

        //Variables for Payment Methods
        public string PaymentType { get; set; }
        public PaymentInformationModel Payment { get; set; }
        public OrderInfoModel MoMoPay { get; set; }


        //Variable for Cart page
        public CartList Cart { get; set; }


        //Variable for receiving e-receipt
        public Email Email { get; set; }


        //Variable for saving email to PR
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        public string MailPR { get; set; }


        //Variables for sorting product
        public string NameSort { get; set; }
        public string Term { get; set; }
        public string OrderBy { get; set; }


        //Variables for order detail
        public IQueryable<HomeViewModel> Order { get; set; }
        public IQueryable<HomeViewModel> OrderDetail { get; set; }
        public int OrderId { get; set; }
        public string OrderDate { get; set; }
        public string TableNo { get; set; }
        public float OrderPrice { get; set; }

        public Chat Chat { get; set; }

        //Lock option in index page
        public string LockType { get; set; }

        //Show coupon to the page
        public string CouponShow { get; set; }

        public int ProductQuantity { get; set; }
        public int OrderDetailId { get; set; }
        public string ProductName { get; set; }
        public string Size { get; set; }
        public int ProQuantity { get; set; }
        public float Price { get; set; }
        public float SubTotal { get; set; }
        public string Status { get; set; }
        public string OrderType { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your name")]
        public string CusName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your address")]
        public string Address { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your phone number")]
        public string PhoneNumber { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please choose time to pick up")]
        public TimeSpan PickTime { get; set; }
    }
}
