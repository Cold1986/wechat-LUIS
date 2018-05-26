using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebService.Contract;
using WeChatLogic;
using CommonLogic;

namespace WebService.Facade
{
    public class RemovePic
    {
        LogHelper _SendLog = new LogHelper("SendLog");
        private MaterialBiz _materialBiz = new MaterialBiz();

        public  string DelPic(BatchgetRequest bRequest)
        {
            try
            {
                var bResponse = _materialBiz.GetBatchgetResponse(bRequest);
                if (bResponse.itemlist != null)
                {
                    bResponse.itemlist.ForEach(x =>
                    {
                        if (x.filename != "nullImage.jpg" && x.filename != "abc.jpg")
                        {
                            _materialBiz.DelMateria(x.media_id);
                        }
                    });
                }
                return "success";
            }
            catch (Exception ex)
            {
                _SendLog.WriteLog(ex.Message);
                return "failed";
            }
        }
    }
}
