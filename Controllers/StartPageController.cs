using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static FiveInARowWeb.Models.StartPageModel;

namespace FiveInARowWeb.Controllers {
	public class StartPageController : Controller {
		public IActionResult Index() {
			return View();
		}

        public IActionResult StartGame([FromBody] StartGameValueModel data) {
			if (DataCore.config.Setting.PassCode == data.PassCode) {
                HttpContext.Session.SetString("PlayerTeamID",data.Team);
                return Json(new { value = 0 ,url= UrlPath.ChessPageUrl });
            }
			else {
				return Json(new { value = -1 });//密码验证不通过
			}
		}

    }
}
