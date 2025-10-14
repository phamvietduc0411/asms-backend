using ASMS.Repositories.Entities;
using ASMS.Services.Model;
using ASMS.Services.Model.Services;
using ASMS.Services.Model.StorageBlocks;
using ASMS.Services.Model.TrackingHistories;
using ASMS.Services.Model.WorkflowSteps;
using ASMS.Services.Model.WorkflowTemplates;
using ASMS.Services.Model.Building;
using ASMS.Services.Model.Role;
using ASMS.Services.Model;
using ASMS.Services.Model.Services;
using ASMS.Services.Model.StorageBlocks;
using ASMS.Services.Model.TrackingHistories;
using ASMS.Services.Model.WorkflowSteps;
using ASMS.Services.Model.WorkflowTemplates;
using AutoMapper;

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
            #region Storage Block
            CreateMap<StorageBlock, StorageBlockResponse>();

            CreateMap<CreateStorageBlockRequest, StorageBlock>()
                .ForMember(d => d.IsActive, o => o.Ignore())
                .ForMember(d => d.StorageCodeNavigation, o => o.Ignore());

            CreateMap<UpdateStorageBlockRequest, StorageBlock>()
                .ForMember(d => d.StorageBlockCode, o => o.Ignore())
                .ForMember(d => d.StorageCodeNavigation, o => o.Ignore())
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
            #region Building
            CreateMap<CreateBuildingRequest, Building>()
                .ForMember(b => b.BuildingCode, bl => bl.MapFrom(src => src.BuildingCode));
            CreateMap<UpdateBuildingRequest, Building>()
                .ForMember(b => b.BuildingCode, bl => bl.MapFrom(src => src.BuildingCode));
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
            #region Storage Block
            CreateMap<StorageBlock, StorageBlockResponse>();

            CreateMap<CreateStorageBlockRequest, StorageBlock>()
                .ForMember(d => d.IsActive, o => o.Ignore())
                .ForMember(d => d.StorageCodeNavigation, o => o.Ignore());

            CreateMap<UpdateStorageBlockRequest, StorageBlock>()
                .ForMember(d => d.StorageBlockCode, o => o.Ignore())
                .ForMember(d => d.StorageCodeNavigation, o => o.Ignore())
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
            #endregion
            #region Building
            CreateMap<CreateBuildingRequest, Building>()
                .ForMember(b => b.BuildingCode, bl => bl.MapFrom(src => src.BuildingCode));
            CreateMap<UpdateBuildingRequest, Building>()
                .ForMember(b => b.BuildingCode, bl => bl.MapFrom(src => src.BuildingCode));
            #endregion
        }


    }
}
