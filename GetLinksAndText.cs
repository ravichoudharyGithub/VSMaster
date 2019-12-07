using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSMaster.Models;

namespace VSMaster
{
    public static class GetLinksAndText
    {
        static IConfiguration config;
        static List<GetLink> getLinks = new List<GetLink>();

        static GetLinksAndText()
        {
            config = Configuration.Default.WithJavaScript().WithCss();
        }

        public static List<GetLink> GetLinks(string fileUrl)
        {
            var parser = new HtmlParser(config);
            var source = File.ReadAllText(fileUrl);
            var document = parser.Parse(source);
            var body = document.QuerySelector("body");
            var bodyChildNodes = body.ChildNodes;

            foreach (var bChild in bodyChildNodes)
            {
                if (GetPageWords.skipItmes.FindIndex(x => x.Equals(bChild.NodeName, StringComparison.OrdinalIgnoreCase)) != -1) continue;
                ProcessForGetLinks(bChild);
            }

            if (getLinks.Count > 0)
            {
                var words = getLinks.Select(x => x.TagName).ToList();
                var res = string.Join(" ", words);

                var csvString = Helper.ToCsv(getLinks);
                File.WriteAllText("GetLinks1.csv", csvString);
            }

            return getLinks;
        }

        public static void ProcessForGetLinks(INode node)
        {
            var txtContent = string.Empty;
            var textOfElement = string.Empty;
            var IElemNode = node as IElement;

            if (Helper.IsHiddenElement(IElemNode))
                return;

            var resultNode = Helper.GetAnchorImageNodeValue(node);
            if (resultNode.TagName != null)
            {
                getLinks.Add(resultNode);
            }

            if (IElemNode != null && (IElemNode.Children.Length > 0 || node.ChildNodes.Length > 0))
            {
                foreach (var nd in node.ChildNodes)
                {
                    IElemNode = nd as IElement;
                    if (GetPageWords.skipItmes.FindIndex(x => x.Equals(nd.NodeName, StringComparison.OrdinalIgnoreCase)) != -1) continue;
                    ProcessForGetLinks(nd);
                }
            }
        }
    }
}
