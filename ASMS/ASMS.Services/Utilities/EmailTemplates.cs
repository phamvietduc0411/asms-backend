using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Utilities
{
    public static class EmailTemplates
    {
        public static string ResetPassword(string email, string password, string senderEmail) => $@"
<html>
<body style='font-family:Arial,sans-serif;'>
    <div style='border:1px solid #ccc; padding:20px; border-radius:10px; max-width:600px; margin:auto;'>
        <h2 style='text-align:center; color:#4285F4;'>ASMS</h2>
        <p>Chào bạn,</p>
        <p>Bạn vừa yêu cầu <b>khôi phục mật khẩu</b> cho tài khoản của mình ({email}).</p>
        <p>Mật khẩu tạm thời của bạn là:</p>
        <p style='font-size:24px; text-align:center; font-weight:bold; margin:20px 0;'>{password}</p>
        <p>Mã này sẽ hết hạn sau 24 giờ. Vui lòng đổi mật khẩu ngay khi đăng nhập.</p>
        <p>Nếu bạn không yêu cầu việc này, vui lòng bỏ qua email này.</p>
        <p>Trân trọng,<br/>Đội ngũ ASMS ({senderEmail})</p>
    </div>
</body>
</html>";

        public static string NewAccount(string email, string password, string senderEmail) => $@"
<html>
<body style='font-family:Arial,sans-serif;'>
    <div style='border:1px solid #ccc; padding:20px; border-radius:10px; max-width:600px; margin:auto;'>
        <h2 style='text-align:center; color:#4285F4;'>ASMS</h2>
        <p>Chào bạn,</p>
        <p>Tài khoản mới đã được tạo cho bạn:</p>
        <ul>
            <li>Email: <b>{email}</b></li>
            <li>Mật khẩu: <b>{password}</b></li>
        </ul>
        <p>Vui lòng đăng nhập và đổi mật khẩu ngay để bảo mật.</p>
        <p>Trân trọng,<br/>Đội ngũ ASMS ({senderEmail})</p>
    </div>
</body>
</html>";
        public static string ForgotPassword(string email, string newPassword, string senderEmail) => $@"
<html>
<body style='font-family:Arial,sans-serif;'>
    <div style='border:1px solid #ccc; padding:20px; border-radius:10px; max-width:600px; margin:auto;'>
        <h2 style='text-align:center; color:#4285F4;'>ASMS</h2>
        <p>Chào bạn,</p>
        <p>Bạn vừa yêu cầu <b>khôi phục mật khẩu</b> cho tài khoản {email}.</p>
        <p>Mật khẩu mới của bạn là:</p>
        <p style='font-size:24px; text-align:center; font-weight:bold; margin:20px 0;'>{newPassword}</p>
        <p>Vui lòng đăng nhập và đổi mật khẩu ngay để bảo mật.</p>
        <p>Nếu bạn không yêu cầu việc này, vui lòng bỏ qua email này.</p>
        <p>Trân trọng,<br/>Đội ngũ ASMS ({senderEmail})</p>
    </div>
</body>
</html>";
    }

}
