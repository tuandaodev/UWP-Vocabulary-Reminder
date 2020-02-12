using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace VocabularyReminder
{
    public class Vocabulary
    {
        public static string word;

        public Vocabulary()
        {

        }


        public static void load(string word)
        {
            string mp3Url = "";
            if (word == "Education")
            {
                mp3Url = "https://www.oxfordlearnersdictionaries.com/media/english/uk_pron/e/edu/educa/education__gb_4.mp3";
            } else
            {
                mp3Url = "https://www.oxfordlearnersdictionaries.com/media/english/uk_pron/h/hom/home_/home__gb_1.mp3";
            }

            ToastContent content = new ToastContent()
            {
                //Launch = "vocabulary-reminderxxx",
                Audio = new ToastAudio() { Silent = true },
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = word,
                                HintMaxLines = 1
                            },

                            new AdaptiveText()
                            {
                                Text = "/həʊm/"
                            },

                            new AdaptiveText()
                            {
                                Text = "Nhà cửa"
                            }
                        },
                        HeroImage = new ToastGenericHeroImage()
                        {
                            Source = "https://picsum.photos/364/180?image=1043"
                        },

                    }
                },
                Scenario = ToastScenario.Reminder,
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                        {
                            new ToastButton("Play", new QueryString()
                            {
                                { "word", "home" },
                                { "action", "play" },
                                { "url", mp3Url }
                            }.ToString())
                            {
                                //ActivationType = ToastActivationType.Foreground,
                                ActivationType = ToastActivationType.Background,
                                ActivationOptions = new ToastActivationOptions()
                                {
                                    AfterActivationBehavior = ToastAfterActivationBehavior.PendingUpdate
                                }
                            },
                            new ToastButton("Skip", "action=viewdetails&contentId=351")
                            {
                                ActivationType = ToastActivationType.Foreground
                            },

                            new ToastButton("View", new QueryString()
                                {
                                    { "action", "view" },
                                    { "url", "https://www.oxfordlearnersdictionaries.com/definition/english/home_1?q=home" }

                                }.ToString())
                        }
                },

            };

            var item = new ToastNotification(content.GetXml())
            {
                Tag = "Vocabulary",
                Group = "Reminder",
            };
            ToastNotificationManager.CreateToastNotifier().Show(item);
        }
    }

}
