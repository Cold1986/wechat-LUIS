using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using WebService.Contract;

namespace WeChatLogic
{
    public class MenuBiz : BaseBiz
    {
        public MenuCreateResponse Create<T>(T request)
        {
            string strJson = JsonConvert.SerializeObject(request);
            //throw new Exception(strJson);
            //_SendLog.WriteLog("new: " + strJson);
            base.url = "https://qyapi.weixin.qq.com/cgi-bin/menu/create?agentid=5&access_token=" + TokenBiz.GetAccessToken(str_corpid, str_corpsecret);
            base.para = strJson;
            base.method = "POST";
            base.needAccessToken = true;
            return base.GetUrlReturn<MenuCreateResponse>();
        }
    }
}
