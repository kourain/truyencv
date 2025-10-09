import os
import json
import base64
import pandas as pd
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart
from email.mime.base import MIMEBase
from email import encoders
import threading
import time
from datetime import datetime

from flask import Flask, render_template, request, redirect, url_for, flash, jsonify, session
from google.auth.transport.requests import Request
from google.oauth2.credentials import Credentials
from google_auth_oauthlib.flow import Flow
from googleapiclient.discovery import build
from googleapiclient.errors import HttpError

# Cho phép HTTP trong môi trường development (chỉ dùng cho local testing)
os.environ['OAUTHLIB_INSECURE_TRANSPORT'] = '1'

app = Flask(__name__)
app.secret_key = 'your-secret-key-change-this-in-production'

# Cấu hình OAuth2
SCOPES = ['https://www.googleapis.com/auth/gmail.send']
CLIENT_SECRETS_FILE = 'credentials.json'
TOKEN_FILE = 'token.json'

# Biến toàn cục để theo dõi trạng thái gửi email
email_status = {
    'total': 0,
    'sent': 0,
    'failed': 0,
    'in_progress': False,
    'logs': []
}

def get_gmail_service():
    """Tạo service Gmail API"""
    creds = None
    
    # Kiểm tra token đã lưu
    if os.path.exists(TOKEN_FILE):
        try:
            creds = Credentials.from_authorized_user_file(TOKEN_FILE, SCOPES)
        except ValueError as e:
            # Token file bị lỗi, xóa và yêu cầu xác thực lại
            print(f"Token file corrupted: {e}")
            os.remove(TOKEN_FILE)
            creds = None
    
    # Nếu không có credentials hợp lệ, yêu cầu authorization
    if not creds or not creds.valid:
        if creds and creds.expired and creds.refresh_token:
            try:
                creds.refresh(Request())
            except Exception as e:
                print(f"Failed to refresh token: {e}")
                # Xóa token và yêu cầu xác thực lại
                if os.path.exists(TOKEN_FILE):
                    os.remove(TOKEN_FILE)
                return None
        else:
            return None
    
    # Lưu credentials
    with open(TOKEN_FILE, 'w') as token:
        token.write(creds.to_json())
    
    return build('gmail', 'v1', credentials=creds)

def create_message(sender, to, subject, message_text, html_content=None, attachments=None):
    """Tạo email message"""
    if html_content:
        message = MIMEMultipart('alternative')
    else:
        message = MIMEMultipart()
    
    message['to'] = to
    message['from'] = sender
    message['subject'] = subject
    
    # Thêm nội dung text
    text_part = MIMEText(message_text, 'plain', 'utf-8')
    message.attach(text_part)
    
    # Thêm nội dung HTML nếu có
    if html_content:
        html_part = MIMEText(html_content, 'html', 'utf-8')
        message.attach(html_part)
    
    # Thêm attachments nếu có
    if attachments:
        for file_path in attachments:
            if os.path.isfile(file_path):
                with open(file_path, "rb") as attachment:
                    part = MIMEBase('application', 'octet-stream')
                    part.set_payload(attachment.read())
                    encoders.encode_base64(part)
                    part.add_header(
                        'Content-Disposition',
                        f'attachment; filename= {os.path.basename(file_path)}'
                    )
                    message.attach(part)
    
    return {'raw': base64.urlsafe_b64encode(message.as_bytes()).decode()}

def send_email_batch(service, sender_email, recipients, subject, text_content, html_content=None, delay=1):
    """Gửi email hàng loạt"""
    global email_status
    
    email_status['total'] = len(recipients)
    email_status['sent'] = 0
    email_status['failed'] = 0
    email_status['in_progress'] = True
    email_status['logs'] = []
    
    for i, recipient in enumerate(recipients):
        try:
            # Tạo email
            message = create_message(
                sender_email, 
                recipient, 
                subject, 
                text_content, 
                html_content
            )
            
            # Gửi email
            service.users().messages().send(userId='me', body=message).execute()
            email_status['sent'] += 1
            
            log_entry = {
                'timestamp': datetime.now().strftime('%H:%M:%S'),
                'recipient': recipient,
                'status': 'success',
                'message': 'Email sent successfully'
            }
            email_status['logs'].append(log_entry)
            
            print(f"Email sent to {recipient} ({i+1}/{len(recipients)})")
            
            # Delay giữa các email để tránh rate limiting
            if i < len(recipients) - 1:
                time.sleep(delay)
                
        except HttpError as error:
            email_status['failed'] += 1
            log_entry = {
                'timestamp': datetime.now().strftime('%H:%M:%S'),
                'recipient': recipient,
                'status': 'error',
                'message': f'Error: {str(error)}'
            }
            email_status['logs'].append(log_entry)
            print(f"Failed to send email to {recipient}: {error}")
        except Exception as error:
            email_status['failed'] += 1
            log_entry = {
                'timestamp': datetime.now().strftime('%H:%M:%S'),
                'recipient': recipient,
                'status': 'error',
                'message': f'Unexpected error: {str(error)}'
            }
            email_status['logs'].append(log_entry)
            print(f"Unexpected error sending to {recipient}: {error}")
    
    email_status['in_progress'] = False
    print("Batch email sending completed!")

@app.route('/')
def index():
    """Trang chủ"""
    return render_template('index.html')

@app.route('/auth')
def auth():
    """Bắt đầu quá trình OAuth2"""
    flow = Flow.from_client_secrets_file(
        CLIENT_SECRETS_FILE,
        scopes=SCOPES,
        redirect_uri="https://ht-kourain.maiquyen.name.vn/callback"
    )
    
    authorization_url, state = flow.authorization_url(
        access_type='offline',
        include_granted_scopes='true',
        prompt='consent'  # Buộc hiển thị consent screen để lấy refresh token
    )
    
    session['state'] = state
    return redirect(authorization_url)

@app.route('/callback')
def callback():
    """Xử lý callback từ Google OAuth2"""
    state = session.get('state')
    
    flow = Flow.from_client_secrets_file(
        CLIENT_SECRETS_FILE,
        scopes=SCOPES,
        state=state,
        redirect_uri="https://ht-kourain.maiquyen.name.vn/callback"
    )
    
    flow.fetch_token(authorization_response=request.url.replace('http://', 'https://'))
    
    # Lưu credentials
    credentials = flow.credentials
    with open(TOKEN_FILE, 'w') as token:
        token.write(credentials.to_json())
    
    flash('Authentication successful!', 'success')
    return redirect(url_for('compose'))

@app.route('/compose')
def compose():
    """Trang soạn email"""
    # Kiểm tra xem đã authenticate chưa
    service = get_gmail_service()
    if not service:
        flash('Please authenticate with Gmail first.', 'error')
        return redirect(url_for('index'))
    
    return render_template('compose.html')

@app.route('/send', methods=['POST'])
def send_emails():
    """Xử lý gửi email hàng loạt"""
    service = get_gmail_service()
    if not service:
        flash('Please authenticate with Gmail first.', 'error')
        return redirect(url_for('index'))
    
    # Lấy dữ liệu từ form
    sender_email = request.form['sender_email']
    subject = request.form['subject']
    text_content = request.form['text_content']
    html_content = request.form.get('html_content', '')
    delay = int(request.form.get('delay', 1))
    
    # Xử lý danh sách email
    email_list_type = request.form['email_list_type']
    recipients = []
    
    if email_list_type == 'manual':
        email_text = request.form['email_list']
        recipients = [email.strip() for email in email_text.split('\n') if email.strip()]
    
    elif email_list_type == 'file':
        if 'email_file' not in request.files:
            flash('No file uploaded', 'error')
            return redirect(url_for('compose'))
        
        file = request.files['email_file']
        if file.filename == '':
            flash('No file selected', 'error')
            return redirect(url_for('compose'))
        
        # Xử lý file Excel/CSV
        try:
            if file.filename.endswith('.xlsx') or file.filename.endswith('.xls'):
                df = pd.read_excel(file)
            elif file.filename.endswith('.csv'):
                df = pd.read_csv(file)
            else:
                flash('Unsupported file format. Please use Excel or CSV.', 'error')
                return redirect(url_for('compose'))
            
            # Lấy cột email (giả sử cột đầu tiên hoặc cột có tên 'email')
            if 'email' in df.columns:
                recipients = df['email'].dropna().tolist()
            else:
                recipients = df.iloc[:, 0].dropna().tolist()
                
        except Exception as e:
            flash(f'Error reading file: {str(e)}', 'error')
            return redirect(url_for('compose'))
    
    if not recipients:
        flash('No valid email addresses found', 'error')
        return redirect(url_for('compose'))
    
    # Bắt đầu gửi email trong background thread
    thread = threading.Thread(
        target=send_email_batch,
        args=(service, sender_email, recipients, subject, text_content, html_content, delay)
    )
    thread.start()
    
    flash(f'Started sending emails to {len(recipients)} recipients', 'success')
    return redirect(url_for('status'))

@app.route('/status')
def status():
    """Trang theo dõi trạng thái gửi email"""
    return render_template('status.html')

@app.route('/api/status')
def api_status():
    """API endpoint để lấy trạng thái gửi email"""
    return jsonify(email_status)

@app.route('/logout')
def logout():
    """Đăng xuất và xóa token"""
    if os.path.exists(TOKEN_FILE):
        os.remove(TOKEN_FILE)
    session.clear()
    flash('Logged out successfully', 'success')
    return redirect(url_for('index'))

if __name__ == '__main__':
    # Tạo thư mục templates nếu chưa có    
    app.run(debug=True, host='localhost', port=16503)
