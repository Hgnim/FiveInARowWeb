using Microsoft.AspNetCore.Mvc;
using static FiveInARowWeb.Models.ChessModel;
using static FiveInARowWeb.DataCore;
using static FiveInARowWeb.Logger;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace FiveInARowWeb.Controllers {
	public class ChessController : BaseController {
		public IActionResult ChessPage() {
			if (PlayerTeamID != null) {
                DoClientLog("获取Chess/ChessPage页面", 1);
				return View();
			}
			else {
                DoClientLog("获取Chess/ChessPage页面被拒绝，重定向至根页面", 3);
                return Redirect(UrlPath.RootUrl);
			}
		}


		[HttpPost]
		public IActionResult SendDoChess([FromBody] SendDoChessValueModel data) {
			{
				const string endtxt= "而导致执行棋子被拒绝，重定向至根页面";
				IActionResult r = Redirect(UrlPath.RootUrl);
				if (PlayerTeamID == null || PlayerRoomID == null) {
					DoClientLog($"因数据丢失{endtxt}", 5);
					return r;
				}
				else if (!IsPlayerDataTrue()) {
					DoClientLog($"用户名验证错误，将无法执行棋子", 5);
					return Json(new {b=false,canDoChess=false});//用户名验证错误，为旁观模式
				}
				else if (!IsPlayerLink()) {
					DoClientLog($"因玩家连接超时{endtxt}", 5);
					return r;
				}
			}
            ChessGame.ChessPos cp;
			{
				string[] sp = data.V.Split('-');
				cp = new() {
					x = byte.Parse(sp[0]),
					y = byte.Parse(sp[1])
				};
			}
			return Json(new { b = 
				DataCore.chessGame[(int)PlayerRoomID].DoChess(cp,
				(ChessGame.Team)Enum.Parse(typeof(ChessGame.Team), PlayerTeamID), (int)PlayerRoomID)
			});
		}
		[HttpGet]
		public IActionResult GetLocalTeam() {
			{
				const string endtxt = "而导致获取当前队伍被拒绝，重定向至根页面";
				IActionResult r = Redirect(UrlPath.RootUrl);
				if (PlayerTeamID == null || PlayerRoomID == null) {//避免信息丢失后请求而导致错误
					DoClientLog($"因数据丢失{endtxt}", 5);
					return r;
				}
				else if (!IsPlayerDataTrue()) {
					DoClientLog($"用户名验证错误，进入旁观模式", 5);
					return Json(new { canDoChess = false, icon = UrlPath.Img.CP.Cn });//用户名验证错误，为旁观模式
				}
				else if (!IsPlayerLink()) {
					DoClientLog($"因玩家连接超时{endtxt}", 5);
					return r;
				}					
			}
			string teamID = PlayerTeamID;
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
			{
				const string endtxt = "而导致获取棋局数据更新被拒绝，重定向至根页面";
				IActionResult r = Redirect(UrlPath.RootUrl);
				if (PlayerTeamID == null || PlayerRoomID == null) {
					DoClientLog($"因数据丢失{endtxt}", 5);
					return r;
				}
				else if (!IsPlayerDataTrue()) {
					DoClientLog($"以旁观者模式请求↴", 5);
				}
				else if (!IsPlayerLink()) {
					DoClientLog($"因玩家连接超时{endtxt}", 5);
					return r;
				}
			}
			DoClientLog("获取棋局数据更新", 4);
            ChessUpdateValueModel sendValue = null!;
			await Task.Run(() => {
				AutoResetEvent are = new(false);
				void DoEvent() {
					sendValue = new() {
						ChessImgUrl = [UrlPath.Img.CP.N, UrlPath.Img.CP.Cw, UrlPath.Img.CP.Cb],
						ChessData = new ChessGame.ChessType[15][],						
						WhoDoChess = DataCore.chessGame[(int)PlayerRoomID].WhoDoChess,
						WhoClickRestartButton = chessGame[(int)PlayerRoomID].RestartGame,
					};
					if (DataCore.chessGame[(int)PlayerRoomID].LastDo != null) {
#pragma warning disable CS8602
						sendValue.LastDo = [DataCore.chessGame[(int)PlayerRoomID].LastDo.x, DataCore.chessGame[(int)PlayerRoomID].LastDo.y];
#pragma warning restore CS8602
					}
					for(int i = 0; i < 15; i++) {
						sendValue.ChessData[i] = new ChessGame.ChessType[15];
						for (int j = 0; j < 15; j++) {
							sendValue.ChessData[i][j] =
							DataCore.chessGame[(int)PlayerRoomID].ChessData[i,j];
						} }
					if (DataCore.chessGame[(int)PlayerRoomID].winData != null) {
#pragma warning disable CS8602
						sendValue.WinData_IsTie = DataCore.chessGame[(int)PlayerRoomID].winData.isTie;
						sendValue.WinData_WinTeam=DataCore.chessGame[(int)PlayerRoomID].winData.winTeam;
						sendValue.WinData_WinChessPos = new int[5][];
						if (DataCore.chessGame[(int)PlayerRoomID].winData.winChessPos != null) {
							for (int i = 0; i < 5; i++) {
								sendValue.WinData_WinChessPos[i] = [DataCore.chessGame[(int)PlayerRoomID].winData.winChessPos[i].x, DataCore.chessGame[(int)PlayerRoomID].winData.winChessPos[i].y];
							}
						}
#pragma warning restore CS8602
					}
					{
						sendValue.PlayerData = new ChessUpdateValueModel.PlayerD[2];
						for (int i = 0; i < 2; i++) {
							if (chessGame[(int)PlayerRoomID].PlayerData[i] != null) {
								sendValue.PlayerData[i] = new() {
									UserName = chessGame[(int)PlayerRoomID].PlayerData[i].userName,
									Team = chessGame[(int)PlayerRoomID].PlayerData[i].team
								};
							}
						}
					}
					/*if (sendValue.WinData != null) sendValue.HaveWinner = true;
					else sendValue.HaveWinner = false;*/
					if ((ChessGame.Team)Enum.Parse(typeof(ChessGame.Team), PlayerTeamID) == sendValue.WhoDoChess)
						sendValue.IsYouDoChess = true;
					else
						sendValue.IsYouDoChess = false;
					are.Set();
				}
				if ( data.IsForce) {
					DoEvent();
				}
				else {
					DataCore.chessGame[(int)PlayerRoomID].SomethingUpdate += DoEvent;
					are.WaitOne();
					DataCore.chessGame[(int)PlayerRoomID].SomethingUpdate -= DoEvent;
				}
			});

			return Json(sendValue);
		}
		[HttpGet]
		public async Task<IActionResult> RestartGame() {
			{
				const string endtxt = "而导致点击重启游戏按钮被拒绝，重定向至根页面";
				IActionResult r = Redirect(UrlPath.RootUrl);
				if (PlayerTeamID == null || PlayerRoomID == null) {
					DoClientLog($"因数据丢失{endtxt}", 5);
					return r;
				}
				else if (!IsPlayerDataTrue()) {
					DoClientLog($"旁观者模式请求点击重启游戏按钮被拒绝", 5);
					return Json(new { b = false, canDoChess = false });
				}
				else if (!IsPlayerLink()) {
					DoClientLog($"因玩家连接超时{endtxt}", 5);
					return r;
				}		
			}
			DoClientLog("点击重启游戏按钮", 3);
            JsonResult jr = Json(new { b = false});
			await Task.Run(() => {
				AutoResetEvent are = new(false);
				void DoEvent() {
					switch (PlayerTeamID) {
						case "white":
                        PlayerTeamID= "black";
							break;
						case "black":
                        PlayerTeamID="white";
							break;
					}

					jr = Json(new { b = true ,url= UrlPath.ChessPageUrl });
					are.Set();
				}
				DataCore.chessGame[(int)PlayerRoomID].DoRestartGame += DoEvent;
				DataCore.chessGame[(int)PlayerRoomID].SetRestartGameNum((ChessGame.Team)Enum.Parse(typeof(ChessGame.Team), PlayerTeamID), (int)PlayerRoomID);
				are.WaitOne();
					DataCore.chessGame[(int)PlayerRoomID].DoRestartGame -= DoEvent;
			});
			return jr;
		}
		[HttpGet]
		public IActionResult HeartBeat() {
			{
				const string endtxt = "而导致无法处理保持连接请求，重定向至根页面";
				IActionResult r = Redirect(UrlPath.RootUrl);
				if (PlayerTeamID == null || PlayerRoomID == null) {
					DoClientLog($"因数据丢失{endtxt}", 5);
					return r;
				}
				else if (!IsPlayerDataTrue()) {
					DoClientLog($"旁观者模式请求保持连接请求被拒绝", 5);
					return Json(new {b=false, canDoChess = false });
				}
				else if (!IsPlayerLink()) {
					DoClientLog($"因玩家连接超时{endtxt}", 5);
					return r;
				}					
			}
			chessGame[(int)PlayerRoomID].PlayerData[(int)Enum.Parse(typeof(ChessGame.Team), PlayerTeamID)].linkHeartbeat = true;
			return Json( new {b=true});
		}
		/// <summary>
		/// 判断玩家数据是否正确
		/// </summary>
		/// <returns></returns>
		private bool IsPlayerDataTrue() {
			if (PlayerTeamID != null && PlayerRoomID != null) {
					if (PlayerUserName == chessGame[(int)PlayerRoomID].PlayerData[(int)Enum.Parse(typeof(ChessGame.Team), PlayerTeamID)].userName)
						return true;
					return false;
			}
			else
				return false;
		}
		/// <summary>
		/// 判断玩家当前是否仍然保持连接
		/// </summary>
		/// <returns></returns>
		private bool IsPlayerLink() {
			if (PlayerTeamID != null && PlayerRoomID != null) {
				if (chessGame[(int)PlayerRoomID].PlayerData[(int)Enum.Parse(typeof(ChessGame.Team), PlayerTeamID)].linkHeartbeat!=null)
					return true;
				return false;
			}
			else
				return false;
		}
	}
}
