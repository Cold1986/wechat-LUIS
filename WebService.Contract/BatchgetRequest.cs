using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebService.Contract
{
    /// <summary>
    /// 获取应用素材素材列表请求参数
    /// </summary>
    public class BatchgetRequest
    {
        /// <summary>
        /// 素材类型，可以为图文(mpnews)、图片（image）、音频（voice）、视频（video）、文件（file）
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 企业应用的id，整型。可在应用的设置页面查看
        /// </summary>
        public int agentid { get; set; }

        /// <summary>
        /// 从该类型素材的该偏移位置开始返回，0表示从第一个素材 返回
        /// </summary>
        public int offset { get; set; }

        /// <summary>
        /// 返回素材的数量，取值在1到50之间
        /// </summary>
        public int count { get; set; }
    }
}
