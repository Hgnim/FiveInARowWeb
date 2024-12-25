using Force.DeepCloner;
using static FiveInARowWeb.ChessGame;
using static FiveInARowWeb.DataCore;
using static FiveInARowWeb.Logger;

namespace FiveInARowWeb {
    public class ChessGame {
        public ChessGame() 
        {
			StartHearbeatThread();
		}
        /// <summary>
        /// 当有数据更改后需要进行数据更新时的事件
        /// </summary>
        public event Action? SomethingUpdate;
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
                case Team.white: whoDoChess = Team.black; break;
                case Team.black: whoDoChess = Team.white; break;
            }
        }


        public class ChessPos {
            internal byte x;
            internal byte y;
        }
        private ChessPos? lastDo = null;
        /// <summary>
        /// 上一个走棋的位置
        /// </summary>
        internal ChessPos? LastDo => lastDo;

        public enum ChessType {
            none, white, black
        }
        private readonly ChessType[,] chessData = new ChessType[15, 15];
        /// <summary>
        /// 当前棋盘上的棋子数据
        /// </summary>
        internal ChessType[,] ChessData => chessData;

        internal bool DoChess(ChessPos pos, Team fromTeam, int roomID) {
            if (fromTeam == WhoDoChess &&
                winData == null &&
                chessData[pos.x, pos.y] == ChessType.none
                ) {
                switch (fromTeam) {
                    case Team.white:
                        chessData[pos.x, pos.y] = ChessType.white;
                        WriteLog($"[room-{roomID}] 白方执棋({pos.x},{pos.y})", 2);
                        Task.Run(() => {
                            for (int i = 0; i < 15; i++) {
                                for (int j = 0; j < 15; j++) {
                                    if (chessData[i, j] == ChessType.none) {
                                        goto exit;
                                    }
                                }
                            }//判断是否棋盘下满，如果下满则和局
                            winData = new() {
                                winChessPos = null,
                                winTeam = null,
                                isTie =true
                            };
                            WriteLog($"[room-{roomID}] 游戏结束，被迫和局", 2);
                        exit:;
                        });
                        break;
                    case Team.black:
                        chessData[pos.x, pos.y] = ChessType.black;
                        WriteLog($"[room-{roomID}] 黑方执棋({pos.x},{pos.y})", 2);
                        break;
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
                                        WriteLog($"[room-{roomID}] 游戏结束，{fromTeam}胜利", 2);
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
                SomethingUpdate?.Invoke();
                return true;
            }
            return false;
        }

        public class WinData {
            internal ChessPos[]? winChessPos = new ChessPos[5];
            internal Team? winTeam;
            internal bool isTie = false;
        }
        /// <summary>
        /// 如果该字段不为null，则代表已经有胜利者了
        /// </summary>
        internal WinData? winData = null;

        /// <summary>
        /// 重启游戏票数，当两个布尔值同时为true时重启游戏
        /// </summary>
        private readonly bool[] restartGame = [false, false];

        internal bool[] RestartGame => restartGame;
        public void SetRestartGameNum(Team team,int roomID) {
            switch (team) {
                case Team.white:
                    restartGame[0] = true;
                    break;
                case Team.black:
                    restartGame[1] = true;
                    break;
            }
            if (RestartGame[0] == true && RestartGame[1] == true) {
                Player[] oldPlayerData = PlayerData.DeepClone();
                WriteLog($"[room-{roomID}] 游戏重启", 2);
                DoRestartGame?.Invoke();
                chessGame[roomID] = new ChessGame() {
                    playerData = [oldPlayerData[1],oldPlayerData[0]],
                }.DeepClone();
            }else
				SomethingUpdate?.Invoke();
		}

        /// <summary>
        /// 玩家信息
        /// </summary>
        internal class Player {
            public string userName="";
            public Team team;

			/// <summary>
			/// 心跳检测，调用StartHearbeatThread()后启用心跳检测。当心跳再次为null时，则代表连接已死亡
			/// </summary>
			internal bool? linkHeartbeat = true;           
		}
        /*delegate void PlayerDataTeam(Team team);
		event PlayerDataTeam StartHearbeatThread;*/
        void StartHearbeatThread() {
			for (int tid = 0; tid < 2; tid++) {
                if (HearbeatThread[tid] == null || !HearbeatThread[tid].IsAlive) {
					HearbeatThread[tid] = new(new ParameterizedThreadStart((object? teamObj)=> {
                        Team team;
                        if (teamObj != null)
                            team = (Team)teamObj;
                        else return;
                        if (playerData[(int)team] != null) {
                            try {
                                SomethingUpdate?.Invoke();
                                while (playerData[(int)team].linkHeartbeat != null) {
                                    Thread.Sleep(6000);
                                    if (playerData[(int)team].linkHeartbeat == true)
                                        playerData[(int)team].linkHeartbeat = false;
                                    else
                                        playerData[(int)team].linkHeartbeat = null;
                                }
                                playerData[(int)team] = null!;
                                SomethingUpdate?.Invoke();
                            } 
                            catch (ObjectDisposedException) { } 
                            catch (NullReferenceException) { }
                        }
					}));HearbeatThread[tid].Start(tid);
				}
            }
		}
		readonly Thread[] HearbeatThread=new Thread[2];
        

		Player[] playerData=new Player[2];
		/// <summary>
		/// 玩家信息。0为白方，1为黑方
		/// </summary>
		internal Player[] PlayerData=>playerData;
		/// <summary>
		/// 快速将玩家信息输入至数据堆中
		/// </summary>
		/// <param name="team">队伍</param>
		/// <param name="userName">用户昵称</param>
		internal void InputPlayerData(Team team,string userName) {
            if (playerData[(int)team] == null|| playerData[(int)team].userName==userName) {
				playerData[(int)team] = new() {
					userName = userName,
					team = team,
					linkHeartbeat = true//开启心跳检测
				};
				StartHearbeatThread();
            }
		}
		/// <summary>
		/// 快速将玩家信息输入至数据堆中
		/// </summary>
		/// <param name="team">队伍</param>
		/// <param name="userName">用户昵称</param>
		internal void InputPlayerData(string team, string userName) {
            InputPlayerData((Team)Enum.Parse(typeof(Team), team), userName);
		}
	}
}
