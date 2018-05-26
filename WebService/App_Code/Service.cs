using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Diagnostics;
using System.Data.OleDb;
using System.Data;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using WebService.Enum;
using WebService.Contract;
using CommonLogic;
using WeChatLogic;
using WebService.Facade;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。 
// [System.Web.Script.Services.ScriptService]

public class Service : System.Web.Services.WebService
{
    LogHelper _SendLog = new LogHelper("SendLog");

    static string str_corpid = System.Configuration.ConfigurationSettings.AppSettings["CorpID"];//"wx975e51a5a2d78d74";
    static string str_corpsecret = System.Configuration.ConfigurationSettings.AppSettings["Secret"];
    string str_AccessToken = TokenBiz.GetAccessToken(str_corpid, str_corpsecret);

    public Service()
    {

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string RemovePic(int offset)
    {
        RemovePic rPic = new RemovePic();
        BatchgetRequest bRequest = new BatchgetRequest();
        bRequest.agentid = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["Agentid"]);
        bRequest.count = 50;
        bRequest.type = EnumMediaType.image.ToString();
        bRequest.offset = offset;

        var res = rPic.DelPic(bRequest);

        return res;

    }

    [WebMethod]
    public string Send(string key, string user)
    {
        _SendLog.WriteLog("key: " + key + " |user: " + user);
        SendMessage.Start(key, user);
        //start(key, user);
        return Server.MapPath("") + "\\bop.exe ";
    }

    #region 旧方法
    //void start(string monthkey, string user)
    //{
    //    string filePath = @"\\10.176.49.189\test1\";
    //    string fileURL = "http://boptest.rrd.com/test1/";
    //    string strConn = @"Provider=Microsoft.Ace.OLEDB.12.0;Data Source=" + filePath + "Report Mapping.xlsx;Extended Properties='Excel 12.0;HDR=Yes;IMEX=1'";
    //    DataTable ExcelTable = SendMessage.GetDTFromExcel(strConn);

    //    DateTime nowtime = DateTime.Now;

    //    string year1 = nowtime.Year.ToString();
    //    string year = nowtime.Year.ToString().Substring(2, 2);
    //    string month = nowtime.Month.ToString().PadLeft(2, '0');

    //    //微信中直接传年月过来（eg：201606）
    //    if (monthkey.Length == 6)
    //    {
    //        year1 = monthkey.Substring(0, 4);
    //        year = monthkey.Substring(2, 2);
    //        month = monthkey.Substring(4, 2);
    //    }

    //    DataView dataView = ExcelTable.DefaultView;
    //    DataTable dataTableDistinct = dataView.ToTable(true, "Name");

    //    if (user == "")
    //    {
    //        for (int i = 0; i < dataTableDistinct.Rows.Count; i++)
    //        {
    //            string account = Convert.ToString(dataTableDistinct.Rows[i][0]);

    //            string strJson1 = "";
    //            string strJson = "";
    //            strJson = strJson + "{";
    //            strJson = strJson + "\"touser\":\"" + account + "\",";
    //            //strJson = strJson + "\"touser\":\"kevin\",";
    //            strJson = strJson + "\"msgtype\":\"mpnews\",";
    //            strJson = strJson + "\"agentid\":\"5\",";
    //            strJson = strJson + "\"mpnews\":{";
    //            strJson = strJson + "\"articles\":[";


    //            DataRow[] dataRow = ExcelTable.Select("Name='" + account + "'");
    //            string strJsonImg = "";
    //            Boolean bl = false;
    //            for (int j = 0; j < dataRow.Length; j++)
    //            {

    //                string name = Convert.ToString(dataRow[j][2]);
    //                string title = name.Split('_')[1];
    //                String fileName = year + month + "_" + name + ".jpg";
    //                string ImgName = filePath + fileName;
    //                string ImgNameURL = fileURL + fileName;

    //                if (!System.IO.File.Exists(@"D:\WebSite\RRD_SalesForecast_Portal\RRD.BC.Portal.web\test1\" + fileName))
    //                {
    //                    _SendLog.WriteLog("图片缺失： " + fileName);
    //                    continue;
    //                }
    //                else
    //                {

    //                    UploadResponse media_UpLoadInfo = MediaBiz.CreateInstance().Upload(ImgName, str_AccessToken, EnumMediaType.image);

    //                    string media_id = media_UpLoadInfo.media_id;

    //                    if (j == 0)
    //                    {

    //                        strJson1 = strJson1 + "{";
    //                        strJson1 = strJson1 + "\"title\":\"RRD Efficiency Metrics " + year1 + month + "\",";
    //                        strJson1 = strJson1 + "\"thumb_media_id\":\"" + media_id + "\",";
    //                        strJson1 = strJson1 + "\"author\":\"RR Donnelley\",";

    //                    }

    //                    strJsonImg = strJsonImg + "<div>" + title + "</div><div><img  src='" + ImgNameURL + "'/></div>";
    //                    bl = true;
    //                }
    //            }
    //            if (!bl)
    //            {
    //                UploadResponse media_UpLoadInfo = MediaBiz.CreateInstance().Upload(filePath + "nullImage.jpg", str_AccessToken, EnumMediaType.image);
    //                string media_id = media_UpLoadInfo.media_id;
    //                strJson1 = strJson1 + "{";
    //                strJson1 = strJson1 + "\"title\":\"RRD Efficiency Metrics使用指南\",";
    //                strJson1 = strJson1 + "\"thumb_media_id\":\"" + media_id + "\",";
    //                strJson1 = strJson1 + "\"author\":\"RR Donnelley\",";
    //                strJsonImg = "<div>" + monthkey + "</div><div><img  src='" + fileURL + "nullImage.jpg" + "'/></div>";
    //            }
    //            strJson1 = strJson1 + "\"content\":\"" + strJsonImg + "\",";


    //            if (bl)
    //            {
    //                strJson1 = strJson1 + "\"digest\":\"RRD Efficiency Metrics " + year1 + month + "\",";
    //            }
    //            else
    //            {
    //                strJson1 = strJson1 + "\"digest\":\"RRD Efficiency Metrics使用指南\",";
    //            }
    //            strJson1 = strJson1 + "\"show_cover_pic\":\"0\"";
    //            strJson1 = strJson1 + "}";
    //            strJson = strJson + strJson1;
    //            strJson = strJson + "]";
    //            strJson = strJson + "},";
    //            strJson = strJson + "\"safe\":\"1\"";
    //            strJson = strJson + "}";
    //            RequestHelper.SendRequest("https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + str_AccessToken, strJson, "POST");

    //        }

    //    }
    //    else
    //    {
    //        string account = user;

    //        string strJson1 = "";
    //        string strJson = "";
    //        strJson = strJson + "{";
    //        strJson = strJson + "\"touser\":\"" + account + "\",";
    //        //strJson = strJson + "\"touser\":\"kevin\",";
    //        strJson = strJson + "\"msgtype\":\"mpnews\",";
    //        strJson = strJson + "\"agentid\":\"5\",";
    //        strJson = strJson + "\"mpnews\":{";
    //        strJson = strJson + "\"articles\":[";


    //        DataRow[] dataRow = ExcelTable.Select("Name='" + account + "'");
    //        string strJsonImg = "";
    //        Boolean bl = false;

    //        for (int j = 0; j < dataRow.Length; j++)
    //        {

    //            string name = Convert.ToString(dataRow[j][2]);
    //            string title = name.Split('_')[1];
    //            String fileName = year + month + "_" + name + ".jpg";
    //            string ImgName = filePath + fileName;
    //            string ImgNameURL = fileURL + fileName;

    //            if (!System.IO.File.Exists(@"D:\WebSite\RRD_SalesForecast_Portal\RRD.BC.Portal.web\test1\" + fileName))
    //            {
    //                _SendLog.WriteLog("图片缺失： " + fileName);
    //                continue;
    //            }
    //            else
    //            {
    //                UploadResponse media_UpLoadInfo = MediaBiz.CreateInstance().Upload(ImgName, str_AccessToken, EnumMediaType.image);
    //                string media_id = media_UpLoadInfo.media_id;
    //                if (j == 0)
    //                {
    //                    strJson1 = strJson1 + "{";
    //                    strJson1 = strJson1 + "\"title\":\"RRD Efficiency Metrics " + year1 + month + "\",";
    //                    strJson1 = strJson1 + "\"thumb_media_id\":\"" + media_id + "\",";
    //                    strJson1 = strJson1 + "\"author\":\"RR Donnelley\",";
    //                }
    //                strJsonImg = strJsonImg + "<div>" + title + "</div><div><img  src='" + ImgNameURL + "'/></div>";
    //                bl = true;
    //            }
    //        }
    //        if (!bl)
    //        {
    //            UploadResponse media_UpLoadInfo = MediaBiz.CreateInstance().Upload(filePath + "nullImage.jpg", str_AccessToken, EnumMediaType.image);
    //            string media_id = media_UpLoadInfo.media_id;
    //            strJson1 = strJson1 + "{";
    //            strJson1 = strJson1 + "\"title\":\"RRD Efficiency Metrics使用指南\",";
    //            strJson1 = strJson1 + "\"thumb_media_id\":\"" + media_id + "\",";
    //            strJson1 = strJson1 + "\"author\":\"RR Donnelley\",";
    //            strJsonImg = "<div>" + monthkey + "</div><div><img  src='" + fileURL + "nullImage.jpg" + "'/></div>";
    //        }

    //        strJson1 = strJson1 + "\"content\":\"" + strJsonImg + "\",";

    //        if (bl)
    //        {
    //            strJson1 = strJson1 + "\"digest\":\"RRD Efficiency Metrics " + year1 + month + "\",";
    //        }
    //        else
    //        {
    //            strJson1 = strJson1 + "\"digest\":\"RRD Efficiency Metrics使用指南\",";
    //        }
    //        strJson1 = strJson1 + "\"show_cover_pic\":\"0\"";
    //        strJson1 = strJson1 + "}";
    //        strJson = strJson + strJson1;
    //        strJson = strJson + "]";
    //        strJson = strJson + "},";
    //        strJson = strJson + "\"safe\":\"1\"";
    //        strJson = strJson + "}";
    //        _SendLog.WriteLog("old: " + strJson);
    //        RequestHelper.SendRequest("https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + str_AccessToken, strJson, "POST");

    //    }







    //}
    #endregion
}