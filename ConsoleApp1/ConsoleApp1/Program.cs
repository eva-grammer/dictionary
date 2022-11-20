using System;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
      
        static void Main(string[] args)
        {
            var path = @"C:\BaiduNetdiskDownload\markdown\github\dictionary\docs1";
          var files=  System.IO.Directory.GetFiles(path);
            foreach (var item in files)
            {
                var file = new FileInfo(item);
                var name = file.Name;
                if (name.StartsWith("第") && name.EndsWith(".md"))
                {


                 FileHandle.   ReplaceOneFile(item);

                }
            }
            Console.WriteLine("finished...");
            Console.ReadLine();
        }
    }
}
