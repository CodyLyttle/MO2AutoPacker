﻿namespace MO2AutoPacker.Library.Messages;

public enum PathKey
{
    ModOrganizerRoot,
    ModOrganizerProfiles
}

public class PathChangedMessage
{
    public PathChangedMessage(PathKey key, string path)
    {
        Key = key;
        Path = path;
    }

    public PathKey Key { get; }
    public string Path { get; }
}