using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Services.Model.Contact;

namespace ASMS.Services.Interfaces
{
    public interface IContactService
    {
        Task<PaginatedContactResponse> GetWithFilterAsync(
            int pageNumber,
            int pageSize,
            string? customerCode,
            string? employeeCode,
            string? orderCode);

        Task<ContactResponse?> GetByIdAsync(int contactId);
        Task<ContactResponse> CreateAsync(CreateContactRequest request);
        Task<bool> UpdateAsync(int contactId, UpdateContactRequest request);
        Task<ToggleContactActiveResponse> ToggleActiveAsync(int contactId);
        Task<ContactResponse> CreateWithEmailAsync(CreateContactRequest request);
        Task<int> CountRequestToRetrieveByOrderCodeAsync(string orderCode);
    }
}
