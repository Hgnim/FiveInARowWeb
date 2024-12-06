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
            for(int i=0;i< DataCore.config.Setting.PassCode.Length;i++) {
                if(DataCore.config.Setting.PassCode[i] == data.PassCode) {
                    PlayerRoomID = i; break;
                }
            }
			if (PlayerRoomID !=null ) {
                PlayerTeamID = data.Team;
                return Json(new { value = 0 ,url= UrlPath.ChessPageUrl });
            }
			else {
                DoClientLog("输入密码错误: "+data.PassCode, 4);
                return Json(new { value = -1 });//密码验证不通过
			}
		}

        
    }
}
