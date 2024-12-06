using static FiveInARowWeb.DataCore;
using static FiveInARowWeb.FilePath;

namespace FiveInARowWeb {
	internal struct DataCore {
		internal static Config config = new();
		internal static ChessGame chessGame = new();
	}
	internal struct FilePath {
		internal const string dataDir = "fiarw_data/";
		internal const string configFile = dataDir + "config.yml";
	}
	internal struct UrlPath {
		internal static string RootUrl => config.Website.Url.UrlRoot + "/";
		internal static string ChessPageUrl => config.Website.Url.UrlRoot + "/Chess/ChessPage";
		internal struct Img {
			private const string dir = "/img/";
			/// <summary>
			/// ChessPage
			/// </summary>
			internal struct CP {
				/// <summary>
				/// Get Background
				/// </summary>
				/// <param name="id">id</param>
				/// <returns></returns>
				internal static string GetBg(byte id) => 
					config.Website.Url.UrlRoot + $"{dir}cp_bg{id}.png";
				/// <summary>
				/// chess black
				/// </summary>
				internal static string Cb => config.Website.Url.UrlRoot + $"{dir}cp_cb.png";
				/// <summary>
				/// chess white
				/// </summary>
				internal static string Cw => config.Website.Url.UrlRoot + $"{dir}cp_cw.png";
				/// <summary>
				/// null
				/// </summary>
				internal static string N => config.Website.Url.UrlRoot + $"{dir}cp_n.png";
			}
		}
	}
	
	
}
