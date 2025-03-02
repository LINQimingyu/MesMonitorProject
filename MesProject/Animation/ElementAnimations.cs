using System.Windows;
using System.Windows.Media.Animation;

namespace MesProject.Animation
{
    public static class ElementAnimations
    {
        public static void FadeInAndSlideFromBottom(this FrameworkElement element, float seconds)
        {
            var storyboard = new Storyboard();
            storyboard.AddFadeIn(seconds);
            double offset = element.ActualHeight > 0 ? element.ActualHeight : 50;
            storyboard.AddSlideFromBottom(seconds, offset);
            storyboard.Begin(element);
        }

        public static void FadeOutAndSlideToBottom(this FrameworkElement element, float seconds)
        {
            var storyboard = new Storyboard();
            storyboard.AddFadeOut(seconds);
            double offset = element.ActualHeight > 0 ? element.ActualHeight : 50;
            storyboard.AddSlideToBottom(seconds, offset);
            storyboard.Completed += (s, e) =>
            {
                element.Visibility = Visibility.Hidden;
            };
            storyboard.Begin(element);
        }
    }
}
