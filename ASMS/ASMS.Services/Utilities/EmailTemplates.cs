using ASMS.Repositories.Entities;
using System.Text;

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

        public static string OrderInvoice(
    Order order,
    string senderEmail)
        {
            var detailRows = new StringBuilder();

            foreach (var d in order.OrderDetails)
            {
                detailRows.Append($@"
            <tr>
                <td>{d.ContainerCodeNavigation?.ContainerType?.Type}</td>
                <td>{d.ContainerQuantity}</td>
                <td>{d.Price:N0} đ</td>
                <td>{d.Quantity}</td>
                <td>{d.ShelfQuantity}</td>
                <td>{d.SubTotal:N0} đ</td>
            </tr>");
            }

            return $@"
<html>
<body style='font-family:Arial,sans-serif;'>
    <div style='border:1px solid #ccc; padding:20px; border-radius:10px; max-width:700px; margin:auto;'>

        <h2 style='text-align:center; color:#4285F4;'>ASMS - Xác nhận đơn hàng</h2>

        <p>Chào <b>{order.CustomerName}</b>,</p>
        <p>Đơn hàng <b>{order.OrderCode}</b> của bạn đã được tạo thành công.</p>

        <h3>📌 Thông tin đơn hàng</h3>
        <table style='width:100%; border-collapse:collapse;'>
            <tr><td><b>Mã đơn:</b></td><td>{order.OrderCode}</td></tr>
            <tr><td><b>Mã khách hàng:</b></td><td>{order.CustomerCode}</td></tr>
            <tr><td><b>Ngày tạo đơn:</b></td><td>{order.OrderDate:dd/MM/yyyy}</td></tr>
            <tr><td><b>Ngày gửi hàng:</b></td><td>{order.DepositDate:dd/MM/yyyy}</td></tr>
            <tr><td><b>Ngày trả hàng:</b></td><td>{order.ReturnDate:dd/MM/yyyy}</td></tr>
            <tr><td><b>Trạng thái thanh toán:</b></td><td>{order.PaymentStatus}</td></tr>
            <tr><td><b>Tổng tiền:</b></td><td>{order.TotalPrice:N0} đ</td></tr>
            <tr><td><b>Còn nợ:</b></td><td>{order.UnpaidAmount:N0} đ</td></tr>
            <tr><td><b>Ghi chú:</b></td><td>{order.Note}</td></tr>
            <tr><td><b>Địa chỉ:</b></td><td>{order.Address}</td></tr>
            <tr><td><b>Số điện thoại liên hệ:</b></td><td>{order.PhoneContact}</td></tr>
        </table>

        <h3 style='margin-top:20px;'>📦 Chi tiết sản phẩm</h3>

        <table style='width:100%; border-collapse:collapse;' border='1' cellpadding='8'>
            <tr style='background:#f1f1f1; font-weight:bold;'>
                <th>Loại container</th>
                <th>Số lượng container</th>
                <th>Giá</th>
                <th>Số lượng</th>
                <th>Số lượng kệ</th>
                <th>Tạm tính</th>
            </tr>
            {detailRows}
        </table>

        <h3 style='margin-top:20px;'>💰 Tổng cộng: {order.TotalPrice:N0} đ</h3>
        <p>Nếu còn nợ: <b>{order.UnpaidAmount:N0} đ</b></p>

        <p style='margin-top:25px;'>Nếu có thắc mắc hãy liên hệ với chúng tôi.</p>

        <p>Trân trọng,<br>ASMS Team ({senderEmail})</p>
    </div>
</body>
</html>";
        }

        public static string OrderPassKey(string orderCode, string passKey, string senderEmail) => $@"
<html>
<body style='font-family:Arial,sans-serif;'>
    <div style='border:1px solid #ccc; padding:20px; border-radius:10px; max-width:600px; margin:auto;'>
        <h2 style='text-align:center; color:#4285F4;'>ASMS - Mã truy cập Self Storage</h2>
        <p>Chào bạn,</p>
        <p>Đơn hàng <b>{orderCode}</b> của bạn đã sẵn sàng để sử dụng.</p>
        <p>Mã truy cập (PassKey) của bạn là:</p>
        <p style='font-size:32px; text-align:center; font-weight:bold; color:#4285F4; margin:20px 0; letter-spacing:5px;'>{passKey}</p>
        <p>Vui lòng sử dụng mã này để truy cập kho self-storage của bạn.</p>
        <p><b>Lưu ý:</b> Mã này chỉ có hiệu lực cho đơn hàng này và sẽ hết hiệu lực khi đơn hàng hoàn thành.</p>
        <p>Trân trọng,<br/>Đội ngũ ASMS ({senderEmail})</p>
    </div>
</body>
</html>";
    }
}
