using System.Windows;
using System.Windows.Media.Animation;

namespace MesProject.Animation
{
    public static class StoryboardHelpers
    {
        /// <summary>
        /// 淡入动画
        /// </summary>
        /// <param name="storyboard"></param>
        /// <param name="seconds"></param>
        public static void AddFadeIn(this Storyboard storyboard, float seconds)
        {
            var animation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
            };
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// 淡出动画
        /// </summary>
        /// <param name="storyboard"></param>
        /// <param name="seconds"></param>
        public static void AddFadeOut(this Storyboard storyboard, float seconds)
        {
            var animation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
            };
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// 从底部向上滑入动画
        /// </summary>
        /// <param name="storyboard"></param>
        /// <param name="seconds"></param>
        public static void AddSlideFromBottom(
            this Storyboard storyboard,
            float seconds,
            double offset,
            float decelerationRatio = 0.9f
        )
        {
            var animation = new ThicknessAnimation
            {
                From = new Thickness(0, offset, 0, -offset),
                To = new Thickness(0),
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                DecelerationRatio = decelerationRatio,
            };
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// 从顶部向下滑出动画
        /// </summary>
        /// <param name="storyboard"></param>
        /// <param name="seconds"></param>
        public static void AddSlideToBottom(
            this Storyboard storyboard,
            float seconds,
            double offset,
            float decelerationRatio = 0.9f
        )
        {
            var animation = new ThicknessAnimation
            {
                From = new Thickness(0),
                To = new Thickness(0, offset, 0, -offset),
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                DecelerationRatio = decelerationRatio,
            };
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));
            storyboard.Children.Add(animation);
        }
    }
}
