using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AngleSharp;
using AngleSharp.Parser.Html;
using VSMaster.Models;
using static VSMaster.Helper;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Dom.Events;

namespace VSMaster
{
    public static class GetPageWords
    {
        public static List<string> skipItmes = new List<string> { "body", "html", "head", "script", "#comment","SCRIPT" };
        public static void ParsePAge(string url)
        {
            var gcc = new GlobusHttpHelper();

            var html = gcc.getHtmlfromUrl(new Uri(url));
            html = File.ReadAllText("F:/Matrix.html");
            var hd = new HtmlAgilityPack.HtmlDocument();
            hd.LoadHtml(html);

            var hn = hd.DocumentNode.SelectNodes("//*");
        }

        public static void CheckNoFrames(string fileName)
        {
            string fullPath;
            IHtmlDocument document;
            int count;
            HtmlParser parser;
            IElement frameSet;

            Console.WriteLine("Checking " + fileName);

            fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            parser = new HtmlParser(Configuration.Default.WithJavaScript().WithCss());
            parser.Context.ParseError += Context_ParseError;

            using (Stream stream = File.OpenRead(fullPath))
            {
                document = parser.Parse(stream);
            }
            var ddd = document.QuerySelector("frame");
            //frameSet = (IElement)document.DocumentElement.SelectSingleNode("//noframes");

            // Console.WriteLine("Child elements: " + frameSet.ChildElementCount);
        }

        private static void Context_ParseError(object sender, AngleSharp.Dom.Events.Event ev)
        {
            Console.WriteLine(((HtmlErrorEvent)ev).Message);
        }

        public static List<string> lstPunctuation = new List<string>();
        public static List<string> lstInheritedTags = new List<string>();

        private static void AddPunctuationMarks()
        {
            lstPunctuation.Add(".");
            lstPunctuation.Add("!");
            lstPunctuation.Add(",");
            lstPunctuation.Add(":");
            lstPunctuation.Add("-");
            lstPunctuation.Add("[");
            lstPunctuation.Add("{");
            lstPunctuation.Add("}");
            lstPunctuation.Add("}");
            lstPunctuation.Add("(");
            lstPunctuation.Add(")");
            lstPunctuation.Add("'");
            lstPunctuation.Add("¿");
        }

        public static void ParseWithAngleSharp(string fileUrl)
        {

            var vsMasterModel = new List<VSMasterModel>();
            //var skipItmes = new List<string> { "body", "html", "head", "script", "#comment" };
            var orderId = -1;
            var vsModelProperties = GetVSMasterModelProperties();
            var tagName = string.Empty;
            // generating a list with punctuation mark
            AddPunctuationMarks();
            var charArrayList = lstPunctuation.Select(str => str.ToCharArray()).ToList();
            //We require a custom configuration
            var config = Configuration.Default.WithJavaScript().WithCss();
            //Let's create a new parser using this configuration
            //var parser = new HtmlParser(new HtmlParserOptions {IsEmbedded = true }, config);
            var parser = new HtmlParser(config);

            // https://github.com/glienard/html-to-sql/blob/master/htmlexample.html
            var source = File.ReadAllText(fileUrl);
            var document = parser.Parse(source);
            var pageData = new Page();

            #region For All Page Data
            var robots = document.All.FirstOrDefault(x => x.LocalName == "meta" && x.GetAttribute("Name") == "robots");
            if ((robots as IHtmlMetaElement) != null)
            {
                var content = (robots as IHtmlMetaElement).Content;
                if (content.IndexOf("NoFollow", StringComparison.OrdinalIgnoreCase) != -1) pageData.NoFollow = true;
                if (content.IndexOf("NoIndex", StringComparison.OrdinalIgnoreCase) != -1) pageData.NoIndex = true;
                if (content.IndexOf("NoArchive", StringComparison.OrdinalIgnoreCase) != -1) pageData.NoArchive = true;
                if (content.IndexOf("NoImageIndex", StringComparison.OrdinalIgnoreCase) != -1) pageData.NoImageIndex = true;
                if (content.IndexOf("NoSnippet", StringComparison.OrdinalIgnoreCase) != -1) pageData.NoSnippet = true;
            }
            #endregion

            #region For All Links Of Page
            var linkTags = document.All.Where(x => x.Attributes["href"] != null || x.Attributes["src"] != null);
            var frame = document.QuerySelectorAll("frame");

            var lstLinks = new List<Links>();
            foreach (var link in linkTags)
            {
                var nofollow = link.Attributes["rel"];
                var nofollowInt = nofollow?.Value == "nofollow" ? 1 : 0;
                lstLinks.Add(new Links
                {
                    Url = link.Attributes["href"] == null ? link.Attributes["src"].Value : link.Attributes["href"].Value,
                    LinkType = link.LocalName,
                    NotFollow = nofollowInt
                });
            }
            #endregion

            var html = document.All.FirstOrDefault(x => x.LocalName == "html");
            var lang = (html as IHtmlElement).Language;

            #region HEADER PARSE
            var heder = document.All.Where(x => x.LocalName == "head");
            var headerTotalTags = parser.Parse(heder.FirstOrDefault().OuterHtml).All;
            foreach (var tag in headerTotalTags)
            {
                var outerHmtl = tag.OuterHtml;

                if (skipItmes.Contains(tag.LocalName))
                    continue;

                var newDoc = parser.ParseFragment(tag.TextContent.ToString(), null);
                var txtContent = newDoc.FirstOrDefault().TextContent;

                if (tag.LocalName == "meta")
                {
                    var localName = Helper.FirstCharToUpper(tag.LocalName);
                    txtContent = tag.GetAttribute("content");
                    if (!string.IsNullOrEmpty(txtContent))
                    {
                        var criteria = false;
                        tagName = string.Empty;

                        if (!tag.HasAttribute("name")) continue;
                        criteria = (tag.GetAttribute("name").Equals("description") || tag.GetAttribute("name").Equals("keywords"));
                        if (!criteria)
                            txtContent = null;
                        else
                        {
                            var name = tag.GetAttribute("name");
                            name = Helper.FirstCharToUpper(name);
                            tagName = $"{"Tag"}{localName}{name}";
                        }
                    }
                }

                // Model for header implimenting here...
                if (!string.IsNullOrEmpty(txtContent))
                {
                    if (tag.LocalName != "meta")
                    {
                        var localName = Helper.FirstCharToUpper(tag.LocalName);
                        tagName = $"{"Tag"}{localName}";
                    }
                    var resultnode = new ResultNode { NodeName = tagName, NodeValue = txtContent };
                    var ielemnode = tag as IHtmlElement;
                    //var language1 =  ielemnode == null ? lang : (tag as IHtmlElement).Language;
                    //if (language1.Contains("-"))
                    //{
                    var langCountry = lang.Split('-');
                    resultnode.Lang = langCountry[0].ToString();
                    resultnode.Country = langCountry[1].ToString();
                    //}
                    var vsModelCollection = VSMasterSupport.FillingVSMasterModel(resultnode, charArrayList, orderId);
                    if (vsModelCollection != null && vsModelCollection.Count > 0)
                    {
                        orderId = 0;
                        foreach (var vsModel in vsModelCollection)
                            vsMasterModel.Add(vsModel);
                    }
                }
            }
            #endregion

            #region BODY PARSE
            skipItmes.Add("title");
            tagName = string.Empty;

            var body = document.QuerySelector("body");
            var language = (body as IHtmlElement).Language;
            if (language.Contains("-"))
            {
                var langCountry = language.Split('-');
                pageData.Lang = language[0].ToString();
                pageData.Country = language[1].ToString();
            }

            var bodyChildNodes = body.ChildNodes;

            // new code 
            foreach (var node in bodyChildNodes)
            {
                var vsModelCollection = new List<VSMasterModel>();
                var txtContent = node.TextContent;
                var IElenode = node as IElement;
                var resultNode = new ResultNode();
                if (string.IsNullOrEmpty(txtContent.Trim()) && IElenode == null) continue;
                if (skipItmes.FindIndex(x => x.Equals(node.NodeName, StringComparison.OrdinalIgnoreCase)) != -1) continue;

                ProcessNode(node, skipItmes, charArrayList, vsMasterModel, IElenode);
            }
            #endregion

            if (vsMasterModel.Count > 0)
            {
                var words = vsMasterModel.Select(x => x.Word).ToList();
                var res = string.Join(" ", words);

                var csvString = ToCsv(vsMasterModel);
                File.WriteAllText("Output.csv", csvString);
            }
        }

        public static ResultNode intermediateResultNode;
        public static void ProcessNode(INode Node, List<string> skipItmes, List<char[]> charArrayList, List<VSMasterModel> vsMasterModel, IElement IElemNode = null, HashSet<string> parentInheritedNode = null, string parentLink = null, IElement parentElement = null, string parentText = null, string parentLanguage = null)
        {
            var isParentNode = false;
            if (parentElement == null) isParentNode = true;
            if (Node.NodeName.ToLower() == "script")
            {
                var data = IElemNode.ChildNodes.OfType<IText>().Select(m => m.Text.Trim());
            }
            if (IElemNode != null)
            {
                if (parentElement == null) parentElement = IElemNode;
                var textOfElement1 = string.Join("", IElemNode.ChildNodes.OfType<IText>().Select(m => m.Text.Trim()));
                if (string.IsNullOrEmpty(textOfElement1?.Trim()) && IElemNode.Children.Length == 0 && !(IElemNode.HasAttribute("label") || IElemNode.HasAttribute("alt"))) return;
            }

            var resultNode = GetNodeWithValue(Node, parentElement, ref parentInheritedNode, parentLink, parentText);
            if (resultNode?.NodeValue.Trim().ToLower() == "menu")
            {

            }
            if (resultNode != null && !isParentNode && !string.IsNullOrEmpty(resultNode.NodeValue?.Trim()) && IsContinueWord(resultNode.NodeValue, Node))
            {
                if (intermediateResultNode != null && !string.IsNullOrEmpty(intermediateResultNode.NodeValue.Trim()))
                    intermediateResultNode.NodeValue = intermediateResultNode.NodeValue + resultNode.NodeValue;
                else
                    intermediateResultNode = resultNode;
                resultNode = null;
            }

            if (IElemNode != null && (IElemNode.Children.Length > 0 || Node.ChildNodes.Length > 0))
            {
                if (resultNode.IsLabelValue) // is label value true, means this have some data in there alt or lable 
                {
                    AddResultNodeToMasterModel(resultNode, ref vsMasterModel, charArrayList);
                }
                foreach (var INodeElem in Node.ChildNodes)
                {
                    //if ((INodeElem as IElement) == null) continue;
                    if (skipItmes.FindIndex(x => x.Equals(INodeElem.NodeName, StringComparison.OrdinalIgnoreCase)) != -1) continue;
                    var lang = string.IsNullOrEmpty(resultNode.Lang) ? "" : $"{resultNode.Lang}-{resultNode.Country}";
                    ProcessNode(INodeElem, skipItmes, charArrayList, vsMasterModel, INodeElem as IElement, parentInheritedNode, resultNode.Link, parentElement, "", lang);
                }
            }
            else
            {
                if (resultNode != null && !string.IsNullOrEmpty(resultNode.NodeValue.Trim()) && intermediateResultNode == null)
                {
                    AddResultNodeToMasterModel(resultNode, ref vsMasterModel, charArrayList);
                }
                else if (intermediateResultNode != null && !string.IsNullOrEmpty(intermediateResultNode.NodeValue.Trim()) && intermediateResultNode != null && IsLastSibling(Node, parentElement))
                {
                    AddResultNodeToMasterModel(intermediateResultNode, ref vsMasterModel, charArrayList);
                    intermediateResultNode = null;
                    if (resultNode != null && !string.IsNullOrEmpty(resultNode.NodeValue?.Trim())) // add result node too, this can be a case when the sibling doesnt follows the continue word rule
                    {
                        AddResultNodeToMasterModel(resultNode, ref vsMasterModel, charArrayList);
                    }
                }
            }
        }

        public static ResultNode GetNodeWithValue(INode node, IElement IElemNode, ref HashSet<string> parentInheritedNode, string ParentLink, string ParentText)
        {
            var txtContent = node.TextContent;
            var isLabelValue = false;
            if ((node as IElement) != null) IElemNode = node as IElement;
            if (IElemNode != null)
            {
                if (node.NodeType != NodeType.Text)
                    txtContent = "";
                //txtContent = string.Join(" ", IElemNode.ChildNodes.OfType<IText>().Select(m => m.Text.Trim()));

                if (IElemNode.HasAttribute("label"))
                {
                    txtContent = IElemNode.GetAttribute("label");
                    isLabelValue = true;
                }

                if (string.IsNullOrEmpty(txtContent))
                {
                    if (IElemNode.HasAttribute("alt"))
                    {
                        txtContent = IElemNode.GetAttribute("alt");
                        isLabelValue = true;
                    }
                }
            }

            if (txtContent.Length == 1 && char.IsPunctuation(txtContent[0]))
                return null;

            // TODO :- Need to handle with previous sibling...of lstPunctuation.
            var nextsib = node.NextSibling;
            if (nextsib != null)
            {
                var child = IElemNode == null ? null : IElemNode.ChildNodes;
                if (child == null || (child.Length == 0 || (child[child.Length - 1] as IHtmlElement) == null))
                {
                    var isPunctuation = lstPunctuation.Contains(nextsib.TextContent.Trim());
                    if (isPunctuation)
                        txtContent = $"{txtContent}{nextsib.TextContent.Trim()}";
                }
            }
            else if (node.ParentElement.LocalName != "body")
            {
                if (node == node.ParentElement.ChildNodes[node.ParentElement.ChildNodes.Length - 1])
                {
                    nextsib = node.ParentElement.NextSibling;
                    if (nextsib != null)
                    {
                        var isPunctuation = lstPunctuation.Contains(nextsib.TextContent.Trim());
                        if (isPunctuation)
                            txtContent = $"{txtContent}{nextsib.TextContent.Trim()}";
                    }
                }
            }
            // var prevSib = node.PreviousSibling;

            var nodeName = node.NodeName.ToLower();
            if (node.NodeName.Trim().Equals("SELECT") && IElemNode != null)
            {
                nodeName = IElemNode.TagName.ToLower();
            }

            var localName = FirstCharToUpper(nodeName);
            var tagName = $"{"Tag"}{localName}";
            var resultNode = new ResultNode { NodeName = tagName, NodeValue = txtContent, IsLabelValue = isLabelValue };

            if (IElemNode != null && IElemNode.Style.Children.Count() > 0)
            {
                var txt = IElemNode.Style.CssText;
                resultNode.CssOrInheritedProperties = txt;
                // for now only adding display, need to add othe properties too
                if (IElemNode.Style.GetPropertyValue("display") == "none" || IElemNode.Style.GetPropertyValue("display") == "hidden")
                    resultNode.InheritedTags.Add("TagHidden");
            }

            if (parentInheritedNode != null)
                resultNode.InheritedTags.UnionWith(parentInheritedNode);

            if (parentInheritedNode == null) parentInheritedNode = new HashSet<string>();
            if (!parentInheritedNode.Contains(tagName)) parentInheritedNode.Add(tagName);

            if (resultNode.InheritedTags.Count > 0)
            {
                if (parentInheritedNode == null) parentInheritedNode = new HashSet<string>();
                parentInheritedNode.UnionWith(resultNode.InheritedTags);
            }

            #region Set Language
            // TODO :- default language should be page language

            var language = GetNodeLanguage(node);
            if (language.Contains("-"))
            {
                var langCountry = language.Split('-');
                resultNode.Lang = langCountry[0].ToString();
                resultNode.Country = langCountry[1].ToString();
            }
            #endregion

            #region Set Link
            var boolNofollow = false;
            if (IElemNode != null && IElemNode.HasAttribute("href"))
            {
                resultNode.Link = IElemNode.GetAttribute("href");
                var nofollow = IElemNode.Attributes["rel"];
                boolNofollow = nofollow?.Value == "nofollow" ? true : false;

            }
            else if (IElemNode != null && IElemNode.HasAttribute("src") && !IElemNode.GetAttribute("src").StartsWith("#"))
            {
                resultNode.Link = IElemNode.GetAttribute("src");
                var nofollow = IElemNode.Attributes["rel"];
                boolNofollow = nofollow?.Value == "nofollow" ? true : false;
            }
            else
                resultNode.Link = ParentLink;

            #endregion

            if (boolNofollow)
                resultNode.InheritedTags.Add("NoFollowLink");

            return resultNode;
        }

        public static List<VSMasterModel> FillVSModel(ResultNode resultNode, List<char[]> charArrayList, int orderId, IElement IElenode)
        {

            var vsModelList = new List<VSMasterModel>();
            if (!string.IsNullOrEmpty(resultNode.NodeValue))
            {
                var vsModelCollection = VSMasterSupport.FillingVSMasterModel(resultNode, charArrayList, orderId);
                if (vsModelCollection != null && vsModelCollection.Count > 0)
                {
                    orderId = 0;
                    foreach (var vsModel in vsModelCollection)
                    {
                        if (IElenode != null)
                        {
                            if (IElenode.HasAttribute("href") && !IElenode.GetAttribute("href").StartsWith("#"))
                            {
                                vsModel.Link = IElenode.GetAttribute("href");
                            }
                            if (IElenode.HasAttribute("src") && !IElenode.GetAttribute("src").StartsWith("#"))
                            {
                                vsModel.Link = IElenode.GetAttribute("src");
                            }
                        }

                        vsModelList.Add(vsModel);
                    }

                    return vsModelList;
                }
            }
            return null;
        }

        public static void ProcessRootNode(INode Node, List<string> skipItmes, List<char[]> charArrayList, List<VSMasterModel> vsMasterModel, IElement IElemNode = null)
        {
            if (Node.NodeName.ToLower() != "text")
            {
                var data1 = IElemNode.ChildNodes;
            }
        }

        public static bool IsContinueWord(string str, INode node)
        {
            if (node.NodeType != NodeType.Text) return false;
            if (string.IsNullOrEmpty(str)) return false;
            if (str.Contains(" ")) return false;
            if (node.Parent.NodeName.ToLower() == "option" || node.Parent.NodeName.ToLower() == "td") return false;   // we need to decide the breaking element, since option is an element which doesn't adds to any other.
            return true;
        }

        public static bool IsLastSibling(INode Node, INode RootParent)
        {
            if (Node.Parent.NodeName.ToLower() == "option" || Node.Parent.NodeName.ToLower() == "td") return true;
            while (Node.NextSibling == null)
            {
                Node = Node.ParentElement;
                if (Node == RootParent) return true;
            }
            return false;
        }

        public static string GetNodeLanguage(INode Node)
        {
            var loop = 3;
            while (Node as IHtmlElement == null)
            {
                Node = Node.Parent;
                loop--;
                if (loop == 0) return "en-gb";
            }

            return (Node as IHtmlElement).Language;
        }

        public static void AddResultNodeToMasterModel(ResultNode node, ref List<VSMasterModel> vsMasterModel, List<char[]> charArrayList)
        {
            char resultchar;
            char.TryParse(node.NodeValue.Trim(), out resultchar);
            if (!char.IsPunctuation(resultchar))
                vsMasterModel.AddRange(FillVSModel(node, charArrayList, orderId: 0, IElenode: null));
        }

        #region
        public static void ProcessNode1(INode Node, List<string> skipItmes, List<char[]> charArrayList, List<VSMasterModel> vsMasterModel, IElement IElemNode = null, HashSet<string> parentInheritedNode = null, string parentLink = null)
        {
            if (Node.NodeName.ToLower() == "h3")
            {
                var data = IElemNode.ChildNodes.OfType<IText>().Select(m => m.Text.Trim());
            }

            var data1 = IElemNode.ChildNodes;
            if (IElemNode != null)
            {
                var textOfElement1 = string.Join("", IElemNode.ChildNodes.OfType<IText>().Select(m => m.Text.Trim()));
                if (string.IsNullOrEmpty(textOfElement1?.Trim()) && IElemNode.Children.Length == 0 && !(IElemNode.HasAttribute("label") || IElemNode.HasAttribute("alt"))) return;
            }

            var resultNode = GetNodeWithValue(Node, IElemNode, ref parentInheritedNode, parentLink, "");
            if (resultNode != null && !string.IsNullOrEmpty(resultNode.NodeValue.Trim()))
            {
                char resultchar;
                char.TryParse(resultNode.NodeValue.Trim(), out resultchar);
                if (!char.IsPunctuation(resultchar))
                    vsMasterModel.AddRange(FillVSModel(resultNode, charArrayList, orderId: 0, IElenode: null));
            }

            if (IElemNode != null && IElemNode.Children.Length > 0)
            {
                var textOfElement = string.Join("", IElemNode.ChildNodes.OfType<IText>().Select(m => m.Text.Trim()));
                foreach (var INodeElem in Node.ChildNodes)
                {
                    if ((INodeElem as IElement) == null) continue;
                    ProcessNode(INodeElem, skipItmes, charArrayList, vsMasterModel, INodeElem as IElement, parentInheritedNode, resultNode.Link);
                }
            }
        }

        public static ResultNode GetNodeWithValue1(INode node, IElement IElemNode, ref List<string> parentInheritedNode, string ParentLink)
        {
            var txtContent = node.TextContent;
            if (IElemNode != null)
            {
                txtContent = string.Join(" ", IElemNode.ChildNodes.OfType<IText>().Select(m => m.Text.Trim()));

                if (IElemNode.HasAttribute("label"))
                    txtContent = IElemNode.GetAttribute("label");

                if (string.IsNullOrEmpty(txtContent))
                {
                    if (IElemNode.HasAttribute("alt"))
                        txtContent = IElemNode.GetAttribute("alt");
                }
            }

            if (txtContent.Length == 1 && char.IsPunctuation(txtContent[0]))
                return null;

            // TODO :- Need to handle with previous sibling...of lstPunctuation.
            var nextsib = node.NextSibling;
            if (nextsib != null)
            {
                var child = IElemNode == null ? null : IElemNode.ChildNodes;
                if (child == null || (child.Length == 0 || (child[child.Length - 1] as IHtmlElement) == null))
                {
                    var isPunctuation = lstPunctuation.Contains(nextsib.TextContent.Trim());
                    if (isPunctuation)
                        txtContent = $"{txtContent}{nextsib.TextContent.Trim()}";
                }
            }
            else if (node.ParentElement.LocalName != "body")
            {
                if (node == node.ParentElement.ChildNodes[node.ParentElement.ChildNodes.Length - 1])
                {
                    nextsib = node.ParentElement.NextSibling;
                    if (nextsib != null)
                    {
                        var isPunctuation = lstPunctuation.Contains(nextsib.TextContent.Trim());
                        if (isPunctuation)
                            txtContent = $"{txtContent}{nextsib.TextContent.Trim()}";
                    }
                }
            }
            // var prevSib = node.PreviousSibling;

            var nodeName = node.NodeName.ToLower();
            if (node.NodeName.Trim().Equals("SELECT") && IElemNode != null)
            {
                nodeName = IElemNode.TagName.ToLower();
            }

            var localName = FirstCharToUpper(nodeName);
            var tagName = $"{"Tag"}{localName}";
            var resultNode = new ResultNode { NodeName = tagName, NodeValue = txtContent };

            if (IElemNode != null && IElemNode.Style.Children.Count() > 0)
            {
                var txt = IElemNode.Style.CssText;
                resultNode.CssOrInheritedProperties = txt;
                // for now only adding display, need to add othe properties too
                if (IElemNode.Style.GetPropertyValue("display") == "none" || IElemNode.Style.GetPropertyValue("display") == "hidden")
                    resultNode.InheritedTags.Add("TagHidden");
            }

            if (parentInheritedNode != null)
                resultNode.InheritedTags.UnionWith(parentInheritedNode);

            if (parentInheritedNode == null) parentInheritedNode = new List<string>();
            parentInheritedNode.Add(tagName);

            if (resultNode.InheritedTags.Count > 0)
            {
                if (parentInheritedNode == null) parentInheritedNode = new List<string>();
                parentInheritedNode.AddRange(resultNode.InheritedTags);
            }

            #region Set Language
            // TODO :- default language should be page language
            var language = IElemNode == null ? "en-gb" : (node as IHtmlElement).Language;
            if (language.Contains("-"))
            {
                var langCountry = language.Split('-');
                resultNode.Lang = langCountry[0].ToString();
                resultNode.Country = langCountry[1].ToString();
            }
            #endregion

            #region Set Link
            var boolNofollow = false;
            if (IElemNode != null && IElemNode.HasAttribute("href") && !IElemNode.GetAttribute("href").StartsWith("#"))
            {
                resultNode.Link = IElemNode.GetAttribute("href");
                var nofollow = IElemNode.Attributes["rel"];
                boolNofollow = nofollow?.Value == "nofollow" ? true : false;

            }
            else if (IElemNode != null && IElemNode.HasAttribute("src") && !IElemNode.GetAttribute("src").StartsWith("#"))
            {
                resultNode.Link = IElemNode.GetAttribute("src");
                var nofollow = IElemNode.Attributes["rel"];
                boolNofollow = nofollow?.Value == "nofollow" ? true : false;
            }
            else
                resultNode.Link = ParentLink;

            #endregion

            if (boolNofollow)
                resultNode.InheritedTags.Add("NoFollowLink");

            return resultNode;
        }
        #endregion
    }

    public class ResultNode
    {
        public string NodeName { get; set; }
        public string NodeValue { get; set; }
        public string CssOrInheritedProperties { get; set; }
        public HashSet<string> InheritedTags = new HashSet<string>();
        public string Lang { get; set; }
        public string Country { get; set; }
        public string Link { get; set; }
        public bool IsLabelValue { get; set; }
    }
}
