using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Models.Customer
{
    public class PhoneValidation
    {
        public string CountryNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your phone number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [StringLength(maximumLength: 10, MinimumLength = 10, ErrorMessage = "Length must be 10")]
        public string PhoneNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your phone number")]
        [Compare(otherProperty: "PhoneNumber", ErrorMessage = "Phone & Phone confirm does not match")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [StringLength(maximumLength: 10, MinimumLength = 10, ErrorMessage = "Length must be 10")]
        public string PhoneConfirmed { get; set; }
    }
}
