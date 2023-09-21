namespace Shared;

public class QosConfig
{
    public int PreFetchSize { get; set; } = 0;

    public int PreFetchCount { get; set; } = 1;

    public bool Global { get; set; } = false;

    public QosConfig()
    {
    }

    public QosConfig(int preFetchSize, int preFetchCount, bool global)
    {
        PreFetchSize = preFetchSize;
        PreFetchCount = preFetchCount;
        Global = global;
    }
}
