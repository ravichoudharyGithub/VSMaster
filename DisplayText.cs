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
    public static class DisplayText
    {
        static IConfiguration config;
        static List<string> resultClass = new List<string>();

        static DisplayText()
        {
            config = Configuration.Default.WithJavaScript().WithCss();
        }

        public static string GetDisplayText(string fileUrl)
        {
            var parser = new HtmlParser(config);
            var source = File.ReadAllText(fileUrl);
            var document = parser.Parse(source);
            var body = document.QuerySelector("body");
            var bodyChildNodes = body.ChildNodes;

            foreach (var bChild in bodyChildNodes)
            {
                if (GetPageWords.skipItmes.FindIndex(x => x.Equals(bChild.NodeName, StringComparison.OrdinalIgnoreCase)) != -1) continue;
                ProcessForDisplayText(bChild);
            }

            return string.Join(" ", resultClass);
        }

        public static void ProcessForDisplayText(INode node)
        {
            var txtContent = string.Empty;
            var textOfElement = string.Empty;
            var IElemNode = node as IElement;

            if (Helper.IsHiddenElement(IElemNode))
                return;

            var resultNode = Helper.GetNodeValue(node);
            if (!string.IsNullOrEmpty(resultNode.Trim()))
            {
                resultClass.Add(resultNode.Trim());
            }

            if (IElemNode != null && (IElemNode.Children.Length > 0 || node.ChildNodes.Length > 0))
            {
                foreach (var nd in node.ChildNodes)
                {
                    IElemNode = nd as IElement;
                    if (GetPageWords.skipItmes.FindIndex(x => x.Equals(nd.NodeName, StringComparison.OrdinalIgnoreCase)) != -1) continue;
                    ProcessForDisplayText(nd);
                }
            }
        }
    }
}
