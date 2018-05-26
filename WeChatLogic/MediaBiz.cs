using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebService.Contract;
using Newtonsoft.Json;
using System.Net;
using WebService.Enum;
using CommonLogic;

namespace WeChatLogic
{
    public class MediaBiz : BaseBiz
    {
        public static MediaBiz CreateInstance()
        {
            return new MediaBiz();
        }

        /// <summary>
        /// 上传临时素材文件
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="token"></param>
        /// <param name="mt"></param>
        /// <returns></returns>
        public UploadResponse Upload(string filepath, string token, EnumMediaType mt)
        {
            using (WebClient client = new WebClient())
            {
                string retdata = string.Empty;

                var cacheValue = CacheHelper.GetCache("file" + filepath);
                if (cacheValue != null)
                    retdata = CacheHelper.GetCache("file" + filepath).ToString();

                if (string.IsNullOrEmpty(retdata))
                {
                    byte[] b = client.UploadFile(string.Format("https://qyapi.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={1}", token, mt.ToString()), filepath);//调用接口上传文件
                    retdata = Encoding.Default.GetString(b);//获取返回值
                }
                if (retdata.Contains("40001"))
                {
                    CacheHelper.RemoveAllCache("file" + filepath);
                    CacheHelper.RemoveAllCache("AccessToken" + str_corpid);//过期移除缓存，这样会重新去请求获取token
                    byte[] b = client.UploadFile(string.Format("https://qyapi.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={1}", TokenBiz.GetAccessToken(str_corpid, str_corpsecret), mt.ToString()), filepath);//调用接口上传文件
                    retdata = Encoding.Default.GetString(b);//获取返回值
                }

                if (retdata.Contains("media_id"))//判断返回值是否包含media_id，包含则说明上传成功，然后将返回的json字符串转换成json
                {
                    CacheHelper.SetCache("file" + filepath, retdata, new TimeSpan(8, 0, 0));
                    return JsonConvert.DeserializeObject<UploadResponse>(retdata);
                }
                else
                {  //否则，写错误日志
                    _SendLog.WriteLog(retdata);
                    return null;
                }
            }
        }
    }
}
