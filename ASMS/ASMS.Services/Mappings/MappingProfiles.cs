using ASMS.Repositories.Entities;
using ASMS.Services.Model;
using AutoMapper;

namespace ASMS.Services.Mappings
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            #region Role
            CreateMap<CreateRoleRequest, EmployeeRole>().ForMember(e => e.EmployeeRoleId, opt => opt.Ignore());
            #endregion
        }
    }
}
