from abc import abstractmethod,ABC
import psutil

class ram(ABC):
    total:int = 0
    
    @staticmethod
    def used() -> int:
        return psutil.virtual_memory().used

    @staticmethod
    def available() -> int:
        return ram.total - ram.used()

    @staticmethod
    def percent() -> float:
        return round((ram.used() / ram.total) * 100, 1)
ram.total = psutil.virtual_memory().total