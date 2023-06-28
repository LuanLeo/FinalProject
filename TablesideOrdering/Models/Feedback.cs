using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Models
{
    public class Feedback
    {
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string FeedBack { get; set; }
    }
}
