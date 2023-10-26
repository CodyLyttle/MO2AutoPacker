using Microsoft.Extensions.Logging;

namespace MO2AutoPacker.Library.Logging;

public static class Logger
{
    public static ILogger Current { get; internal set; } = new DummyLogger();
}