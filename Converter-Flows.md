# Converter Flows (Converter Workspace)

Tài liệu này mô tả các luồng nghiệp vụ dành cho cộng tác viên/chủ converter. Cấu trúc tương tự Admin/User để dễ chuyển thành slide hoặc biểu đồ swimlane.

## Nhóm C – Truy cập & tổng quan workspace

### Flow C1 – Đăng nhập & điều hướng vào Converter workspace

#### C1.1 Mục tiêu chính

- Đảm bảo chỉ user có role Converter truy cập được `/converter/*`.
- Đồng bộ số dư coin/key hiển thị trong dashboard.

#### C1.2 Actor chính

- FE layout `/converter`.
- `AuthController` (`/auth/login`, `/auth/me`).
- Role middleware, profile service.

#### C1.3 Chuỗi bước theo swimlane

| Bước | FE Converter | Auth / Router | Services / DB |
| --- | --- | --- | --- |
| 1 | User đăng nhập chung (flow U1). |  |  |
| 2 | Khi truy cập `/converter`, FE gọi `GET /auth/me`. | Middleware kiểm tra JWT + role Converter. |  |
| 3 | Nếu hợp lệ, API trả hồ sơ (name, coin, key). |  | Profile service đọc DB. |
| 4 | FE render ConverterDashboard, hiển thị số dư và shortcut chức năng. |  |  |
| 5 | Nếu thiếu role, middleware trả 403 → FE điều hướng về trang user mặc định. |  |  |

#### C1.4 Output & dữ liệu cần log

- Số phiên Converter đang hoạt động, lỗi 403 vì thiếu role.
- Lần cuối user truy cập workspace để đánh giá độ gắn bó.

#### C1.5 Gợi ý biểu đồ ca làm việc

- Sequence FE → Auth → Profile với nhánh lỗi “Role mismatch”.

## Nhóm D – Vòng đời truyện & chương

### Flow C2 – CRUD truyện & gán thể loại

#### C2.1 Mục tiêu chính

- Cho phép converter theo dõi danh sách truyện của họ, tạo mới, chỉnh sửa, xóa.
- Chọn thể loại phù hợp từ danh mục chuẩn (đồng bộ với Admin Flow A8/A9).

#### C2.2 Actor chính

- FE module `/converter/comics`.
- `ConverterComicController` (`/Converter/Comic`).
- `ConverterComicCategoryController` (`/converter/ComicCategory/all`).

#### C2.3 Chuỗi bước theo swimlane

| Bước | FE Converter | Converter Comic API | Services / DB |
| --- | --- | --- | --- |
| 1 | FE gọi `GET /Converter/Comic?offset&limit` để lấy danh sách. |  | Repo trả truyện thuộc converter hiện tại. |
| 2 | FE song song gọi `GET /converter/ComicCategory/all` để render dropdown thể loại. |  | Category service trả danh sách cache. |
| 3 | Khi tạo mới, gửi `POST /Converter/Comic` kèm metadata + danh sách category_id. | Validate dữ liệu, kiểm tra giới hạn số truyện. | Lưu DB, map converter_id, cập nhật search index. |
| 4 | Khi chỉnh sửa, gửi `PUT /Converter/Comic/{id}` hoặc `DELETE /Converter/Comic/{id}`. | Xác nhận quyền sở hữu truyện, log thay đổi. | Update DB, cascade liên kết category. |
| 5 | FE invalidates query, cập nhật bảng truyện và thông báo trạng thái duyệt. |  |  |

#### C2.4 Output & dữ liệu cần log

- Số truyện hoạt động theo converter, thời gian duyệt gần nhất.
- Nhật ký thay đổi metadata, thể loại.

#### C2.5 Gợi ý biểu đồ ca làm việc

- Timeline 5 bước với callout “Fetch categories song song với danh sách truyện”.

### Flow C3 – Quản lý chương truyện

#### C3.1 Mục tiêu chính

- Converter nhập ID truyện để xem và thao tác chương.
- Đảm bảo invalidation chính xác theo `comic_id` để dữ liệu luôn mới.

#### C3.2 Actor chính

- FE `/converter/chapters`.
- `ConverterComicChapterController`.
- Chapter service + notification đến admin khi có chương mới.

#### C3.3 Chuỗi bước theo swimlane

| Bước | FE Converter | Converter Chapter API | Services / DB |
| --- | --- | --- | --- |
| 1 | Converter nhập `comic_id`, FE gửi `GET /Converter/ComicChapter/comic/{comicId}`. |  | Repo trả danh sách chương thuộc converter. |
| 2 | FE hiển thị bảng, cho phép mở modal tạo/sửa. |  |  |
| 3 | Khi lưu, gửi `POST` hoặc `PUT /Converter/ComicChapter/{id}`. | Validate chapter_number, độ dài nội dung. | Lưu DB, cập nhật search vector, log lịch sử phiên bản. |
| 4 | Xóa chương dùng `DELETE /Converter/ComicChapter/{id}`. | Kiểm tra quyền sở hữu, chapter đã phát hành chưa. | Soft delete + đánh dấu cần duyệt lại. |
| 5 | FE refetch query `['converter','chapters',comicId]`, cập nhật UI và snackbar kết quả. |  |  |

#### C3.4 Output & dữ liệu cần log

- Số chương tạo/sửa/xóa mỗi ngày theo converter.
- Chapter nào đang chờ admin duyệt sau khi cập nhật.

#### C3.5 Gợi ý biểu đồ ca làm việc

- Swimlane nêu rõ dây chuyền từ nhập `comic_id` đến invalidation query.

## Nhóm E – Cộng đồng & phản hồi

### Flow C4 – Kiểm duyệt bình luận trên truyện riêng

#### C4.1 Mục tiêu chính

- Cho converter xem toàn bộ bình luận liên quan tới truyện của họ và xóa nếu vi phạm.
- Giảm tải cho team admin bằng cách xử lý bước đầu tại nguồn.

#### C4.2 Actor chính

- FE `/converter/comments` (lọc theo comic_id).
- `ConverterComicCommentController`.
- Comment moderation service.

#### C4.3 Chuỗi bước theo swimlane

| Bước | FE Converter | Converter Comment API | Services / DB |
| --- | --- | --- | --- |
| 1 | FE gửi `GET /Converter/ComicComment/comic/{comicId}` sau khi user nhập ID. |  | Repo trả list comment thuộc converter. |
| 2 | Converter đọc nội dung, chọn “Xóa” nếu vi phạm. |  |  |
| 3 | FE gọi `DELETE /Converter/ComicComment/{commentId}`. | Xác thực comment thuộc comic của converter. | Soft delete, log lý do + người thao tác. |
| 4 | FE refetch comments, cập nhật bảng và thống kê số comment đã xử lý. |  |  |

#### C4.4 Output & dữ liệu cần log

- Số comment bị xóa, lý do phổ biến.
- Tỷ lệ comment cần escalate lên admin.

#### C4.5 Gợi ý biểu đồ ca làm việc

- Sankey Comment → Action (Giữ lại / Xóa / Escalate) nhấn mạnh quyền tự chủ của converter.

### Flow C5 – Theo dõi báo cáo từ độc giả

#### C5.1 Mục tiêu chính

- Converter giám sát report liên quan tới truyện của mình để phối hợp với Admin nhanh chóng.
- Hỗ trợ lọc theo trạng thái pending/resolved/rejected.

#### C5.2 Actor chính

- FE `/converter/reports`.
- `ConverterComicReportController`.
- Report service + notification.

#### C5.3 Chuỗi bước theo swimlane

| Bước | FE Converter | Converter Report API | Services / DB |
| --- | --- | --- | --- |
| 1 | FE gửi `GET /Converter/ComicReport?status=pending\|resolved\|...`. |  | Service trả report thuộc converter, phân trang. |
| 2 | Converter mở chi tiết để hiểu lỗi. |  |  |
| 3 | Nếu cần cập nhật nội dung/chương, chuyển sang Flow C3 để sửa. |  |  |
| 4 | Khi admin thay đổi trạng thái report, API trả bản ghi mới → FE tự động cập nhật nhờ React Query cache key `['converter','reports',params]`. |  |  |

#### C5.4 Output & dữ liệu cần log

- Số report/tình trạng theo truyện, thời gian xử lý trung bình.
- Mối quan hệ giữa report và hành động sửa chương.

#### C5.5 Gợi ý biểu đồ ca làm việc

- Biểu đồ stacked bar Pending/Resolved/Rejected theo truyện; overlay timeline chỉnh sửa chương.

## Nhóm F – Theo dõi tài chính nội bộ

### Flow C6 – Lịch sử coin (thu nhập từ chapter)

#### C6.1 Mục tiêu chính

- Hiển thị chi tiết giao dịch coin phát sinh từ việc độc giả mở khóa.
- Cho phép converter đối soát với số dư hiện tại.

#### C6.2 Actor chính

- FE `/converter/coin-history`.
- `ConverterCoinHistoryController` (`/Converter/CoinHistory`).
- Wallet service.

#### C6.3 Chuỗi bước theo swimlane

| Bước | FE Converter | Coin History API | Wallet / DB |
| --- | --- | --- | --- |
| 1 | FE mount trang → gọi `GET /Converter/CoinHistory`. |  | Wallet service truy vấn transaction theo converter_id. |
| 2 | API trả danh sách (type, amount, balance_after). |  |  |
| 3 | FE render bảng, hỗ trợ filter theo thời gian (client-side). |  |  |
| 4 | Converter đối chiếu, nếu phát hiện sai khác → mở ticket (liên kết Admin Flow A7). |  |  |

#### C6.4 Output & dữ liệu cần log

- Tổng coin kiếm được theo ngày/tháng cho từng converter.
- Các giao dịch bị đánh dấu nghi ngờ.

#### C6.5 Gợi ý biểu đồ ca làm việc

- Line chart coin theo ngày + scatter highlight giao dịch bất thường.

### Flow C7 – Lịch sử key (quy đổi / thưởng)

#### C7.1 Mục tiêu chính

- Theo dõi vé/key mà converter nhận được từ sự kiện hoặc đổi thưởng.
- Đảm bảo minh bạch số key đang còn khả dụng.

#### C7.2 Actor chính

- FE `/converter/key-history`.
- `ConverterKeyHistoryController` (`/Converter/KeyHistory`).
- Reward service.

#### C7.3 Chuỗi bước theo swimlane

| Bước | FE Converter | Key History API | Reward / DB |
| --- | --- | --- | --- |
| 1 | FE gọi `GET /Converter/KeyHistory`. |  | Service trả danh sách key events (tặng, thưởng, sử dụng). |
| 2 | API trả JSON, FE render bảng + trạng thái (credit/debit). |  |  |
| 3 | Converter so sánh với số dư key ở dashboard; nếu lệch → liên hệ hỗ trợ. |  |  |

#### C7.4 Output & dữ liệu cần log

- Phân bổ key theo loại (thưởng sự kiện, quy đổi, bị trừ do vi phạm).
- Tỷ lệ converter phản hồi khi có thay đổi key.

#### C7.5 Gợi ý biểu đồ ca làm việc

- Donut chart loại key + timeline highlight mốc sự kiện phát key.
