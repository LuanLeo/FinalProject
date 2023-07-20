using System.ComponentModel.DataAnnotations;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Areas.StoreOwner.ViewModels;
using TablesideOrdering.Models;
using TablesideOrdering.Models.Order;

namespace TablesideOrdering.ViewModels
{
    public class HomeViewModel
    {
        public List<Category> Category { get; set; }
        public List<TopFood> TopProduct { get; set; }
        public List<ProductSize> ProductSizes { get; set; }
        public IQueryable<ProductSizePriceViewModel> Product { get; set; }

        //Variables for Payment Methods
        public string PaymentType { get; set; }
        public PaymentInformationModel Payment { get; set; }
        public OrderInfoModel MoMoPay { get; set; }


        //Variable for Cart page
        public CartList Cart { get; set; }


        //Variable for entering phone number
        public PhoneValidation PhoneValid { get; set; }


        //Variable for saving email to PR
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        public string MailPR { get; set; }


        //Variables for sorting product
        public string NameSort { get; set; }
        public string Term { get; set; }
        public string OrderBy { get; set; }

    }
}
