using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using SharpHttp.HttpClientFactory;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp1.DbHelper;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Web;

namespace ConsoleApp1
{
    class YouDaoParse
    {

        static string wordUrl = "https://www.youdao.com/result?word=lj%3A{0}&lang=en";

        public async Task<List<ExampleEntity>> Parse(string word)
        {
            if (string.IsNullOrWhiteSpace(word))

            {
                throw new Exception($"{ nameof(word)} 不能为空");
            }

            word = word.Trim();
           
          return await ParseExample(word); 
        }

        private async Task<List<ExampleEntity>> ParseExample(string word)
        {

            return await requestContent(string.Format(wordUrl, word));



        }

        private async Task<List<ExampleEntity>> requestContent(string url)
        {
            List<ExampleEntity> examples = new List<ExampleEntity>();
            ISharpHttpClientFactory factory = new SharpHttpClientFactory();
            var client = factory.CreateHttpClient();
            var t = await client.SendAsync(url, HttpMethod.Get, null, "text/html");
            IHtmlDocument document = null;
            if (t.IsSuccessStatusCode)
            {
                string c = t.Content.ReadAsStringAsync().Result;
                var parser = new HtmlParser();
                document = parser.ParseDocumentAsync(c).Result;
                var scripts = document.Scripts.Where(v => string.IsNullOrEmpty(v.Source));
                foreach (var item in scripts)
                {
                    var src = item.InnerHtml;
                    var startStr = "layout:\"search\",data:";
                    var startIndex = src.IndexOf(startStr) + startStr.Length;
                    var endIndex = src.IndexOf(",fetch:{}");
                    src = src.Substring(startIndex, endIndex - startIndex);

                    examples.AddRange(Parseblngsents(ref src));

                    examples.AddRange(Parsemediatype(ref src));
                    examples.AddRange(ParseAuthor(ref src));
                }

            }
            return examples;
        }


        private List<ExampleEntity> Parseblngsents(ref string src, int total = 3)
        {
            List<ExampleEntity> examples = new List<ExampleEntity>();
            var sentencetype = "双语例句";
            for (int i = 0; i < total; i++)
            {


                var sentenceText = Parsesettence(ref src, "{sentence:\"", "\",");
                if (string.IsNullOrWhiteSpace(sentenceText))
                {
                    break;
                }
                sentenceText = handleUrl(sentenceText);
                var sentenceTrans = Parsesettence(ref src, "\"sentence-translation\":\"", "\",");
                sentenceTrans = handleUrl(sentenceTrans);
                var sentenceAudio = Parsesettence(ref src, "\"sentence-speech\":\"", "\"}");
                sentenceAudio = $"https://dict.youdao.com/dictvoice?audio={sentenceAudio}&le=eng&type=2";
                examples.Add(new ExampleEntity(sentencetype, sentenceText, sentenceTrans, sentenceAudio));
            }
            return examples;
        }
        private List<ExampleEntity> Parsemediatype(ref string src, int total = 3)
        {
            List<ExampleEntity> noAudioexamples = new List<ExampleEntity>();
            List<ExampleEntity> examples = new List<ExampleEntity>();
            List<ExampleEntity> exampleWithTranss = new List<ExampleEntity>();
            var sentencetype = "原声例句";
            for (int i = 0; i < 50; i++)
            {

                
                if (examples.Count >=total)
                {
                    break;
                }
                var sentenceAudio = Parsesettence(ref src, "{streamUrl:\"", "\",");

                sentenceAudio = handleUrl(sentenceAudio);

                //var nextStartIndex = src.IndexOf("@mediatype");
                //var sentenceTrans = Parsesettence(ref src, "chn:\"", "\",", nextStartIndex);
                //sentenceTrans = handleUrl(sentenceTrans);

                var sentenceText = Parsesettence(ref src, "eng:\"", "\"}");

                sentenceText = handleUrl(sentenceText);


                var e = new ExampleEntity(sentencetype, sentenceText, string.Empty, sentenceAudio);
                 

                if (string.IsNullOrWhiteSpace(sentenceAudio))
                {

                    noAudioexamples.Add(e);
                }
                else
                {
                    examples.Add(e);

                }


            }
          
            examples.AddRange(noAudioexamples);
            List<ExampleEntity> result = new List<ExampleEntity>();
           
            foreach (var item in examples)
            {
                if (result.Count == total)
                {
                    break;
                }
                result.Add(item);

            } 
            return result;
        }


        private List<ExampleEntity> ParseAuthor(ref string src, int total = 3)
        {
            List<ExampleEntity> examples = new List<ExampleEntity>();

            var sentencetype = "权威例句";
            for (int i = 0; i < total; i++)
            {
                var sentenceAudio = Parsesettence(ref src, "speech:\"", "\",");
                if (string.IsNullOrWhiteSpace(sentenceAudio))
                {
                    break;
                }
                sentenceAudio = $"https://dict.youdao.com/dictvoice?audio={sentenceAudio}&le=eng&type=2";
                var sentenceText = Parsesettence(ref src, "foreign:\"", "\"");
                sentenceText = handleUrl(sentenceText);
                var sentenceTrans = string.Empty;


                examples.Add(new ExampleEntity(sentencetype, sentenceText, sentenceTrans, sentenceAudio));
            }
            return examples;
        }


        private string handleUrl(string src)
        {

            src = src
                  .Replace("\\u003Cb\\u003E", "**")
                    .Replace("\\u003C\\u002Fb\\u003E", "**")
                        .Replace("\\u003Cbr\\u003E", "\n\n")
                .Replace("<b>", "**")
                .Replace("</b>", "**")
                    .Replace("<br>", "\n\n").Replace("\\u002F", "/");
            return src;
        }
        private string Parsesettence(ref string src, string startStr, string endStr, int nextStartIndex = -1)
        {



            var startIndex = src.IndexOf(startStr);
            if (startIndex < 0)
            {
                return string.Empty;
            }
            startIndex += startStr.Length;
            if (nextStartIndex > 0 && startIndex > nextStartIndex)
            {
                return string.Empty;
            }
            var endIndex = src.IndexOf(endStr, startIndex);
            var result = src.Substring(startIndex, endIndex - startIndex);
            src = src.Substring(endIndex + endStr.Length);
            return result;
        }


    }
}
