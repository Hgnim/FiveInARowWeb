using Force.DeepCloner;
using static FiveInARowWeb.DataCore;
using static FiveInARowWeb.Logger;

namespace FiveInARowWeb {
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

        internal bool DoChess(ChessPos pos, Team fromTeam) {
            if (fromTeam == WhoDoChess &&
                winData == null &&
                chessData[pos.x, pos.y] == ChessType.none
                ) {
                switch (fromTeam) {
                    case Team.white:
                        chessData[pos.x, pos.y] = ChessType.white;
                        WriteLog($"白方执棋({pos.x},{pos.y})", 2);
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
                            WriteLog($"游戏结束，被迫和局", 2);
                        exit:;
                        });
                        break;
                    case Team.black:
                        chessData[pos.x, pos.y] = ChessType.black;
                        WriteLog($"黑方执棋({pos.x},{pos.y})", 2);
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
                                        WriteLog($"游戏结束，{fromTeam}胜利", 2);
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
                WriteLog("游戏重启", 2);
                DoRestartGame?.Invoke();
                chessGame = new ChessGame().DeepClone();
            }
        }
    }
}
