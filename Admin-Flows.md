# Admin Flows (Quản trị)

Tài liệu này gom nhóm các luồng vận hành dành cho khối quản trị để dễ dựng slide thuyết trình và sơ đồ ca làm việc. Mỗi flow được mô tả bằng mục tiêu, actor, bảng swimlane và đầu ra cần log để có thể chuyển thành biểu đồ hoặc timeline chi tiết.

## Nhóm A – Bảo mật & truy cập khu vực admin

### Flow A1 – Đăng nhập admin & phân quyền toàn khu vực

#### A1.1 Mục tiêu chính

- Xác thực tài khoản quản trị và cấp JWT chứa claim `Admin`.
- Bảo vệ toàn bộ route `/admin/*` bằng middleware role-based.

#### A1.2 Actor chính

- FE Admin Portal.
- `AuthController` + middleware JWT.
- Identity store (user, role, refresh token).

#### A1.3 Chuỗi bước theo swimlane

| Bước | FE Admin Portal | API Gateway / AuthController | Services / DB |
| --- | --- | --- | --- |
| 1 | Admin mở trang đăng nhập và nhập thông tin. |  |  |
| 2 | Gửi `POST /auth/login`. | Xác thực credential, kiểm tra role Admin. | Identity store trả thông tin user + role. |
| 3 | Nhận JWT + refresh token chứa claim `role=Admin`, lưu vào storage. |  | Ghi log đăng nhập, phát hành refresh token. |
| 4 | Khi truy cập `/admin/*`, FE đính kèm JWT ở header. | Middleware `[Authorize(Roles = Admin)]` validate token. |  |
| 5 | Nếu JWT hợp lệ → cho phép truy cập; nếu không → trả 403 để FE điều hướng về login. |  | Ghi log truy cập thất bại nếu có. |

#### A1.4 Output & dữ liệu cần log

- Thành công/ thất bại đăng nhập, thời điểm, IP, user agent.
- Token phát hành kèm hạn, danh sách refresh token đang hoạt động.

#### A1.5 Gợi ý biểu đồ ca làm việc

- Swimlane 3 hàng (FE Admin, API Gateway, Identity Service) cho 5 bước trên; highlight điểm kiểm soát bảo mật.

## Nhóm B – Giám sát & tổng quan vận hành

### Flow A2 – Dashboard tổng quan

#### A2.1 Mục tiêu chính

- Chuẩn hóa pipeline lấy số liệu (user mới, truyện hot, báo lỗi, doanh thu).
- Tận dụng Redis cache để giảm tải DB cho widget KPI.

#### A2.2 Actor chính

- FE Dashboard `/admin/dashboard`.
- `DashboardController`.
- Reporting service + Redis cache.

#### A2.3 Chuỗi bước theo swimlane

| Bước | FE Dashboard | API Gateway / DashboardController | Reporting / Cache |
| --- | --- | --- | --- |
| 1 | Admin mở dashboard và gửi `GET /admin/dashboard/overview`. |  |  |
| 2 |  | Kiểm tra cache key tổng hợp. | Redis trả snapshot nếu còn hiệu lực. |
| 3 | Nếu cache miss, hiển thị trạng thái loading. |  | Reporting service truy vấn DB, gom KPI, cache kết quả. |
| 4 | FE render card KPI, bảng, biểu đồ dựa trên JSON trả về. |  |  |

#### A2.4 Output & dữ liệu cần log

- Thời gian phản hồi cho từng widget.
- Các KPI chính: tổng user, truyện hot, báo cáo lỗi, doanh thu.

#### A2.5 Gợi ý biểu đồ ca làm việc

- Biểu đồ dạng funnel thể hiện Data Source → Cache → API → Dashboard.

## Nhóm C – Quản lý nội dung & cộng đồng

### Flow A3 – CRUD truyện

#### A3.1 Mục tiêu chính

- Cho phép admin duyệt danh sách truyện, tạo mới, chỉnh sửa, xóa.
- Đồng bộ dữ liệu với cache hoặc chỉ mục tìm kiếm nếu cần.

#### A3.2 Actor chính

- FE module `/admin/comic`.
- `ComicController` trong area Admin.
- `ComicService`, `ComicRepository`, cache tìm kiếm.

#### A3.3 Chuỗi bước theo swimlane

| Bước | FE Admin Portal | API Admin / ComicController | Services / DB |
| --- | --- | --- | --- |
| 1 | Gửi `GET /admin/comic` kèm filter/pagination. |  | Repository chạy query `AsNoTracking`, trả danh sách + tổng. |
| 2 | FE render list, mở form tạo/sửa. |  |  |
| 3 | Submit `POST` hoặc `PUT /admin/comic` với payload chuẩn DTO. | Validate DTO, kiểm tra quyền, map sang entity. | Service lưu DB, cập nhật cache/search. |
| 4 | Xóa dùng `DELETE /admin/comic/{id}`. | Kiểm tra referential constraint. | Soft delete hoặc hard delete + invalidate cache. |
| 5 | FE nhận kết quả, cập nhật UI và log audit. |  |  |

#### A3.4 Output & dữ liệu cần log

- Thay đổi trạng thái truyện, người thực hiện, thời gian.
- Số lượng truyện tạo mới, đã ẩn, đã xóa.

#### A3.5 Gợi ý biểu đồ ca làm việc

- Timeline 4 giai đoạn: List → Form → Validate → Persist.

### Flow A4 – Xử lý báo lỗi truyện

#### A4.1 Mục tiêu chính

- Duyệt và cập nhật trạng thái các report từ user.
- Liên kết flow này với chỉnh sửa truyện/chương liên quan.

#### A4.2 Actor chính

- FE admin module “Báo lỗi”.
- `ComicReportController` (Admin).
- `ComicReportService` + DB report.

#### A4.3 Chuỗi bước theo swimlane

| Bước | FE Admin Portal | API Admin / ComicReportController | Services / DB |
| --- | --- | --- | --- |
| 1 | Gửi `GET /admin/comic-report?status=pending`. |  | Service query danh sách report + thông tin truyện/chương. |
| 2 | Admin chọn report, mở chi tiết. |  |  |
| 3 | Khi cập nhật trạng thái: `PUT /admin/comic-report/{id}` với action (resolve/edit/reject). | Validate action, map sang enum trạng thái. | Ghi DB, trigger cập nhật truyện/chương nếu cần. |
| 4 | FE phản hồi UI, log kết quả để phục vụ audit. |  |  |

#### A4.4 Output & dữ liệu cần log

- Số report pending/đã xử lý, loại lỗi phổ biến.
- Liên kết audit giữa report và hành động sửa chữa.

#### A4.5 Gợi ý biểu đồ ca làm việc

- Biểu đồ Sankey thể hiện Report → Action (Resolve/Reject/Edit).

### Flow A5 – Quản lý đề cử & xếp hạng

#### A5.1 Mục tiêu chính

- Thống kê lượt đề cử từ user và xây bảng xếp hạng.
- Cho phép lọc theo tổng, tháng, năm.

#### A5.2 Actor chính

- FE admin module Ranking.
- `RecommendationController` (Admin).
- Reporting service trên bảng đề cử.

#### A5.3 Chuỗi bước theo swimlane

| Bước | FE Admin Portal | API Admin / RecommendationController | Services / DB |
| --- | --- | --- | --- |
| 1 | FE gửi `GET /admin/recommendation/ranking?range=monthly`. |  | Service tổng hợp lượt đề cử theo range. |
| 2 | API chuẩn hóa response: thứ hạng, truyện, chỉ số đề cử. |  | Redis cache ranking theo range. |
| 3 | FE render bảng, biểu đồ top N, cho phép export. |  |  |

#### A5.4 Output & dữ liệu cần log

- Thứ hạng từng truyện theo range.
- Sai biệt so với kỳ trước (delta đề cử).

#### A5.5 Gợi ý biểu đồ ca làm việc

- Bar chart hoặc line chart theo tuần/tháng; swimlane FE–API–Analytics để thuyết trình pipeline dữ liệu.

## Nhóm D – Quản trị người dùng & phiên

### Flow A6 – Quản lý user & refresh token

#### A6.1 Mục tiêu chính

- Giám sát danh sách user, chi tiết hồ sơ.
- Thu hồi từng refresh token hoặc toàn bộ phiên của user.

#### A6.2 Actor chính

- FE module `/admin/user`.
- `AdminUserController`.
- `UserService`, `RefreshTokenService`.

#### A6.3 Chuỗi bước theo swimlane

| Bước | FE Admin Portal | API Admin / UserController | Services / DB |
| --- | --- | --- | --- |
| 1 | `GET /admin/user` lấy danh sách phân trang. |  | Repository query user + filter. |
| 2 | `GET /admin/user/{id}` xem chi tiết. |  | Service trả hồ sơ + thống kê. |
| 3 | `GET /admin/user/{id}/refresh-tokens` xem phiên. |  | RefreshToken repo trả danh sách. |
| 4 | Thu hồi: gọi `DELETE /admin/user/{id}/refresh-tokens/{tokenId}` hoặc `DELETE /admin/user/{id}/refresh-tokens`. | Validate quyền, đánh dấu token revoked. | DB cập nhật trạng thái revoke, log audit. |
| 5 | FE cập nhật UI, hiển thị thông báo thành công/thất bại. |  |  |

#### A6.4 Output & dữ liệu cần log

- Số lượng phiên hiện tại, thời điểm tạo token.
- Lịch sử admin can thiệp phiên user.

#### A6.5 Gợi ý biểu đồ ca làm việc

- Sequence diagram thể hiện Admin → API → Token Store với nhánh revoke từng phần / toàn phần.

## Nhóm E – Tài chính & doanh thu

### Flow A7 – Quản lý thanh toán & báo cáo doanh thu

#### A7.1 Mục tiêu chính

- Giám sát giao dịch theo user và tổng quan doanh thu.
- Xuất dữ liệu cho bộ phận tài chính hoặc kế toán.

#### A7.2 Actor chính

- FE module `/admin/payment-history`.
- `PaymentHistoryController`.
- `PaymentService`, báo cáo doanh thu.

#### A7.3 Chuỗi bước theo swimlane

| Bước | FE Admin Portal | API Admin / PaymentHistoryController | Services / DB |
| --- | --- | --- | --- |
| 1 | Request danh sách giao dịch: `GET /admin/payment-history` kèm filter. |  | Service truy vấn bảng giao dịch. |
| 2 | Xem theo user: `GET /admin/payment-history/by-user/{userId}`. |  | Repo trả giao dịch + số dư hiện tại. |
| 3 | Lấy summary: `GET /admin/payment-history/revenue/summary?days=N`. | Tổng hợp doanh thu, gom theo ngày. | Reporting service đọc DB, cache nếu cần. |
| 4 | FE render bảng + biểu đồ line/bar, hỗ trợ export CSV. |  |  |

#### A7.4 Output & dữ liệu cần log

- Tổng doanh thu theo ngày/tháng, số giao dịch thành công/failed.
- Audit trail khi admin xuất/điều chỉnh giao dịch.

#### A7.5 Gợi ý biểu đồ ca làm việc

- Biểu đồ kết hợp line (doanh thu) + stacked bar (loại giao dịch); swimlane nêu rõ 3 actor: FE, API, Revenue Service.

## Nhóm F – Taxonomy & tagging truyện

### Flow A8 – Quản lý danh mục thể loại

#### A8.1 Mục tiêu chính

- Duy trì kho danh mục chuẩn hóa để Converter và User có thể lọc truyện.
- Cho phép admin tạo, đổi tên, phân loại Genre/Tag, xóa hoặc khôi phục.

#### A8.2 Actor chính

- FE module `/admin/comic-category`.
- `ComicCategoryController` (Admin area).
- Category service + Redis cache danh mục.

#### A8.3 Chuỗi bước theo swimlane

| Bước | FE Admin Portal | API Admin / ComicCategoryController | Services / DB |
| --- | --- | --- | --- |
| 1 | Gửi `GET /admin/ComicCategory?offset&limit`. |  | Repository trả danh sách + phân trang. |
| 2 | Mở form tạo/sửa, nhập name + `category_type`. |  |  |
| 3 | Submit `POST /admin/ComicCategory` hoặc `PUT /admin/ComicCategory/{id}`. | Validate trùng tên, map DTO → entity. | Service lưu DB, cập nhật cache. |
| 4 | Xóa: `DELETE /admin/ComicCategory/{id}`. | Kiểm tra ràng buộc truyện liên quan. | Soft delete + ghi audit. |
| 5 | FE refresh list, hiển thị toast + trạng thái cache/mock. |  |  |

#### A8.4 Output & dữ liệu cần log

- Tổng số danh mục theo loại, danh sách thay đổi gần nhất.
- Log người thao tác, trường cũ/mới để phục vụ rollback.

#### A8.5 Gợi ý biểu đồ ca làm việc

- Lưu đồ CRUD đơn giản với hai cổng quyết định (validate trùng, kiểm tra liên kết truyện).

### Flow A9 – Gán truyện vào danh mục

#### A9.1 Mục tiêu chính

- Liên kết truyện với nhiều thể loại để cải thiện tìm kiếm và đề xuất.
- Đảm bảo thao tác add/remove đồng bộ với Converter workspace.

#### A9.2 Actor chính

- FE trang chi tiết truyện trong Admin.
- `ComicHaveCategoryController`.
- Service mapping truyện-thể loại.

#### A9.3 Chuỗi bước theo swimlane

| Bước | FE Admin Portal | API Admin / ComicHaveCategoryController | Services / DB |
| --- | --- | --- | --- |
| 1 | Gửi `GET /admin/ComicHaveCategory/comic/{comicId}/categories`. |  | Service trả danh mục đang gán. |
| 2 | Admin chọn thêm/bớt thể loại. |  |  |
| 3 | Thêm: `POST /admin/ComicHaveCategory` với `{ comic_id, category_id }`. | Validate trùng lặp. | Lưu DB, cập nhật search index. |
| 4 | Gỡ: `DELETE /admin/ComicHaveCategory/comic/{comicId}/category/{categoryId}`. | Kiểm tra tồn tại record. | Xóa record, invalidate cache. |
| 5 | FE cập nhật UI, hiển thị tổng số thể loại/ comic. |  |  |

#### A9.4 Output & dữ liệu cần log

- Số lượng thể loại trên từng truyện, lịch sử thay đổi.
- Sai lệch giữa Converter đề xuất và Admin phê duyệt.

#### A9.5 Gợi ý biểu đồ ca làm việc

- Biểu đồ hai chiều Comic ↔ Category, highlight bước kiểm tra trùng.

## Nhóm G – Quản lý chương & nội dung chi tiết

### Flow A10 – CRUD chương truyện

#### A10.1 Mục tiêu chính

- Cho phép admin tạo chương mới cho truyện Converter gửi lên hoặc chỉnh sửa nội dung lỗi.
- Đảm bảo thứ tự chapter và trạng thái phát hành luôn chính xác.

#### A10.2 Actor chính

- FE module `/admin/comic-chapter`.
- `ComicChapterController`.
- Chapter service + storage nội dung.

#### A10.3 Chuỗi bước theo swimlane

| Bước | FE Admin Portal | API Admin / ComicChapterController | Services / DB |
| --- | --- | --- | --- |
| 1 | `GET /admin/ComicChapter/comic/{comicId}` lấy danh sách chương. |  | Repo trả list + metadata. |
| 2 | Mở form tạo/sửa, nhập tiêu đề, số chương, nội dung. |  |  |
| 3 | `POST /admin/ComicChapter` hoặc `PUT /admin/ComicChapter/{id}`. | Validate trùng số chương, chuẩn hóa nội dung. | Lưu DB + cập nhật search vector. |
| 4 | Xóa: `DELETE /admin/ComicChapter/{id}`. | Kiểm tra quyền, ràng buộc comment. | Soft delete, cập nhật trạng thái truyện. |
| 5 | FE cập nhật timeline chương, thông báo Converter nếu cần. |  |  |

#### A10.4 Output & dữ liệu cần log

- Số chương mỗi ngày, chương bị sửa/ẩn, người xử lý.
- Chỉ số thứ tự bị trùng, thời gian hoàn thành cập nhật.

#### A10.5 Gợi ý biểu đồ ca làm việc

- Swimlane 4 bước với điều kiện duy nhất “ChapterNumber unique”.

### Flow A11 – Moderation bình luận truyện/chương

#### A11.1 Mục tiêu chính

- Tập trung các comment xuyên suốt truyện, chương, user để kiểm duyệt nhanh.
- Hỗ trợ ẩn/xóa comment vi phạm và trả lời chính thức nếu cần.

#### A11.2 Actor chính

- FE module `/admin/comic-comment`.
- `ComicCommentController`.
- Comment service + notification.

#### A11.3 Chuỗi bước theo swimlane

| Bước | FE Admin Portal | API Admin / ComicCommentController | Services / DB |
| --- | --- | --- | --- |
| 1 | Gửi `GET /admin/ComicComment/{type}/{id}` (comic/chapter/user). |  | Repo trả list, paginate client-side. |
| 2 | Admin duyệt từng comment, chọn hành động. |  |  |
| 3 | Cập nhật: `PUT /admin/ComicComment/{id}` chỉnh nội dung/trạng thái. | Validate quyền, kiểm tra link ảnh. | Lưu DB, đẩy notification nếu cần. |
| 4 | Xóa: `DELETE /admin/ComicComment/{id}`. |  | Ghi log moderation. |
| 5 | FE refresh danh sách, highlight comment bị xử lý. |  |  |

#### A11.4 Output & dữ liệu cần log

- Tổng số comment đã xử lý theo ngày, tỷ lệ vi phạm.
- Nội dung trước/sau khi chỉnh sửa để audit.

#### A11.5 Gợi ý biểu đồ ca làm việc

- Sankey Comment → Action (Approve/Edit/Delete) tương tự flow report.

## Nhóm H – Phân quyền & hồ sơ quản trị

### Flow A12 – Quản lý vai trò và gán quyền

#### A12.1 Mục tiêu chính

- Kiểm soát vai trò hệ thống (Admin, Moderator, Accountant…).
- Cho phép thiết lập vai trò cho từng user và thu hồi khi cần.

#### A12.2 Actor chính

- FE module `/admin/user-role` và trang chi tiết user.
- `UserHasRoleController` + `AdminUserController`.
- Role service + permission matrix.

#### A12.3 Chuỗi bước theo swimlane

| Bước | FE Admin Portal | API Admin / UserHasRoleController | Services / DB |
| --- | --- | --- | --- |
| 1 | `GET /admin/UserHasRole?offset&limit` lấy danh sách role-user. |  | Repo join user + role. |
| 2 | Tạo role mới hoặc gán role cho user. |  |  |
| 3 | `POST /admin/UserHasRole` hoặc `PUT /admin/UserHasRole/{id}`. | Validate role hợp lệ, không trùng. | Lưu DB, cập nhật cache permission. |
| 4 | `DELETE /admin/UserHasRole/{id}` nếu thu hồi. |  | Ghi audit + revoke session nếu chính sách yêu cầu. |
| 5 | FE đồng bộ lại bảng role, hiển thị badge trên hồ sơ user. |  |  |

#### A12.4 Output & dữ liệu cần log

- Danh sách role hiện hành, user thuộc từng role.
- Lịch sử thêm/xóa role để truy vết trách nhiệm.

#### A12.5 Gợi ý biểu đồ ca làm việc

- Biểu đồ phân rã Role → User, highlight nhánh revoke và refresh permission.

### Flow A13 – Rà soát hồ sơ & bảo mật admin

#### A13.1 Mục tiêu chính

- Cho phép admin xem nhanh hồ sơ cá nhân, trạng thái bảo mật và cảnh báo thiết bị lạ.
- Kết hợp với flow A6 để cưỡng ép đăng xuất nếu phát hiện rủi ro.

#### A13.2 Actor chính

- FE trang `/admin/profile` (sử dụng `/auth/me`).
- `AuthController` + middleware JWT.
- User profile service + RefreshToken service.

#### A13.3 Chuỗi bước theo swimlane

| Bước | FE Admin Portal | API Auth / Profile | Services / DB |
| --- | --- | --- | --- |
| 1 | Mở trang profile → `GET /auth/me`. |  | Service trả thông tin user, role, thời gian tạo tài khoản. |
| 2 | FE hiển thị thông tin + cảnh báo (2FA, thiết bị). |  |  |
| 3 | Nếu phát hiện thiết bị lạ, điều hướng sang flow A6 để revoke token. |  | RefreshToken service xử lý revoke. |
| 4 | FE log hành động vào bảng activity để báo cáo bảo mật. |  |  |

#### A13.4 Output & dữ liệu cần log

- Thời điểm admin kiểm tra hồ sơ, số lần phát hiện rủi ro.
- Liên kết giữa profile check và hành động revoke.

#### A13.5 Gợi ý biểu đồ ca làm việc

- Sequence FE Profile → Auth → Token Store, nhấn mạnh vòng lặp kiểm tra định kỳ.
