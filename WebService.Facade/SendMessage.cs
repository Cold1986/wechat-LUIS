using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using CommonLogic;
using WebService.Contract;
using WeChatLogic;
using WebService.Enum;

namespace WebService.Facade
{
    public class SendMessage
    {
        private static LogHelper _SendLog = new LogHelper("SendLog");
        private static string filePath = @"\\10.176.49.189\test1\";
        private static string fileURL = "http://boptest.rrd.com/test1/";
        private static string str_corpid = System.Configuration.ConfigurationSettings.AppSettings["CorpID"];//"wx975e51a5a2d78d74";
        private static string str_corpsecret = System.Configuration.ConfigurationSettings.AppSettings["Secret"];
        private static string str_AccessToken = TokenBiz.GetAccessToken(str_corpid, str_corpsecret);

        public static void Start(string monthkey, string user)
        {
            MessageBiz messageBiz = new MessageBiz();
            SendMPNewsRequest sendNewsRequest = new SendMPNewsRequest();
            string strConn = @"Provider=Microsoft.Ace.OLEDB.12.0;Data Source=" + filePath + "Report Mapping.xlsx;Extended Properties='Excel 12.0;HDR=Yes;IMEX=1'";
            DataTable ExcelTable = GetDTFromExcel(strConn);
            if (!string.IsNullOrEmpty(user))
            {
                sendNewsRequest = GetSendMPNewsRequest(monthkey, user, ExcelTable);
                messageBiz.Send<SendMPNewsRequest>(sendNewsRequest);

            }
            else
            {
                DataView dataView = ExcelTable.DefaultView;
                DataTable dataTableDistinct = dataView.ToTable(true, "Name");
                for (int i = 0; i < dataTableDistinct.Rows.Count; i++)
                {
                    string account = Convert.ToString(dataTableDistinct.Rows[i][0]);
                    sendNewsRequest = GetSendMPNewsRequest(monthkey, account, ExcelTable);
                    messageBiz.Send<SendMPNewsRequest>(sendNewsRequest);
                }
            }

        }

        /// <summary>
        /// 从Excel获取数据
        /// </summary>
        /// <param name="conn">文件地址</param>
        /// <returns></returns>
        private static DataTable GetDTFromExcel(string strConn)
        {
            DataTable ExcelTable = new DataTable();
            var cacheValue = CacheHelper.GetCache("excel" + strConn);
            if (cacheValue != null)
            {
                ExcelTable = (DataTable)CacheHelper.GetCache("excel" + strConn);
            }
            else
            {
                OleDbConnection conn = new OleDbConnection(strConn);
                conn.Open();
                DataTable dt1 = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                OleDbDataAdapter myCommand = null;
                DataSet ds = null;
                string tmp = dt1.Rows[0]["TABLE_NAME"].ToString();

                string strExcel = String.Format("select * from {0}", String.Format("[{0}]", tmp));
                myCommand = new OleDbDataAdapter(strExcel, strConn);
                ds = new DataSet();
                myCommand.Fill(ds, "table1");
                ExcelTable = ds.Tables[0];
                if (ExcelTable.Rows.Count > 0)
                {
                    CacheHelper.SetCache("excel" + strConn, ExcelTable, new TimeSpan(24, 0, 0));
                }
                conn.Close();
            }
            return ExcelTable;
        }

        private static SendMPNewsRequest GetSendMPNewsRequest(string monthkey, string user, DataTable ExcelTable)
        {
            DateTime nowtime = DateTime.Now;
            string year1 = nowtime.Year.ToString();
            string year = nowtime.Year.ToString().Substring(2, 2);
            string month = nowtime.Month.ToString().PadLeft(2, '0');

            //微信中直接传年月过来（eg：201606）
            if (monthkey.Length == 6)
            {
                year1 = monthkey.Substring(0, 4);
                year = monthkey.Substring(2, 2);
                month = monthkey.Substring(4, 2);
            }

            SendMPNewsRequest _SendMPNewsRequest = new SendMPNewsRequest();
            mpnews _Mpnews = new mpnews();
            _Mpnews.articles = new List<articles>();
            string content = "";

            _SendMPNewsRequest.touser = user;
            _SendMPNewsRequest.msgtype = "mpnews";
            _SendMPNewsRequest.agentid = System.Configuration.ConfigurationSettings.AppSettings["Agentid"];
            _SendMPNewsRequest.safe = "1";

            DataRow[] dataRow = ExcelTable.Select("Name='" + user + "'");
            for (int j = 0; j < dataRow.Length; j++)
            {
                articles article = new articles();
                string name = Convert.ToString(dataRow[j][2]);

                byte[] space = new byte[] { 0xc2, 0xa0 };
                string UTFSpace = Encoding.GetEncoding("UTF-8").GetString(space);
                name = name.Replace(UTFSpace, " ");

                string title = name.Split('_')[1];
                String fileName = year + month + "_" + name + ".jpg";

                string ImgName = filePath + fileName;
                string ImgNameURL = fileURL + fileName;

                if (!System.IO.File.Exists(@"D:\WebSite\RRD_SalesForecast_Portal\RRD.BC.Portal.web\test1\" + fileName))
                {
                    _SendLog.WriteLog("图片缺失： " + fileName);
                    continue;
                }
                else
                {
                    content += "<div>" + title + "</div><div><img  src='" + ImgNameURL + "'/></div>";
                    if (!_Mpnews.articles.Any())//只要一条就可以了
                    {
                        UploadResponse media_UpLoadInfo = MediaBiz.CreateInstance().Upload(ImgName, str_AccessToken, EnumMediaType.image);
                        if (media_UpLoadInfo != null)
                        {
                            article.title = "RRD Efficiency Metrics " + year1 + month;
                            article.thumb_media_id = media_UpLoadInfo.media_id;
                            article.author = "RR Donnelley";
                            article.show_cover_pic = "0";
                            article.digest = "RRD Efficiency Metrics " + year1 + month;
                            _Mpnews.articles.Add(article);
                        }
                    }
                }
            }

            if (!_Mpnews.articles.Any())
            {
                content = "<div>" + monthkey + "</div><div><img  src='" + fileURL + "nullImage.jpg" + "'/></div>";
                articles article = new articles();
                UploadResponse media_UpLoadInfo = MediaBiz.CreateInstance().Upload(filePath + "nullImage.jpg", str_AccessToken, EnumMediaType.image);
                if (media_UpLoadInfo != null)
                {
                    article.title = "RRD Efficiency Metrics使用指南";
                    article.thumb_media_id = media_UpLoadInfo.media_id;
                    article.author = "RR Donnelley";
                    article.show_cover_pic = "0";
                    article.digest = "RRD Efficiency Metrics使用指南";
                    _Mpnews.articles.Add(article);
                }
            }

            _Mpnews.articles.ForEach(x => x.content = content);

            _SendMPNewsRequest.mpnews = _Mpnews;
            return _SendMPNewsRequest;
        }
    }
}
