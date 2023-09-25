using MO2AutoPacker.Library.Models;

namespace MO2AutoPacker.Library.Messages;

public class ProfileChangedMessage
{
    public ProfileChangedMessage(Profile? profile)
    {
        Profile = profile;
    }

    public Profile? Profile { get; }
}