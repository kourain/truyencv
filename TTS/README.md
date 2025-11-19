@app.py#L145 Hàm infer

Tham số đầu vào:
text=sentence

Văn bản đầu vào cần chuyển thành giọng nói
Là từng câu trong danh sách sentences đã được xử lý
language=LANGUAGE

Ngôn ngữ của văn bản ("vi" cho tiếng Việt)
Được định nghĩa ở dòng 29: LANGUAGE = "vi"
gpt_cond_latent=gpt_cond_latent

Vector điều kiện từ mô hình GPT, chứa thông tin về phong cách nói
Lấy từ conditioning[0] (dòng 140)
Giúp mô hình tạo ra giọng nói tự nhiên theo phong cách của âm thanh tham chiếu
speaker_embedding=speaker_embedding

Vector embedding đặc trưng của người nói
Lấy từ conditioning[1] (dòng 141)
Định danh giọng nói cụ thể từ âm thanh tham chiếu
Tham số điều chỉnh (sampling parameters):
temperature=0.3

Độ ngẫu nhiên trong quá trình sinh âm thanh
Giá trị thấp (0.3) tạo ra kết quả ổn định, ít sáng tạo
Phạm vi: 0.0-1.0
length_penalty=1.0

Hình phạt độ dài, điều chỉnh tốc độ nói
1.0 = tốc độ bình thường
1.0 = nói chậm hơn, <1.0 = nói nhanh hơn

repetition_penalty=10.0

Hình phạt lặp lại, tránh lặp âm thanh
Giá trị cao (10.0) giảm thiểu việc lặp từ/cụm từ
top_k=30

Chỉ xem xét 30 token có xác suất cao nhất
Giảm thiểu các lựa chọn không liên quan
top_p=0.85

Cutoff xác suất tích lũy 85%
Chỉ chọn các token có tổng xác suất ≤0.85
enable_text_splitting=True

Cho phép tự động tách văn bản dài thành các đoạn nhỏ
Giúp xử lý văn bản dài hiệu quả hơn