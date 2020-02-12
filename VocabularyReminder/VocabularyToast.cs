using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using DataAccessLibrary;
using VocabularyReminder.Services;

namespace VocabularyReminder
{
    public class VocabularyToast
    {
        const string viewDicOnlineUrl = "https://www.oxfordlearnersdictionaries.com/definition/english/";
        public static void loadByVocabulary(Vocabulary _item)
        {
            if (_item.Id == 0)
            {
                Helper.ShowToast("Chưa có dữ liệu từ điển. Vui lòng import.");
                return;
            }
            ToastContent content;
            if (String.IsNullOrEmpty(_item.PlayURL))
            {
                content = getToastContentWithoutPlay(_item);
            } else
            {
                content = getToastContent(_item);
            }
            
            var _toastItem = new ToastNotification(content.GetXml())
            {
                Tag = "Vocabulary",
                Group = "Reminder",
            };
            ToastNotificationManager.CreateToastNotifier().Show(_toastItem);
        }

        private static ToastContent getToastContent(Vocabulary _item)
        {
            ToastContent content = new ToastContent()
            {
                Launch = "vocabulary-reminder",
                Audio = new ToastAudio() { Silent = true },
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = _item.Word,
                                HintMaxLines = 1
                            },

                            new AdaptiveText()
                            {
                                Text = _item.Ipa,
                            },

                            new AdaptiveText()
                            {
                                Text = _item.Translate
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
                                { "WordId", _item.Id.ToString() },
                                { "action", "play" },
                                { "url", _item.PlayURL.ToString() }
                            }.ToString())
                            {
                                ActivationType = ToastActivationType.Background,
                                ActivationOptions = new ToastActivationOptions()
                                {
                                    AfterActivationBehavior = ToastAfterActivationBehavior.PendingUpdate
                                }
                            },
                            new ToastButton("Next", new QueryString()
                            {
                                { "WordId", _item.Id.ToString() },
                                { "action", "next" },
                            }.ToString())
                            {
                                ActivationType = ToastActivationType.Background,
                                ActivationOptions = new ToastActivationOptions()
                                {
                                    AfterActivationBehavior = ToastAfterActivationBehavior.PendingUpdate
                                }
                            },
                            new ToastButton("View", new QueryString()
                            {
                                    { "action", "view" },
                                    { "url", viewDicOnlineUrl + _item.Word }

                             }.ToString()),
                            new ToastButton("Close", "dismiss")
                            {
                                ActivationType = ToastActivationType.Background
                            },
                        }
                },

            };

            return content;
        }

        private static ToastContent getToastContentWithoutPlay(Vocabulary _item)
        {
            ToastContent content = new ToastContent()
            {
                Launch = "vocabulary-reminder",
                Audio = new ToastAudio() { Silent = true },
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = _item.Word,
                                HintMaxLines = 1
                            },

                            new AdaptiveText()
                            {
                                Text = _item.Ipa,
                            },

                            new AdaptiveText()
                            {
                                Text = _item.Translate
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
                            new ToastButton("Next", new QueryString()
                            {
                                { "WordId", _item.Id.ToString() },
                                { "action", "next" },
                            }.ToString())
                            {
                                ActivationType = ToastActivationType.Background,
                                ActivationOptions = new ToastActivationOptions()
                                {
                                    AfterActivationBehavior = ToastAfterActivationBehavior.PendingUpdate
                                }
                            },
                            new ToastButton("View", new QueryString()
                                {
                                    { "action", "view" },
                                    { "url", viewDicOnlineUrl + _item.Word }

                                }.ToString()),
                            new ToastButton("Skip", "dismiss")
                            {
                                ActivationType = ToastActivationType.Background
                            },
                        }
                },

            };

            return content;
        }
    }

}
