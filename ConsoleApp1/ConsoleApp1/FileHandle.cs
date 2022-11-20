using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class FileHandle
    {

        //const string regStr = @"\*\*[A-Za-z]+\*\*\x20*\\\[.+?\]";
        //const string regStr1 = @"\*\*[A-Za-z]+\*\*\x20";
         const string regStr = @"\S+\x20*\\\[.+?\][^\u4e00-\u9fa5]+";
        
        const string regStr1 = @"\x20*\\\[.+?\]";
        internal static void ReplaceOneFile(string fileName)
        {
            var content = System.IO.File.ReadAllText(fileName);


            var matchs = Regex.Matches(content, @regStr);
            foreach (Match item in matchs)
            {
                var match = Regex.Match(item.Value, regStr1);
                var yinbiao= match.Value.Trim('*', ' ');
                var word =  item.Value.Substring(0, match.Index).Trim();
                yinbiao = item.Value.Substring( match.Index);
                var link = $"[英音](https://dict.youdao.com/dictvoice?audio={word}\\&type=1)  [美音](https://dict.youdao.com/dictvoice?audio={word}\\&type=2) ";
                content = content.Replace(item.Value, $"**{word}** {yinbiao} {link}")
                    
                    ;
            }
            content = content
                   .Replace("助记：", "**助记：**")
                     .Replace("词源：", "**词源：**")
                       .Replace("释义：", "**释义：**")
                             .Replace("解释：", "**解释：**")
                                .Replace("【课堂笔记】", "**【课堂笔记】**")
                                       .Replace("【 课堂笔记】", "**【课堂笔记】**")
                    .Replace("搭配：", "**搭配：**");
            System.IO.File.WriteAllText(fileName, content);
        }
    }
}
