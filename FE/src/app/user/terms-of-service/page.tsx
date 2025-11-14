import React from 'react';
import { Metadata } from 'next';

export const metadata: Metadata = {
  title: 'Điều khoản dịch vụ - Truyện CV',
  description: 'Điều khoản và điều kiện sử dụng dịch vụ Truyện CV',
};

const TermsOfServicePage = () => {
  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-6">Điều khoản dịch vụ</h1>
      
      <section className="mb-8">
        <h2 className="text-2xl font-semibold mb-4">1. Giới thiệu</h2>
        <p className="mb-4">
          Chào mừng bạn đến với Truyện CV. Bằng cách truy cập hoặc sử dụng dịch vụ của chúng tôi, bạn đồng ý tuân thủ các điều khoản và điều kiện sau đây.
        </p>
      </section>

      <section className="mb-8">
        <h2 className="text-2xl font-semibold mb-4">2. Điều kiện sử dụng</h2>
        <ul className="list-disc pl-6 space-y-2">
          <li>Bạn phải từ đủ 13 tuổi trở lên để sử dụng dịch vụ của chúng tôi.</li>
          <li>Bạn chịu trách nhiệm bảo mật thông tin tài khoản của mình.</li>
          <li>Không được sử dụng dịch vụ cho mục đích bất hợp pháp hoặc trái phép.</li>
        </ul>
      </section>

      <section className="mb-8">
        <h2 className="text-2xl font-semibold mb-4">3. Quyền sở hữu trí tuệ</h2>
        <p className="mb-4">
          Tất cả nội dung trên trang web này, bao gồm nhưng không giới hạn ở văn bản, đồ họa, logo, hình ảnh, là tài sản của chúng tôi hoặc được cấp phép cho chúng tôi sử dụng.
        </p>
      </section>

      <section className="mb-8">
        <h2 className="text-2xl font-semibold mb-4">4. Giới hạn trách nhiệm</h2>
        <p className="mb-4">
          Chúng tôi không chịu trách nhiệm đối với bất kỳ thiệt hại trực tiếp, gián tiếp, ngẫu nhiên, đặc biệt hoặc hậu quả nào phát sinh từ việc sử dụng hoặc không thể sử dụng dịch vụ của chúng tôi.
        </p>
      </section>

      <section>
        <h2 className="text-2xl font-semibold mb-4">5. Thay đổi điều khoản</h2>
        <p>
          Chúng tôi có quyền cập nhật các điều khoản này vào bất kỳ lúc nào. Chúng tôi sẽ thông báo cho bạn về bất kỳ thay đổi nào bằng cách đăng các điều khoản mới trên trang web này.
        </p>
      </section>
    </div>
  );
};

export default TermsOfServicePage;
