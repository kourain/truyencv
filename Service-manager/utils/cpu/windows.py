from typing import override
import psutil
from utils.cpu.type import cpu
class Windows(cpu):
    interval=0.5
    @override
    @staticmethod
    def get_system_metrics():
        cpu_percent = psutil.cpu_percent()  # Remove interval for non-blocking call
        return {
            "cpu_percent": round(cpu_percent, 1),
        }
    @override
    @staticmethod
    def freq() -> int:
        return psutil.cpu_freq().current

    @override
    @staticmethod
    def percent()->float:
        return round(psutil.cpu_percent(interval=Windows.interval), 1)
