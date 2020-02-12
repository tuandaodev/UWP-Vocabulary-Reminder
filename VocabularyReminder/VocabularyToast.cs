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
                Mp3.preloadMp3File(_item);
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
            string _Ipa = _item.Ipa;
            if (_item.Ipa != _item.Ipa2) {
                _Ipa = _item.Ipa + " " + _item.Ipa2;
            }
            
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
                                Text = _item.Define,
                            },

                            new AdaptiveText()
                            {
                                Text = _item.Example,
                            },

                            new AdaptiveText()
                            {
                                Text = _item.Example2,
                            },

                            new AdaptiveGroup()
                            {
                                Children =
                                {
                                    new AdaptiveSubgroup()
                                    {
                                        Children =
                                        {
                                            new AdaptiveText()
                                            {
                                                Text = _item.Word,
                                                HintStyle = AdaptiveTextStyle.Base
                                            },
                                            new AdaptiveText()
                                            {
                                                Text = _Ipa,
                                                HintStyle = AdaptiveTextStyle.Base
                                            },
                                            new AdaptiveText()
                                            {
                                                Text = _item.Type,
                                                HintStyle = AdaptiveTextStyle.CaptionSubtle
                                            }
                                        }
                                    },
                                    new AdaptiveSubgroup()
                                    {
                                        Children =
                                        {
                                            new AdaptiveText()
                                            {
                                                Text = _item.Translate,
                                                HintStyle = AdaptiveTextStyle.Base
                                            },
                                            new AdaptiveText()
                                            {
                                                Text = _item.Example,
                                                HintStyle = AdaptiveTextStyle.Base
                                            },
                                            new AdaptiveText()
                                            {
                                                Text = _item.Example2,
                                                HintStyle = AdaptiveTextStyle.CaptionSubtle
                                            }
                                        }
                                    },
                                }
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
                            new ToastButton("\u25B6", new QueryString()
                            {
                                { "WordId", _item.Id.ToString() },
                                { "action", "play" },
                                { "url", _item.PlayURL.ToString() }
                            }.ToString()) {
                                ActivationType = ToastActivationType.Background,
                                ActivationOptions = new ToastActivationOptions()
                                {
                                    AfterActivationBehavior = ToastAfterActivationBehavior.PendingUpdate
                                }
                            },
                            new ToastButton("\u25B7", new QueryString()
                            {
                                { "WordId", _item.Id.ToString() },
                                { "action", "play" },
                                { "url", _item.PlayURL2.ToString() }
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
