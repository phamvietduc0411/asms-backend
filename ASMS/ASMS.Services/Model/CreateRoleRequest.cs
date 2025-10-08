namespace ASMS.Services.Model
{
    public class CreateRoleRequest
    {
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
    }
}
