using ShoppingOnline.Application.Common.Contacts.Dtos;
using ShoppingOnline.Application.Common.Feedbacks.Dtos;

namespace ShoppingOnline.WebApplication.Models
{
    public class ContactPageViewModel
    {
        public ContactViewModel Contact { set; get; }

        public FeedbackViewModel Feedback { set; get; }
    }
}