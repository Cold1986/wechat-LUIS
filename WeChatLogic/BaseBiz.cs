using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLogic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeChatLogic
{
    public class BaseBiz
    {
        public LogHelper _SendLog = new LogHelper("SendLog");

        public string str_corpid = System.Configuration.ConfigurationSettings.AppSettings["CorpID"];//"wx975e51a5a2d78d74";
        public string str_corpsecret = System.Configuration.ConfigurationSettings.AppSettings["Secret"];
        public string url { get; set; }
        public string para { get; set; }
        public string method { get; set; }
        public bool needAccessToken { get; set; }


        /// <summary>
        /// 获取请求返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="para"></param>
        /// <param name="needAccessToken"></param>
        /// <returns></returns>
        public T GetUrlReturn<T>()
        {
            string content = string.Empty;
            content = SendRequest(url, para, method, needAccessToken);
            string tmpContent = "[" + content + "]";
            JArray ja = (JArray)JsonConvert.DeserializeObject(tmpContent);
            int errcode = 0;
            int.TryParse(ja[0]["errcode"].ToString(), out errcode);
            if (errcode == 40001 && needAccessToken)//accessToken 可能过期，重新获取
            {
                CacheHelper.RemoveAllCache("AccessToken" + str_corpid);//过期移除缓存，这样会重新去请求获取token
                content = SendRequest(url, para, method, needAccessToken);
            }
            return JsonHelper.JsonDeserialize<T>(content);
        }

        public virtual string SendRequest(string url, string para, string method = "GET", bool needAccessToken = true)
        {
            return RequestHelper.SendRequest(url, para, method);
        }

    }
}
