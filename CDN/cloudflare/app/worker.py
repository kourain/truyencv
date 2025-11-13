from workers import WorkerEntrypoint
from app.main import app
import asgi
class Default(WorkerEntrypoint):
    async def fetch(self, request):

        return await asgi.fetch(app, request.js_object, self.env)
    