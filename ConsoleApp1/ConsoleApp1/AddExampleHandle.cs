using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class AddExampleHandle
    {

      
        const string regStr1 = @"\*\*[A-Za-z]+\*\*\x20";
        const string regStr = @"\*\*\S+\x20*\\\[.+?\][^\u4e00-\u9fa5]+";


        internal static void AddExample(string fileName)
        {
            var content = System.IO.File.ReadAllText(fileName);


            var matchs = Regex.Matches(content, @regStr, RegexOptions.RightToLeft);
            var totalCount = matchs.Count;

            for (int i = 0; i < totalCount; i++)
            {
                var m = matchs[i];
                var match = Regex.Match(m.Value, regStr1);
                var word = match.Value.Trim('*', ' ');
                if (string.IsNullOrWhiteSpace(word))
                {
                    continue;
                }
                var examples = new YouDaoParse().Parse(word).Result;
                if (examples.Count > 0)
                {
                    string text = "\n\n\n\n";
                    foreach (var item in examples)
                    {
                        text += $"**{item.ExampleType}:** {item.Example}";
                        if (!string.IsNullOrEmpty(item.Audio))
                        {
                            text += $" [播放]({item.Audio})";
                        }
                        text += "\n\n";
                        if (!string.IsNullOrWhiteSpace(item.Translate))
                        {
                            text += $"{item.Translate} \n\n";
                        }
                     

                    }
                    int position = -1;
                    if (i == 0)
                    {

                          position = findPosition(content);
                    }
                    else
                    { 
                        position= matchs[i-1].Index;
                    }
                    position = findLastPosition(content, position);
                   content = content.Insert(position - 2, text);
                }
            }
            var d=fileName.LastIndexOf("\\");
            
            var path = fileName.Substring(0, d + 1);
            var d2 = fileName.LastIndexOf(".");
            var name = fileName.Substring(d + 1, d2-d-1);
            var newName = name + "_含例句";
            var newPath = path + newName + ".md";
            System.IO.File.WriteAllText(newPath, content);
        }

        private static int findPosition(string content)
        {

            var index = content.IndexOf("思维导图");
          
            return index;
        }

        private static int findLastPosition(string content,int startindex)
        {


            var index = content.LastIndexOf("***", startindex);
            return index;
        }
    }
}
