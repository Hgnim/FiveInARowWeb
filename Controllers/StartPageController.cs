using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static FiveInARowWeb.Models.StartPageModel;
using static FiveInARowWeb.Logger;

namespace FiveInARowWeb.Controllers {
	public class StartPageController : BaseController {
		public IActionResult Index() {
            DoClientLog( "获取StartPage/Index页面",1);
			return View();
		}

        public IActionResult StartGame([FromBody] StartGameValueModel data) {
			if (DataCore.config.Setting.PassCode == data.PassCode) {
                HttpContext.Session.SetString("PlayerTeamID",data.Team);
                return Json(new { value = 0 ,url= UrlPath.ChessPageUrl });
            }
			else {
                DoClientLog("输入密码错误: "+data.PassCode, 4);
                return Json(new { value = -1 });//密码验证不通过
			}
		}

        
    }
}
