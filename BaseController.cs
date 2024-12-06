using Microsoft.AspNetCore.Mvc;
using System.Reflection.Emit;
using static FiveInARowWeb.Logger;
using static FiveInARowWeb.DataCore;

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
    }
}
