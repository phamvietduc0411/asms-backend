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

        public static string OrderInvoice(Order order, string senderEmail)
        {

            bool hasContainerType = order.OrderDetails.Any(d => d.ContainerTypeNavigation != null);
            bool hasContainerQuantity = order.OrderDetails.Any(d => d.ContainerQuantity.HasValue && d.ContainerQuantity > 0);
            bool hasShelfType = order.OrderDetails.Any(d => d.ShelfTypeNavigation != null);
            bool hasShelfQuantity = order.OrderDetails.Any(d => d.ShelfQuantity.HasValue && d.ShelfQuantity > 0);
            bool hasStorageType = order.OrderDetails.Any(d => d.StorageTypeNavigation != null);
            bool hasPrice = order.OrderDetails.Any(d => d.Price.HasValue && d.Price > 0);
            bool hasQuantity = order.OrderDetails.Any(d => !string.IsNullOrEmpty(d.Quantity));
            bool hasSubTotal = order.OrderDetails.Any(d => d.SubTotal.HasValue && d.SubTotal > 0);

            var headerCells = new StringBuilder();
            if (hasContainerType) headerCells.Append("<th>Loại container</th>");
            if (hasContainerQuantity) headerCells.Append("<th>SL container</th>");
            if (hasShelfType) headerCells.Append("<th>Loại kệ</th>");
            if (hasShelfQuantity) headerCells.Append("<th>SL kệ</th>");
            if (hasStorageType) headerCells.Append("<th>Loại kho</th>");
            if (hasPrice) headerCells.Append("<th>Giá</th>");
            if (hasQuantity) headerCells.Append("<th>Số lượng</th>");
            if (hasSubTotal) headerCells.Append("<th>Tạm tính</th>");

            var detailRows = new StringBuilder();
            foreach (var d in order.OrderDetails)
            {
                var cells = new StringBuilder();

                if (hasContainerType)
                {
                    var value = d.ContainerTypeNavigation?.Type ?? "";
                    cells.Append($"<td>{value}</td>");
                }

                if (hasContainerQuantity)
                {
                    var value = d.ContainerQuantity ?? 0;
                    cells.Append($"<td>{value}</td>");
                }

                if (hasShelfType)
                {
                    var value = d.ShelfTypeNavigation?.Name ?? "";
                    cells.Append($"<td>{value}</td>");
                }

                if (hasShelfQuantity)
                {
                    var value = d.ShelfQuantity ?? 0;
                    cells.Append($"<td>{value}</td>");
                }

                if (hasStorageType)
                {
                    var value = d.StorageTypeNavigation?.Name ?? "";
                    cells.Append($"<td>{value}</td>");
                }

                if (hasPrice)
                {
                    var value = d.Price ?? 0;
                    cells.Append($"<td>{value:N0} đ</td>");
                }

                if (hasQuantity)
                {
                    var value = d.Quantity ?? "";
                    cells.Append($"<td>{value}</td>");
                }

                if (hasSubTotal)
                {
                    var value = d.SubTotal ?? 0;
                    cells.Append($"<td>{value:N0} đ</td>");
                }

                detailRows.Append($"<tr>{cells}</tr>");
            }


            return $@"
<html>
<body style='font-family:Arial,sans-serif;'>
    <div style='border:1px solid #ccc; padding:20px; border-radius:10px; max-width:900px; margin:auto;'>
        <h2 style='text-align:center; color:#4285F4;'>ASMS - Xác nhận đơn hàng</h2>
        <p>Chào <b>{order.CustomerName ?? "Quý khách"}</b>,</p>
        <p>Đơn hàng <b>{order.OrderCode}</b> của bạn đã được tạo thành công.</p>
        
        <h3>📌 Thông tin đơn hàng</h3>
        <table style='width:100%; border-collapse:collapse;'>
            <tr><td style='width:40%;'><b>Mã đơn:</b></td><td>{order.OrderCode}</td></tr>
            <tr><td><b>Mã khách hàng:</b></td><td>{order.CustomerCode ?? ""}</td></tr>
            <tr><td><b>Ngày tạo đơn:</b></td><td>{order.OrderDate?.ToString("dd/MM/yyyy") ?? ""}</td></tr>
            <tr><td><b>Ngày gửi hàng:</b></td><td>{order.DepositDate?.ToString("dd/MM/yyyy") ?? ""}</td></tr>
            <tr><td><b>Ngày trả hàng:</b></td><td>{order.ReturnDate?.ToString("dd/MM/yyyy") ?? ""}</td></tr>
            <tr><td><b>Trạng thái thanh toán:</b></td><td>{order.PaymentStatus ?? ""}</td></tr>
            <tr><td><b>Tổng tiền:</b></td><td>{order.TotalPrice:N0} đ</td></tr>
            <tr><td><b>Còn nợ:</b></td><td>{order.UnpaidAmount:N0} đ</td></tr>
            {(string.IsNullOrEmpty(order.Note) ? "" : $"<tr><td><b>Ghi chú:</b></td><td>{order.Note}</td></tr>")}
            {(string.IsNullOrEmpty(order.Address) ? "" : $"<tr><td><b>Địa chỉ:</b></td><td>{order.Address}</td></tr>")}
            {(string.IsNullOrEmpty(order.PhoneContact) ? "" : $"<tr><td><b>Số điện thoại liên hệ:</b></td><td>{order.PhoneContact}</td></tr>")}
        </table>
        
        <h3 style='margin-top:20px;'>📦 Chi tiết sản phẩm</h3>
        <table style='width:100%; border-collapse:collapse;' border='1' cellpadding='8'>
            <tr style='background:#f1f1f1; font-weight:bold;'>
                {headerCells}
            </tr>
            {detailRows}
        </table>
        
        <h3 style='margin-top:20px;'>💰 Tổng cộng: {order.TotalPrice:N0} đ</h3>
        {(order.UnpaidAmount > 0 ? $"<p>Còn nợ: <b>{order.UnpaidAmount:N0} đ</b></p>" : "")}
        
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
        /// <summary>
        /// Email thông báo khi khách hàng gửi yêu cầu refund/báo hư hại
        /// </summary>
        public static string ContactRefundRequest(string customerName, string message, string? orderCode, List<string>? imageUrls, string supportEmail)
        {
            var orderInfo = !string.IsNullOrEmpty(orderCode)
                ? $"<p><strong>Mã đơn hàng:</strong> {orderCode}</p>"
                : "";

            var imagesHtml = "";
            if (imageUrls != null && imageUrls.Any())
            {
                imagesHtml = @"
            <div style='margin: 15px 0;'>
                <p><strong>Hình ảnh minh chứng:</strong></p>
                <div style='display: flex; flex-wrap: wrap; gap: 10px;'>";

                foreach (var imageUrl in imageUrls)
                {
                    imagesHtml += $@"
                    <div style='width: 150px; height: 150px; border: 1px solid #ddd; overflow: hidden;'>
                        <img src='{imageUrl}' alt='Evidence' style='width: 100%; height: 100%; object-fit: cover;'>
                    </div>";
                }

                imagesHtml += @"
                </div>
            </div>";
            }

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #f44336; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9f9f9; padding: 20px; margin: 20px 0; }}
        .alert-box {{ background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 15px 0; }}
        .message-box {{ background-color: white; border-left: 4px solid #f44336; padding: 15px; margin: 15px 0; }}
        .info-box {{ background-color: #e3f2fd; border-left: 4px solid #2196F3; padding: 15px; margin: 15px 0; }}
        .footer {{ text-align: center; color: #666; font-size: 12px; margin-top: 20px; }}
        .btn {{ display: inline-block; padding: 10px 20px; background-color: #f44336; color: white; text-decoration: none; border-radius: 5px; margin-top: 15px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>⚠️ Yêu cầu Refund/Báo hư hại</h1>
        </div>
        
        <div class='content'>
            <p>Xin chào <strong>{customerName}</strong>,</p>
            
            <p>Chúng tôi đã nhận được yêu cầu refund/báo hư hại của bạn. Đội ngũ VStorage ASMS sẽ kiểm tra và xử lý trong vòng <strong>24-48 giờ</strong>.</p>
            
            {orderInfo}
            
            <div class='message-box'>
                <p><strong>Nội dung báo cáo:</strong></p>
                <p>{message}</p>
            </div>
            
            {imagesHtml}
            
            <div class='info-box'>
                <p><strong>Quy trình xử lý:</strong></p>
                <ol>
                    <li>Đội ngũ kỹ thuật sẽ kiểm tra hình ảnh và nội dung báo cáo</li>
                    <li>Xác định mức độ hư hại và trách nhiệm</li>
                    <li>Tính toán số tiền refund (nếu có)</li>
                    <li>Thông báo kết quả và tiến hành hoàn tiền</li>
                </ol>
            </div>
            
            <div class='alert-box'>
                <p><strong>Lưu ý:</strong> Vui lòng giữ nguyên hiện trạng hàng hóa cho đến khi có nhân viên đến kiểm tra (nếu cần thiết).</p>
            </div>
            
            <p>Mọi thắc mắc vui lòng liên hệ: <a href='mailto:{supportEmail}'>{supportEmail}</a></p>
            
            <p>Trân trọng,<br>
            <strong>VStorage ASMS Support Team</strong></p>
        </div>
        
        <div class='footer'>
            <p>Email này được gửi tự động. Vui lòng không trả lời email này.</p>
            <p>&copy; 2025 VStorage ASMS. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
