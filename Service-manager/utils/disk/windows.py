from typing import override

import psutil
from utils.disk.type import disk

class Windows(disk):
    @override
    @staticmethod
    def init():
        Windows.path = 'C:\\'
        Windows.set_total()

    # @staticmethod
    # def init():
    #     memory = psutil.disk_usage('/')
    #     disk.total = memory.total
        
    # @override
    # @staticmethod
    # @classproperty
    # def used() -> int:
    #     return psutil.disk_usage('/').used
        