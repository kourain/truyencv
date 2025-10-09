class Mail:
    def __init__(self):
        self.Id: int = 0
        self.sender: str = "Kourain"
        self.recipients: list[str] = []
        self.to: str = ""
        self.subject: str = ""
        self.message_text: str = ""
        self.html_content: str = ""
        self.attachments: list[str] = []
class Status:
    def __init__(self,Id):
        self.id: int = Id
        self.total: int = 0
        self.sent: int = 0
        self.failed: int = 0
        self.in_progress: bool = False
        self.logs: dict[str, dict] = {}