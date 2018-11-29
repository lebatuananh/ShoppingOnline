using System.Collections.Generic;
using ShoppingOnline.Application.Common.Feedbacks.Dtos;
using ShoppingOnline.Utilities.Dtos;

namespace ShoppingOnline.Application.Common.Feedbacks
{
    public interface IFeedbackService
    {
        void Add(FeedbackViewModel feedbackVm);

        void Update(FeedbackViewModel feedbackVm);

        void Delete(int id);

        List<FeedbackViewModel> GetAll();

        PagedResult<FeedbackViewModel> GetAllPaging(string keyword, int page, int pageSize);

        FeedbackViewModel GetById(int id);

        void SaveChanges();
    }
}