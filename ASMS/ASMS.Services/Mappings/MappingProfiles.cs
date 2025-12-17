using ASMS.Repositories.Entities;
using ASMS.Services.Model;
using ASMS.Services.Model.Building;
using ASMS.Services.Model.Container;
using ASMS.Services.Model.ContainerLocationLog;
using ASMS.Services.Model.ContainerType;
using ASMS.Services.Model.Customer;
using ASMS.Services.Model.Employee;
using ASMS.Services.Model.Floor;
using ASMS.Services.Model.FloorBlocks;
using ASMS.Services.Model.OrderDetail;
using ASMS.Services.Model.Orders;
using ASMS.Services.Model.ProductType;
using ASMS.Services.Model.Services;
using ASMS.Services.Model.Shelves;
using ASMS.Services.Model.StorageBlocks;
using ASMS.Services.Model.Storages;
using ASMS.Services.Model.StorageTypes;
using ASMS.Services.Model.TrackingHistories;
using ASMS.Services.Model.WorkflowSteps;
using ASMS.Services.Model.WorkflowTemplates;
using AutoMapper;
using ASMS.Services.Model.FloorBlocks;
using ASMS.Services.Model.ContainerType;
using ASMS.Services.Model.Shelves;
using ASMS.Services.Model.ContainerLocationLog;
using ASMS.Services.Model.PaymentHistory;
using ASMS.Services.Model.EmployeeRole;
using ASMS.Services.Model.ShelfType;
using ASMS.Services.Model.Pricing;

namespace ASMS.Services.Mappings
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            #region Role
            CreateMap<CreateRoleRequest, EmployeeRole>();
            CreateMap<UpdateRoleRequest, EmployeeRole>();
            #endregion
            #region WorkflowTemplate
            CreateMap<WorkflowTemplate, WorkflowTemplateResponse>();
            CreateMap<CreateWorkflowTemplateRequest, WorkflowTemplate>();
            CreateMap<UpdateWorkflowTemplateRequest, WorkflowTemplate>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            #endregion
            #region WorkflowStep
            CreateMap<WorkflowStep, WorkflowStepResponse>()
                .ForMember(dest => dest.WorkflowTemplateName,
                    opt => opt.MapFrom(src => src.WorkflowTemplate != null ? src.WorkflowTemplate.Name : null));

            CreateMap<CreateWorkflowStepRequest, WorkflowStep>()
                .ForMember(dest => dest.WorkflowStepId, opt => opt.Ignore())
                .ForMember(dest => dest.WorkflowTemplate, opt => opt.Ignore());

            CreateMap<UpdateWorkflowStepRequest, WorkflowStep>()
                .ForMember(dest => dest.WorkflowStepId, opt => opt.Ignore())
                .ForMember(dest => dest.WorkflowTemplate, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            #endregion
            #region Service
            CreateMap<ASMS.Repositories.Entities.Service, ServiceResponse>();

            CreateMap<CreateServiceRequest, ASMS.Repositories.Entities.Service>();

            CreateMap<UpdateServiceRequest, ASMS.Repositories.Entities.Service>()
                .ForMember(dest => dest.ServiceId, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            #endregion
            #region Tracking History
            CreateMap<TrackingHistory, TrackingHistoryResponse>()
                .ForMember(dest => dest.Image, opt => opt.Ignore());

            CreateMap<CreateTrackingHistoryRequest, TrackingHistory>()
                .ForMember(d => d.TrackingHistoryId, o => o.Ignore())
                .ForMember(d => d.OrderCodeNavigation, o => o.Ignore()); 

            CreateMap<UpdateTrackingHistoryRequest, TrackingHistory>()
                .ForMember(d => d.TrackingHistoryId, o => o.Ignore())
                .ForMember(d => d.OrderCodeNavigation, o => o.Ignore()) 
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateTrackingStatusRequest, TrackingHistory>()
            .ForMember(dest => dest.TrackingHistoryId, opt => opt.Ignore()) 
            .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => DateOnly.FromDateTime(DateTime.Now)))
            .ForMember(dest => dest.OrderCodeNavigation, opt => opt.Ignore()); 
            #endregion
            #region Building
            CreateMap<CreateBuildingRequest, Building>()
                .ForMember(b => b.BuildingCode, bl => bl.MapFrom(src => src.BuildingCode));
            CreateMap<UpdateBuildingRequest, Building>()
                .ForMember(b => b.BuildingCode, bl => bl.MapFrom(src => src.BuildingCode));
            CreateMap<Building, GetBuildingResponse>();
            #endregion
            #region ProductType
            CreateMap<CreateTypeRequest, ProductType>();
            CreateMap<UpdateTypeRequest, ProductType>();
            CreateMap<ProductType, GetProductTypeResponse>();
            #endregion
            #region Floor
            CreateMap<CreateFloorRequest, Floor>();
            CreateMap<UpdateFloorRequest, Floor>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Floor, FloorResponse>();
            #endregion
            #region Container Type
            CreateMap<CreateContainerTypeRequest, ContainerType>();
            CreateMap<UpdateContainerTypeRequest, ContainerType>();
            CreateMap<ContainerType, GetContainerTypeResponse>();
            #endregion
            #region Shelf
            CreateMap<Shelf, ShelfResponse>();

            CreateMap<CreateShelfRequest, Shelf>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.StorageCodeNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.Floors, opt => opt.Ignore());

            CreateMap<UpdateShelfRequest, Shelf>()
                .ForMember(dest => dest.ShelfCode, opt => opt.Ignore())
                .ForMember(dest => dest.StorageCodeNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.Floors, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            #endregion
            #region ContainerLocationLog
            CreateMap<CreateContainerLocationLogRequest, ContainerLocationLog>();
            CreateMap<UpdateContainerLocationLogRequest, ContainerLocationLog>()
                .ForMember(dest => dest.ContainerLocationLogId, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<ContainerLocationLog, ContainerLocationLogResponse>();
            #endregion
            #region OrderDetail
            CreateMap<CreateOrderDetailRequest, OrderDetail>();
            CreateMap<UpdateOrderDetailRequest, OrderDetail>()
                .ForMember(dest => dest.OrderDetailId, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<OrderDetail, OrderDetailResponse>();
            CreateMap<OrderDetail, OrderDetailItemResponse>()
                .ForMember(dest => dest.ContainerType, opt => opt.MapFrom(src => src.ContainerType))
                .ForMember(dest => dest.FloorCode, opt => opt.MapFrom(src => src.ContainerCodeNavigation != null ? src.ContainerCodeNavigation.FloorCode : null))
                .ForMember(dest => dest.FloorNumber, opt => opt.MapFrom(src => src.ContainerCodeNavigation != null && src.ContainerCodeNavigation.FloorCodeNavigation != null ? src.ContainerCodeNavigation.FloorCodeNavigation.FloorNumber : null))
                .ForMember(dest => dest.ProductTypeNames, opt => opt.MapFrom(src =>
        src.OrderDetailProductTypes
            .Where(odpt => odpt.ProductType != null)
            .Select(odpt => odpt.ProductType.Name)
            .ToList()))
    .ForMember(dest => dest.ServiceNames, opt => opt.MapFrom(src =>
        src.OrderDetailServices
            .Where(ods => ods.Service != null)
            .Select(ods => ods.Service.Name)
            .ToList()));
            #endregion
            #region Container
            CreateMap<Container, ContainerResponse>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.ContainerType != null ? src.ContainerType.Type : null));

            CreateMap<CreateContainerRequest, Container>()
                .ForMember(dest => dest.FloorCodeNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.ContainerLocationLogs, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore());

            CreateMap<UpdateContainerRequest, Container>()
                .ForMember(dest => dest.ContainerCode, opt => opt.Ignore())
                .ForMember(dest => dest.FloorCodeNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.ContainerLocationLogs, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            #endregion
            #region Customer
            CreateMap<CreateCustomerRequest, Customer>();    
            CreateMap<UpdateCustomerRequest, Customer>();
            CreateMap<Customer, GetCustomerResponse>();
            #endregion
            #region Employee
            CreateMap<CreateEmployeeRequest, Employee>();    
            CreateMap<UpdateEmployeeRequest, Employee>();
            CreateMap<Employee, GetEmployeeResponse>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.EmployeeRole != null ? src.EmployeeRole.Name : null));
            CreateMap<EmployeeRole, GetEmployeeRoleResponse>();
            #endregion
            #region Storage
            CreateMap<Storage, StorageResponse>()
                .ForMember(d => d.BuildingCode, o => o.MapFrom(s => s.BuildingId))
                .ForMember(d => d.StorageTypeName, o => o.MapFrom(s => s.StorageType!.Name))
                .ForMember(d => d.ProductTypeName, o => o.MapFrom(s => s.ProductType!.Name));

            CreateMap<CreateStorageRequest, Storage>();
            CreateMap<UpdateStorageRequest, Storage>()
                .ForMember(d => d.StorageCode, o => o.Ignore());
            #endregion
            #region Order
            CreateMap<Order, OrderResponse>();
            CreateMap<CreateOrderRequest, Order>();
            CreateMap<UpdateOrderRequest, Order>()
                .ForMember(d => d.OrderCode, o => o.Ignore());
            CreateMap<UpdateOrderProcessRequest, TrackingHistory>();

            #endregion
            #region StorageType
            CreateMap<StorageType, StorageTypeResponse>();
            CreateMap<CreateStorageTypeRequest, StorageType>();
            CreateMap<UpdateStorageTypeRequest, StorageType>()
                .ForMember(d => d.StorageTypeId, o => o.Ignore());
            #endregion
            #region PaymentHistory
            CreateMap<CreatePaymentHistoryRequest, PaymentHistory>();

            CreateMap<UpdatePaymentHistoryRequest, PaymentHistory>()
                .ForMember(dest => dest.PaymentHistoryCode, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<PaymentHistory, PaymentHistoryResponse>();
            #endregion
            #region Shelftype
            CreateMap<ShelfType, GetShelfTypeResponse>();
            CreateMap<CreateShelfTypeRequest, ShelfType>();
            CreateMap<UpdateShelfTypeRequest, ShelfType>();
            #endregion
            // Pricing mappings
            CreateMap<Pricing, PricingResponse>();
            CreateMap<CreatePricingRequest, Pricing>();
            CreateMap<UpdatePricingRequest, Pricing>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.PricingId, opt => opt.Ignore());

            // ShippingRate mappings
            CreateMap<ShippingRate, ShippingRateResponse>()
                .ForMember(dest => dest.DistanceRangeDisplay, opt => opt.Ignore())
                .ForMember(dest => dest.ContainerQtyDisplay, opt => opt.Ignore());
            CreateMap<CreateShippingRateRequest, ShippingRate>();
            CreateMap<UpdateShippingRateRequest, ShippingRate>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ShippingRateId, opt => opt.Ignore());

            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.EmployeeRoleName,
                    opt => opt.MapFrom(src => src.EmployeeRole != null ? src.EmployeeRole.Name : null))
                .ForMember(dest => dest.BuildingName,
                    opt => opt.MapFrom(src => src.Building != null ? src.Building.Name : null));
            CreateMap<UpdateEmployeeRequest, Repositories.Entities.Employee>()
            .ForMember(dest => dest.EmployeeRoleId, opt => opt.MapFrom(src => src.EmployeeRoleId))
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EmployeeRole, opt => opt.Ignore())
            .ForMember(dest => dest.Building, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore());
        }


    }
}
