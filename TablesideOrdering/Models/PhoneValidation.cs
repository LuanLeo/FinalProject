using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Models
{
    public class PhoneValidation
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your phone number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [MaxLength(10, ErrorMessage = "Length must be 10")]
        public string PhoneNumber { get; set; }
    }
}
