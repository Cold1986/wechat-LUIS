using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebService.Contract;
using WeChatLogic;
using WebService.Enum;
using CommonLogic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using System.Net;
using System.Xml;
using System.IO;

namespace WebService.Facade
{
    public class AIMessage
    {
        private static string str_corpid = System.Configuration.ConfigurationSettings.AppSettings["CorpID"];//"wx975e51a5a2d78d74";
        private static string str_corpsecret = System.Configuration.ConfigurationSettings.AppSettings["Secret"];
        private static string Agentid = System.Configuration.ConfigurationSettings.AppSettings["Agentid"];
        private static string xmlPath = HttpContext.Current.Request.PhysicalApplicationPath + System.Configuration.ConfigurationSettings.AppSettings["XmlPath"];

        private static string luis_Key = System.Configuration.ConfigurationSettings.AppSettings["luis_Ocp_Apim_Subscription_Key"];
        private static string luis_Url = System.Configuration.ConfigurationSettings.AppSettings["luis_Url"];
        MessageBiz messageBiz = new MessageBiz();


        public string SendMessage(string inputMessage, string user)
        {
            string xmlFilePath = xmlPath;
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);

            var rp = GetLusiResult(inputMessage);
            if (rp != null)
            {
                var res = rp.intents.OrderByDescending(x => x.score).First();
                if (res.intent.ToLower() == "none" && rp.entities.Count == 0)//没有查到结果返回默认结果
                {
                    XmlNodeList NoResultNodeList = doc.SelectNodes("Results/NoResult");
                    if (NoResultNodeList != null)
                    {
                        foreach (XmlNode noResultNode in NoResultNodeList)
                        {
                            SendSingleMessage(user, noResultNode);
                        }
                    }
                }
                //else if (res.intent.ToLower() != "none" && rp.entities.Count == 0)//有搜到，但不能精确定位
                //{
                //    SendTextRequest str = new SendTextRequest();
                //    str.agentid = Agentid;
                //    str.safe = "0";
                //    str.touser = user;
                //    SendTextRequest.Text text = new SendTextRequest.Text();
                //    str.msgtype = "text";
                //    text.content = "您是要搜索" + res.intent + "么？请再提供更多关键字来精确定位吧。";
                //    str.text = text;
                //    messageBiz.Send<SendTextRequest>(str);
                //}
                else
                {
                    bool HasFind = false;
                    XmlNodeList ResultNodeList = doc.SelectNodes("Results/Result");
                    if (ResultNodeList != null)
                    {
                        foreach (XmlNode ResultNode in ResultNodeList)
                        {
                            if (rp.entities.Any() && ResultNode.Attributes["name"].Value != "" && res.intent.ToLower() == ResultNode.Attributes["name"].Value.ToLower())//查询贺卡/邀请卡
                            {
                                foreach (XmlNode SubResultNode in ResultNode)
                                {
                                    if (rp.entities.First().type == SubResultNode.Attributes["name"].Value)//贺卡/邀请卡::StarProducts"
                                    {
                                        foreach (XmlNode TSubResultNode in SubResultNode)
                                        {
                                            if (rp.entities.First().entity.ToLower() == TSubResultNode.Attributes["name"].Value)
                                            {
                                                if (TSubResultNode.SelectSingleNode("msgtype").InnerText == "text")
                                                {
                                                    HasFind = true;
                                                    SendSingleMessage(user, TSubResultNode);
                                                }
                                                else if (TSubResultNode.SelectSingleNode("msgtype").InnerText == "mpnews")
                                                {
                                                    HasFind = true;
                                                    SendMpMessage(user, TSubResultNode);
                                                }
                                                else if (TSubResultNode.SelectSingleNode("msgtype").InnerText == "image")
                                                {
                                                    HasFind = true;
                                                    SendImageMessage(user, TSubResultNode);
                                                }
                                                else if (TSubResultNode.SelectSingleNode("msgtype").InnerText == "news")
                                                {
                                                    HasFind = true;
                                                    SendNewsMessage(user, TSubResultNode);
                                                }
                                               
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (!HasFind)
                        {
                            foreach (XmlNode ResultNode in ResultNodeList)
                            {
                                if (ResultNode.Attributes["name"].Value != "" && res.intent.ToLower() == ResultNode.Attributes["name"].Value.ToLower())//查询贺卡/邀请卡
                                {
                                    foreach (XmlNode SubResultNode in ResultNode)
                                    {
                                        if (SubResultNode.Attributes["name"].Value.ToLower() == "default")//查询贺卡/邀请卡
                                        {
                                            if (SubResultNode.SelectSingleNode("msgtype").InnerText == "text")
                                            {
                                                HasFind = true;
                                                SendSingleMessage(user, SubResultNode);
                                            }
                                            else if (SubResultNode.SelectSingleNode("msgtype").InnerText == "mpnews")
                                            {
                                                HasFind = true;
                                                SendMpMessage(user, SubResultNode);
                                            }
                                            else if (SubResultNode.SelectSingleNode("msgtype").InnerText == "image")
                                            {
                                                HasFind = true;
                                                SendImageMessage(user, SubResultNode);
                                            }
                                            else if (SubResultNode.SelectSingleNode("msgtype").InnerText == "news")
                                            {
                                                HasFind = true;
                                                SendNewsMessage(user, SubResultNode);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (!HasFind)
                        {
                            SendTextRequest str = new SendTextRequest();
                            str.agentid = Agentid;
                            str.safe = "0";
                            str.touser = user;
                            SendTextRequest.Text text = new SendTextRequest.Text();
                            str.msgtype = "text";
                            text.content = "您是要搜索" + res.intent + "么？请再提供更多关键字来精确定位吧。";
                            str.text = text;
                            messageBiz.Send<SendTextRequest>(str);
                        }
                    }
                }
            }
            return "";

        }

        public void SendSingleMessage(string user, XmlNode TSubResultNode)
        {
            SendTextRequest str = new SendTextRequest();
            str.agentid = Agentid;
            str.safe = "0";
            str.touser = user;
            SendTextRequest.Text text = new SendTextRequest.Text();
            str.msgtype = "text"; // TSubResultNode.SelectSingleNode("msgtype").InnerText;
            text.content = TSubResultNode.SelectSingleNode("content").InnerText;
            str.text = text;

            messageBiz.Send<SendTextRequest>(str);

        }

        public void SendMpMessage(string user, XmlNode TSubResultNode)
        {

            SendMPNewsRequest _SendMPNewsRequest = new SendMPNewsRequest();
            mpnews _Mpnews = new mpnews();
            _Mpnews.articles = new List<articles>();
            string str_AccessToken = TokenBiz.GetAccessToken(str_corpid, str_corpsecret);
            string filePath = HttpContext.Current.Request.PhysicalApplicationPath + TSubResultNode.SelectSingleNode("content").InnerText;
            string content = File.ReadAllText(filePath);

            _SendMPNewsRequest.touser = user;
            _SendMPNewsRequest.msgtype = "mpnews";
            _SendMPNewsRequest.agentid = Agentid;
            _SendMPNewsRequest.safe = "0";


            if (!_Mpnews.articles.Any())//只要一条就可以了
            {
                articles article = new articles();
                string picPath = HttpContext.Current.Request.PhysicalApplicationPath + TSubResultNode.SelectSingleNode("banner").InnerText;
                UploadResponse media_UpLoadInfo = MediaBiz.CreateInstance().Upload(picPath, str_AccessToken, EnumMediaType.image);
                if (media_UpLoadInfo != null)
                {
                    article.title = TSubResultNode.SelectSingleNode("title").InnerText;
                    article.thumb_media_id = media_UpLoadInfo.media_id;
                    //article.author = "RR Donnelley";
                    article.show_cover_pic = "0";
                    article.digest = TSubResultNode.SelectSingleNode("digest").InnerText;
                    _Mpnews.articles.Add(article);
                }
            }


            _Mpnews.articles.ForEach(x => x.content = content);
            _SendMPNewsRequest.mpnews = _Mpnews;
            messageBiz.Send<SendMPNewsRequest>(_SendMPNewsRequest);
        }

        public void SendImageMessage(string user, XmlNode TSubResultNode)
        {
            string str_AccessToken = TokenBiz.GetAccessToken(str_corpid, str_corpsecret);
            SendImageRequest str = new SendImageRequest();
            str.agentid = Agentid;
            str.safe = "0";
            str.touser = user;
            SendImageRequest.Text image = new SendImageRequest.Text();
            str.msgtype = TSubResultNode.SelectSingleNode("msgtype").InnerText;

            string picPath = HttpContext.Current.Request.PhysicalApplicationPath + TSubResultNode.SelectSingleNode("imagePath").InnerText;
            UploadResponse media_UpLoadInfo = MediaBiz.CreateInstance().Upload(picPath, str_AccessToken, EnumMediaType.image);
            if (media_UpLoadInfo != null)
            {
                image.media_id = media_UpLoadInfo.media_id;
            }

            str.image = image;

            messageBiz.Send<SendImageRequest>(str);

            if (!string.IsNullOrEmpty(TSubResultNode.SelectSingleNode("content").InnerText))
            {
                SendSingleMessage(user, TSubResultNode);
            }
        }

        public void SendNewsMessage(string user, XmlNode TSubResultNode)
        {
            string str_AccessToken = TokenBiz.GetAccessToken(str_corpid, str_corpsecret);
            SendNewsRequest str = new SendNewsRequest();
            str.agentid = Agentid;
            str.touser = user;
            str.msgtype = TSubResultNode.SelectSingleNode("msgtype").InnerText;
            SendNewsRequest.newsRequest news = new SendNewsRequest.newsRequest();
            news.articles = new List<SendNewsRequest.articles>();
            SendNewsRequest.articles article=new SendNewsRequest.articles();
            article.title = TSubResultNode.SelectSingleNode("title").InnerText;
            article.description = TSubResultNode.SelectSingleNode("description").InnerText;
            article.url = TSubResultNode.SelectSingleNode("url").InnerText;
            article.picurl = TSubResultNode.SelectSingleNode("picurl").InnerText;
            news.articles.Add(article);
            str.news = news;
       
            messageBiz.Send<SendNewsRequest>(str);

           
        }

        public Lusi GetLusiResult(string inputMessage)
        {
            string strResult = "";
            string uri = luis_Url + "&q=" + inputMessage;
            System.Net.WebRequest wrq = System.Net.WebRequest.Create(uri);
            wrq.Headers.Add("Ocp-Apim-Subscription-Key", luis_Key);
            wrq.Method = "GET";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls; //SSL3协议替换成TLS协议
            System.Net.WebResponse wrp = wrq.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(wrp.GetResponseStream(), System.Text.Encoding.GetEncoding("utf-8"));
            strResult = sr.ReadToEnd();
            Lusi ro = JsonHelper.JsonDeserialize<Lusi>(strResult);

            return ro;
        }
    }


}
