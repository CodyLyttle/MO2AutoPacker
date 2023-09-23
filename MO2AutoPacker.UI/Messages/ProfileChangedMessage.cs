using MO2AutoPacker.UI.Models;

namespace MO2AutoPacker.UI.Messages;

public class ProfileChangedMessage
{
    public Profile? Profile { get; }

    public ProfileChangedMessage(Profile? profile)
    {
        Profile = profile;
    }
}