using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebService.Contract;
using CommonLogic;
using System.IO;
using System.Net;
using System.Web;
using WeChatLogic;
using WebService.Enum;

namespace WebService.Facade
{
    public class MiddleServiceMessage
    {
        private static string str_corpid = System.Configuration.ConfigurationSettings.AppSettings["CorpID"];//"wx975e51a5a2d78d74";
        private static string str_corpsecret = System.Configuration.ConfigurationSettings.AppSettings["Secret"];
        private static string MiddleServiceUrl = System.Configuration.ConfigurationSettings.AppSettings["middleService"];
        private static string Agentid = System.Configuration.ConfigurationSettings.AppSettings["Agentid"];
        CommonLogic.LogHelper _SendLog = new CommonLogic.LogHelper("SendLog");

        MessageBiz messageBiz = new MessageBiz();

        public void SendMessage(string inputMessage, string user)
        {
            string userContextIdKey = user + "ContextIdKey";

            QueryInfo QueryInfo = new QueryInfo();
            QueryInfo.QueryMessage = inputMessage;
            QueryInfo.QueryChannel = "2";
            var cacheContextId = CommonLogic.CacheHelper.GetCache(userContextIdKey);
            if (cacheContextId != null)
            {
                QueryInfo.ContextId = cacheContextId.ToString();
            }

            string data = JsonHelper.JsonSerializer(QueryInfo);
            var result = GetPage(MiddleServiceUrl, data);
            _SendLog.WriteLog(result);
            ResponseInfo<WeChatAnswerList> JsonResullt = new ResponseInfo<WeChatAnswerList>();
            JsonResullt = JsonHelper.JsonDeserialize<ResponseInfo<WeChatAnswerList>>(result);
            switch (JsonResullt.responseInfo._msgtype.ToLower())
            {
                case "text":
                    SendSingleMessage(user, JsonResullt);
                    break;
                case "mpnews":
                    SendMpMessage(user, JsonResullt);
                    break;
                case "news":
                    SendNewsMessage(user, JsonResullt);
                    break;
                default:
                    //_wechatBiz = new WechatBiz();
                    break;
            }

            if (JsonResullt.queryInfo.ContextId == null || JsonResullt.queryInfo.ContextId == "")
            {
                CommonLogic.CacheHelper.RemoveAllCache(userContextIdKey);
            }
            else
            {
                CommonLogic.CacheHelper.SetCache(userContextIdKey, JsonResullt.queryInfo.ContextId, new TimeSpan(1, 0, 0));
            }
        }

        private void SendSingleMessage(string user, ResponseInfo<WeChatAnswerList> JsonResullt)
        {
            SendSingleMessage(user, JsonResullt.responseInfo._content);
        }

        private void SendSingleMessage(string user, string message)
        {
            SendTextRequest str = new SendTextRequest();
            str.agentid = Agentid;
            str.safe = "0";
            str.touser = user;
            SendTextRequest.Text text = new SendTextRequest.Text();
            str.msgtype = "text";
            text.content = message;
            str.text = text;

            messageBiz.Send<SendTextRequest>(str);
        }


        private void SendNewsMessage(string user, ResponseInfo<WeChatAnswerList> JsonResullt)
        {
            BotService.WebService1SoapClient BotService = new BotService.WebService1SoapClient();

            string str_AccessToken = TokenBiz.GetAccessToken(str_corpid, str_corpsecret);
            SendNewsRequest str = new SendNewsRequest();
            str.agentid = Agentid;
            str.touser = user;
            str.msgtype = "news";
            SendNewsRequest.newsRequest news = new SendNewsRequest.newsRequest();
            news.articles = new List<SendNewsRequest.articles>();

            if (!string.IsNullOrEmpty(JsonResullt.responseInfo._title))
            {
                SendNewsRequest.articles article = new SendNewsRequest.articles();
                article.title = JsonResullt.responseInfo._title;
                article.description = JsonResullt.responseInfo._description;
                article.url = JsonResullt.responseInfo._url;
                if (!string.IsNullOrEmpty(JsonResullt.responseInfo._picurl)) article.picurl = JsonResullt.responseInfo._picurl;

                news.articles.Add(article);
                str.news = news;
                messageBiz.Send<SendNewsRequest>(str);
            }
            else
            {
                var booksResult = BotService.GetBooksList(JsonResullt.responseInfo._actionsid.ToString());
                List<Books> booksListResult = JsonHelper.JsonDeserialize<List<Books>>(booksResult);
                if (booksListResult.Any())
                {
                    booksListResult.ForEach(x =>
                    {
                        SendNewsRequest.articles article = new SendNewsRequest.articles();
                        article.title = x._name;
                        article.description = x._description;
                        article.url = x._url;
                        if (!string.IsNullOrEmpty(x._picurl)) article.picurl = x._picurl;
                        news.articles.Add(article);
                    });

                    str.news = news;
                    messageBiz.Send<SendNewsRequest>(str);
                }
                else
                {
                    SendSingleMessage(user, "Sorry,we haven't that kinds of books .");
                }

            }




        }

        private void SendMpMessage(string user, ResponseInfo<WeChatAnswerList> JsonResullt)
        {

            SendMPNewsRequest _SendMPNewsRequest = new SendMPNewsRequest();
            mpnews _Mpnews = new mpnews();
            _Mpnews.articles = new List<articles>();
            string str_AccessToken = TokenBiz.GetAccessToken(str_corpid, str_corpsecret);
            //string filePath = HttpContext.Current.Request.PhysicalApplicationPath + TSubResultNode.SelectSingleNode("content").InnerText;
            //string content = File.ReadAllText(filePath);

            _SendMPNewsRequest.touser = user;
            _SendMPNewsRequest.msgtype = "mpnews";
            _SendMPNewsRequest.agentid = Agentid;
            _SendMPNewsRequest.safe = "0";


            if (!_Mpnews.articles.Any())//只要一条就可以了
            {
                articles article = new articles();
                string picPath = HttpContext.Current.Request.PhysicalApplicationPath + "LusiResult/Banner/" + JsonResullt.responseInfo._banner;
                _SendLog.WriteLog(picPath);
                UploadResponse media_UpLoadInfo = MediaBiz.CreateInstance().Upload(picPath, str_AccessToken, EnumMediaType.image);
                if (media_UpLoadInfo != null)
                {
                    article.title = JsonResullt.responseInfo._title;
                    article.thumb_media_id = media_UpLoadInfo.media_id;
                    //article.author = "RR Donnelley";
                    article.show_cover_pic = "0";
                    article.digest = JsonResullt.responseInfo._digest;
                    _Mpnews.articles.Add(article);
                }
            }

            _Mpnews.articles.ForEach(x => x.content = JsonResullt.responseInfo._content);
            _SendMPNewsRequest.mpnews = _Mpnews;
            messageBiz.Send<SendMPNewsRequest>(_SendMPNewsRequest);
        }

        private string GetPage(string posturl, string postData)
        {
            Stream outstream = null;
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;


            Encoding encoding = Encoding.UTF8;
            byte[] data = encoding.GetBytes(postData);
            // 准备请求...
            try
            {
                // 设置参数
                request = WebRequest.Create(posturl) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                //request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                request.Timeout = 5 * 60 * 1000;
                outstream = request.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Close();
                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                instream = response.GetResponseStream();
                sr = new StreamReader(instream, encoding);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                string err = string.Empty;
                return content;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                HttpContext.Current.Response.Write(err);
                return string.Empty;
            }
        }
    }
}
