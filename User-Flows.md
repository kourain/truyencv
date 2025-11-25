# User Flows (Người dùng)

Tài liệu mô tả các luồng nghiệp vụ phía người dùng để tiện dựng slide và biểu đồ ca làm việc. Mỗi flow gồm mục tiêu, actor, bảng swimlane, đầu ra cần log và gợi ý trực quan hóa.

## Nhóm U – Onboarding & bảo mật

### Flow U1 – Đăng ký / đăng nhập / đăng xuất

#### U1.1 Mục tiêu chính

- Cho phép người dùng tạo tài khoản, đăng nhập, đăng xuất an toàn.
- Đảm bảo phát hành và thu hồi JWT + refresh token đúng chuẩn.

#### U1.2 Actor chính

- FE pages `/user/auth/register`, `/user/auth/login`.
- `AuthController`, `AuthService`, JWT middleware.
- Refresh token store.

#### U1.3 Chuỗi bước theo swimlane

| Bước | FE User Portal | Auth API | Auth/Token Store |
| --- | --- | --- | --- |
| 1 | Người dùng mở form đăng ký hoặc đăng nhập. |  |  |
| 2 | Gửi `POST /auth/register` hoặc `POST /auth/login` với payload hợp lệ. | Validate dữ liệu, hash password, kiểm tra trạng thái tài khoản. | Ghi user mới hoặc lấy hồ sơ cũ. |
| 3 | API sinh `access_token`, `refresh_token`, trả về JSON theo snake_case. |  | Lưu refresh token (IP, UA, expired_at). |
| 4 | FE lưu token vào storage, điều hướng `/user/home`. |  |  |
| 5 | Khi logout, FE gọi `POST /auth/logout` hoặc `/auth/logout-all`. | Đánh dấu refresh token tương ứng (hoặc toàn bộ) là revoked. | Cập nhật store, ghi log session. |

#### U1.4 Output & dữ liệu cần log

- Tỷ lệ đăng ký thành công, lỗi validation.
- Số phiên hoạt động, tần suất logout từng loại.

#### U1.5 Gợi ý biểu đồ ca làm việc

- Swimlane 3 hàng (FE, Auth API, Token Store) với nhánh logout riêng để làm rõ vòng đời phiên.

### Flow U2 – Xác nhận email sau đăng nhập

#### U2.1 Mục tiêu chính

- Buộc tài khoản mới xác thực email trước khi truy cập tính năng nhạy cảm.
- Giảm tỷ lệ email giả bằng việc theo dõi trạng thái verified.

#### U2.2 Actor chính

- FE banner nhắc xác thực.
- `UserProfileController`.
- Email service / verification token store.

#### U2.3 Chuỗi bước theo swimlane

| Bước | FE User Portal | Profile API | Email/Verification |
| --- | --- | --- | --- |
| 1 | Sau login, FE kiểm tra flag `email_verified`. Nếu false hiển thị CTA. |  |  |
| 2 | User bấm nút, FE gọi `POST /user/profile/verify-email`. | Service tạo verification token, gửi email kèm OTP/link. | Lưu token, hạn sử dụng. |
| 3 | User nhập OTP hoặc bấm link. | API xác thực OTP, cập nhật cờ `email_verified = true`. | Xóa token, ghi audit. |
| 4 | FE ẩn cảnh báo, cập nhật UI trạng thái đã xác minh. |  |  |

#### U2.4 Output & dữ liệu cần log

- Tỷ lệ xác minh thành công/ thất bại, thời gian trung bình từ đăng ký đến verify.
- Số lần gửi lại OTP.

#### U2.5 Gợi ý biểu đồ ca làm việc

- Funnel: Login chưa verify → Gửi OTP → Nhập OTP → Verified.

### Flow U3 – Đặt lại mật khẩu (quên mật khẩu)

#### U3.1 Mục tiêu chính

- Tạo quy trình 2 bước (request/confirm) để bảo vệ tài khoản.
- Thu hồi toàn bộ refresh token sau khi đổi mật khẩu.

#### U3.2 Actor chính

- FE màn “Quên mật khẩu”.
- `AuthController` endpoints `/password-reset/request`, `/password-reset/confirm`.
- OTP/Email service.

#### U3.3 Chuỗi bước theo swimlane

| Bước | FE User Portal | Auth API | OTP Store |
| --- | --- | --- | --- |
| 1 | User nhập email, gửi `POST /auth/password-reset/request`. | Kiểm tra email tồn tại, tạo OTP và mã yêu cầu. | Lưu OTP + expiry. |
| 2 | FE hiển thị form nhập OTP + mật khẩu mới. |  | Gửi email OTP. |
| 3 | User gửi `POST /auth/password-reset/confirm` (email, OTP, mật khẩu mới). | Xác thực OTP, cập nhật password hash, revoke tokens. | Xóa OTP. |
| 4 | FE điều hướng về `/user/auth/login`, hiển thị thông báo thành công. |  |  |

#### U3.4 Output & dữ liệu cần log

- Số yêu cầu reset mỗi ngày, tỷ lệ xác nhận thành công.
- Thời gian từ request tới confirm.

#### U3.5 Gợi ý biểu đồ ca làm việc

- Sequence diagram với nhánh OTP invalid / expired để nêu rõ kiểm soát bảo mật.

## Nhóm V – Trải nghiệm đọc truyện

### Flow V1 – Trang chủ người dùng

#### V1.1 Mục tiêu chính

- Cung cấp dữ liệu tổng hợp (lịch sử, đề cử, top, truyện mới) ngay khi user truy cập.
- Tối ưu truy vấn bằng cache nếu cần.

#### V1.2 Actor chính

- FE page `/user/home`.
- `HomeController` (User area).
- Recommendation service, cache layer.

#### V1.3 Chuỗi bước theo swimlane

| Bước | FE User Portal | Home API | Service/Cache |
| --- | --- | --- | --- |
| 1 | User truy cập `/user/home`, gửi `GET /user/home` kèm JWT. |  |  |
| 2 |  | API đọc lịch sử đọc, danh sách đề cử, top trending, truyện mới. | Query DB (AsNoTracking) hoặc lấy từ cache. |
| 3 | API gom dữ liệu theo từng block, trả JSON. |  |  |
| 4 | FE render card lịch sử, slider đề cử, bảng top, danh sách truyện mới. |  |  |

#### V1.4 Output & dữ liệu cần log

- Thời gian phản hồi, tỉ lệ cache hit.
- Block nào được user tương tác nhiều (scroll, click).

#### V1.5 Gợi ý biểu đồ ca làm việc

- Heatmap hoặc stacked bar thể hiện tỷ lệ thời gian dành cho từng block.

### Flow V2 – Xem chi tiết truyện + danh sách chương

#### V2.1 Mục tiêu chính

- Hiển thị meta SEO, thông tin truyện và trạng thái cá nhân (bookmark, đã đọc, lượt đề cử).
- Tải danh sách chương phân trang.

#### V2.2 Actor chính

- FE page `/user/comic/[slug]`.
- `ComicController` (User area).
- Search/SEO service.

#### V2.3 Chuỗi bước theo swimlane

| Bước | FE User Portal | Comic API | Services / DB |
| --- | --- | --- | --- |
| 1 | Gửi `GET /user/comic/seo/{slug}` để lấy meta. |  | SEO service trả title, description, image. |
| 2 | Gửi `GET /user/comic/{slug}` kèm JWT. | API lấy thông tin truyện, trạng thái bookmark, lượt đọc, unlock. | Repository join các bảng liên quan. |
| 3 | Khi user chọn “Xem tất cả chương”, FE gọi `GET /user/comic/{slug}/chapters?cursor=...`. |  | Trả danh sách chapter với trạng thái đã mở khóa. |
| 4 | FE render tabs thông tin, danh sách chương, button hành động (bookmark, đề cử, báo lỗi). |  |  |

#### V2.4 Output & dữ liệu cần log

- Thời gian user ở trang chi tiết, tỷ lệ chuyển sang đọc chương.
- Thống kê slug được truy cập nhiều nhất.

#### V2.5 Gợi ý biểu đồ ca làm việc

- Timeline 4 bước (SEO → Detail → Chapter list → CTA) để minh họa conversion.

### Flow V3 – Đọc chương & lưu lịch sử

#### V3.1 Mục tiêu chính

- Phục vụ nội dung chương đúng quyền truy cập.
- Ghi lại lịch sử đọc để cá nhân hóa.

#### V3.2 Actor chính

- FE page `/user/comic/[slug]/chapters/[chapter]`.
- `ChapterController` (User area).
- Read history service.

#### V3.3 Chuỗi bước theo swimlane

| Bước | FE Reader | Chapter API | Services / DB |
| --- | --- | --- | --- |
| 1 | FE gửi `GET /user/comic/{slug}/chapters/{chapterNumber}` kèm JWT. | Validate quyền (đã mở khóa, gói VIP, v.v.). |  |
| 2 | API trả nội dung chương, metadata điều hướng chapter trước/sau. |  |  |
| 3 | Đồng thời ghi lịch sử đọc bằng background task hoặc trực tiếp. |  | ReadHistory repo ghi `(user_id, comic_id, chapter, read_at)`. |
| 4 | FE render nội dung, nút next/prev, action AI/TTS. |  |  |

#### V3.4 Output & dữ liệu cần log

- Số lần truy cập mỗi chapter, thời lượng đọc trung bình.
- Lịch sử để hiển thị ở trang chủ và tab đọc gần đây.

#### V3.5 Gợi ý biểu đồ ca làm việc

- Sequence diagram hiển thị nhánh ghi lịch sử song song với trả nội dung.

### Flow V4 – Mở khóa chương bằng coin/key

#### V4.1 Mục tiêu chính

- Cho phép user thanh toán coin/key để đọc chương premium.
- Đảm bảo cập nhật số dư và trạng thái unlock trong cùng giao dịch.

#### V4.2 Actor chính

- FE button “Mở khóa”.
- `ComicUnlockController`.
- Wallet service.

#### V4.3 Chuỗi bước theo swimlane

| Bước | FE Reader | Unlock API | Wallet/DB |
| --- | --- | --- | --- |
| 1 | User bấm mở khóa, FE gửi `POST /user/comic-unlock` với comic_id, chapter. |  |  |
| 2 | API kiểm tra chapter có trả phí, user đã mở trước chưa. |  |  |
| 3 | Wallet service kiểm tra số dư coin/key, trừ số dư bằng `ExecuteUpdateAsync`. |  | Lưu transaction và bản ghi unlock. |
| 4 | API trả trạng thái thành công, chapter được đánh dấu unlocked. |  |  |
| 5 | FE cập nhật UI, cho phép đọc ngay. |  |  |

#### V4.4 Output & dữ liệu cần log

- Số coin/key tiêu thụ theo ngày, tỷ lệ unlock thành công.
- Danh sách chapter premium phổ biến.

#### V4.5 Gợi ý biểu đồ ca làm việc

- Sankey Coin/Key → Chapter premium để minh họa dòng tiền ảo.

### Flow V5 – Lịch sử đọc

#### V5.1 Mục tiêu chính

- Cho phép user xem lại truyện/chapter đã đọc gần đây.
- Hỗ trợ thao tác xóa từng mục.

#### V5.2 Actor chính

- FE trang “Lịch sử đọc”.
- `ReadHistoryController`.
- ReadHistory repository.

#### V5.3 Chuỗi bước theo swimlane

| Bước | FE User Portal | ReadHistory API | DB |
| --- | --- | --- | --- |
| 1 | Gửi `GET /user/read-history?limit=n`. |  | Repo truy vấn theo user_id, order by read_at desc. |
| 2 | FE render danh sách, hiển thị nút “Xóa”. |  |  |
| 3 | Khi xóa, FE gọi `DELETE /user/read-history/{comicId}`. | API xác thực quyền, xóa bản ghi tương ứng. | Thực hiện `ExecuteDeleteAsync`. |

#### V5.4 Output & dữ liệu cần log

- Số bản ghi lịch sử trên user, tỷ lệ bị xóa.
- Lượt quay lại đọc từ trang lịch sử.

#### V5.5 Gợi ý biểu đồ ca làm việc

- Bar chart thể hiện số lượt đọc theo ngày, kèm line shows retention.

### Flow V6 – Gợi ý truyện liên quan (Embedded)

#### V6.1 Mục tiêu chính

- Gợi ý truyện cùng chủ đề/nguồn ngay trong trang chi tiết để tăng thời gian on-site.
- Tái sử dụng dữ liệu được cache 5 phút nhằm giảm số lần gọi API.

#### V6.2 Actor chính

- FE section “Có thể bạn thích” trong `/user/comic/[slug]`.
- `EmbeddedController` (User area, endpoint `/user/comic/{slug}/embedded`).
- Recommendation/Embedding service.

#### V6.3 Chuỗi bước theo swimlane

| Bước | FE User Portal | Embedded API | Service / Cache |
| --- | --- | --- | --- |
| 1 | Khi trang chi tiết load xong slug, FE gọi `GET /user/comic/{slug}/embedded`. |  |  |
| 2 | API kiểm tra cache theo slug. | Nếu có cache → trả ngay danh sách liên quan. | Redis hoặc memory cache lưu tối đa 5 phút. |
| 3 | Nếu cache miss, Service chạy truy vấn embedding (similarity vector). |  | Trả về top N truyện + metadata. |
| 4 | API trả JSON, FE render carousel “Truyện liên quan”. |  |  |
| 5 | Người dùng click item → điều hướng sang chi tiết truyện mới, flow lặp lại. |  |  |

#### V6.4 Output & dữ liệu cần log

- CTR từ block embedded, dwell time trước/sau khi hiển thị.
- Cache hit ratio theo slug.

#### V6.5 Gợi ý biểu đồ ca làm việc

- Funnel Embedded Impression → Click → Đọc chapter đầu tiên của truyện gợi ý.

### Flow V7 – Tìm kiếm truyện theo từ khóa

#### V7.1 Mục tiêu chính

- Cho phép user tìm truyện nhanh với phân trang và fallback dữ liệu mock khi API lỗi.
- Giảm call rỗng bằng cách chỉ kích hoạt query khi keyword hợp lệ.

#### V7.2 Actor chính

- FE trang `/user/search` hoặc thanh tìm kiếm global.
- `SearchController` (`GET /user/comic/search`).
- Search service + mock builder.

#### V7.3 Chuỗi bước theo swimlane

| Bước | FE User Portal | Search API | Search Service |
| --- | --- | --- | --- |
| 1 | User nhập keyword, FE debounce rồi gọi `GET /user/comic/search?keyword=...&page=...`. |  |  |
| 2 | API kiểm tra keyword rỗng → trả 400 hoặc bỏ qua. |  |  |
| 3 | Nếu query thành công, service trả danh sách kết quả + tổng, trang, page_size. |  | Query Postgres full-text hoặc PGVector. |
| 4 | Nếu API lỗi, FE nhận dữ liệu mock từ fallback builder (tên, cover, mô tả). |  | Service log lỗi để dev theo dõi. |
| 5 | FE render grid kết quả, nút phân trang, highlight keyword. |  |  |

#### V7.4 Output & dữ liệu cần log

- Tỷ lệ tìm kiếm thành công, số lần fallback mock, top keyword.
- Số trang user duyệt trung bình cho một keyword.

#### V7.5 Gợi ý biểu đồ ca làm việc

- Heatmap keyword theo giờ + biểu đồ cột thể hiện % fallback.

## Nhóm W – Tương tác & cộng đồng

### Flow W1 – Bookmark truyện

#### W1.1 Mục tiêu chính

- Lưu trạng thái theo dõi truyện của user.
- Đồng bộ UI giữa trang truyện và danh sách bookmark.

#### W1.2 Actor chính

- FE button Bookmark.
- `BookmarkController`.
- Bookmark repository/cache.

#### W1.3 Chuỗi bước theo swimlane

| Bước | FE User Portal | Bookmark API | DB/Cache |
| --- | --- | --- | --- |
| 1 | FE gửi `POST /user/bookmark` (comic_id). | Validate user, comic. | Upsert bản ghi (user_id, comic_id). |
| 2 | API trả trạng thái mới (đã bookmark). |  |  |
| 3 | Khi bỏ theo dõi, FE gọi `DELETE /user/bookmark/{comicId}`. | Repo xóa bản ghi. | Cập nhật cache danh sách bookmark. |

#### W1.4 Output & dữ liệu cần log

- Số bookmark theo truyện, tỷ lệ hủy.
- Danh sách truyện top bookmark để hiển thị trong dashboard.

#### W1.5 Gợi ý biểu đồ ca làm việc

- Pareto chart thể hiện 20% truyện chiếm % bookmark.

### Flow W2 – Bình luận truyện

#### W2.1 Mục tiêu chính

- Thu nhận bình luận gắn với user, truyện, chapter.
- Cho phép FE hiển thị realtime (append) sau khi gửi.

#### W2.2 Actor chính

- FE khung bình luận.
- `ComicCommentController`.
- Comment moderation service.

#### W2.3 Chuỗi bước theo swimlane

| Bước | FE User Portal | Comment API | Services / DB |
| --- | --- | --- | --- |
| 1 | FE thu input, gửi `POST /user/comic-comment`. | Validate nội dung (length, banned words). |  |
| 2 | API gắn `userId` từ JWT, lưu DB. |  | Comment repo insert, trả DTO (id, content, created_at). |
| 3 | FE append comment mới vào list, scroll tới vị trí. |  |  |

#### W2.4 Output & dữ liệu cần log

- Tổng bình luận theo truyện, tỷ lệ vi phạm.
- Thời gian phản hồi trung bình.

#### W2.5 Gợi ý biểu đồ ca làm việc

- Line chart số bình luận/ngày, highlight spike khi có truyện hot.

### Flow W3 – Báo lỗi truyện

#### W3.1 Mục tiêu chính

- Thu nhận phản hồi lỗi nội dung từ user để admin xử lý.
- Lưu trạng thái report (pending/resolved/rejected).

#### W3.2 Actor chính

- FE nút “Báo lỗi”.
- `ComicReportController` (User area).
- Report service.

#### W3.3 Chuỗi bước theo swimlane

| Bước | FE User Portal | ComicReport API | DB |
| --- | --- | --- | --- |
| 1 | User chọn lý do, FE gửi `POST /user/comic-report`. |  | Repo lưu report kèm user, comic, chapter. |
| 2 | API trả trạng thái pending, hiển thị cảm ơn. |  |  |
| 3 | Admin xử lý ở flow A4, kết quả đồng bộ lại. |  | Update trạng thái, gửi thông báo nếu cần. |

#### W3.4 Output & dữ liệu cần log

- Số report theo loại, thời gian xử lý trung bình.
- Liên kết giữa report và phiên bản chapter đã sửa.

#### W3.5 Gợi ý biểu đồ ca làm việc

- Sankey User → Loại lỗi → Trạng thái xử lý.

### Flow W4 – Đề cử truyện

#### W4.1 Mục tiêu chính

- Ghi nhận lượt đề cử mỗi truyện từ user.
- Làm dữ liệu đầu vào cho bảng xếp hạng ở khu Admin.

#### W4.2 Actor chính

- FE nút “Đề cử”.
- `RecommendController` (User area).
- Ranking service, cache.

#### W4.3 Chuỗi bước theo swimlane

| Bước | FE User Portal | Recommend API | Services / DB |
| --- | --- | --- | --- |
| 1 | FE gửi `POST /user/recommend/{comicId}`. | API kiểm tra quota (số lượt đề cử/ngày). | Repo ghi lượt đề cử. |
| 2 | API trả tổng điểm đề cử hiện tại. |  | Trigger cập nhật cache ranking. |
| 3 | FE update badge “Đã đề cử”. |  |  |

#### W4.4 Output & dữ liệu cần log

- Tổng đề cử theo truyện, delta so với kỳ trước.
- Số user unique tham gia đề cử.

#### W4.5 Gợi ý biểu đồ ca làm việc

- Line chart top 5 truyện theo tuần để minh họa biến động đề cử.

## Nhóm X – Ví người dùng & thanh toán

### Flow X1 – Theo dõi lịch sử ví (coin, key, thanh toán)

#### X1.1 Mục tiêu chính

- Cho phép user xem transaction coin, key và giao dịch thanh toán trên cùng màn hình.
- Mỗi tab gọi API riêng để giảm tải dữ liệu.

#### X1.2 Actor chính

- FE trang “Tài khoản / Lịch sử”.
- `WalletController`, `PaymentHistoryController` (User area).
- Wallet/Payment services.

#### X1.3 Chuỗi bước theo swimlane

| Bước | FE User Portal | Wallet/Payment API | Services / DB |
| --- | --- | --- | --- |
| 1 | Khi mở tab coin: `GET /user/coin-history?limit=n`. |  | Wallet service trả transactions (type, amount, balance). |
| 2 | Tab key tương tự `GET /user/key-history`. |  |  |
| 3 | Tab thanh toán gọi `GET /user/payment-history`. | Payment service trả giao dịch (provider, status, amount). |  |
| 4 | FE hiển thị bảng, cho phép filter theo thời gian. |  |  |

#### X1.4 Output & dữ liệu cần log

- Số coin/key tiêu thụ, nguồn nạp coin (in-app, event).
- Tổng doanh thu từ phía user.

#### X1.5 Gợi ý biểu đồ ca làm việc

- Dashboard stacked bar (coin vs key) + line doanh thu.

## Nhóm Y – Hồ sơ & phiên đăng nhập

### Flow Y1 – Quản lý hồ sơ và phiên đang hoạt động

#### Y1.1 Mục tiêu chính

- Cho phép user xem thông tin tài khoản, chỉnh sửa, quản lý refresh tokens.
- Tăng minh bạch về phiên đăng nhập trên nhiều thiết bị.

#### Y1.2 Actor chính

- FE trang `/user/profile`.
- `AuthController` (`GET /auth/me`), `ProfileController`, `RefreshTokenController` (User area).
- Token store.

#### Y1.3 Chuỗi bước theo swimlane

| Bước | FE User Portal | Profile/Token API | DB |
| --- | --- | --- | --- |
| 1 | FE gọi `GET /auth/me` để lấy thông tin cơ bản. |  |  |
| 2 | Gọi `GET /user/profile/refresh-tokens` hiển thị danh sách thiết bị. | API truy vấn token store theo user. |  |
| 3 | Khi đổi mật khẩu/email, FE gửi `PUT /user/profile/...`. | Service validate, cập nhật DB. |  |
| 4 | Khi thu hồi phiên, FE gọi `DELETE /user/profile/refresh-tokens/{id}` hoặc toàn bộ. | Token store đặt cờ revoked, API trả kết quả. |  |

#### Y1.4 Output & dữ liệu cần log

- Số thiết bị hoạt động mỗi user, số phiên bị revoke thủ công.
- Các thay đổi hồ sơ gần nhất để audit.

#### Y1.5 Gợi ý biểu đồ ca làm việc

- Sequence diagram hiển thị user chủ động revoke một phiên cụ thể và toàn bộ phiên.

## Nhóm Z – Tính năng AI & TTS

### Flow Z1 – Chuyển chương sang thuần Việt (AI)

#### Z1.1 Mục tiêu chính

- Cho phép người đọc chuyển nội dung convert sang bản thuần Việt qua dịch vụ AI.
- Đảm bảo không lưu bản AI nếu user không yêu cầu.

#### Z1.2 Actor chính

- FE reader button “Chuyển sang thuần Việt”.
- `ChapterAIController`.
- AI convert service.

#### Z1.3 Chuỗi bước theo swimlane

| Bước | FE Reader | AI API | AI Service |
| --- | --- | --- | --- |
| 1 | User chọn chương đang đọc, bấm nút AI. |  |  |
| 2 | FE gửi `POST /user/comic/{slug}/chapters/{chapterNumber}/convert-tv` kèm nội dung cần chuyển. | Validate quota, chuẩn hóa payload. | Gọi microservice chuyển đổi. |
| 3 | AI service trả văn bản thuần Việt. | API trả kết quả, log thời gian xử lý. |  |
| 4 | FE hiển thị bản AI song song bản gốc, cho phép user chuyển qua lại. |  |  |

#### Z1.4 Output & dữ liệu cần log

- Số chương được chuyển AI, thời gian xử lý trung bình.
- Đánh giá chất lượng (nếu có feedback).

#### Z1.5 Gợi ý biểu đồ ca làm việc

- Line chart thể hiện thời gian xử lý AI theo chương, highlight SLA.

### Flow Z2 – TTS chương truyện

#### Z2.1 Mục tiêu chính

- Cung cấp audio cho chương truyện với lựa chọn giọng đọc.
- Tái sử dụng kết quả TTS nếu chương chưa đổi nội dung.

#### Z2.2 Actor chính

- FE player “Nghe chương”.
- `ChapterTTSController`.
- TTS service / media storage.

#### Z2.3 Chuỗi bước theo swimlane

| Bước | FE Reader | TTS API | TTS Service / Storage |
| --- | --- | --- | --- |
| 1 | Nếu cần danh sách giọng, FE gọi `GET /user/comic/tts/voices`. | API trả danh sách giọng, ngôn ngữ. |  |
| 2 | User chọn giọng, gửi `POST /user/comic/{slug}/chapters/{chapterNumber}/tts`. |  | Service kiểm tra cache audio; sinh mới nếu chưa có. |
| 3 | TTS service trả file audio, lưu vào storage/CDN. | API trả URL/stream token. |  |
| 4 | FE phát audio trong player, hiển thị trạng thái tiến trình. |  |  |

#### Z2.4 Output & dữ liệu cần log

- Số chương được nghe, thời lượng nghe trung bình.
- Cache hit ratio giữa audio cũ và audio sinh mới.

#### Z2.5 Gợi ý biểu đồ ca làm việc

- Biểu đồ donut thể hiện tỷ lệ chương nghe vs đọc, kèm heatmap giọng đọc phổ biến.
