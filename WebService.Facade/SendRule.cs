using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Net;
using CommonLogic;

namespace WebService.Facade
{
    public class SendRule
    {
        LogHelper _SendLog = new LogHelper("SendLog");
        static string str_corpid = System.Configuration.ConfigurationSettings.AppSettings["CorpID"];//"wx975e51a5a2d78d74";
        static string str_corpsecret = System.Configuration.ConfigurationSettings.AppSettings["Secret"];
        static string str_AccessToken = "";

        public static void start(string monthkey, string user)
        {
            string filePath = @"\\10.176.49.189\test1\";
            string fileURL = "http://boptest.rrd.com/test1/";
            string strConn = @"Provider=Microsoft.Ace.OLEDB.12.0;Data Source=" + filePath + "Report Mapping.xlsx;Extended Properties='Excel 12.0;HDR=Yes;IMEX=1'";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            DataTable dt1 = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            string strExcel;
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            string tmp = dt1.Rows[0]["TABLE_NAME"].ToString();

            strExcel = String.Format("select * from {0}", String.Format("[{0}]", tmp));
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            myCommand.Fill(ds, "table1");
            DataTable ExcelTable = ds.Tables[0];
            conn.Close();
            AccessToken at = new AccessToken();
            str_AccessToken = at.getAccessToken(str_corpid, str_corpsecret);

            DateTime nowtime = DateTime.Now;

            string year1 = nowtime.Year.ToString();
            string year = nowtime.Year.ToString().Substring(2, 2);
            string month = nowtime.Month.ToString();
            if (month.Length < 2)
            {
                month = "0" + month;
            }
            //微信中直接传年月过来（eg：201606）
            if (monthkey.Length == 6)
            {
                year1 = monthkey.Substring(0, 4);
                year = monthkey.Substring(2, 2);
                month = monthkey.Substring(4, 2);
            }
            if (monthkey == "Currentmonthkey")
            {

            }
            if (monthkey == "Lastmonthkey")
            {
                nowtime = DateTime.Now;
                nowtime = nowtime.AddMonths(-1);
                year1 = nowtime.Year.ToString();
                year = nowtime.Year.ToString().Substring(2, 2);
                month = nowtime.Month.ToString();
                if (month.Length < 2)
                {
                    month = "0" + month;
                }
            }
            if (monthkey == "monthagokey")
            {
                nowtime = DateTime.Now;
                nowtime = nowtime.AddMonths(-2);
                year1 = nowtime.Year.ToString();
                year = nowtime.Year.ToString().Substring(2, 2);
                month = nowtime.Month.ToString();
                if (month.Length < 2)
                {
                    month = "0" + month;
                }
            }
            DataView dataView = ExcelTable.DefaultView;

            DataTable dataTableDistinct = dataView.ToTable(true, "Name");

            if (user == "")
            {
                for (int i = 0; i < dataTableDistinct.Rows.Count; i++)
                {




                    string account = Convert.ToString(dataTableDistinct.Rows[i][0]);

                    string strJson1 = "";
                    string strJson = "";
                    strJson = strJson + "{";
                    strJson = strJson + "\"touser\":\"" + account + "\",";
                    //strJson = strJson + "\"touser\":\"kevin\",";
                    strJson = strJson + "\"msgtype\":\"mpnews\",";
                    strJson = strJson + "\"agentid\":\"5\",";
                    strJson = strJson + "\"mpnews\":{";
                    strJson = strJson + "\"articles\":[";


                    DataRow[] dataRow = ExcelTable.Select("Name='" + account + "'");
                    string strJsonImg = "";
                    Boolean bl = false;
                    for (int j = 0; j < dataRow.Length; j++)
                    {

                        string name = Convert.ToString(dataRow[j][2]);
                        string title = name.Split('_')[1];
                        String fileName = year + month + "_" + name + ".jpg";
                        string ImgName = filePath + fileName;
                        string ImgNameURL = fileURL + fileName;

                        if (!System.IO.File.Exists(@"D:\WebSite\RRD_SalesForecast_Portal\RRD.BC.Portal.web\test1\" + fileName))
                        {
                            continue;
                        }
                        else
                        {

                            UpLoadInfo media_UpLoadInfo = WxUpLoad(ImgName, str_AccessToken, EnumMediaType.image);

                            string media_id = media_UpLoadInfo.media_id;

                            if (j == 0)
                            {

                                strJson1 = strJson1 + "{";
                                strJson1 = strJson1 + "\"title\":\"RRD Efficiency Metrics " + year1 + month + "\",";
                                strJson1 = strJson1 + "\"thumb_media_id\":\"" + media_id + "\",";
                                strJson1 = strJson1 + "\"author\":\"RR Donnelley\",";

                            }

                            strJsonImg = strJsonImg + "<div>" + title + "</div><div><img  src='" + ImgNameURL + "'/></div>";
                            bl = true;
                        }
                    }
                    if (!bl)
                    {
                        UpLoadInfo media_UpLoadInfo = WxUpLoad(filePath + "nullImage.jpg", str_AccessToken, EnumMediaType.image);
                        string media_id = media_UpLoadInfo.media_id;
                        strJson1 = strJson1 + "{";
                        strJson1 = strJson1 + "\"title\":\"RRD Efficiency Metrics使用指南\",";
                        strJson1 = strJson1 + "\"thumb_media_id\":\"" + media_id + "\",";
                        strJson1 = strJson1 + "\"author\":\"RR Donnelley\",";
                        strJsonImg = "<div>" + monthkey + "</div><div><img  src='" + fileURL + "nullImage.jpg" + "'/></div>";
                    }
                    strJson1 = strJson1 + "\"content\":\"" + strJsonImg + "\",";


                    if (bl)
                    {
                        strJson1 = strJson1 + "\"digest\":\"RRD Efficiency Metrics " + year1 + month + "\",";
                    }
                    else
                    {
                        strJson1 = strJson1 + "\"digest\":\"RRD Efficiency Metrics使用指南\",";
                    }
                    strJson1 = strJson1 + "\"show_cover_pic\":\"0\"";
                    strJson1 = strJson1 + "}";
                    strJson = strJson + strJson1;
                    strJson = strJson + "]";
                    strJson = strJson + "},";
                    strJson = strJson + "\"safe\":\"1\"";
                    strJson = strJson + "}";
                    at.GetPage("https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + str_AccessToken, strJson);

                }

            }
            else
            {
                string account = user;

                string strJson1 = "";
                string strJson = "";
                strJson = strJson + "{";
                strJson = strJson + "\"touser\":\"" + account + "\",";
                //strJson = strJson + "\"touser\":\"kevin\",";
                strJson = strJson + "\"msgtype\":\"mpnews\",";
                strJson = strJson + "\"agentid\":\"5\",";
                strJson = strJson + "\"mpnews\":{";
                strJson = strJson + "\"articles\":[";


                DataRow[] dataRow = ExcelTable.Select("Name='" + account + "'");
                string strJsonImg = "";
                Boolean bl = false;

                for (int j = 0; j < dataRow.Length; j++)
                {

                    string name = Convert.ToString(dataRow[j][2]);
                    string title = name.Split('_')[1];
                    String fileName = year + month + "_" + name + ".jpg";
                    string ImgName = filePath + fileName;
                    string ImgNameURL = fileURL + fileName;

                    if (!System.IO.File.Exists(@"D:\WebSite\RRD_SalesForecast_Portal\RRD.BC.Portal.web\test1\" + fileName))
                    {
                        continue;
                    }
                    else
                    {
                        UpLoadInfo media_UpLoadInfo = WxUpLoad(ImgName, str_AccessToken, EnumMediaType.image);
                        string media_id = media_UpLoadInfo.media_id;
                        if (j == 0)
                        {
                            strJson1 = strJson1 + "{";
                            strJson1 = strJson1 + "\"title\":\"RRD Efficiency Metrics " + year1 + month + "\",";
                            strJson1 = strJson1 + "\"thumb_media_id\":\"" + media_id + "\",";
                            strJson1 = strJson1 + "\"author\":\"RR Donnelley\",";

                        }
                        strJsonImg = strJsonImg + "<div>" + title + "</div><div><img  src='" + ImgNameURL + "'/></div>";
                        bl = true;
                    }
                }
                if (!bl)
                {
                    UpLoadInfo media_UpLoadInfo = WxUpLoad(filePath + "nullImage.jpg", str_AccessToken, EnumMediaType.image);
                    string media_id = media_UpLoadInfo.media_id;
                    strJson1 = strJson1 + "{";
                    strJson1 = strJson1 + "\"title\":\"RRD Efficiency Metrics使用指南\",";
                    strJson1 = strJson1 + "\"thumb_media_id\":\"" + media_id + "\",";
                    strJson1 = strJson1 + "\"author\":\"RR Donnelley\",";
                    strJsonImg = "<div>" + monthkey + "</div><div><img  src='" + fileURL + "nullImage.jpg" + "'/></div>";
                }

                strJson1 = strJson1 + "\"content\":\"" + strJsonImg + "\",";

                if (bl)
                {
                    strJson1 = strJson1 + "\"digest\":\"RRD Efficiency Metrics " + year1 + month + "\",";
                }
                else
                {
                    strJson1 = strJson1 + "\"digest\":\"RRD Efficiency Metrics使用指南\",";
                }
                strJson1 = strJson1 + "\"show_cover_pic\":\"0\"";
                strJson1 = strJson1 + "}";
                strJson = strJson + strJson1;
                strJson = strJson + "]";
                strJson = strJson + "},";
                strJson = strJson + "\"safe\":\"1\"";
                strJson = strJson + "}";
                at.GetPage("https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + str_AccessToken, strJson);
            }
        }

        private static UpLoadInfo WxUpLoad(string filepath, string token, EnumMediaType mt)
        {
            using (WebClient client = new WebClient())
            {
                //https://qyapi.weixin.qq.com/cgi-bin/material/add_material?agentid=AGENTID&type=TYPE&access_token=ACCESS_TOKEN
                byte[] b = client.UploadFile(string.Format("https://qyapi.weixin.qq.com/cgi-bin/material/add_material?agentid=5&access_token={0}&type={1}", token, mt.ToString()), filepath);//调用接口上传文件
                //byte[] b = client.UploadFile(string.Format("https://qyapi.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={1}", token, mt.ToString()), filepath);//调用接口上传文件
                string retdata = Encoding.Default.GetString(b);//获取返回值

                if (retdata.Contains("media_id"))//判断返回值是否包含media_id，包含则说明上传成功，然后将返回的json字符串转换成json
                {
                    return JsonConvert.DeserializeObject<UpLoadInfo>(retdata);
                }
                else
                {//否则，写错误日志
                    //WriteBug(retdata);//写错误日志
                    return null;
                }
            }
        }
    }
}
