using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebService.Contract;
using Newtonsoft.Json;
using CommonLogic;


namespace WeChatLogic
{
    public class MaterialBiz : BaseBiz
    {
        /// <summary>
        /// 获取应用素材素材列表
        /// </summary>
        /// <returns></returns>
        public BatchgetResponse GetBatchgetResponse(BatchgetRequest bRequest)
        {
            string strJson = JsonConvert.SerializeObject(bRequest);
            base.url = "https://qyapi.weixin.qq.com/cgi-bin/material/batchget?access_token=" + TokenBiz.GetAccessToken(str_corpid, str_corpsecret);
            base.para = strJson;
            base.method = "POST";
            base.needAccessToken = true;
            var res = base.GetUrlReturn<BatchgetResponse>();
            return res;
        }

        /// <summary>
        /// 通过media_id删除上传的图文消息、图片、语音、文件、视频素材。
        /// </summary>
        /// <param name="media_id"></param>
        public void DelMateria(string media_id)
        {
            base.url = "https://qyapi.weixin.qq.com/cgi-bin/material/del";
            base.para = "agentid=5&media_id=" + media_id + "&access_token=" + TokenBiz.GetAccessToken(str_corpid, str_corpsecret);
            base.method = "GET";
            base.needAccessToken = true;
            base.GetUrlReturn<Object>();
        }
    }
}
