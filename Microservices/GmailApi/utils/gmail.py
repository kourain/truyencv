import os,base64,time,threading,sys
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart
from email.mime.base import MIMEBase
from email import encoders
from datetime import datetime
from queue import Queue
from googleapiclient.discovery import build, Resource
from google.oauth2.credentials import Credentials
from google.auth.transport.requests import Request
from utils.classes import Mail,Status
SCOPES = ['https://www.googleapis.com/auth/gmail.send']
DELAY = 1 # 1s
class Gmail:
    def __init__(self,token_file:str='token.json'):
        print("Initializing Gmail API...",file=sys.stderr)
        print("Using token file:", token_file,file=sys.stderr)
        self.Id:int = 1
        self.email_status:dict[int, Status] = {}
        self.service: Resource = self.get_gmail_service(token_file)
        self.queue:Queue[Mail] = Queue()
        self.lock = threading.Lock()
    def get_gmail_service(self, token_file:str) -> Resource:
        """Tạo service Gmail API"""
        creds = None
        # Kiểm tra token đã lưu
        if os.path.exists(token_file):
            try:
                creds = Credentials.from_authorized_user_file(token_file, SCOPES)
            except ValueError as e:
                # Token file bị lỗi, xóa và yêu cầu xác thực lại
                print(f"Token file corrupted: {e}", file=sys.stderr)
                os.remove(token_file)
                creds = None
        # Nếu không có credentials hợp lệ, yêu cầu authorization
        if not creds or not creds.valid:
            if creds and creds.expired and creds.refresh_token:
                try:
                    creds.refresh(Request())
                except Exception as e:
                    print(f"Failed to refresh token: {e}", file=sys.stderr)
                    # Xóa token và yêu cầu xác thực lại
                    # if os.path.exists(token_file):
                        # os.remove(token_file)
                    return None
            else:
                return None  
        # Lưu credentials
        with open(token_file, 'w',encoding="utf-8") as token:
            token.write(creds.to_json())
        return build('gmail', 'v1', credentials=creds)

    def create_message(self,sender, to, subject, message_text="", html_content="", attachments=None):
        """Tạo email message"""
        if html_content:
            message = MIMEMultipart('alternative')
        else:
            message = MIMEMultipart()
        message['to'] = to
        message['from'] = sender
        message['subject'] = subject
        # Thêm nội dung text
        if message_text != "":
            text_part = MIMEText(message_text, 'plain', 'utf-8')
            message.attach(text_part)
        # Thêm nội dung HTML nếu có
        if html_content != "":
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
    def send_email(self, mail:Mail):
        """Gửi một email"""
        for recipient in mail.recipients:
            try:
                message = self.create_message(
                    mail.sender,
                    recipient,
                    mail.subject,
                    mail.message_text,
                    mail.html_content,
                    mail.attachments
                )
                sent_message = self.service.users().messages().send(userId='me', body=message).execute()
                return sent_message
            except Exception as error:
                print(f'An error occurred: {error}')
                return None
    def send_email_batch(self):
        """Gửi email hàng loạt"""
        while True:
            with self.lock:
                while not self.queue.empty():
                    sender_email = self.queue.get()
                    self.email_status[sender_email.Id] = Status(sender_email.Id)
                    self.email_status[sender_email.Id].total = len(sender_email.recipients)
                    self.email_status[sender_email.Id].sent = 0
                    self.email_status[sender_email.Id].failed = 0
                    self.email_status[sender_email.Id].in_progress = True
                    self.email_status[sender_email.Id].logs = {}
                    for recipient in sender_email.recipients:
                        try:
                            # Tạo email
                            message = self.create_message(
                                sender_email.sender,
                                recipient,
                                sender_email.subject,
                                sender_email.message_text,
                                sender_email.html_content
                            )
                            # Gửi email
                            self.service.users().messages().send(userId='me', body=message).execute()
                            self.email_status[sender_email.Id].sent += 1

                            self.email_status[sender_email.Id].logs[recipient] = {
                                'timestamp': datetime.now().strftime('%H:%M:%S'),
                                'status': 'success',
                                'message': 'Email sent successfully'
                            }

                        except Exception as error:
                            self.email_status[sender_email.Id].failed += 1
                            self.email_status[sender_email.Id].logs[recipient] = {
                                'timestamp': datetime.now().strftime('%H:%M:%S'),
                                'status': 'error',
                                'message': f'Unexpected error: {str(error)}'
                            }
                    print(self.email_status[sender_email.Id].logs[recipient])
                    self.email_status[sender_email.Id].in_progress = False
                    print(sender_email.Id)
            time.sleep(1)
    def enQueue(self,mail:Mail):
        mail.Id = self.Id
        with self.lock:
            self.queue.put(mail)
            self.Id += 1
    def getStatus(self,id:int) -> Status|None:
        return self.email_status.get(id)