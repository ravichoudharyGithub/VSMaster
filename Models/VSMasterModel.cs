using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSMaster.Models
{

    [Serializable]
    public class VSMasterModel
    {
        public int OrderId { get; set; }
        public string Word { get; set; }
        public bool TagH1 { get; set; }
        public bool TagH2 { get; set; }
        public bool TagH3 { get; set; }
        public bool TagH4 { get; set; }
        public bool TagH5 { get; set; }
        public bool TagH6 { get; set; }
        public bool TagA { get; set; }

        public string Link { get; set; }
        public bool NoFollowLink { get; set; }
        public bool TagArea { get; set; }
        public bool TagHidden { get; set; }
        public bool TagImg { get; set; }
        public bool TagButton { get; set; }
        public bool TagMetaDescription { get; set; }
        public bool TagMetaKeywords { get; set; }
        public bool TagTitle { get; set; }
        public bool TagSmall { get; set; }
        public bool TagStrike { get; set; }
        public bool TagSub { get; set; }
        public bool TagSup { get; set; }
        public bool TagB { get; set; }
        public bool TagBig { get; set; }
        public bool TagEm { get; set; }
        public bool TagI { get; set; }
        public bool TagStrong { get; set; }
        public bool TagU { get; set; }
        public bool TagOption { get; set; }
        public string Lang { get; set; } = "en";
        public string Country { get; set; } = "gb";
        public int DomailId { get; set; } = 1;
        public int HostId { get; set; } = 1;
        public string PunctuationMarkBefore { get; set; }
        public string PunctuationMarkAfter { get; set; }
        public bool FirstLetterUppercase { get; set; }
        public bool AllInUpperCase { get; set; }
        public bool TagDel { get; set; }
        public bool TagDfn { get; set; }
        public bool TagIns { get; set; }
        public bool TagMark { get; set; }
        public bool TagOptgroup { get; set; }
    }

    [Serializable]
    public class Page
    {
        public bool NoFollow { get; set; }
        public bool NoIndex { get; set; }
        public bool NoArchive { get; set; }
        public bool NoImageIndex { get; set; }
        public bool NoSnippet { get; set; }
        public string Lang { get; set; }
        public string Country { get; set; }
    }

    [Serializable]
    public class Links
    {
        public string Url { get; set; }
        public string LinkType { get; set; }
        public int NotFollow { get; set; }
    }

    [Serializable]
    public class GetLink
    {
        public string TagName { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
    }

    //[Serializable]
    //public class VSMasterModel
    //{
    //    public int OrderId { get; set; }
    //    public string Word { get; set; }
    //    public int TagH1 { get; set; }
    //    public int TagH2 { get; set; }
    //    public int TagH3 { get; set; }
    //    public int TagH4 { get; set; }
    //    public int TagH5 { get; set; }
    //    public int TagH6 { get; set; }
    //    public int? TagA { get; set; }

    //    public string Link { get; set; }
    //    public int NoFollowLink { get; set; }
    //    public int TagArea { get; set; }
    //    public int TagHidden { get; set; }
    //    public int TagImage { get; set; }
    //    public int TagButton { get; set; }
    //    public int TagMetaDescription { get; set; }
    //    public int TagMetaKeywords { get; set; }
    //    public int TagTitle { get; set; }
    //    public int TagSmall { get; set; }
    //    public int TagStrike { get; set; }
    //    public int TagSub { get; set; }
    //    public int TagSup { get; set; }
    //    public int TagB { get; set; }
    //    public int TagBig { get; set; }
    //    public int TagEm { get; set; }
    //    public int TagI { get; set; }
    //    public int TagStrong { get; set; }
    //    public int TagU { get; set; }
    //    public int TagOption { get; set; }
    //    public string Lang { get; set; } = "en";
    //    public string Country { get; set; } = "gb";
    //    public int DomailId { get; set; } = 1;
    //    public int HostId { get; set; } = 1;
    //    public string PunctuationMarkBefore { get; set; }
    //    public string PunctuationMarkAfter { get; set; }
    //    public int FirstLetterUppercase { get; set; }
    //    public int AllInUpperCase { get; set; }
    //    public int TagDel { get; set; }
    //    public int TagDfn { get; set; }
    //    public int TagIns { get; set; }
    //    public int TagMark { get; set; }
    //    public int TagOptgroup { get; set; }
    //}
}
