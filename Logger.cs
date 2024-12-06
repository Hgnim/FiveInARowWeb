using static FiveInARowWeb.DataCore;
namespace FiveInARowWeb {
    public struct Logger{
        private static string GetNowTime() => DateTime.Now.ToString("[yy/MM/dd HH:mm:ss.fff]");
        internal static async void WriteLog(string message,byte level) {
            await Task.Run(() => {
                if (config.DebugMode >= level)
                    ForceWriteLog(message);
            });
        }
        internal static void ForceWriteLog(string message) {
            Console.WriteLine($"{GetNowTime()} {message}");
        }
    }
}
