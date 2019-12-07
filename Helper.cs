using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VSMaster.Models;
using System.IO;
using AngleSharp.Dom;

namespace VSMaster
{
    static class Helper
    {
        public static List<string> GetVSMasterModelProperties()
        {
            var properties = new List<string>();
            // get all properties of VSMasterModel type
            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(VSMasterModel).GetProperties();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                properties.Add(propertyInfo.Name);
            }

            return properties;
        }

        public static string FirstCharToUpper(string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        public static bool IsAllUpper(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (!char.IsUpper(input[i]))
                    return false;
            }

            return true;
        }

        public static string ToCsv<T>(IEnumerable<T> objectlist)
        {
            var csvdata = new StringBuilder();
            try
            {
                var separator = "\",\"";
                var fields = typeof(T).GetFields();
                var properties = typeof(T).GetProperties().ToArray();
                var heading = new string[properties.Length];
                var i = 0;
                foreach (var item in properties)
                {
                    heading[i] = GetDisplayName(item);
                    i++;
                }
                var header = "\"" + string.Join(separator, heading) + "\"";
                csvdata.AppendLine(header);
                foreach (var o in objectlist)
                {
                    var res = string.Join(separator, fields.Select(f => (f.GetValue(o) ?? "").ToString()).Concat(properties.Select(p => (p.GetValue(o, null) ?? "0").ToBoolToIntCustom())).ToArray());
                    //if (res.ToUpper() == "TRUE") res = "1";
                    //if (res.ToUpper() == "FALSE") res = "0";
                    res = "\"" + res + "\"";
                    csvdata.AppendLine(res);
                }
                return csvdata.ToString();
            }
            catch
            {
                return csvdata.ToString();
            }
        }

        public static string GetDisplayName(PropertyInfo info)
        {
            try
            {
                if (info.GetCustomAttributesData().Any())
                {
                    var propertyInfo = info.GetCustomAttributesData().FirstOrDefault();
                    var customAttributeNamedArgument = propertyInfo?.NamedArguments?.FirstOrDefault();
                    if (customAttributeNamedArgument != null)
                    {
                        var value = customAttributeNamedArgument.Value.ToString().Substring(7);
                        return value.Replace("\"", string.Empty).Trim();
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return info.Name;
        }

        public static bool IsHiddenElement(IElement IElemNode)
        {
            if (IElemNode != null
                && (IElemNode.Style.GetPropertyValue("display") == "none" || IElemNode.Style.GetPropertyValue("display") == "hidden"))
                return true;

            return false;
        }

        public static string CheckValueWithNodeName(IElement IElemNode)
        {
            var nodeValue = string.Empty;

            if (IElemNode != null)
            {
                switch (IElemNode.NodeName)
                {
                    case "IMG":
                    case "AREA":
                        nodeValue = IElemNode.GetAttribute("alt");
                        break;

                    // add more node as per requirement....
                    default:
                        break;
                }
            }

            return nodeValue;
        }

        public static string CheckValueWithAttributeName(IElement IElemNode, string attributeName)
        {
            var nodeValue = string.Empty;

            if (IElemNode != null)
            {
                var attribute = IElemNode.HasAttribute(attributeName);
                if (attribute)
                {
                    nodeValue = IElemNode.GetAttribute(attributeName);
                }
            }


            return nodeValue;
        }

        public static string GetNodeValue(INode node)
        {
            var txtContent = node.TextContent;
            var IElemNode = node as IElement;
            if (IElemNode != null)
            {
                if (node.NodeType != NodeType.Text && node.ChildNodes.Count() > 0)
                    txtContent = "";

                if (IElemNode.HasAttribute("label"))
                {
                    txtContent = IElemNode.GetAttribute("label");
                }

                if (string.IsNullOrEmpty(txtContent))
                {
                    if (IElemNode.HasAttribute("alt"))
                    {
                        txtContent = IElemNode.GetAttribute("alt");
                    }
                }
            }
            return txtContent;
        }

        public static GetLink GetAnchorImageNodeValue(INode node)
        {
            var getLink = new GetLink();
            var IElemNode = node as IElement;
            if (IElemNode != null)
            {
                //if (node.NodeType != NodeType.Text && node.ChildNodes.Count() > 0)
                //    txtContent = "";

                if (IElemNode.NodeName.Equals("IMG") || IElemNode.NodeName.Equals("img"))
                {
                    getLink.TagName = "Img";
                    if (IElemNode.HasAttribute("src"))
                        getLink.Url = IElemNode.GetAttribute("src");
                    if (IElemNode.HasAttribute("alt"))
                        getLink.Text = IElemNode.GetAttribute("alt");
                }

                else if (IElemNode.NodeName.Equals("A") || IElemNode.NodeName.Equals("a"))
                {
                    getLink.TagName = "a";
                    if (IElemNode.HasAttribute("href"))
                    {
                        getLink.Url = IElemNode.GetAttribute("href");
                        getLink.Text = IElemNode.TextContent;
                    }
                }
            }

            return getLink;
        }
    }

    public class VSMasterSupport
    {
        static readonly List<string> lstPunctuation = GetPageWords.lstPunctuation;
        static readonly List<string> vsModelProperties = Helper.GetVSMasterModelProperties();
        public static int OrderId = 0;
        public static List<VSMasterModel> FillingVSMasterModel(ResultNode resultnode, List<char[]> charArrayList, int orderId)
        {
            var vsModelList = new List<VSMasterModel>();

            // Model implimenting here...
            if (!string.IsNullOrEmpty(resultnode.NodeValue))
            {
                //var asciiEncoding = Encoding.Unicode;
                //var bytes = asciiEncoding.GetBytes(resultnode.NodeValue);
                //var txt = asciiEncoding.GetString(bytes);
                var words = resultnode.NodeValue.Split(new char[] { ' ' });

                foreach (var word in words)
                {
                    var vsModel = new VSMasterModel();
                    var wordForModel = word;
                    for (int i = 0; i <= charArrayList.Count - 1; i++)
                    {
                        var newWord = word.Split(charArrayList[i]);
                        if (newWord.Length > 1)
                        {
                            if (lstPunctuation[i] == ".")
                            {
                                var startIndex = 0;
                                var indexOfPunctuation = word.IndexOf(lstPunctuation[i]);
                                if (indexOfPunctuation != word.Length - 1)
                                {
                                    var nextWord = word[indexOfPunctuation + 1].ToStringCustom();
                                    if (!string.IsNullOrEmpty(nextWord))
                                    {
                                        vsModel.PunctuationMarkBefore = null;
                                        vsModel.PunctuationMarkAfter = null;
                                        wordForModel = word;
                                        continue;
                                    }
                                }
                            }
                            if (newWord[1] == "") // PunctuationMarkBefore--> null
                            {
                                vsModel.PunctuationMarkBefore = null;
                                vsModel.PunctuationMarkAfter = lstPunctuation[i].ToString();
                                wordForModel = newWord[0];
                                break;
                            }
                            else  // PunctuationMarkAfter--> null
                            {
                                vsModel.PunctuationMarkBefore = lstPunctuation[i].ToString();
                                vsModel.PunctuationMarkAfter = null;
                                wordForModel = newWord[1];
                                break;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(wordForModel))
                    {
                        vsModel.Word = wordForModel.Trim();
                        var charArray = wordForModel.ToCharArray();
                        if (char.IsUpper(charArray[0]))
                            vsModel.FirstLetterUppercase = AnkCommonFunctions.SetTrue;

                        var isAllUpper = Helper.IsAllUpper(wordForModel);
                        vsModel.AllInUpperCase = isAllUpper ? AnkCommonFunctions.SetTrue : AnkCommonFunctions.SetFalse;
                    }
                    else
                        continue;

                    if (vsModelProperties.Contains(resultnode.NodeName))
                    {
                        var value = AnkCommonFunctions.SetTrue;
                        var propertyInfo = vsModel.GetType().GetProperty(resultnode.NodeName);
                        propertyInfo.SetValue(vsModel, value, null);
                    }
                    // for inherited Tags
                    if (resultnode.InheritedTags.Count > 0)
                    {
                        foreach (var inheritedTag in resultnode.InheritedTags)
                        {
                            if (vsModelProperties.Contains(inheritedTag))
                            {
                                var value = AnkCommonFunctions.SetTrue;
                                var propertyInfo = vsModel.GetType().GetProperty(inheritedTag);
                                propertyInfo.SetValue(vsModel, value, null);
                            }
                        }
                    }

                    // set orderId...
                    vsModel.OrderId = OrderId++;
                    vsModel.Lang = resultnode.Lang;
                    vsModel.Country = resultnode.Country;
                    vsModel.Link = resultnode.Link;
                    vsModelList.Add(vsModel);
                }
            }
            return vsModelList;
        }
    }
}
