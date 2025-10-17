using Microsoft.CodeAnalysis.Elfie.Extensions;

namespace TruyenCV.Helpers;
public class SnowflakeIdGenerator
{
    private static readonly DateTime Epoch = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static readonly object[] _lock = new object[1023];
    private static readonly ulong[] _lastTimestamp = new ulong[1023];
    private static readonly ulong[] _sequence = new ulong[4096];

    private const uint MachineIdBits = 10;
    private const uint SequenceBits = 12;

    private const ulong MaxMachineId = 1023;
    private const ulong MaxSequence = 4095;
    private static uint MachineId = 0;
    private const int MachineIdShift = 12;
    private const int TimestampShift = 22;
    public static void Init(uint _machineId)
    {
        MachineId = _machineId;
        for (uint i = 0; i < 1023; i++)
        {
            _lock[i] = new object();
        }
    }
    public static long NextId()
    {
        if (MachineId < 0 || MaxMachineId - MachineId < 0)
            throw new UserRequestException($"MachineId phải nằm trong khoảng 0 - {MaxMachineId}");
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

            return (long)((timestamp << TimestampShift) | (MachineId << MachineIdShift) | _sequence[MachineId]);
        }
    }

    private static ulong GetCurrentTimestamp()
        => (ulong)(DateTime.UtcNow - Epoch).TotalMilliseconds;
}
