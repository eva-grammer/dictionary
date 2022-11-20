using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class FileHandle
    {
        
        const string regStr = @"\*\*[A-Za-z]+\*\*\x20*\\\[.+?\]";
        const string regStr1 = @"\*\*[A-Za-z]+\*\*\x20";
         internal static void ReplaceOneFile(string fileName)
        {
            var content = System.IO.File.ReadAllText(fileName);


            var matchs = Regex.Matches(content, @regStr);
            foreach (Match item in matchs)
            {
                var match = Regex.Match(item.Value, regStr1);
                var word = match.Value.Trim('*', ' ');
                var link = $"[英音](https://dict.youdao.com/dictvoice?audio={word}\\&type=1)  [美音](https://dict.youdao.com/dictvoice?audio={word}\\&type=2) ";
                content = content.Replace(item.Value, $"{item.Value} {link}");
            }

            System.IO.File.WriteAllText(fileName, content);
        }
    }
}
