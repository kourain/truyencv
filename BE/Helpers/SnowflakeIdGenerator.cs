namespace TruyenCV.Helpers;
public class SnowflakeIdGenerator
{
    private static readonly DateTime Epoch = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static readonly object[] _lock = new object[1024];
    private static readonly long[] _lastTimestamp = new long[1024];
    private static readonly long[] _sequence = new long[4096];

    private const int MachineIdBits = 10;
    private const int SequenceBits = 12;

    private const long MaxMachineId = 1023;
    private const long MaxSequence = 4095;
    private static int MachineId = 0;
    private const int MachineIdShift = 12;
    private const int TimestampShift = 22;
    public static void Init(int _machineId)
    {
        MachineId = _machineId;
        for (int i = 0; i < _lock.Length; i++)
        {
            _lock[i] = new object();
        }
    }
    public static long NextId()
    {
        if (MachineId < 0 || MaxMachineId - MachineId < 0)
            throw new ArgumentException($"MachineId phải nằm trong khoảng 0 - {MaxMachineId}");
        lock (_lock[MachineId])
        {
            var timestamp = GetCurrentTimestamp();
            if (timestamp == _lastTimestamp[MachineId])
            {
                _sequence[MachineId] = (_sequence[MachineId] + 1) & MaxSequence;
            }
            else
            {
                _sequence[MachineId] = 0;
            }

            _lastTimestamp[MachineId] = timestamp;

            return ((timestamp << TimestampShift) | (MachineId << MachineIdShift) | _sequence[MachineId]);
        }
    }

    private static long GetCurrentTimestamp()
        => (long)(DateTime.UtcNow - Epoch).TotalMilliseconds;
}
