from concurrent.futures import process
from datetime import datetime
from fastapi import FastAPI, HTTPException, Request, WebSocket,Form, status
from fastapi.responses import RedirectResponse,HTMLResponse
from fastapi.staticfiles import StaticFiles
from fastapi.templating import Jinja2Templates
from starlette.middleware.base import BaseHTTPMiddleware
from starlette.requests import Request as StarletteRequest
from typing import Dict, Any, Optional
import os,gc,logging,time,threading,subprocess,json
import signal
import asyncio
import psutil
import uvicorn,requests
gc.enable()
if os.name == 'nt':
    from utils.cpu.windows import Windows as cpu
    from utils.ram.windows import Windows as ram
    from utils.disk.windows import Windows as disk
else:
    from utils.cpu.linux import Linux as cpu
    from utils.ram.linux import Linux as ram
    from utils.disk.linux import Linux as disk
WSisOn = False
# Setup logging
logging.basicConfig(level=logging.DEBUG)
logger = logging.getLogger(__name__)

# Global dictionary to store running processes
services: Dict[str, subprocess.Popen | None] = {}
PROCESSPID: Dict[int, psutil.Process] = {}
PROCESSINFO: Dict[int, Dict[str, Any]] = {}
CREATETIME: Dict[int, float] = {}
cpu.init()
disk.init()

from contextlib import asynccontextmanager


def SVkill(process: subprocess.Popen | None):
    """Kill a service process"""
    if process is None:
        return

    try:
        process.send_signal(signal.SIGINT)
        process.terminate()
        try:
            process.wait(timeout=5)
        except subprocess.TimeoutExpired:
            process.send_signal(signal.SIGKILL)
            process.wait()
    except Exception as e:
        logger.error(f"Error terminating process {process.pid}: {e}")
@asynccontextmanager
async def lifespan(app: FastAPI):
	yield
	# shutdown logic here
	logger.info("Shutting down...")
	for service_id, service in services.items():
		if service is not None:
			SVkill(service)
	logger.info("All services terminated")

app = FastAPI(title="Service Manager", description="Web interface để quản lý các dịch vụ từ xa", lifespan=lifespan)
templates = Jinja2Templates(directory="views")
app.mount("/static", StaticFiles(directory="static"), name="static")

# --- Đăng nhập ---
SESSION_COOKIE = "service_manager_session"
session_cookie = "your_session_cookie_value"  # Đổi giá trị này nếu muốn bảo mật hơn
USERNAME = "kourain.me"
PASSWORD = "1408"

def is_authenticated(request: StarletteRequest):
    session = request.cookies.get(SESSION_COOKIE)
    return session == session_cookie

class AuthMiddleware(BaseHTTPMiddleware):
    async def dispatch(self, request: StarletteRequest, call_next):
        # Cho phép truy cập /login, /static, /ws không cần đăng nhập
        url = request.url.path.lower()
        if url.startswith("/login") or url.startswith("/status"):
            return await call_next(request)
        if not is_authenticated(request):
            return RedirectResponse(url="/login", status_code=status.HTTP_302_FOUND)
        return await call_next(request)

app.add_middleware(AuthMiddleware)

@app.get("/login", response_class=HTMLResponse)
async def login_form(request: Request):
    return templates.TemplateResponse("login.html", {"request": request, "error": None})

@app.post("/login", response_class=HTMLResponse)
async def login_submit(request: Request, username: str = Form(...), password: str = Form(...)):
    global session_cookie
    if username == USERNAME and password == PASSWORD:
        response = RedirectResponse(url="/", status_code=status.HTTP_302_FOUND)
        session_cookie = datetime.now().isoformat()[::-1].encode().hex().lower()  # Generate a new session cookie
        response.set_cookie(key=SESSION_COOKIE, value=session_cookie, httponly=True, max_age=60*60*8)
        return response
    else:
        return templates.TemplateResponse("login.html", {"request": request, "error": "Sai tên đăng nhập hoặc mật khẩu!"})

@app.get("/logout")
async def logout():
    response = RedirectResponse(url="/login", status_code=status.HTTP_302_FOUND)
    response.delete_cookie(SESSION_COOKIE)
    return response

def load_commands() -> Dict[str, Any]:
    """Load commands from command.json file"""
    try:
        with open("command.json", "r", encoding="utf-8") as f:
            return json.load(f)
    except FileNotFoundError:
        logger.error("command.json file not found")
        return {"commands": {}}
    except json.JSONDecodeError as e:
        logger.error(f"Error parsing command.json: {e}")
        return {"commands": {}}

def get_process_info(pid: int) -> Optional[Dict[str, Any]]:
    """Get detailed process information"""
    try:
        process = PROCESSPID[pid]
        if process is None:
            logger.error(f"Process {pid} not found")
            return None

        memory_info = process.memory_info()
        memory_percent = process.memory_percent()
        
        return {
            "pid": pid,
            "name": process.name(),
            "status": process.status(),
            "cpu_percent": process.cpu_percent(interval=1),
            "ram_mb": memory_info.rss / (1024 * 1024),
            "ram_percent": memory_percent,
            "ram_info": memory_info._asdict(),
            "create_time": datetime.fromtimestamp(CREATETIME[pid]).isoformat(),
            "cmdline": " ".join(process.cmdline()),
        }
    except:
        return None

def updateProcessInfo():
    """Get status of a specific service"""
    global WSisOn,PROCESSINFO,PROCESSPID,services
    while True:
        if WSisOn:
            for service_id, process in list(services.items()):
                if process is not None:
                    pid = process.pid
                    if pid in PROCESSPID:
                        process_info = get_process_info(pid)
                        if process_info is not None:
                            PROCESSINFO[pid] = process_info
        time.sleep(1)
threading.Thread(target=updateProcessInfo, daemon=True).start()
async def ReloadByNetw(service_id: str,url:str):
	"""Reload a service by network request"""
			# try:
	if url == "":
		return
	response = requests.get(url,verify=True)
	if response.status_code != 200:
		await restart_service(service_id)

def start_reload_thread():
	while True:
		commands_data = load_commands()
		reload_commands:dict[str,dict] = commands_data.get("reload-by-netw", {})
		interval = commands_data.get("reload-by-netw-interval", 5)
		for service_id, config in reload_commands.items():
			asyncio.run(ReloadByNetw(service_id, config.get("url", "")))
		time.sleep(interval)
threading.Thread(target=start_reload_thread, daemon=True).start()


def get_PID(service_id: str)->int:
    """Get status of a specific service"""
    if service_id in services:
        process = services[service_id]
        if process is not None and process.poll() is None:  # Process is still running
            return process.pid
    return -1

@app.get("/status")
async def get_status():
	return {"status":"OK"}

@app.get("/", response_class=HTMLResponse)
async def read_root(request: Request):
    """Serve the main web interface"""
    return templates.TemplateResponse("index.html", {"request": request})

@app.post("/services/{service_id}/start")
async def start_service(service_id: str):
    """Start a service"""
    commands_data = load_commands()
    commands = commands_data.get("commands", {})
    
    if service_id not in commands:
        raise HTTPException(status_code=404, detail=f"Service '{service_id}' not found")
    
    # Check if service is already running
    status = get_PID(service_id)
    if status != -1:
        raise HTTPException(status_code=400, detail=f"Service '{service_id}' is already running")
    
    service_config:dict = commands[service_id]
    command:str = service_config["command"]
    working_dir:str = service_config.get("workingDirectory", ".")
    env_vars:dict = service_config.get("env", {})
    
    try:
        # Prepare environment variables
        env = os.environ.copy()
        env.update(env_vars)
        
        # Expand working directory if it starts with ~
        if working_dir.startswith("~"):
            working_dir = os.path.expanduser(working_dir)
        # Start the process
        if os.name == 'nt':
            command = command.replace("&&", "&")
        else:
            command = f'/bin/bash -c "source /root/truyencv/.venv/bin/activate && {command.replace("uv", "/root/.local/bin/uv")}"'
        process = subprocess.Popen(
            command,
            shell=True,
            cwd=working_dir,
            env=env,
            # stdout=subprocess.PIPE,
            # stderr=subprocess.PIPE,
            text=True
        )
        
        services[service_id] = process
        PROCESSPID[process.pid] = psutil.Process(process.pid)
        CREATETIME[process.pid] = time.time()
        logger.info(f"Started service '{service_id}' with PID {process.pid}")
        
        return {
            "message": f"Service '{service_id}' started successfully",
            "pid": process.pid,
            "service_id": service_id
        }
        
    except Exception as e:
        logger.error(f"Error starting service '{service_id}': {e}")
        raise HTTPException(status_code=500, detail=f"Failed to start service: {str(e)}")

@app.post("/services/{service_id}/stop")
async def stop_service(service_id: str):
    """Stop a service"""
    if service_id not in services:
        raise HTTPException(status_code=404, detail=f"Service '{service_id}' is not running")
    if services[service_id] is None:
        raise HTTPException(status_code=404, detail=f"Service '{service_id}' is not running")
    try:
        process: subprocess.Popen | None = services[service_id]
        SVkill(process)
        services[service_id] = None
        logger.info(f"Stopped service '{service_id}'")
        
        return {
            "message": f"Service '{service_id}' stopped successfully",
            "service_id": service_id
        }
        
    except Exception as e:
        logger.error(f"Error stopping service '{service_id}': {e}")
        raise HTTPException(status_code=500, detail=f"Failed to stop service: {str(e)}")

@app.post("/services/{service_id}/restart")
async def restart_service(service_id: str):
    """Restart a service"""
    try:
        # Try to stop the service if it's running
        try:
            await stop_service(service_id)
        except HTTPException:
            pass  # Service might not be running
        
        # Start the service
        result = await start_service(service_id)
        result["message"] = f"Service '{service_id}' restarted successfully"
        return result
        
    except Exception as e:
        logger.error(f"Error restarting service '{service_id}': {e}")
        raise HTTPException(status_code=500, detail=f"Failed to restart service: {str(e)}")
system_info = {
        "cpu_count": "loading...",
        "cpu_percent": "loading...",
        "cpu_freq": "loading...",
        "memory": {
            "total": ram.total,
            "available":  "loading...",
            "used":  "loading...",
            "percent":  "loading..."
        },
        "disk": {
            "total": disk.total,
            "used": "loading...",
            "free": "loading...",
            "percent": "loading...",
            "path": disk.path,
        },
        "timestamp": "loading..."
    }
def get_system_info():
    global WSisOn,system_info
    while True:
        if WSisOn:
            system_info["cpu_count"] = cpu.count()
            system_info["cpu_percent"] = cpu.percent()
            system_info["cpu_freq"] = cpu.freq()
            system_info["memory"] = {
                "total": ram.total,
                "available": ram.available(),
                "used": ram.used(),
                "percent": ram.percent()
            }
            system_info["disk"] = {
                "total": disk.total,
                "used": disk.used(),
                "free": disk.available(),
                "percent": disk.percent(),
                "path": disk.path,
            }
            system_info["timestamp"] = datetime.now().strftime("%H:%M:%S %d-%m-%Y ")
        else:
            system_info["cpu_count"] = "loading..."
            system_info["cpu_percent"] = "loading..."
            system_info["cpu_freq"] = "loading..."
            system_info["memory"] = {
                "total": ram.total,
                "available": "loading...",
                "used": "loading...",
                "percent": "loading..."
            }
            system_info["disk"] = {
                "total": disk.total,
                "used": "loading...",
                "free": "loading...",
                "percent": "loading...",
                "path": disk.path,
            }
            system_info["timestamp"] = "loading..."
        time.sleep(1)  # Update every 1 second
threading.Thread(target=get_system_info, daemon=True).start()
        
@app.websocket("/ws")
async def websocket_endpoint(websocket: WebSocket):
	"""WebSocket endpoint for real-time data streaming"""
	await websocket.accept()
	global WSisOn
	logger.info("WebSocket connection established")
	try:
		while True:
			WSisOn = True
			commands_data = load_commands()
			commands:dict = commands_data.get("commands", {})
            
			services_data = []
			total_cpu = 0
			total_ram_mb = 0
			total_ram_percent = 0
            
			for service_id, config in commands.items():
                # Get service status
				Popen: subprocess.Popen | None = services.get(service_id)
				process_info = None
				if Popen is not None and Popen.poll() is None:  # Process is still running
					process_info = PROCESSINFO.get(Popen.pid, None)
					if process_info is not None:
						total_cpu += process_info.get("cpu_percent", 0)
						total_ram_mb += process_info.get("ram_mb", 0)
						total_ram_percent += process_info.get("ram_percent", 0)
				service_data = {
                    "id": service_id,
                    "name": config.get("name", service_id),
                    "workingDirectory": config.get("workingDirectory", "."),
                    "process_info": process_info,
                }
				services_data.append(service_data)
            # Send combined data
			message = {
                "type": "data_update",
                "services": services_data,
                "totals": {
                    "cpu_percent": round(total_cpu, 1),
                    "ram_mb": round(total_ram_mb, 1),
                    "ram_percent": round(total_ram_percent, 1)
                },
                "system_info": system_info,
                "timestamp": datetime.now().isoformat()
            }
            
			await websocket.send_text(json.dumps(message))
			await asyncio.sleep(1)  # Update every 500ms for faster response
	except Exception as e:
		logger.error(f"WebSocket error: {e}")
	finally:
		WSisOn = False
		logger.info("WebSocket connection closed")
uvicorn.run(app, host="0.0.0.0", port=9099)
