using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsCE.Forms;
using System.Drawing;

namespace Mobile
{
    /// <summary>
    /// This class provides methods for showing notifications
    /// </summary>
    public class NotificationHelper
    {

        #region Private

        private static readonly int defaultDuration = 10;

        private static void OnNotificationChanged(object sender, BalloonChangedEventArgs e)
        {
            if (e.Visible == false)
                (sender as Notification).Dispose();
        }

        #endregion

        /// <summary>
        /// Shows a notification on screen
        /// </summary>
        /// <param name="caption">The title</param>
        /// <param name="text">The text to show</param>
        /// <param name="critical">True if critical</param>
        /// <param name="duration">How much the notification will remain on screen</param>
        /// <param name="icon"></param>
        /// <param name="onChanged"></param>
        public static void ShowNotification(String caption, String text, bool critical, int duration, Icon icon, BalloonChangedEventHandler onChanged)
        {
            Notification notification = new Notification();
            notification.BalloonChanged += OnNotificationChanged;
            if (onChanged != null)
                notification.BalloonChanged += onChanged;

            notification.Caption = caption;
            notification.Text = text;
            notification.Critical = critical;
            notification.InitialDuration = duration;
            notification.Visible = true;
        }

        /// <summary>
        /// Shows a notification on screen
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="text"></param>
        /// <param name="duration"></param>
        /// <param name="onChanged"></param>
        public static void ShowNotification(String caption, String text, int duration, BalloonChangedEventHandler onChanged)
        {
            ShowNotification(caption, text, false, duration, null, onChanged);
        }

        /// <summary>
        /// Shows a notification on screen
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="text"></param>
        public static void ShowNotification(String caption, String text)
        {
            ShowNotification(caption, text, defaultDuration, null);
        }

        
    }
}
