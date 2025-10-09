from abc import abstractmethod,ABC
import psutil

class disk(ABC):
    total:int = 0
    path:str
    
    @staticmethod
    @abstractmethod
    def init():
        """
			Initialize the disk class.
			This method should be overridden in subclasses to set the total disk space.
		"""
        pass
    @classmethod
    def set_total(cls):
        cls.total = psutil.disk_usage(cls.path).total

    @classmethod
    @abstractmethod
    def used(cls) -> int:
        return psutil.disk_usage(cls.path).used

    @classmethod
    @abstractmethod
    def available(cls) -> int:
        return cls.total - cls.used()

    @classmethod
    @abstractmethod
    def percent(cls) -> float:
        return round((cls.used() / cls.total) * 100, 1)
