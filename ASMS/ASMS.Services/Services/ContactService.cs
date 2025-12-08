using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Contact;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace ASMS.Services.Services
{
    public class ContactService : IContactService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ContactService> _logger;

        public ContactService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ContactService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedContactResponse> GetWithFilterAsync(
            int pageNumber,
            int pageSize,
            string? customerCode,
            string? employeeCode,
            string? orderCode)
        {
            var contacts = await _unitOfWork.Contacts.GetWithFilterAsync(
                pageNumber, pageSize, customerCode, employeeCode, orderCode);

            var totalCount = await _unitOfWork.Contacts.GetTotalCountWithFilterAsync(
                customerCode, employeeCode, orderCode);

            var contactResponses = contacts.Select(c => new ContactResponse
            {
                ContactId = c.ContactId,
                CustomerCode = c.CustomerCode,
                CustomerName = c.CustomerCodeNavigation?.Name,
                EmployeeCode = c.EmployeeCode,
                OrderCode = c.OrderCode,
                Name = c.Name,
                PhoneContact = c.PhoneContact,
                Email = c.Email,
                Message = c.Message,
                IsActive = c.IsActive,
            }).ToList();

            return new PaginatedContactResponse
            {
                Data = contactResponses,
                Page = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<ContactResponse?> GetByIdAsync(int contactId)
        {
            var contact = await _unitOfWork.Contacts.GetEntityByIdAsync(contactId);

            if (contact == null)
                return null;

            return new ContactResponse
            {
                ContactId = contact.ContactId,
                CustomerCode = contact.CustomerCode,
                CustomerName = contact.CustomerCodeNavigation?.Name,
                EmployeeCode = contact.EmployeeCode,
                OrderCode = contact.OrderCode,
                Name = contact.Name,
                PhoneContact = contact.PhoneContact,
                Email = contact.Email,
                Message = contact.Message
            };
        }

        public async Task<ContactResponse> CreateAsync(CreateContactRequest request)
        {
            try
            {
                // Validate CustomerCode if provided
                if (!string.IsNullOrEmpty(request.CustomerCode))
                {
                    var customer = await _unitOfWork.Customer.GetByCodeAsync(request.CustomerCode);
                    if (customer == null)
                    {
                        _logger.LogWarning("Customer {Code} not found", request.CustomerCode);
                        throw new ArgumentException($"Customer {request.CustomerCode} not found");
                    }
                }

                // Validate OrderCode if provided
                if (!string.IsNullOrEmpty(request.OrderCode))
                {
                    var order = await _unitOfWork.Orders.GetByCodeAsync(request.OrderCode);
                    if (order == null)
                    {
                        _logger.LogWarning("Order {Code} not found", request.OrderCode);
                        throw new ArgumentException($"Order {request.OrderCode} not found");
                    }
                }

                var contact = new Contact
                {
                    CustomerCode = request.CustomerCode,
                    EmployeeCode = request.EmployeeCode,
                    OrderCode = request.OrderCode,
                    Name = request.Name,
                    PhoneContact = request.PhoneContact,
                    Email = request.Email,
                    Message = request.Message
                };

                await _unitOfWork.Contacts.AddAsync(contact);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Created contact {Id}", contact.ContactId);

                return await GetByIdAsync(contact.ContactId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contact");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int contactId, UpdateContactRequest request)
        {
            try
            {
                var contact = await _unitOfWork.Contacts.GetEntityByIdAsync(contactId);

                if (contact == null)
                {
                    _logger.LogWarning("Contact {Id} not found", contactId);
                    return false;
                }

                // Validate CustomerCode if provided and changed
                if (!string.IsNullOrEmpty(request.CustomerCode) &&
                    request.CustomerCode != contact.CustomerCode)
                {
                    var customer = await _unitOfWork.Customer.GetByCodeAsync(request.CustomerCode);
                    if (customer == null)
                    {
                        _logger.LogWarning("Customer {Code} not found", request.CustomerCode);
                        throw new ArgumentException($"Customer {request.CustomerCode} not found");
                    }
                }

                // Validate OrderCode if provided and changed
                if (!string.IsNullOrEmpty(request.OrderCode) &&
                    request.OrderCode != contact.OrderCode)
                {
                    var order = await _unitOfWork.Orders.GetByCodeAsync(request.OrderCode);
                    if (order == null)
                    {
                        _logger.LogWarning("Order {Code} not found", request.OrderCode);
                        throw new ArgumentException($"Order {request.OrderCode} not found");
                    }
                }

                // Update fields
                contact.CustomerCode = request.CustomerCode ?? contact.CustomerCode;
                contact.EmployeeCode = request.EmployeeCode ?? contact.EmployeeCode;
                contact.OrderCode = request.OrderCode ?? contact.OrderCode;
                contact.Name = request.Name ?? contact.Name;
                contact.PhoneContact = request.PhoneContact ?? contact.PhoneContact;
                contact.Email = request.Email ?? contact.Email;
                contact.Message = request.Message ?? contact.Message;
                contact.IsActive = request.IsActive ?? contact.IsActive;

                await _unitOfWork.Contacts.UpdateAsync(contact);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Updated contact {Id}", contactId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contact {Id}", contactId);
                throw;
            }
        }
        public async Task<ToggleContactActiveResponse> ToggleActiveAsync(int contactId)
        {
            try
            {
                var contact = await _unitOfWork.Contacts.GetEntityByIdAsync(contactId);

                if (contact == null)
                {
                    _logger.LogWarning("Contact {Id} not found", contactId);
                    return null;
                }

                // Toggle IsActive
                contact.IsActive = !(contact.IsActive ?? false);

                await _unitOfWork.Contacts.UpdateAsync(contact);
                await _unitOfWork.CompleteAsync();

                var status = contact.IsActive == true ? "activated" : "deactivated";
                _logger.LogInformation("Contact {Id} {Status}", contactId, status);

                return new ToggleContactActiveResponse
                {
                    ContactId = contactId,
                    IsActive = contact.IsActive ?? false,
                    Message = $"Contact successfully {status}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling active status for contact {Id}", contactId);
                throw;
            }
        }

    }
}
