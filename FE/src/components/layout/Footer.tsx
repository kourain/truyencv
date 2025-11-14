import React from 'react';

const Footer = () => {
  return (
    <footer className="bg-gray-50 border-t border-gray-200 mt-12">
      <div className="container mx-auto px-4 py-6">
        <div className="text-center text-sm text-gray-600">
          <p className="mb-2">
            © {new Date().getFullYear()} Truyện CV. Tất cả các quyền được bảo lưu.
          </p>
          <div className="max-w-3xl mx-auto bg-yellow-50 border-l-4 border-yellow-400 p-4 mb-4">
            <p className="text-yellow-700">
              <strong>Chú ý:</strong> Đây chỉ là một đề tài đồ án tốt nghiệp của sinh viên chuyên ngành CNTT, 
              dữ liệu trang web chỉ mang mục đích trình diễn, không phải là trang web mang tính thương mại. 
              Bất kỳ vấn đề gì vui lòng liên hệ qua email: 
              <a href="mailto:ht.kourain@gmail.com" className="text-blue-600 hover:underline">
                ht.kourain@gmail.com
              </a>
            </p>
          </div>
          <div className="flex flex-wrap justify-center gap-4 mt-4">
            <a 
              href="/user/terms-of-service" 
              className="text-blue-600 hover:underline hover:text-blue-800"
            >
              Điều khoản dịch vụ
            </a>
            <span className="text-gray-400">•</span>
            <a 
              href="/user/privacy-policy" 
              className="text-blue-600 hover:underline hover:text-blue-800"
            >
              Chính sách bảo mật
            </a>
          </div>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
