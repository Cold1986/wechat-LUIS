using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLogic;
using WeChatLogic;
using WebService.Contract;

namespace WebService.Facade
{
    public class CreateMenu
    {
        LogHelper _SendLog = new LogHelper("SendLog");
        private MenuBiz _materialBiz = new MenuBiz();

        public string Create(MenuCreateRequest bRequest)
        {
            //try
            //{
            var bResponse = _materialBiz.Create(bRequest);
            return "success";
            //}
            //catch (Exception ex)
            //{
            //    _SendLog.WriteLog(ex.Message);
            //    return "failed";
            //}
        }


        public void MenuCreate()
        {
            #region 微信报表菜单
            //MenuCreateRequest bRequest = new MenuCreateRequest();
            //bRequest.button = new List<button>();
            //button btn = new button();
            //btn.sub_button = new List<sub_button>();
            //btn.name = "New Report ";


            //List<sub_button> subBtn = new List<sub_button>();
            //subBtn.Add(new sub_button { type = "click", name = getMonth(1), key = getMonth(1) });
            //subBtn.Add(new sub_button { type = "click", name = getMonth(2), key = getMonth(2) });

            //btn.sub_button = subBtn;
            //bRequest.button.Add(btn);

            //this.Create(bRequest);
            #endregion

            #region AI 菜单
            MenuCreateRequest bRequest = new MenuCreateRequest();
            bRequest.button = new List<button>();

            button btn = new button();
            btn.sub_button = new List<sub_button>();
            btn.name = "海阅说";

            List<sub_button> subBtn = new List<sub_button>();
            subBtn.Add(new sub_button { type = "view", name = "写给海阅", url = "http://biaodan100.com/web/formview/582142800cf24d86c4594663?from=singlemessage&isappinstalled=0&winzoom=1" });
            subBtn.Add(new sub_button { type = "view", name = "品牌故事", url = "http://m.haibook.cn/abouthaiyue" });
            subBtn.Add(new sub_button { type = "click", name = "联系我们", key = "联系我们" });

            btn.sub_button = subBtn;
            bRequest.button.Add(btn);

            button btn2 = new button();
            btn2.name = "海阅集市";
            btn2.type = "view";
            btn2.url = "http://m.haibook.cn/";
            bRequest.button.Add(btn2);

            button btn3 = new button();
            btn3.sub_button = new List<sub_button>();
            btn3.name = "我的海阅";

            List<sub_button> subBtn3 = new List<sub_button>();
            subBtn3.Add(new sub_button { type = "view", name = "我的订单", url = "http://m.haibook.cn/usercenter?winzoom=1" });
            subBtn3.Add(new sub_button { type = "view", name = "购书须知", url = "http://m.haibook.cn/notes-on-books?winzoom=1" });
            subBtn3.Add(new sub_button { type = "view", name = "海阅官网", url = "http://www.haibook.cn/" });

            btn3.sub_button = subBtn3;
            bRequest.button.Add(btn3);

            this.Create(bRequest);
            #endregion
        }

        // 获取菜单月份
        public string getMonth(int mon)
        {
            DateTime time = Convert.ToDateTime("2016-08-01"); //DateTime.Now;
            if (mon > 0)
            {
                time = time.AddMonths(-mon);
            }
            String yearmon = time.Year.ToString();
            String month = time.Month.ToString();
            if (month.Length < 2)
            {
                month = "0" + month;
            }
            yearmon = yearmon + month;
            return yearmon;
        }
    }
}
