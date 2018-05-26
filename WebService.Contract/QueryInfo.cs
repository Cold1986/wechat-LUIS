using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebService.Contract
{
    public class QueryInfo
    {
        public string QueryMessage { get; set; }

        public string QueryChannel { get; set; }

        /// <summary>
        /// To continue the dialog, append the answer to the question along with the contextID as request parameter
        /// </summary>
        public string ContextId { get; set; }
    }

    public class ResponseInfo<T>
    {
        public QueryInfo queryInfo { get; set; }

        public T responseInfo { get; set; }
    }

    public class WeChatAnswerList
    {
        public WeChatAnswerList()
        { }
        #region Model
        public int? _id { get; set; }
        public int? _conversationid { get; set; }
        public int? _actionsid { get; set; }
        public string _msgtype { get; set; }
        public string _content { get; set; }
        public string _banner { get; set; }
        public string _title { get; set; }
        public string _digest { get; set; }
        public string _imagepath { get; set; }
        public string _description { get; set; }
        public string _url { get; set; }
        public string _picurl { get; set; }

        #endregion Model
    }

    public class Books
    {
        public Books()
        { }
        #region Model
        public int _id { get; set; }
        public string _booktype { get; set; }
        public string _booklanguage { get; set; }
        public string _name { get; set; }
        public string _description { get; set; }
        public string _url { get; set; }
        public string _picurl { get; set; }
        #endregion Model
    }
}
