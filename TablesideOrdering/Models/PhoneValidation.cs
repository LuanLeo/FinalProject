using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Models
{
    public class PhoneValidation
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your phone number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [StringLength(maximumLength: 10, MinimumLength = 10, ErrorMessage = "Length must be 10")]
        public string PhoneNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your phone number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [Compare(otherProperty: "PhoneNumber", ErrorMessage = "Phone & Phone confirm does not match")]
        [StringLength(maximumLength: 10, MinimumLength = 10, ErrorMessage = "Length must be 10")]
        public string PhoneConfirmed { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your table No")]
        public string TableNo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your name")]
        public string CusName { get; set; }
    }
}
