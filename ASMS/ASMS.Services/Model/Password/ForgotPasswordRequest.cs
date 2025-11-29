namespace ASMS.Services.Model.Password
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = null!;
        public bool IsEmployee { get; set; }
    }
}
