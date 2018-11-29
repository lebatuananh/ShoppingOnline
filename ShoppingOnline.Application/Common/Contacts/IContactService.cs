using System.Collections.Generic;
using ShoppingOnline.Application.Common.Contacts.Dtos;
using ShoppingOnline.Utilities.Dtos;

namespace ShoppingOnline.Application.Common.Contacts
{
    public interface IContactService
    {
        void Add(ContactViewModel contactVm);

        void Update(ContactViewModel contactVm);

        void Delete(string id);

        List<ContactViewModel> GetAll();

        PagedResult<ContactViewModel> GetAllPaging(string keyword, int page, int pageSize);

        ContactViewModel GetById(string id);

        void SaveChanges();
    }
}