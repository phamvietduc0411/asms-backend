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
            CreateMap<WorkflowTemplate, WorkflowTemplateResponse>()
                .ForMember(dest => dest.StorageTypeName,
                    opt => opt.MapFrom(src => src.StorageType != null ? src.StorageType.Name : null));
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

            CreateMap<CreateServiceRequest, ASMS.Repositories.Entities.Service>()
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore());

            CreateMap<UpdateServiceRequest, ASMS.Repositories.Entities.Service>()
                .ForMember(dest => dest.ServiceId, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            #endregion
            #region Tracking History
            CreateMap<TrackingHistory, TrackingHistoryResponse>();

            CreateMap<CreateTrackingHistoryRequest, TrackingHistory>()
                .ForMember(d => d.TrackingHistoryId, o => o.Ignore())
                .ForMember(d => d.OrderCodeNavigation, o => o.Ignore()); 

            CreateMap<UpdateTrackingHistoryRequest, TrackingHistory>()
                .ForMember(d => d.TrackingHistoryId, o => o.Ignore())
                .ForMember(d => d.OrderCodeNavigation, o => o.Ignore()) 
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
            #endregion
            #region Building
            CreateMap<CreateBuildingRequest, Building>()
                .ForMember(b => b.BuildingCode, bl => bl.MapFrom(src => src.BuildingCode));
            CreateMap<UpdateBuildingRequest, Building>()
                .ForMember(b => b.BuildingCode, bl => bl.MapFrom(src => src.BuildingCode));
            #endregion
            #region ProductType
            CreateMap<CreateTypeRequest, ProductType>();
            CreateMap<UpdateTypeRequest, ProductType>();
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
            #endregion
            #region Container
            CreateMap<Container, ContainerResponse>()
                .ForMember(dest => dest.FloorStatus, opt => opt.MapFrom(src => src.FloorCodeNavigation != null ? src.FloorCodeNavigation.Status : null));

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
            #endregion
            #region Employee
            CreateMap<CreateEmployeeRequest, Employee>();    
            CreateMap<UpdateEmployeeRequest, Employee>();
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
        }


    }
}
