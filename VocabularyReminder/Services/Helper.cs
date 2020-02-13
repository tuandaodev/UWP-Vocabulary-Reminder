using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace VocabularyReminder.Services
{
    class Helper
    {
        public static void ClearToast()
        {
            ToastNotificationManager.History.Clear();
        }

        //public static void ShowToast(string msg, string subMsg = null)
        //{
        //    if (subMsg == null) subMsg = "";

        //    Debug.WriteLine(msg + "\n" + subMsg);

        //    var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

        //    var toastTextElements = toastXml.GetElementsByTagName("text");
        //    toastTextElements[0].AppendChild(toastXml.CreateTextNode(msg));
        //    toastTextElements[1].AppendChild(toastXml.CreateTextNode(subMsg));

        //    //ToastNotificationManager.History.Clear();
        //    var toast = new ToastNotification(toastXml);
        //    ToastNotificationManager.CreateToastNotifier().Show(toast);
        //}


        public static void ShowToast(string msg, string subMsg = null)
        {
            if (subMsg == null) subMsg = "";

            Debug.WriteLine(msg + "\n" + subMsg);

            ToastContent content = getToastContent(msg, subMsg);

            var _toastItem = new ToastNotification(content.GetXml())
            {
                Tag = "Vocabulary",
                Group = "Notification",
            };
            ToastNotificationManager.CreateToastNotifier().Show(_toastItem);
        }

        private static ToastContent getToastContent(string msg, string subMsg = null)
        {
            if (subMsg == null) subMsg = "";
            ToastContent content = new ToastContent()
            {
                Launch = "vocabulary-notification",
                Audio = new ToastAudio() { Silent = true },
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = msg,
                            },

                            new AdaptiveText()
                            {
                                Text = subMsg,
                            },
                        },
                    },
                },
            };

            return content;
        }

    }
}
