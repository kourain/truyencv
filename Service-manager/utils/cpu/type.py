from abc import abstractmethod,ABC
import os

class cpuinfo:
	def __init__(self,  min_freq=0, max_freq=0,current_freq=0):
		self.max_freq = max_freq
		self.min_freq = min_freq
		self.current_freq = current_freq

class cpu(ABC):
    rs:dict[int,cpuinfo] = {}
    total:int = 0
    @staticmethod
    @abstractmethod
    def init():
        pass
    
    @staticmethod
    @abstractmethod
    def get_system_metrics()->dict:
        pass
    
    @staticmethod
    @abstractmethod
    def percent()->float:
        pass
    @staticmethod
    @abstractmethod
    def freq()->int:
        """Returns the current CPU frequency."""
        if not cpu.rs:
            return 0
        else:
            current = 0
            for i in cpu.rs:
                current += cpu.rs[i].current_freq
            return current / cpu.count() / 1000  # Return in MHz
    @staticmethod
    def count()-> int:
        """Returns the number of logical CPUs in the system."""
        return os.cpu_count() or 1  # Default to 1 if os.cpu_count() returns None
    