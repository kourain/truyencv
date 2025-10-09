import psutil
from typing import override
from utils.cpu.type import cpu, cpuinfo
import os
class Linux(cpu):
	@override
	@staticmethod
	def init():
		for i in range(Linux.count()):
			max_freq = int(psutil.cpu_freq(percpu=True)[i].max)
			min_freq = int(psutil.cpu_freq(percpu=True)[i].min)
			Linux.rs[i] = cpuinfo(min_freq, max_freq, 0)
			Linux.total += int(max_freq)
   
	@override
	@staticmethod
	def get_system_metrics():
		for i in range(Linux.count()):
			with open(f"/sys/devices/system/cpu/cpu{i}/cpufreq/scaling_cur_freq") as f:
				current = f.read().strip()
			Linux.rs[i].current_freq = int(current)
		return Linux.rs

	@override
	@staticmethod
	def freq() -> int:
		"""Returns the current CPU frequency."""
		try:
			return int(psutil.cpu_freq().current)
		except Exception as e:
			return cpu.freq()
	@override
	@staticmethod
	def percent()->float:
		"""Returns the CPU metrics for each logical CPU."""
		try:
			return psutil.cpu_percent(interval=1, percpu=False)
		except Exception as e:
			current = 0
			for i in range(Linux.count()):
				current += Linux.get_system_metrics()[i].current_freq
			return round(current/Linux.total * 100,1)  # Return as a percentage of total frequency
