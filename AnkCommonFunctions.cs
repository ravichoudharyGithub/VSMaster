using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public static class AnkCommonFunctions
    {

    public static bool SetTrue = true;
    public static bool SetFalse = false;
    public static string ToStringCustom(this object Value)
    {
        try
        {
            if (Value == null)
                return "";
            else
                return Convert.ToString(Value == null ? "" : Value);
        }
        catch
        {
           // MessageBox.Show("Cannot Convert To string");
            return "";
        }
    }

    #region ToInt
  
   
    public static int ToInt(this string Value)
    {
        try
        {
            if (Value == null || Value == "")
                return 0;
            else
                return Convert.ToInt32(Value == null ? "0" : Value);
        }
        catch
        {
           
            return 0;
        }
    }

    public static int ToInt(this Object Value)
    {
        try
        {
            if (Value == null)
                return 0;
            else
                return Convert.ToInt32(Value);
        }
        catch
        {
           
            return 0;
        }
    }

    #endregion

    public static string ReplaceRecursively(this string sIn, string search, string replace)
    {
        string s = sIn;
        if (String.IsNullOrEmpty(s))
            return "";
        while (s.Contains(search))
            s = s.Replace(search, replace);
        return s;
    }

    /// <summary>
    /// Finds a substring, but do not fail is the start index is outside of the bounds of the string.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="startIndex"></param>
    /// <returns></returns>
    public static string SubstringLimit(this string s, int startIndex)
    {
        return SubstringLimit(s, startIndex, s.Length);
    }

    /// <summary>
    /// Finds a substring, but do not fail is the start index or lengths are outside of the bounds of the string.
    /// if startIndex less than 0 will return "";
    /// </summary>
    /// <param name="s"></param>
    /// <param name="startIndex"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static string SubstringLimit(this string s, int startIndex, int maxLength)
    {
        if (String.IsNullOrEmpty(s) || startIndex < 0 || startIndex >= s.Length)
            return "";
        int todoLength = Math.Min(s.Length - startIndex, maxLength);
        if (todoLength > 0)
            return s.Substring(startIndex, todoLength);
        return "";
    }

    /// <summary>
    /// Determines if input string contains comparison string, ignoring case.  
    /// If so, returns first position of match
    /// </summary>
    /// <param name="s">input string</param>
    /// <param name="compare">comparison string</param>
    /// <returns>position of match</returns>
    public static int IndexOfCaseInsensitive(this string s, string compare, int startIndex)
    {
        // a blank string cannot contain anything
        if (String.IsNullOrEmpty(s))
            return -1;
        // a blank compare string is always in the main string
        if (String.IsNullOrEmpty(compare))
            return -1;

        string ucS = s.ToUpper();
        string ucSCompare = compare.ToUpper();
        return ucS.IndexOf(ucSCompare, startIndex);
    }

    public static string ForceNumeric(string sNum)
    {
        if (string.IsNullOrEmpty(sNum))
            return "";

        string outStr = "";
        bool usedDecimal = false;

        for (int i = 0; i < sNum.Length; i++)
        {
            string ch = sNum.Substring(i, 1);
            int value;
            if (int.TryParse(ch, out value))
                outStr += ch;
            else if (ch == "." && !usedDecimal)
            {
                outStr += ch;
                usedDecimal = false; // alter this to true if you want to get only one decimal : ankur
            }
            else if (ch == "-" && outStr.Length == 0)
                outStr += ch;
        }
        return outStr.Length > 0 ? outStr : "0";
    }

    public static string ParseStringInTwoSteps(string s, string start1, string stop1, string start2, string stop2)
    {
        string match;
        int pos = Parse(s, 0, start1, stop1, out match);
        if (pos > 0)
            Parse(match, 0, start2, stop2, out match);
        return match;
    }

    public static int Parse(string inText, int startIndex, string startString, string stopString, out string match)
    {
        return Parse(inText, startIndex, startString, stopString, out match, false);
    }

    public static int Parse(string inText, int startIndex, string startString, string stopString, out string match, bool caseSensitive)
    {
        match = "";
        if (startIndex < 0)
            return -1;

        if (String.IsNullOrEmpty(inText))
            return -1;

        if (startIndex >= inText.Length)
            return -1;
        int iPos;

        // if no search string, match at beginning
        if (String.IsNullOrEmpty(startString))
            iPos = startIndex;
        else
        {
            if (caseSensitive)
                iPos = inText.IndexOf(startString, startIndex);
            else
                iPos = inText.IndexOfCaseInsensitive(startString, startIndex);
        }

        if (iPos < 0)
            return -1;
        iPos += startString.Length;

        // find text that is not a blank
        for (int i = iPos; i < inText.Length; i++)
        {
            iPos = i;
            char ch = inText[i];
            if (ch != ' ' && ch != '\r' && ch != '\n' && ch != '\t')
                break;
        }
        int iEndPos;

        if (caseSensitive)
            iEndPos = inText.IndexOf(stopString, iPos);
        else
            iEndPos = inText.IndexOfCaseInsensitive(stopString, iPos);

        if (iEndPos < 0)
        {
            match = inText.SubstringLimit(iPos).Trim();
            return iPos;
        }

        string temp = inText.SubstringLimit(iPos, iEndPos - iPos).Trim();
        if (temp.Length > 0)
        {
            match = temp;
            return iPos;
        }
        return -1;
    }

    public static string ToBoolToIntCustom(this object obj)
    {
        if (obj == null) return "";
        var str = obj.ToString();
        if (str.ToLower() == "true") return "1";
        else if (str.ToLower() == "false") return "0";
        else return str;
    }

}

