using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebService.Contract
{
    public class SendNewsRequest
    {
        /// <summary>
        /// 成员ID列表（消息接收者，多个接收者用‘|’分隔，最多支持1000个）。特殊情况：指定为@all，则向关注该企业应用的全部成员发送
        /// </summary>
        public string touser { get; set; }

        /// <summary>
        /// 部门ID列表，多个接收者用‘|’分隔，最多支持100个。当touser为@all时忽略本参数
        /// </summary>
        public string toparty { get; set; }

        /// <summary>
        /// 标签ID列表，多个接收者用‘|’分隔。当touser为@all时忽略本参数
        /// </summary>
        public string totag { get; set; }

        /// <summary>
        /// 消息类型，此时固定为：mpnews （不支持主页型应用）
        /// </summary>
        public string msgtype { get; set; }

        /// <summary>
        /// 企业应用的id，整型。可在应用的设置页面查看
        /// </summary>
        public string agentid { get; set; }

        public newsRequest news { get; set; }

        public class newsRequest
        {
            public List<articles> articles { get; set; }
        }

        /// <summary>
        /// 图文消息，一个图文消息支持1到8个图文
        /// </summary>
        public class articles
        {
            /// <summary>
            /// 图文消息的标题，不超过128个字节，超过会自动截断
            /// </summary>
            public string title { get; set; }

            /// <summary>
            /// 描述，不超过512个字节，超过会自动截断
            /// </summary>
            public string description { get; set; }

            /// <summary>
            /// 点击后跳转的链接。
            /// </summary>
            public string url { get; set; }

            /// <summary>
            /// 图文消息的图片链接，支持JPG、PNG格式，较好的效果为大图640*320，小图80*80。如不填，在客户端不显示图片
            /// </summary>
            public string picurl { get; set; }
        }
    }


}
