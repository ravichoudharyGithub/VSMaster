using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace VSMaster
{
    class GetGoogleAdWords
    {
        public static List<string> Scrape(string Html)
        {
            var urls = new List<string>();
            var gcc = new GlobusHttpHelper();
            //Html = gcc.getHtmlfromUrl(new Uri("https://www.google.com/search?q=mobile+phones+under+5000&oq=mobile+phone"));
            var hd = new HtmlDocument();
            hd.LoadHtml(Html);
           
            var nodeCommercialUnit = hd.DocumentNode.SelectSingleNode("//div[contains(@class, 'commercial-unit')]");
            if (nodeCommercialUnit.SelectNodes(".//*[@class='pla-unit-container']") != null)
            {
                var nodeUnitContainer = nodeCommercialUnit.SelectNodes(".//*[@class='pla-unit-container']").Descendants().Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "D6nsM");
                foreach (var node in nodeUnitContainer)
                {
                    foreach (var link in node.Descendants().Where(x => x.Attributes.Contains("href")).Select(y => y.Attributes["href"].Value))
                    {
                        if (!(link.StartsWith("https://www.googleadservices.com/") || link.StartsWith("/aclk?")))
                            urls.Add(link);
                    }
                }
            }
            else if (nodeCommercialUnit.SelectNodes(".//div/div//a[2]") != null)
            {
                var nodes = nodeCommercialUnit.SelectNodes(".//div/div//a[2]");
                foreach (var node in nodes)
                {
                    var link = node.GetAttributeValue("href", "");
                    if (!(link.StartsWith("https://www.googleadservices.com/") || link.StartsWith("/aclk?")))
                        urls.Add(link);
                }
            }
            //var nods = nodeCommercialUnit.SelectNodes(".//td/div/div//a[2]");
            
            return urls.Distinct().ToList();
        }
    }
}
