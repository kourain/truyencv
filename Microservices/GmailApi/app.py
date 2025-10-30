import threading
from fastapi import FastAPI, Request, Form, UploadFile, File
import uvicorn
from utils.classes import Mail
from utils.gmail import Gmail
import logging,sys
def log(message: str):
    print(message,file=sys.stderr)
app = FastAPI()
logger = logging.getLogger('uvicorn.error')
# Cấu hình OAuth2
CLIENT_SECRETS_FILE = 'ht.kourain@credentials.json'
TOKEN_FILE = 'ht.kourain@token.json'
gmail = Gmail(TOKEN_FILE)
@app.get('/reset')
async def reset(token:str):
    if(token != "rain"):
        return {"status": "error", "message": "Invalid token"}
    global gmail
    gmail = Gmail(TOKEN_FILE)
    return {"status": "ok"}
@app.post('/send')
async def send_emails(request: Request):
    if not gmail.service:
        return { "status": "error", "message": "Please authenticate with Gmail first." }
    # Lấy dữ liệu từ form
    data:dict = await request.json()
    subject = data['subject']
    text_content = data.get('text_content', "")
    html_content = data.get('html_content', '')
    recipients = data.get('recipients', [])
    mail = Mail()
    mail.subject = subject
    mail.message_text = text_content
    mail.html_content = html_content
    mail.recipients = recipients
    # gmail.enQueue(mail)
    gmail.send_email(mail)
    log(f"Sent email to: {recipients}")
    return {"status": "ok", "message": mail.Id}
@app.post("/status")
async def get_status(request:Request):
    data:dict = await request.json()
    status = gmail.getStatus(int(data.get("id")))
    if status:
        return {"status": "ok", "data": status}
    return {"status": "error", "message": "Status not found"}
@app.get("/ping")
def ping():
    return {"status": "ok"}
# thread = threading.Thread(target=gmail.send_email_batch,)
# thread.start()
if __name__ == "__main__":
    uvicorn.run(app, host="127.0.0.1", port=16503, log_level="trace")