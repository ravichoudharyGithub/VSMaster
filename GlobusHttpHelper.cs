using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

public class GlobusHttpHelper
{
    public CookieCollection gCookies;

    private HttpWebRequest gRequest;

    private HttpWebResponse gResponse;

    public string UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:18.0) Gecko/20100101 Firefox/18.0";

    private string proxyAddress = string.Empty;

    private int port = 80;

    private string proxyUsername = string.Empty;

    private string proxyPassword = string.Empty;

    public GlobusHttpHelper()
    {
    }

    public void ChangeProxy(string proxyAddress, int port, string proxyUsername, string proxyPassword)
    {
        try
        {
            WebProxy webProxy = new WebProxy(proxyAddress, port)
            {
                BypassProxyOnLocal = false
            };
            if ((string.IsNullOrEmpty(proxyUsername) ? false : !string.IsNullOrEmpty(proxyPassword)))
            {
                webProxy.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
            }
            this.gRequest.Proxy = webProxy;
        }
        catch (Exception exception)
        {
        }
    }

    public string getHtmlfromAsx(Uri url)
    {
        string str;
        this.setExpect100Continue();
        this.gRequest = (HttpWebRequest)WebRequest.Create(url);
        this.gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:18.0) Gecko/20100101 Firefox/18.0";
        this.gRequest.CookieContainer = new CookieContainer();
        this.gRequest.ContentType = "video/x-ms-asf";
        if ((this.gCookies == null ? false : this.gCookies.Count > 0))
        {
            this.gRequest.CookieContainer.Add(this.gCookies);
            this.setExpect100Continue();
        }
        this.gResponse = (HttpWebResponse)this.gRequest.GetResponse();
        this.setExpect100Continue();
        if (this.gResponse.StatusCode == HttpStatusCode.OK)
        {
            this.gResponse.Cookies = this.gRequest.CookieContainer.GetCookies(this.gRequest.RequestUri);
            this.setExpect100Continue();
            if (this.gResponse.Cookies.Count > 0)
            {
                if (this.gCookies == null)
                {
                    this.gCookies = this.gResponse.Cookies;
                }
                else
                {
                    foreach (Cookie cooky in this.gResponse.Cookies)
                    {
                        bool flag = false;
                        foreach (Cookie gCooky in this.gCookies)
                        {
                            if (gCooky.Name != cooky.Name)
                            {
                                continue;
                            }
                            gCooky.Value = cooky.Value;
                            flag = true;
                            break;
                        }
                        if (flag)
                        {
                            continue;
                        }
                        this.gCookies.Add(cooky);
                    }
                }
            }
            StreamReader streamReader = new StreamReader(this.gResponse.GetResponseStream());
            string end = streamReader.ReadToEnd();
            streamReader.Close();
            str = end;
        }
        else
        {
            str = "Error";
        }
        return str;
    }

    public string getHtmlfromUrl(Uri url)
    {
        string str;
        string empty = string.Empty;
        try
        {
            this.setExpect100Continue();
            this.gRequest = (HttpWebRequest)WebRequest.Create(url);
            this.gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:18.0) Gecko/20100101 Firefox/18.0";
            this.gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            this.gRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
            this.gRequest.Headers["Accept-Language"] = "en-us,en;q=0.5";
            this.gRequest.KeepAlive = true;
            this.gRequest.AllowAutoRedirect = true;
            this.gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            this.gRequest.CookieContainer = new CookieContainer();
            this.gRequest.Method = "GET";
           this.ChangeProxy(this.proxyAddress, this.port, this.proxyUsername, this.proxyPassword);
            if ((this.gCookies == null ? false : this.gCookies.Count > 0))
            {
                this.setExpect100Continue();
                try
                {
                    this.gRequest.CookieContainer.Add(this.gCookies);
                }
                catch
                {
                    foreach (Cookie gCooky in this.gCookies)
                    {
                        gCooky.Domain = url.Host;
                        this.gRequest.CookieContainer.Add(gCooky);
                    }
                }
            }
            this.setExpect100Continue();
            this.gResponse = (HttpWebResponse)this.gRequest.GetResponse();
            if (this.gResponse.StatusCode == HttpStatusCode.OK)
            {
                this.setExpect100Continue();
                this.gResponse.Cookies = this.gRequest.CookieContainer.GetCookies(this.gRequest.RequestUri);
                if (this.gResponse.Cookies.Count > 0)
                {
                    if (this.gCookies == null)
                    {
                        this.gCookies = this.gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie cooky in this.gResponse.Cookies)
                        {
                            bool flag = false;
                            foreach (Cookie value in this.gCookies)
                            {
                                if (value.Name != cooky.Name)
                                {
                                    continue;
                                }
                                value.Value = cooky.Value;
                                flag = true;
                                break;
                            }
                            if (flag)
                            {
                                continue;
                            }
                            this.gCookies.Add(cooky);
                        }
                    }
                }
                StreamReader streamReader = new StreamReader(this.gResponse.GetResponseStream());
                empty = streamReader.ReadToEnd();
                streamReader.Close();
                str = empty;
            }
            else
            {
                str = "Error";
            }
        }
        catch
        {
            return empty;
        }
        return str;
    }

    public string getHtmlfromUrlProxy(Uri url, string proxyAddress, int port, string proxyUsername, string proxyPassword)
    {
        
        string str;
        string empty = string.Empty;
        try
        {
            this.setExpect100Continue();
            this.gRequest = (HttpWebRequest)WebRequest.Create(url);
            this.gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:18.0) Gecko/20100101 Firefox/18.0";
            this.gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            this.gRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
            this.gRequest.Headers["Accept-Language"] = "en-us,en;q=0.5";
            this.gRequest.KeepAlive = true;
            this.gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            this.gRequest.Headers["X-IsAJAXForm"] = "1";
            this.gRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            this.proxyAddress = proxyAddress;
            this.port = port;
            this.proxyUsername = proxyUsername;
            this.proxyPassword = proxyPassword;
            this.gRequest.ProtocolVersion=HttpVersion.Version10;           
            
            this.ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);
            this.gRequest.CookieContainer = new CookieContainer();
            this.gRequest.Method = "GET";
            this.gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            if ((this.gCookies == null ? false : this.gCookies.Count > 0))
            {
                this.setExpect100Continue();
                this.gRequest.CookieContainer.Add(this.gCookies);
            }
            this.setExpect100Continue();
            this.gResponse = (HttpWebResponse)this.gRequest.GetResponse();
            if (this.gResponse.StatusCode == HttpStatusCode.OK)
            {
                this.setExpect100Continue();
                this.gResponse.Cookies = this.gRequest.CookieContainer.GetCookies(this.gRequest.RequestUri);
                if (this.gResponse.Cookies.Count > 0)
                {
                    if (this.gCookies == null)
                    {
                        this.gCookies = this.gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie cooky in this.gResponse.Cookies)
                        {
                            bool flag = false;
                            foreach (Cookie gCooky in this.gCookies)
                            {
                                if (gCooky.Name != cooky.Name)
                                {
                                    continue;
                                }
                                gCooky.Value = cooky.Value;
                                flag = true;
                                break;
                            }
                            if (flag)
                            {
                                continue;
                            }
                            this.gCookies.Add(cooky);
                        }
                    }
                }
                StreamReader streamReader = new StreamReader(this.gResponse.GetResponseStream());
                empty = streamReader.ReadToEnd();
                streamReader.Close();
                str = empty;
            }
            else
            {
                str = "Error";
            }
        }
        catch
        {
            return empty;
        }
        return str;
    }

    public void HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc, string proxyAddress, int proxyPort, string proxyUsername, string proxyPassword)
    {
        long ticks = DateTime.Now.Ticks;
        string str = string.Concat("---------------------------", ticks.ToString("x"));
        byte[] bytes = Encoding.ASCII.GetBytes(string.Concat("\r\n--", str, "\r\n"));
        this.gRequest = (HttpWebRequest)WebRequest.Create(url);
        this.gRequest.ContentType = string.Concat("multipart/form-data; boundary=", str);
        this.gRequest.Method = "POST";
        this.gRequest.KeepAlive = true;
        this.gRequest.Credentials = CredentialCache.DefaultCredentials;
        this.ChangeProxy(proxyAddress, proxyPort, proxyUsername, proxyPassword);
        this.gRequest.CookieContainer = new CookieContainer();
        if ((this.gCookies == null ? false : this.gCookies.Count > 0))
        {
            this.gRequest.CookieContainer.Add(this.gCookies);
        }
        Stream requestStream = this.gRequest.GetRequestStream();
        string str1 = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
        foreach (string key in nvc.Keys)
        {
            requestStream.Write(bytes, 0, (int)bytes.Length);
            string str2 = string.Format(str1, key, nvc[key]);
            byte[] numArray = Encoding.UTF8.GetBytes(str2);
            requestStream.Write(numArray, 0, (int)numArray.Length);
        }
        requestStream.Write(bytes, 0, (int)bytes.Length);
        string str3 = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", paramName, file, contentType);
        byte[] bytes1 = Encoding.UTF8.GetBytes(str3);
        requestStream.Write(bytes1, 0, (int)bytes1.Length);
        FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
        byte[] numArray1 = new byte[4096];
        int num = 0;
        while (true)
        {
            int num1 = fileStream.Read(numArray1, 0, (int)numArray1.Length);
            num = num1;
            if (num1 == 0)
            {
                break;
            }
            requestStream.Write(numArray1, 0, num);
        }
        fileStream.Close();
        byte[] bytes2 = Encoding.ASCII.GetBytes(string.Concat("\r\n--", str, "--\r\n"));
        requestStream.Write(bytes2, 0, (int)bytes2.Length);
        requestStream.Close();
        if ((this.gCookies == null ? false : this.gCookies.Count > 0))
        {
            this.gRequest.CookieContainer.Add(this.gCookies);
        }
        WebResponse response = null;
        try
        {
            try
            {
                response = this.gRequest.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());
            }
            catch (Exception exception)
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
        }
        finally
        {
            this.gRequest = null;
        }
    }

    public string HttpUploadFileBackground(string url, string file, string paramName, string contentType, NameValueCollection nvc, bool IsLocalFile, ref string status)
    {
        string str;
        string empty = string.Empty;
        long ticks = DateTime.Now.Ticks;
        string str1 = string.Concat("---------------------------", ticks.ToString());
        byte[] bytes = Encoding.ASCII.GetBytes(string.Concat("\r\n--", str1, "\r\n"));
        this.gRequest = (HttpWebRequest)WebRequest.Create("http://www.linkedin.com/mupld/upload");
        this.gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:12.0) Gecko/20100101 Firefox/12.0";
        this.gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        this.gRequest.Headers["Accept-Language"] = "en-us,en;q=0.5";
        this.gRequest.KeepAlive = true;
        this.gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        string str2 = string.Concat(str1, "\r\n");
        byte[] numArray = Encoding.ASCII.GetBytes(string.Concat("--", str1, "\r\n"));
        this.gRequest.ContentType = string.Concat("multipart/form-data; boundary=", str2);
        this.gRequest.Method = "POST";
        this.gRequest.Credentials = CredentialCache.DefaultCredentials;
        this.gRequest.Referer = "http://www.linkedin.com/mupld/upload";
        this.ChangeProxy(this.proxyAddress, this.port, this.proxyUsername, this.proxyPassword);
        this.gRequest.CookieContainer = new CookieContainer();
        if ((this.gCookies == null ? false : this.gCookies.Count > 0))
        {
            this.gRequest.CookieContainer.Add(this.gCookies);
        }
        Stream requestStream = this.gRequest.GetRequestStream();
        string str3 = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
        int num = 0;
        foreach (string key in nvc.Keys)
        {
            if (num == 0)
            {
                requestStream.Write(numArray, 0, (int)numArray.Length);
                num++;
            }
            else
            {
                requestStream.Write(bytes, 0, (int)bytes.Length);
            }
            string str4 = string.Format(str3, key, nvc[key]);
            byte[] bytes1 = Encoding.UTF8.GetBytes(str4);
            requestStream.Write(bytes1, 0, (int)bytes1.Length);
        }
        requestStream.Write(bytes, 0, (int)bytes.Length);
        if (IsLocalFile)
        {
            string str5 = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"Logo.png\"\r\nContent-Type: image/png {2}\r\n\r\n", "file", file, "image/png");
            byte[] numArray1 = Encoding.UTF8.GetBytes(str5);
            requestStream.Write(numArray1, 0, (int)numArray1.Length);
            if (!string.IsNullOrEmpty(file))
            {
                FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                byte[] numArray2 = new byte[4096];
                int num1 = 0;
                while (true)
                {
                    int num2 = fileStream.Read(numArray2, 0, (int)numArray2.Length);
                    num1 = num2;
                    if (num2 == 0)
                    {
                        break;
                    }
                    requestStream.Write(numArray2, 0, num1);
                }
                fileStream.Close();
            }
        }
        byte[] bytes2 = Encoding.ASCII.GetBytes(string.Concat("\r\n--", str1, "--\r\n"));
        requestStream.Write(bytes2, 0, (int)bytes2.Length);
        requestStream.Close();
        if ((this.gCookies == null ? false : this.gCookies.Count > 0))
        {
            this.gRequest.CookieContainer.Add(this.gCookies);
        }
        WebResponse response = null;
        try
        {
            response = this.gRequest.GetResponse();
            this.gResponse = (HttpWebResponse)this.gRequest.GetResponse();
            StreamReader streamReader = new StreamReader(this.gResponse.GetResponseStream());
            empty = streamReader.ReadToEnd();
            streamReader.Close();
            status = "okay";
            str = empty;
        }
        catch (Exception exception)
        {
            if (response != null)
            {
                response.Close();
                response = null;
            }
            str = null;
        }
        return str;
    }

    public string HttpUploadFileBackground1(string url, string file, string paramName, string contentType, NameValueCollection nvc, bool IsLocalFile, ref string status)
    {
        string str;
        string empty = string.Empty;
        long ticks = DateTime.Now.Ticks;
        string str1 = string.Concat("---------------------------", ticks.ToString());
        byte[] bytes = Encoding.ASCII.GetBytes(string.Concat("\r\n--", str1, "\r\n"));
        this.gRequest = (HttpWebRequest)WebRequest.Create("http://www.linkedin.com/mupld/upload");
        this.gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:12.0) Gecko/20100101 Firefox/12.0";
        this.gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        this.gRequest.Headers["Accept-Language"] = "en-us,en;q=0.5";
        this.gRequest.KeepAlive = true;
        this.gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        string str2 = string.Concat(str1, "\r\n");
        byte[] numArray = Encoding.ASCII.GetBytes(string.Concat("--", str1, "\r\n"));
        this.gRequest.ContentType = string.Concat("multipart/form-data; boundary=", str2);
        this.gRequest.Method = "POST";
        this.gRequest.Credentials = CredentialCache.DefaultCredentials;
        this.gRequest.Referer = "http://www.linkedin.com/mupld/upload";
        this.ChangeProxy(this.proxyAddress, this.port, this.proxyUsername, this.proxyPassword);
        this.gRequest.CookieContainer = new CookieContainer();
        if ((this.gCookies == null ? false : this.gCookies.Count > 0))
        {
            this.gRequest.CookieContainer.Add(this.gCookies);
        }
        Stream requestStream = this.gRequest.GetRequestStream();
        string str3 = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
        int num = 0;
        foreach (string key in nvc.Keys)
        {
            if (num == 0)
            {
                requestStream.Write(numArray, 0, (int)numArray.Length);
                num++;
            }
            else
            {
                requestStream.Write(bytes, 0, (int)bytes.Length);
            }
            string str4 = string.Format(str3, key, nvc[key]);
            byte[] bytes1 = Encoding.UTF8.GetBytes(str4);
            requestStream.Write(bytes1, 0, (int)bytes1.Length);
        }
        requestStream.Write(bytes, 0, (int)bytes.Length);
        if (IsLocalFile)
        {
            string str5 = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"Logo.png\"\r\nContent-Type: image/png {2}\r\n\r\n", "file", file, "image/jpeg");
            byte[] numArray1 = Encoding.UTF8.GetBytes(str5);
            requestStream.Write(numArray1, 0, (int)numArray1.Length);
            if (!string.IsNullOrEmpty(file))
            {
                FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                byte[] numArray2 = new byte[4096];
                int num1 = 0;
                while (true)
                {
                    int num2 = fileStream.Read(numArray2, 0, (int)numArray2.Length);
                    num1 = num2;
                    if (num2 == 0)
                    {
                        break;
                    }
                    requestStream.Write(numArray2, 0, num1);
                }
                fileStream.Close();
            }
        }
        byte[] bytes2 = Encoding.ASCII.GetBytes(string.Concat("\r\n--", str1, "--\r\n"));
        requestStream.Write(bytes2, 0, (int)bytes2.Length);
        requestStream.Close();
        if ((this.gCookies == null ? false : this.gCookies.Count > 0))
        {
            this.gRequest.CookieContainer.Add(this.gCookies);
        }
        WebResponse response = null;
        try
        {
            response = this.gRequest.GetResponse();
            this.gResponse = (HttpWebResponse)this.gRequest.GetResponse();
            StreamReader streamReader = new StreamReader(this.gResponse.GetResponseStream());
            empty = streamReader.ReadToEnd();
            streamReader.Close();
            status = "okay";
            str = empty;
        }
        catch (Exception exception)
        {
            if (response != null)
            {
                response.Close();
                response = null;
            }
            str = null;
        }
        return str;
    }

    public void MultiPartImageUpload(string Username, string Password, string localImagePath)
    {
    }

    public void MultiPartImageUpload(string Username, string Password, string localImagePath, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword)
    {
    }

    public string postFormData(Uri formActionUrl, string postData, string Refer = "")
    {
        string str;
        string empty = string.Empty;
        try
        {
            this.gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);
            this.gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:18.0) Gecko/20100101 Firefox/18.0";
            this.gRequest.CookieContainer = new CookieContainer();
            this.gRequest.Method = "POST";
            this.gRequest.Accept = " text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";
            this.gRequest.KeepAlive = true;
            this.gRequest.ContentType = "application/x-www-form-urlencoded";
            if (Refer != "")
                this.gRequest.Referer = Refer;
            this.ChangeProxy(this.proxyAddress, this.port, this.proxyUsername, this.proxyPassword);
            if ((this.gCookies != null && this.gCookies.Count > 0))
            {
                this.setExpect100Continue();
                this.gRequest.CookieContainer.Add(this.gCookies);
            }
            try
            {
                this.setExpect100Continue();
                string.Format(postData, new object[0]);
                byte[] bytes = Encoding.GetEncoding(1252).GetBytes(postData);
                this.gRequest.ContentLength = (long)((int)bytes.Length);
                Stream requestStream = this.gRequest.GetRequestStream();
                requestStream.Write(bytes, 0, (int)bytes.Length);
                requestStream.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            try
            {
                this.gResponse = (HttpWebResponse)this.gRequest.GetResponse();
            }
            catch (Exception exception1)
            {
                Console.WriteLine(exception1);
            }
            if (this.gResponse.StatusCode == HttpStatusCode.OK)
            {
                this.setExpect100Continue();
                this.gResponse.Cookies = this.gRequest.CookieContainer.GetCookies(this.gRequest.RequestUri);
                if (this.gResponse.Cookies.Count > 0)
                {
                    if (this.gCookies == null)
                    {
                        this.gCookies = this.gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie cooky in this.gResponse.Cookies)
                        {
                            bool flag = false;
                            foreach (Cookie gCooky in this.gCookies)
                            {
                                if (gCooky.Name != cooky.Name)
                                {
                                    continue;
                                }
                                gCooky.Value = cooky.Value;
                                flag = true;
                                break;
                            }
                            if (flag)
                            {
                                continue;
                            }
                            this.gCookies.Add(cooky);
                        }
                    }
                }
                StreamReader streamReader = new StreamReader(this.gResponse.GetResponseStream());
                empty = streamReader.ReadToEnd();
                streamReader.Close();
                str = empty;
            }
            else
            {
                str = "Error in posting data";
            }
        }
        catch
        {
            return empty;
        }
        return str;
    }

    public string postFormDataProxy(Uri formActionUrl, string postData, string proxyAddress, int port, string proxyUsername, string proxyPassword)
    {
        string str;
        this.gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);
        this.gRequest.UserAgent = "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.16) Gecko/20110319 Firefox/3.6.16";
        this.gRequest.CookieContainer = new CookieContainer();
        this.gRequest.Method = "POST";
        this.gRequest.Accept = " text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";
        this.gRequest.KeepAlive = true;
        this.gRequest.ContentType = "application/x-www-form-urlencoded";
        this.ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);
        if ((this.gCookies == null ? false : this.gCookies.Count > 0))
        {
            this.setExpect100Continue();
            this.gRequest.CookieContainer.Add(this.gCookies);
        }
        try
        {
            this.setExpect100Continue();
            string.Format(postData, new object[0]);
            byte[] bytes = Encoding.GetEncoding(1252).GetBytes(postData);
            this.gRequest.ContentLength = (long)((int)bytes.Length);
            Stream requestStream = this.gRequest.GetRequestStream();
            requestStream.Write(bytes, 0, (int)bytes.Length);
            requestStream.Close();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
        try
        {
            this.gResponse = (HttpWebResponse)this.gRequest.GetResponse();
        }
        catch (Exception exception1)
        {
            Console.WriteLine(exception1);
        }
        if (this.gResponse.StatusCode == HttpStatusCode.OK)
        {
            this.setExpect100Continue();
            this.gResponse.Cookies = this.gRequest.CookieContainer.GetCookies(this.gRequest.RequestUri);
            if (this.gResponse.Cookies.Count > 0)
            {
                if (this.gCookies == null)
                {
                    this.gCookies = this.gResponse.Cookies;
                }
                else
                {
                    foreach (Cookie cooky in this.gResponse.Cookies)
                    {
                        bool flag = false;
                        foreach (Cookie gCooky in this.gCookies)
                        {
                            if (gCooky.Name != cooky.Name)
                            {
                                continue;
                            }
                            gCooky.Value = cooky.Value;
                            flag = true;
                            break;
                        }
                        if (flag)
                        {
                            continue;
                        }
                        this.gCookies.Add(cooky);
                    }
                }
            }
            StreamReader streamReader = new StreamReader(this.gResponse.GetResponseStream());
            string end = streamReader.ReadToEnd();
            streamReader.Close();
            str = end;
        }
        else
        {
            str = "Error in posting data";
        }
        return str;
    }

    public string postFormDataRef(Uri formActionUrl, string postData, string Referes, string Token, string AccountUserAgent)
    {
        string str;
        this.gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);
        if (!string.IsNullOrEmpty(AccountUserAgent))
        {
            this.gRequest.UserAgent = AccountUserAgent;
        }
        else
        {
            this.gRequest.UserAgent = this.UserAgent;
        }
        this.gRequest.CookieContainer = new CookieContainer();
        this.gRequest.Method = "POST";
        this.gRequest.Accept = "application/json,text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";
        this.gRequest.KeepAlive = true;
        this.gRequest.ContentType = "application/x-www-form-urlencoded";
        this.gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        this.gRequest.Headers.Add("Accept-Encoding", "gzip");
        if (!string.IsNullOrEmpty(Referes))
        {
            this.gRequest.Referer = Referes;
        }
        if (!string.IsNullOrEmpty(Token))
        {
            this.gRequest.Headers.Add("X-CSRFToken", Token);
            this.gRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
        }
        this.ChangeProxy(this.proxyAddress, this.port, this.proxyUsername, this.proxyPassword);
        if ((this.gCookies == null ? false : this.gCookies.Count > 0))
        {
            this.setExpect100Continue();
            this.gRequest.CookieContainer.Add(this.gCookies);
        }
        try
        {
            this.setExpect100Continue();
            string.Format(postData, new object[0]);
            byte[] bytes = Encoding.GetEncoding(1252).GetBytes(postData);
            this.gRequest.ContentLength = (long)((int)bytes.Length);
            Stream requestStream = this.gRequest.GetRequestStream();
            requestStream.Write(bytes, 0, (int)bytes.Length);
            requestStream.Close();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
        try
        {
            this.gResponse = (HttpWebResponse)this.gRequest.GetResponse();
        }
        catch (Exception exception1)
        {
            Console.WriteLine(exception1);
        }
        if (this.gResponse.StatusCode == HttpStatusCode.OK)
        {
            this.setExpect100Continue();
            this.gResponse.Cookies = this.gRequest.CookieContainer.GetCookies(this.gRequest.RequestUri);
            if (this.gResponse.Cookies.Count > 0)
            {
                if (this.gCookies == null)
                {
                    this.gCookies = this.gResponse.Cookies;
                }
                else
                {
                    foreach (Cookie cooky in this.gResponse.Cookies)
                    {
                        bool flag = false;
                        foreach (Cookie gCooky in this.gCookies)
                        {
                            if (gCooky.Name != cooky.Name)
                            {
                                continue;
                            }
                            gCooky.Value = cooky.Value;
                            flag = true;
                            break;
                        }
                        if (flag)
                        {
                            continue;
                        }
                        this.gCookies.Add(cooky);
                    }
                }
            }
            StreamReader streamReader = new StreamReader(this.gResponse.GetResponseStream());
            string end = streamReader.ReadToEnd();
            streamReader.Close();
            str = end;
        }
        else
        {
            str = "Error in posting data";
        }
        return str;
    }

    public string postFormDataRef(Uri formActionUrl, string postData, string Referes, string Token, string XRequestedWith, string XPhx, string Origin)
    {
        string str;
        this.gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);
        this.gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:12.0) Gecko/20100101 Firefox/12.0";
        this.gRequest.CookieContainer = new CookieContainer();
        this.gRequest.Method = "POST";
        this.gRequest.Accept = " text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";
        this.gRequest.KeepAlive = true;
        this.gRequest.ContentType = "application/x-www-form-urlencoded";
        this.gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        if (!string.IsNullOrEmpty(Referes))
        {
            this.gRequest.Referer = Referes;
        }
        if (!string.IsNullOrEmpty(Token))
        {
            this.gRequest.Headers.Add("X-CSRFToken", Token);
        }
        if (!string.IsNullOrEmpty(XRequestedWith))
        {
            this.gRequest.Headers.Add("X-Requested-With", XRequestedWith);
        }
        if (!string.IsNullOrEmpty(XPhx))
        {
            this.gRequest.Headers.Add("X-PHX", XPhx);
        }
        if (!string.IsNullOrEmpty(Origin))
        {
            this.gRequest.Headers.Add("Origin", Origin);
        }
        this.ChangeProxy(this.proxyAddress, this.port, this.proxyUsername, this.proxyPassword);
        if ((this.gCookies == null ? false : this.gCookies.Count > 0))
        {
            this.setExpect100Continue();
            this.gRequest.CookieContainer.Add(this.gCookies);
        }
        try
        {
            this.setExpect100Continue();
            string.Format(postData, new object[0]);
            byte[] bytes = Encoding.GetEncoding(1252).GetBytes(postData);
            this.gRequest.ContentLength = (long)((int)bytes.Length);
            Stream requestStream = this.gRequest.GetRequestStream();
            requestStream.Write(bytes, 0, (int)bytes.Length);
            requestStream.Close();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
        try
        {
            this.gResponse = (HttpWebResponse)this.gRequest.GetResponse();
        }
        catch (Exception exception1)
        {
            Console.WriteLine(exception1);
        }
        if (this.gResponse.StatusCode == HttpStatusCode.OK)
        {
            this.setExpect100Continue();
            this.gResponse.Cookies = this.gRequest.CookieContainer.GetCookies(this.gRequest.RequestUri);
            if (this.gResponse.Cookies.Count > 0)
            {
                if (this.gCookies == null)
                {
                    this.gCookies = this.gResponse.Cookies;
                }
                else
                {
                    foreach (Cookie cooky in this.gResponse.Cookies)
                    {
                        bool flag = false;
                        foreach (Cookie gCooky in this.gCookies)
                        {
                            if (gCooky.Name != cooky.Name)
                            {
                                continue;
                            }
                            gCooky.Value = cooky.Value;
                            flag = true;
                            break;
                        }
                        if (flag)
                        {
                            continue;
                        }
                        this.gCookies.Add(cooky);
                    }
                }
            }
            StreamReader streamReader = new StreamReader(this.gResponse.GetResponseStream());
            string end = streamReader.ReadToEnd();
            streamReader.Close();
            str = end;
        }
        else
        {
            str = "Error in posting data";
        }
        return str;
    }

    public void setExpect100Continue()
    {
        if (ServicePointManager.Expect100Continue)
        {
            ServicePointManager.Expect100Continue = false;
        }
    }

    public void setExpect100ContinueToTrue()
    {
        if (!ServicePointManager.Expect100Continue)
        {
            ServicePointManager.Expect100Continue = true;
        }
    }



    //public static bool SetAllowUnsafeHeaderParsing20()
    //{
    //    //Get the assembly that contains the internal class
    //    Assembly aNetAssembly=Assembly.GetAssembly(typeof(System.Net.Configuration.SettingsSection));
    //    if(aNetAssembly!=null)
    //    {
    //        //Use the assembly in order to get the internal type for the internal class
    //        Type aSettingsType=aNetAssembly.GetType("System.Net.Configuration.SettingsSectionInternal");
    //        if(aSettingsType!=null)
    //        {
    //            //Use the internal static property to get an instance of the internal settings class.
    //            //If the static instance isn't created allready the property will create it for us.
    //            object anInstance=aSettingsType.InvokeMember("Section",
    //              BindingFlags.Static|BindingFlags.GetProperty|BindingFlags.NonPublic, null, null, new object[] { });

    //            if(anInstance!=null)
    //            {
    //                //Locate the private bool field that tells the framework is unsafe header parsing should be allowed or not
    //                FieldInfo aUseUnsafeHeaderParsing=aSettingsType.GetField("useUnsafeHeaderParsing", BindingFlags.NonPublic|BindingFlags.Instance);
    //                if(aUseUnsafeHeaderParsing!=null)
    //                {
    //                    aUseUnsafeHeaderParsing.SetValue(anInstance, true);
    //                    return true;
    //                }
    //            }
    //        }
    //    }
    //    return false;
    //}
}