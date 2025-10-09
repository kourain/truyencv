from typing import override
from utils.disk.type import disk

class Linux(disk):
    @override
    @staticmethod
    def init():
        Linux.path = '/'
        Linux.set_total()

    # @staticmethod
    # def init():
    #     memory = psutil.disk_usage('/')
    #     disk.total = memory.total
        
    # @override
    # @staticmethod
    # @classproperty
    # def used() -> int:
    #     return psutil.disk_usage('/').used