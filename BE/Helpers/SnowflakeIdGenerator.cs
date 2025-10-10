namespace TruyenCV.Helpers;
public class SnowflakeIdGenerator
{
    private static readonly DateTime Epoch = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static readonly object[] _lock = new object[1024];
    private static readonly ulong[] _lastTimestamp = new ulong[1024];
    private static readonly ulong[] _sequence = new ulong[4096];

    private const int MachineIdBits = 10;
    private const int SequenceBits = 12;

    private const ulong MaxMachineId = 1023;
    private const ulong MaxSequence = 4095;

    private const int MachineIdShift = 12;
    private const int TimestampShift = 22;
	public static void Init()
	{
		for (int i = 0; i < _lock.Length; i++)
		{
			_lock[i] = new object();
		}
	}
    public static ulong NextId(ulong machineId = 0)
    {
        if (machineId < 0 || machineId > MaxMachineId)
            throw new ArgumentException($"MachineId phải nằm trong khoảng 0 - {MaxMachineId}");
        lock (_lock[machineId])
        {
            var timestamp = GetCurrentTimestamp();
            if (timestamp == _lastTimestamp[machineId])
            {
                _sequence[machineId] = (_sequence[machineId] + 1) & MaxSequence;
            }
            else
            {
                _sequence[machineId] = 0;
            }

            _lastTimestamp[machineId] = timestamp;

            return ((timestamp << TimestampShift) |
                    (machineId << MachineIdShift) |
                    _sequence[machineId]);
        }
    }

    private static ulong GetCurrentTimestamp()
        => (ulong)(DateTime.UtcNow - Epoch).TotalMilliseconds;
}
