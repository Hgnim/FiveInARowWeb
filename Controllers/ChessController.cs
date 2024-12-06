using Microsoft.AspNetCore.Mvc;
using static FiveInARowWeb.Models.ChessModel;
using static FiveInARowWeb.Logger;

namespace FiveInARowWeb.Controllers {
	public class ChessController : BaseController {
		public IActionResult ChessPage() {
			if (HttpContext.Session.GetString("PlayerTeamID") != null) {
                DoClientLog("获取Chess/ChessPage页面", 1);
				return View();
			}
			else {
                DoClientLog("获取Chess/ChessPage页面被拒绝，重定向至根页面", 3);
                return Redirect(UrlPath.RootUrl);
			}
		}


		[HttpPost]
		public ActionResult SendDoChess([FromBody] SendDoChessValueModel data) {
			ChessGame.ChessPos cp;
			{
				string[] sp = data.V.Split('-');
				cp = new() {
					x = byte.Parse(sp[0]),
					y = byte.Parse(sp[1])
				};
			}
			return Json(new { b = 
				DataCore.chessGame.DoChess(cp,
				(ChessGame.Team)Enum.Parse(typeof(ChessGame.Team), HttpContext.Session.GetString("PlayerTeamID")!))
			});
		}
		[HttpGet]
		public IActionResult GetLocalTeam() {
            string teamID = HttpContext.Session.GetString("PlayerTeamID")!;
            DoClientLog("获取当前队伍: " +teamID, 4);
            string teamIcon="";
			switch (teamID) {
				case "white":
					teamIcon = UrlPath.Img.CP.Cw;break;
				case "black":
					teamIcon = UrlPath.Img.CP.Cb; break;
			}
			return Json(new {id=teamID, icon = teamIcon });
		}
		[HttpPost]
		public async Task<IActionResult> GetChessUpdate([FromBody] ChessUpdatePostValueModel data) {
            DoClientLog("获取棋局数据更新", 4);
            ChessUpdateValueModel sendValue = null!;
			await Task.Run(() => {
				AutoResetEvent are = new(false);
				void DoEvent() {
					sendValue = new() {
						ChessImgUrl = [UrlPath.Img.CP.N, UrlPath.Img.CP.Cw, UrlPath.Img.CP.Cb],
						ChessData = new ChessGame.ChessType[15][],						
						WhoDoChess = DataCore.chessGame.WhoDoChess,
					};
					if (DataCore.chessGame.LastDo != null) {
						sendValue.LastDo = [DataCore.chessGame.LastDo.x, DataCore.chessGame.LastDo.y];
					}
					for(int i = 0; i < 15; i++) {
						sendValue.ChessData[i] = new ChessGame.ChessType[15];
						for (int j = 0; j < 15; j++) {
							sendValue.ChessData[i][j] =
							DataCore.chessGame.ChessData[i,j];
						} }
					if (DataCore.chessGame.winData != null) {
						sendValue.WinData_IsTie = DataCore.chessGame.winData.isTie;
						sendValue.WinData_WinTeam=DataCore.chessGame.winData.winTeam;
						sendValue.WinData_WinChessPos = new int[5][];
						if (DataCore.chessGame.winData.winChessPos != null) {
							for (int i = 0; i < 5; i++) {
								sendValue.WinData_WinChessPos[i] = [DataCore.chessGame.winData.winChessPos[i].x, DataCore.chessGame.winData.winChessPos[i].y];
							}
						}
					}
					/*if (sendValue.WinData != null) sendValue.HaveWinner = true;
					else sendValue.HaveWinner = false;*/
					if ((ChessGame.Team)Enum.Parse(typeof(ChessGame.Team), HttpContext.Session.GetString("PlayerTeamID")!) == sendValue.WhoDoChess)
						sendValue.IsYouDoChess = true;
					else
						sendValue.IsYouDoChess = false;
					are.Set();
				}
				if ( data.IsForce) {
					DoEvent();
				}
				else {
					DataCore.chessGame.SomeOneDoChess += DoEvent;
					are.WaitOne();
					DataCore.chessGame.SomeOneDoChess -= DoEvent;
				}
			});

			return Json(sendValue);
		}
		[HttpGet]
		public async Task<IActionResult> RestartGame() {
            DoClientLog("点击重启游戏按钮", 3);
            JsonResult jr = Json(new { b = false});
			await Task.Run(() => {
				AutoResetEvent are = new(false);
				void DoEvent() {
					switch (HttpContext.Session.GetString("PlayerTeamID")!) {
						case "white":
							HttpContext.Session.SetString("PlayerTeamID", "black");
							break;
						case "black":
							HttpContext.Session.SetString("PlayerTeamID", "white");
							break;
					}

					jr = Json(new { b = true ,url= UrlPath.ChessPageUrl });
					are.Set();
				}
					DataCore.chessGame.DoRestartGame += DoEvent;
				DataCore.chessGame.SetRestartGameNum((ChessGame.Team)Enum.Parse(typeof(ChessGame.Team), HttpContext.Session.GetString("PlayerTeamID")!));
				are.WaitOne();
					DataCore.chessGame.DoRestartGame -= DoEvent;				
			});
			return jr;
		}
	}
}
