using Microsoft.AspNetCore.Mvc;
using System.Reflection.Emit;
using static FiveInARowWeb.Logger;
using static FiveInARowWeb.DataCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FiveInARowWeb {
    public class BaseController:Controller {
        private string GetClientIP() {
            try {
                if (DataCore.config.Website.UseXFFRequestHeader)
                    return Request.Headers["X-Forwarded-For"].ToString();
                else
                    return HttpContext.Connection.RemoteIpAddress!.ToString();
            } catch { return "GetIPError"; }
        }
        public async void DoClientLog(string message,byte level) {
            if (config.DebugMode >= level)
                await Task.Run(() => {
                 ForceWriteLog($"{GetClientIP()} {message}");
                });
        }

        /// <summary>
        /// 玩家队伍ID
        /// white;black
        /// </summary>
        public string? PlayerTeamID {
            set { HttpContext.Session.SetString("PlayerTeamID", value!); }
            get => HttpContext.Session.GetString("PlayerTeamID");
        }
        /// <summary>
        /// 玩家房间ID
        /// </summary>
        public int? PlayerRoomID {
            set { HttpContext.Session.SetInt32("PlayerRoomID", (int)value!); }
            get => HttpContext.Session.GetInt32("PlayerRoomID");
        }
        /// <summary>
        /// 玩家昵称
        /// </summary>
        public string? PlayerUserName {
			set { HttpContext.Session.SetString("PlayerUserName", value!); }
			get => HttpContext.Session.GetString("PlayerUserName");
		}
    }
}
