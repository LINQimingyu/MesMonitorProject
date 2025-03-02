using System.Windows;
using MesProject.Animation;

namespace MesProject.AttachedProperties
{
    public class AnimationAttachedProperties
    {
        public static bool GetFadeInAndSlideFromBottom(DependencyObject obj)
        {
            return (bool)obj.GetValue(FadeInAndSlideFromBottomProperty);
        }

        public static void SetFadeInAndSlideFromBottom(DependencyObject obj, bool value)
        {
            obj.SetValue(FadeInAndSlideFromBottomProperty, value);
        }

        public static readonly DependencyProperty FadeInAndSlideFromBottomProperty =
            DependencyProperty.RegisterAttached(
                "FadeInAndSlideFromBottom",
                typeof(bool),
                typeof(AnimationAttachedProperties),
                new PropertyMetadata(OnFadeInAndSlideFromBottomPropertyChanged)
            );

        private static void OnFadeInAndSlideFromBottomPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            if (!(d is FrameworkElement element))
                return;

            element.Visibility = Visibility.Visible;

            bool value = (bool)e.NewValue;
            if (value)
            {
                element.FadeInAndSlideFromBottom(0.3f);
            }
            else
            {
                element.FadeOutAndSlideToBottom(0.3f);
            }
        }
    }
}
