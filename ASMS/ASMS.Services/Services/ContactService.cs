using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Authentication;
using ASMS.Services.Model.Contact;
using ASMS.Services.Utilities;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASMS.Services.Services
{
    public class ContactService : IContactService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ContactService> _logger;
        private readonly IPasswordService _passwordService;
        private readonly ProjectMailConfig _mailConfig;
        private const string CONTACT_TYPE_DAMAGE_REPORT = "damage report";
        private const string CONTACT_TYPE_REFUND = "refund";
        private const string CONTACT_TYPE_REQUEST_TO_RETRIEVE = "request to retrieve";
        private const string CONTACT_TYPE_OTHER = "other";

        public ContactService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ContactService> logger,
            IPasswordService passwordService,
            IOptions<ProjectMailConfig> mailConfig)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _passwordService = passwordService;
            _mailConfig = mailConfig.Value;
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
                OrderDetailId = c.OrderDetailId,
                Name = c.Name,
                PhoneContact = c.PhoneContact,
                Email = c.Email,
                Message = c.Message,
                IsActive = c.IsActive,
                Image = DeserializeImageUrls(c.Image, c.ContactId),
                ContactDate = c.ContactDate, 
                RetrievedDate = c.RetrievedDate,
                ContactType = c.ContactType
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
                OrderDetailId = contact.OrderDetailId,
                Name = contact.Name,
                PhoneContact = contact.PhoneContact,
                Email = contact.Email,
                Message = contact.Message,
                IsActive = contact.IsActive,
                Image = DeserializeImageUrls(contact.Image, contact.ContactId),
                ContactDate = contact.ContactDate,
                RetrievedDate = contact.RetrievedDate,
                ContactType = contact.ContactType
            };
        }

        public async Task<ContactResponse> CreateAsync(CreateContactRequest request)
        {
            try
            {

                var contactType = string.IsNullOrWhiteSpace(request.ContactType)
                    ? CONTACT_TYPE_OTHER
                    : request.ContactType.ToLower();

                var validContactTypes = new[] {
                CONTACT_TYPE_DAMAGE_REPORT,
                CONTACT_TYPE_REFUND,
                CONTACT_TYPE_REQUEST_TO_RETRIEVE,
                CONTACT_TYPE_OTHER
            };

                if (!validContactTypes.Contains(contactType))
                {
                    throw new ArgumentException($"Invalid ContactType. Must be one of: {string.Join(", ", validContactTypes)}");
                }

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
                Order order = null;
                if (!string.IsNullOrEmpty(request.OrderCode))
                {
                    order = await _unitOfWork.Orders.GetByCodeAsync(request.OrderCode);
                    if (order == null)
                    {
                        _logger.LogWarning("Order {Code} not found", request.OrderCode);
                        throw new ArgumentException($"Order {request.OrderCode} not found");
                    }
                }

                // Validate OrderDetailId if provided
                if (request.OrderDetailId.HasValue)
                {
                    var orderDetail = await _unitOfWork.OrderDetails.GetByIdNoIncludeAsync(request.OrderDetailId.Value);

                    if (orderDetail == null)
                    {
                        throw new ArgumentException($"OrderDetail with ID {request.OrderDetailId.Value} not found");
                    }

                    if (!string.IsNullOrEmpty(request.OrderCode) &&
                        orderDetail.OrderCode != request.OrderCode)
                    {
                        throw new ArgumentException(
                            $"OrderDetail {request.OrderDetailId.Value} does not belong to Order {request.OrderCode}");
                    }

                    if (string.IsNullOrEmpty(request.OrderCode))
                    {
                        request.OrderCode = orderDetail.OrderCode;
                        order = await _unitOfWork.Orders.GetByCodeAsync(request.OrderCode);
                    }
                }

                string? imageJson = null;
                if (request.Image != null && request.Image.Any())
                {
                    try
                    {
                        imageJson = JsonSerializer.Serialize(request.Image);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error serializing ImageUrls for contact creation");
                    }
                }

                var contact = new Contact
                {
                    CustomerCode = request.CustomerCode,
                    EmployeeCode = request.EmployeeCode,
                    OrderCode = request.OrderCode,
                    OrderDetailId = request.OrderDetailId,
                    Name = request.Name,
                    PhoneContact = request.PhoneContact,
                    Email = request.Email,
                    Message = request.Message,
                    Image = imageJson,
                    ContactDate = request.ContactDate ?? GetVietnamToday(),
                    RetrievedDate = request.RetrievedDate,
                    ContactType = contactType
                };

                await _unitOfWork.Contacts.AddAsync(contact);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Created contact {Id} with type {Type}", contact.ContactId, contactType);

                return await GetByIdAsync(contact.ContactId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contact");
                throw;
            }
        }

        /// <summary>
        /// Tạo contact và gửi email tự động theo ContactType
        /// </summary>
        public async Task<ContactResponse> CreateWithEmailAsync(CreateContactRequest request)
        {
            try
            {
                var contactResponse = await CreateAsync(request);

                var contactType = contactResponse.ContactType?.ToLower() ?? CONTACT_TYPE_OTHER;

                if (contactType != CONTACT_TYPE_OTHER && !string.IsNullOrEmpty(request.Email))
                {
                    try
                    {
                        Order order = null;
                        if (!string.IsNullOrEmpty(request.OrderCode))
                        {
                            order = await _unitOfWork.Orders.GetByCodeAsync(request.OrderCode);
                        }

                        await SendContactEmailAsync(request, order, contactType);
                        _logger.LogInformation($"Email sent for contact type '{contactType}' to {request.Email}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending email for contact type '{contactType}' to {request.Email}");
                    }
                }

                return contactResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contact with email");
                throw;
            }
        }

        /// <summary>
        /// Gửi email theo ContactType
        /// </summary>
        private async Task<bool> SendContactEmailAsync(CreateContactRequest request, Order order, string contactType)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email)) return false;

                string subject = "";
                string emailContent = "";

                switch (contactType)
                {
                    case CONTACT_TYPE_DAMAGE_REPORT:
                        subject = $"Xác nhận báo hư hại - Đơn hàng {request.OrderCode ?? "N/A"}";
                        emailContent = EmailTemplates.DamageReportConfirmation(
                            request.Name ?? "Khách hàng",
                            request.Message ?? "",
                            request.OrderCode,
                            request.OrderDetailId,
                            request.Image,
                            _mailConfig.Email
                        );
                        break;

                    case CONTACT_TYPE_REFUND:
                        subject = $"Thông báo đền bù thiệt hại - Đơn hàng {request.OrderCode ?? "N/A"}";
                        emailContent = EmailTemplates.RefundNotification(
                            request.Name ?? "Khách hàng",
                            request.Message ?? "",
                            request.OrderCode,
                            order?.Refund ?? 0,
                            request.Image,
                            _mailConfig.Email
                        );
                        break;

                    case CONTACT_TYPE_REQUEST_TO_RETRIEVE:
                        subject = $"Yêu cầu nhận hàng - Đơn hàng {request.OrderCode ?? "N/A"}";
                        emailContent = EmailTemplates.RequestToRetrieve(
                            request.Name ?? "Khách hàng",
                            request.OrderCode,
                            order?.ReturnDate,
                            _mailConfig.Email
                        );
                        break;

                    default:
                        return false;
                }

                await _passwordService.SendEmailAsync(request.Email, subject, emailContent);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending contact email");
                return false;
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

                // Validate ContactType nếu có
                if (!string.IsNullOrWhiteSpace(request.ContactType))
                {
                    var validContactTypes = new[] {
                    CONTACT_TYPE_DAMAGE_REPORT,
                    CONTACT_TYPE_REFUND,
                    CONTACT_TYPE_REQUEST_TO_RETRIEVE,
                    CONTACT_TYPE_OTHER
                };

                    if (!validContactTypes.Contains(request.ContactType.ToLower()))
                    {
                        throw new ArgumentException($"Invalid ContactType");
                    }
                }

                // Validate CustomerCode if provided and changed
                if (!string.IsNullOrEmpty(request.CustomerCode) &&
                    request.CustomerCode != contact.CustomerCode)
                {
                    var customer = await _unitOfWork.Customer.GetByCodeAsync(request.CustomerCode);
                    if (customer == null)
                    {
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
                        throw new ArgumentException($"Order {request.OrderCode} not found");
                    }
                }

                // Validate OrderDetailId if provided
                if (request.OrderDetailId.HasValue)
                {
                    var orderDetail = await _unitOfWork.OrderDetails.GetByIdAsync(request.OrderDetailId.Value);

                    if (orderDetail == null)
                    {
                        throw new ArgumentException($"OrderDetail with ID {request.OrderDetailId.Value} not found");
                    }

                    // Kiểm tra OrderDetail có thuộc Order không
                    var orderCode = request.OrderCode ?? contact.OrderCode;
                    if (!string.IsNullOrEmpty(orderCode) && orderDetail.OrderCode != orderCode)
                    {
                        throw new ArgumentException(
                            $"OrderDetail {request.OrderDetailId.Value} does not belong to Order {orderCode}");
                    }
                }

                string? imageJson = null;
                if (request.Image != null && request.Image.Any())
                {
                    try
                    {
                        imageJson = JsonSerializer.Serialize(request.Image);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Error serializing ImageUrls for contact {contactId}");
                    }
                }

                // Update fields
                contact.CustomerCode = request.CustomerCode ?? contact.CustomerCode;
                contact.EmployeeCode = request.EmployeeCode ?? contact.EmployeeCode;
                contact.OrderCode = request.OrderCode ?? contact.OrderCode;
                contact.OrderDetailId = request.OrderDetailId ?? contact.OrderDetailId;
                contact.Name = request.Name ?? contact.Name;
                contact.PhoneContact = request.PhoneContact ?? contact.PhoneContact;
                contact.Email = request.Email ?? contact.Email;
                contact.Message = request.Message ?? contact.Message;
                contact.IsActive = request.IsActive ?? contact.IsActive;
                contact.ContactType = request.ContactType ?? contact.ContactType;

                if (imageJson != null)
                {
                    contact.Image = imageJson;
                }

                if (request.ContactDate.HasValue)
                {
                    contact.ContactDate = request.ContactDate;
                }

                if (request.RetrievedDate.HasValue)
                {
                    contact.RetrievedDate = request.RetrievedDate;
                }

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
        /// <summary>
        /// Đếm số contact có type "request_to_retrieve" của một order
        /// </summary>
        public async Task<int> CountRequestToRetrieveByOrderCodeAsync(string orderCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(orderCode))
                {
                    throw new ArgumentException("OrderCode cannot be null or empty");
                }

                var count = await _unitOfWork.Contacts.CountRequestToRetrieveByOrderCodeAsync(orderCode);

                _logger.LogInformation("Order {Code} has {Count} request_to_retrieve contacts", orderCode, count);

                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting request_to_retrieve contacts for order {Code}", orderCode);
                throw;
            }
        }

        /// <summary>
        /// Helper method để deserialize Image từ JSON string thành List<string>
        /// </summary>
        private List<string>? DeserializeImageUrls(string? imageJson, int contactId)
        {
            if (string.IsNullOrEmpty(imageJson))
                return new List<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(imageJson);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Error deserializing ImageUrls for contact {contactId}");
                return new List<string>();
            }
        }
        /// <summary>
        /// Lấy ngày hiện tại theo múi giờ Việt Nam
        /// </summary>
        private DateOnly GetVietnamToday()
        {
            try
            {
                var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
                return DateOnly.FromDateTime(vietnamNow);
            }
            catch (TimeZoneNotFoundException)
            {
                try
                {
                    var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
                    var vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
                    return DateOnly.FromDateTime(vietnamNow);
                }
                catch
                {
                    var vietnamNow = DateTime.UtcNow.AddHours(7);
                    return DateOnly.FromDateTime(vietnamNow);
                }
            }
        }


    }
}
