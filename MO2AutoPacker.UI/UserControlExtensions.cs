using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MO2AutoPacker.UI;

public static class UserControlExtensions
{
    public static void SetAnimationDuration(this UserControl control , string storyName, int animIndex, TimeSpan time)
    {
        var storyboard = (Storyboard?) control.Resources[storyName];
        if (storyboard == null)
            throw new ArgumentOutOfRangeException(nameof(storyName),
                $"Bad storyboard name '{storyName}'");

        storyboard.Children[animIndex].Duration = new Duration(time);
    }
}