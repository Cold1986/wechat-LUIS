using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebService.Contract
{
    /// <summary>
    /// 创建应用菜单请求参数
    /// </summary>
    public class MenuCreateRequest
    {
        /// <summary>
        /// 一级菜单数组，个数应为1~3个
        /// </summary>
        public List<button> button { get; set; }

        ///// <summary>
        ///// 二级菜单数组，个数应为1~5个
        ///// </summary>
        //public sub_button sub_button { get; set; }

        ///// <summary>
        ///// 菜单的响应动作类型
        ///// </summary>
        //public string type { get; set; }

        ///// <summary>
        ///// 菜单标题，不超过16个字节，子菜单不超过40个字节
        ///// </summary>
        //public string name { get; set; }

        ///// <summary>
        ///// 菜单KEY值，用于消息接口推送，不超过128字节
        ///// </summary>
        //public string key { get; set; }

        ///// <summary>
        ///// 网页链接，成员点击菜单可打开链接，不超过256字节
        ///// </summary>
        //public string url { get; set; }
    }

    public class button
    {
        public string name { get; set; }

        public string type { get; set; }

        public string url { get; set; }

        public List<sub_button> sub_button { get; set; }
    }

    /// <summary>
    /// 二级菜单数组，个数应为1~5个
    /// </summary>
    public class sub_button
    {
        /// <summary>
        /// 菜单的响应动作类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 菜单标题，不超过16个字节，子菜单不超过40个字节
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 菜单KEY值，用于消息接口推送，不超过128字节
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// view类型必须,网页链接，成员点击菜单可打开链接，不超过256字节
        /// </summary>
        public string url { get; set; }
    }


}
