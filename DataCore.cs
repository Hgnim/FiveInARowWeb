using Force.DeepCloner;
using YamlDotNet.Serialization;
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
	public class ChessGame {
		/// <summary>
		/// 当有人执行棋子的时候调用该事件
		/// </summary>
		public event Action? SomeOneDoChess;
		/// <summary>
		/// 执行重启游戏事件
		/// </summary>
		public event Action? DoRestartGame;
		public enum Team {
			white, black
		}
		
		private Team whoDoChess = Team.white;
		/// <summary>
		/// 归哪一方执棋
		/// </summary>
		internal Team WhoDoChess => whoDoChess;
		private void WhoDoChessToggle() {
			switch (whoDoChess) {
				case Team.white: whoDoChess=Team.black; break;
				case Team.black: whoDoChess=Team.white; break;
			}
		}

		
		public class ChessPos {
			internal byte x;
			internal byte y;
		}
		private ChessPos? lastDo =null;
		/// <summary>
		/// 上一个走棋的位置
		/// </summary>
		internal ChessPos? LastDo => lastDo;

		public enum ChessType {
			none, white, black
		}
		private readonly ChessType[,] chessData = new ChessType[15,15];
		/// <summary>
		/// 当前棋盘上的棋子数据
		/// </summary>
		internal ChessType[,] ChessData => chessData;

		internal bool DoChess(ChessPos pos, Team fromTeam) {
			if(fromTeam== WhoDoChess && 
				winData == null && 
				chessData[pos.x, pos.y] == ChessType.none
				) {
						switch (fromTeam) {
							case Team.white:
								chessData[pos.x, pos.y] = ChessType.white; break;
							case Team.black:
								chessData[pos.x, pos.y] = ChessType.black; break;
						}
						lastDo = pos.DeepClone();
						{
							ChessType nowChessType = ChessData[pos.x, pos.y];

							for (int i = 0; i < 4; i++) {
								List<ChessPos> winChessPosTmp = [LastDo.DeepClone()];
								for (int t = 0; t < 2; t++) {
									ChessPos? checkTmpPos = pos.DeepClone();//用于进行位移检查
									do {
										switch (i) {
											case 0:
												switch (t) {
													case 0: checkTmpPos.y--; break;
													case 1: checkTmpPos.y++; break;
												}
												break;
											case 1:
												switch (t) {
													case 0:
														checkTmpPos.y--;
														checkTmpPos.x++;
														break;
													case 1:
														checkTmpPos.y++;
														checkTmpPos.x--;
														break;
												}
												break;
											case 2:
												switch (t) {
													case 0: checkTmpPos.x++; break;
													case 1: checkTmpPos.x--; break;
												}
												break;
											case 3:
												switch (t) {
													case 0:
														checkTmpPos.y++;
														checkTmpPos.x++;
														break;
													case 1:
														checkTmpPos.y--;
														checkTmpPos.x--;
														break;
												}
												break;
										}
										if (((checkTmpPos.x >= 0 && checkTmpPos.x < 15) && (checkTmpPos.y >= 0 && checkTmpPos.y < 15)) &&
											ChessData[checkTmpPos.x, checkTmpPos.y] == nowChessType) {
											winChessPosTmp.Add(checkTmpPos.DeepClone());
											if (winChessPosTmp.Count == 5) {
												winData = new() {
													winTeam = fromTeam,
													winChessPos = [.. winChessPosTmp]
												};
												goto endFor;
											}
										}
										else
											break;
									} while (true);
								}
							}
						endFor:;
						}

						WhoDoChessToggle();
						SomeOneDoChess?.Invoke();
						return true;
			}
			return false;
		}

		public class WinData {
			internal ChessPos[] winChessPos=new ChessPos[5];
			internal Team winTeam;
		}
		/// <summary>
		/// 如果该字段不为null，则代表已经有胜利者了
		/// </summary>
		internal WinData? winData = null;

		/// <summary>
		/// 重启游戏票数，当两个布尔值同时为true时重启游戏
		/// </summary>
		private readonly bool[] restartGame = [false,false];
		
		public bool[] RestartGame => restartGame;
		public void SetRestartGameNum(Team team) {
			switch (team) {
				case Team.white:
					restartGame[0] = true;
					break;
				case Team.black:
					restartGame[1] = true;
					break;
			}
			if (RestartGame[0] == true && RestartGame[1] == true) {
				DoRestartGame?.Invoke();
				chessGame = new ChessGame().DeepClone();
			}
		}
	}
	public class Config {
		/// <summary>
		/// 将配置数据保存至配置文件中
		/// </summary>
		internal static void SaveData() {
			ISerializer yamlS = new SerializerBuilder()
					.Build();
			File.WriteAllText( configFile,yamlS.Serialize(config));
		}
		/// <summary>
		/// 读取数据文件并将数据写入实例中
		/// </summary>
		internal static void ReadData() {
			IDeserializer yamlD = new DeserializerBuilder()
					.Build();
			config= yamlD.Deserialize<Config>(File.ReadAllText(configFile));
		}
		public class SettingC {
			private string passCode = "0000";
			public string PassCode {
				set => passCode = value;
				get => passCode;
			}
		}
		private SettingC setting = new();
		public SettingC Setting {
			set => setting = value;
			get => setting;
		}

		public class WebsiteC {
			/// <summary>
			/// 是否启用 X-Forwarded-For(XFF)请求标头
			/// </summary>
			private bool useXFFRequestHeader = false;
			public bool UseXFFRequestHeader {
				set=>useXFFRequestHeader = value;
				get => useXFFRequestHeader;
			}
			public class UrlC {
				private bool useHttps = false;
				public bool UseHttps {
					set => useHttps = value;
					get => useHttps;
				}
				private string addr = "*";
				public string Addr {
					set => addr = value;
					get => addr;
				}
				///<summary>
				///urlRoot不能只包含单独的斜杠，这里只是起到占位的作用，到引用的时候单独的斜杠会被去掉。<br/>
				///在包含内容的时候，urlRoot前面必须包含斜杠，末尾不能含有斜杠
				///</summary>
				private string urlRoot = "/";
				public string UrlRoot {
					set {//格式化
						string urlRoot_;
						if (value == "/") urlRoot_ = "";//value不能只包含单独的斜杠
						else if (value == "") { urlRoot_ = value; }//如果为空则直接输出
						else {
							if (value[..1] == "/")
								urlRoot_ = value;
							else//如果开头没有斜杠则加上斜杠
								urlRoot_ = "/" + value;
							if (urlRoot_.Substring(urlRoot_.Length - 1, 1) == "/")//如果末尾包含斜杠则去掉
								urlRoot_ = urlRoot_[..(urlRoot_.Length - 1)];
						}
						urlRoot= urlRoot_;
					}
					get => urlRoot;
				}
				private string port = "80";
				public string Port {
					set => port = value;
					get => port;
				}
				internal string Get() {
					string head;

					if (useHttps)
						head = "https";
					else
						head = "http";

					return $"{head}://{addr}:{port}";
				}
			}
			private UrlC url = new();
			public UrlC Url {
				set => url = value;
				get => url;
			}
		}
		private WebsiteC website = new();
		public WebsiteC Website {
			set => website= value;
			get => website;
		}


		private bool debugMode=false;
		/// <summary>
		/// 调试模式，用于输出调试信息等
		/// </summary>
		public bool DebugMode {
			get => debugMode;
			set => debugMode = value;
		}
		private bool updateConfig = true;
		/// <summary>
		/// 设置为true后将在下次启动时更新一次配置文件
		/// </summary>
		public bool UpdateConfig{
			get=> updateConfig;
			set => updateConfig = value;
			}
	}
}
