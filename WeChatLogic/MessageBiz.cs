using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebService.Contract;
using Newtonsoft.Json;

namespace WeChatLogic
{
    public class MessageBiz : BaseBiz
    {
        public SendResponse Send<T>(T request)
        {
            string strJson = JsonConvert.SerializeObject(request);
            //_SendLog.WriteLog("new: " + strJson);
            base.url = "https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + TokenBiz.GetAccessToken(str_corpid, str_corpsecret);
            base.para = strJson;
            base.method = "POST";
            base.needAccessToken = true;
            var res = base.GetUrlReturn<SendResponse>();
            return res;
        }
    }
}
