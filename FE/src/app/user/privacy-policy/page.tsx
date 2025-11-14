import React from 'react';
import { Metadata } from 'next';

export const metadata: Metadata = {
  title: 'Chính sách quyền riêng tư - Truyện CV',
  description: 'Chính sách bảo mật thông tin người dùng của Truyện CV',
};

const PrivacyPolicyPage = () => {
  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-6">Chính sách quyền riêng tư</h1>
      
      <section className="mb-8">
        <h2 className="text-2xl font-semibold mb-4">1. Thông tin chúng tôi thu thập</h2>
        <p className="mb-4">
          Chúng tôi có thể thu thập các loại thông tin sau đây khi bạn sử dụng dịch vụ của chúng tôi:
        </p>
        <ul className="list-disc pl-6 space-y-2">
          <li>Thông tin cá nhân như tên, địa chỉ email, ngày sinh, giới tính</li>
          <li>Thông tin đăng nhập như tên người dùng và mật khẩu</li>
          <li>Dữ liệu sử dụng như lịch sử đọc truyện, tìm kiếm và tương tác</li>
          <li>Thông tin thiết bị và kết nối như địa chỉ IP, loại trình duyệt</li>
        </ul>
      </section>

      <section className="mb-8">
        <h2 className="text-2xl font-semibold mb-4">2. Cách chúng tôi sử dụng thông tin</h2>
        <p className="mb-4">
          Chúng tôi sử dụng thông tin thu thập được cho các mục đích sau:
        </p>
        <ul className="list-disc pl-6 space-y-2">
          <li>Cung cấp, duy trì và cải thiện dịch vụ của chúng tôi</li>
          <li>Phân tích cách người dùng sử dụng dịch vụ của chúng tôi</li>
          <li>Giao tiếp với bạn về các cập nhật, bảo mật và thông báo tài khoản</li>
          <li>Phát hiện, ngăn chặn và giải quyết các vấn đề kỹ thuật hoặc lạm dụng</li>
        </ul>
      </section>

      <section className="mb-8">
        <h2 className="text-2xl font-semibold mb-4">3. Chia sẻ thông tin</h2>
        <p className="mb-4">
          Chúng tôi không bán hoặc cho thuê thông tin cá nhân của bạn cho bên thứ ba. Chúng tôi có thể chia sẻ thông tin trong các trường hợp sau:
        </p>
        <ul className="list-disc pl-6 space-y-2">
          <li>Với nhà cung cấp dịch vụ đáng tin cậy hỗ trợ hoạt động của chúng tôi</li>
          <li>Khi được yêu cầu bởi luật pháp hoặc quy định có hiệu lực</li>
          <li>Để bảo vệ quyền, tài sản hoặc an toàn của chúng tôi hoặc người khác</li>
        </ul>
      </section>

      <section className="mb-8">
        <h2 className="text-2xl font-semibold mb-4">4. Bảo mật thông tin</h2>
        <p className="mb-4">
          Chúng tôi thực hiện các biện pháp bảo mật phù hợp để bảo vệ thông tin của bạn khỏi truy cập, thay đổi, tiết lộ hoặc phá hủy trái phép.
        </p>
      </section>

      <section>
        <h2 className="text-2xl font-semibold mb-4">5. Quyền của bạn</h2>
        <p className="mb-4">Bạn có quyền:</p>
        <ul className="list-disc pl-6 space-y-2 mb-4">
          <li>Truy cập và nhận bản sao thông tin cá nhân của bạn</li>
          <li>Yêu cầu cập nhật hoặc xóa thông tin không chính xác</li>
          <li>Phản đối hoặc hạn chế xử lý thông tin của bạn</li>
          <li>Rút lại sự đồng ý đã cho phép xử lý dữ liệu</li>
        </ul>
        <p>
          Nếu bạn có bất kỳ câu hỏi nào về chính sách quyền riêng tư này, vui lòng liên hệ với chúng tôi qua email: support@truyencv.com
        </p>
      </section>
    </div>
  );
};

export default PrivacyPolicyPage;
