using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebService.Contract
{
    /// <summary>
    /// 获取应用素材素材列表出参
    /// </summary>
    public class BatchgetResponse
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public string type { get; set; }
        public int total_count { get; set; }
        public int item_count { get; set; }
        public List<itemlist> itemlist { get; set; }


    }
    public class itemlist
    {
        /// <summary>
        /// 图文素材的媒体id
        /// </summary>
        public string media_id { get; set; }
        public string filename { get; set; }
        public long update_time { get; set; }
    }
}
