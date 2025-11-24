import os

from flask import Flask
if os.name == 'nt':
    from utils.cpu.windows import Windows as cpu
    from utils.ram.windows import Windows as ram
    from utils.disk.windows import Windows as disk

else:
    from utils.cpu.linux import Linux as cpu
    from utils.ram.linux import Linux as ram
    from utils.disk.linux import Linux as disk
test = Flask(__name__)
cpu.init()

@test.get("/")
def read_root():
    cpu_percent = cpu.percent()
    print(f"CPU Percent: {cpu_percent}")
    return str(cpu_percent)
    # return cpu.count()
    # return os.cpu_count()
    # return utils.cpu.windows.Windows.get_cpu_count()
@test.get("/ram")
def read_ram():
    ram_percent = ram.percent() 
    print("ram Percent:", ram_percent)
    return str(ram_percent)
@test.get("/disk")
def read_disk():
    disk_percent = disk.percent()
    print("disk Percent:", disk_percent)
    return str(disk_percent)
if __name__ == "__main__":
    test.run(host="0.0.0.0", port=8000,debug=True)