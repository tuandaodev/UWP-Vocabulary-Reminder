using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary;
using System.Diagnostics;

namespace VocabularyReminder.Services
{
    class TranslateService
    {
        public static string mainTranslateUrl = "https://dictionary.cambridge.org/vi/dictionary/english-vietnamese/";
        const string xpath_translate = "(//span[@class='trans dtrans'])[1]";
        const string xpath_ipa = "//span[@class='ipa dipa']";
        const string xpath_type = "(//span[@class='pos dpos'])[1]";

        public static string mainGetPlayUrl = "https://www.oxfordlearnersdictionaries.com/definition/english/";
        const string xpath_mp3 = "//span[@class='phonetics']/*";

        public static async void goTranslateAsync(Vocabulary item)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                //try
                //{
                    string _wordUrl = mainTranslateUrl + item.Word;
                    HttpResponseMessage response = await httpClient.GetAsync(_wordUrl);
                    HttpContent content = response.Content;
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(await content.ReadAsStringAsync());
                    // Get Title
                    var translate = document.DocumentNode.SelectSingleNode(xpath_translate);
                    var type = document.DocumentNode.SelectSingleNode(xpath_type);
                    //var ipa = document.DocumentNode.SelectSingleNode(xpath_ipa);

                    item.Translate = (translate != null) ? translate.InnerText : String.Empty;
                    item.Type = (type != null) ? type.InnerText : String.Empty;
                    //item.Ipa = (ipa != null) ? "/" + ipa.InnerText + "/" : String.Empty;

                    DataAccess.UpdateVocabulary(item);
                //} catch (Exception ex)
                //{
                //    Debug.WriteLine(ex.Message);
                //}
            }
        }

        public static async void goGetPlayURLAsync(Vocabulary item)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                //try
                //{
                string _wordUrl = mainGetPlayUrl + item.Word;
                HttpResponseMessage response = await httpClient.GetAsync(_wordUrl);
                HttpContent content = response.Content;
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(await content.ReadAsStringAsync());
                // Get Title

                var DefineNode = document.DocumentNode.SelectSingleNode("(//span[@class='def'])[1]");
                if (DefineNode != null)
                {
                    item.Define = DefineNode.InnerText;
                }

                var ExampleNodes = document.DocumentNode.SelectNodes("(//ul[@class='examples']//span[@class='x'])[position()<3]");
                if (ExampleNodes != null && ExampleNodes.Count > 0)
                {
                    int _count = 0;
                    foreach (var node in ExampleNodes)
                    {
                        _count++;
                        if (_count == 1)
                        {
                            item.Example = (node != null) ? node.InnerText : "";
                        }
                        else if (_count == 2)
                        {
                            item.Example2 = (node != null) ? node.InnerText : "";
                        }
                    }
                }

                var IpaNodes = document.DocumentNode.SelectNodes("//span[@class='phonetics']//div[contains(@class, 'phon')]");
                if (IpaNodes != null && IpaNodes.Count > 0)
                {
                    int _count = 0;
                    foreach (var node in IpaNodes)
                    {
                        _count++;
                        if (_count == 1)
                        {
                            item.Ipa = (node != null) ? node.InnerText : "";
                        }
                        else if (_count == 2)
                        {
                            item.Ipa2 = (node != null) ? node.InnerText : "";
                        }
                    }
                }

                var soundNodes = document.DocumentNode.SelectNodes("//span[@class='phonetics']/div/div[contains(@class, 'sound')][1]");
                if (soundNodes != null && soundNodes.Count > 0)
                {
                    int _count = 0;
                    foreach (var node in soundNodes)
                    {
                        _count++;
                        if (_count == 1)
                        {
                            item.PlayURL = (node != null) ? node.GetAttributeValue("data-src-mp3", "") : "";
                        } else if (_count == 2)
                        {
                            item.PlayURL2 = (node != null) ? node.GetAttributeValue("data-src-mp3", "") : "";
                        }
                    }
                }

                if (!String.IsNullOrEmpty(item.PlayURL))
                {
                    DataAccess.UpdatePlayURL(item);
                }
                
                //} catch (Exception ex)
                //{
                //    Debug.WriteLine(ex.Message);
                //}
            }
        }
    }
}
