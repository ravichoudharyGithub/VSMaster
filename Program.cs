using System;

namespace VSMaster
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome in VSMaster....");
            //GetPageWords.ParseWithAngleSharp("TestVSMaster.txt");
            //DisplayText.GetDisplayText("TestVSMaster.txt");
            var res = GetLinksAndText.GetLinks("TestVSMaster.txt");
            //GetPageWords.ParseWithAngleSharp("F://Matrix.html");
            //var data = System.IO.File.ReadAllText("Google/test-3.html");
            //var urls = GetGoogleAdWords.Scrape(data);
        }
    }
}
