using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLogic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeChatLogic
{
    public class TokenBiz
    {
        static LogHelper _SendLog = new LogHelper("SendLog");

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <param name="corpid"></param>
        /// <param name="corpsecret"></param>
        /// <param name="allowCache">使用缓存</param>
        /// <returns></returns>
        public static string GetAccessToken(string corpid, string corpsecret, bool allowCache = true)
        {
            string result = string.Empty;
            try
            {
                string url = "https://qyapi.weixin.qq.com/cgi-bin/gettoken";
                string para = "corpid=" + corpid + "&corpsecret=" + corpsecret;

                if (allowCache)
                {
                    var cacheValue = CacheHelper.GetCache("AccessToken" + corpid);
                    if (cacheValue != null)
                        result = CacheHelper.GetCache("AccessToken" + corpid).ToString();
                }

                if (string.IsNullOrEmpty(result))
                {
                    var content = RequestHelper.SendRequest(url, para);
                    content = "[" + content + "]";
                    JArray ja = (JArray)JsonConvert.DeserializeObject(content);
                    result = Convert.ToString(ja[0]["access_token"]);
                    if (!string.IsNullOrEmpty(result))
                    {
                        CacheHelper.SetCache("AccessToken" + corpid, result, new TimeSpan(1, 0, 0));
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                result = ex.Message;
                _SendLog.WriteLog(result);
                return string.Empty;
            }
        }
    }
}
