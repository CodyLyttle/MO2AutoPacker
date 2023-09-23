using MO2AutoPacker.Library.Models;

namespace MO2AutoPacker.Library.Messages;

public class ProfileChangedMessage
{
    public Profile? Profile { get; }

    public ProfileChangedMessage(Profile? profile)
    {
        Profile = profile;
    }
}