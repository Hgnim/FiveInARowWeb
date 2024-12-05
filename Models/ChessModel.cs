namespace FiveInARowWeb.Models {
	public static class ChessModel {
		public class SendDoChessValueModel {
			public required string V { get; set; }
		}
		public class ChessUpdateValueModel {
			private string[] chessImgUrl=null!;
			public string[] ChessImgUrl { get => chessImgUrl; set => chessImgUrl = value; }
			private ChessGame.ChessType[][] chessdata=null!;
			public ChessGame.ChessType[][] ChessData { get=>chessdata; set=>chessdata=value; }
			private int[]? lastDo=null;
			public int[]? LastDo {  get=> lastDo; set=>lastDo=value; }
			/*bool haveWinner;
			public bool HaveWinner {  get=>haveWinner; set=>haveWinner=value; }*/
			int[][]? winData_winChessPos = null;
			public int[][]? WinData_WinChessPos { get=> winData_winChessPos; set=> winData_winChessPos = value; }
			ChessGame.Team? winData_winTeam = null;
			public ChessGame.Team? WinData_WinTeam { get => winData_winTeam; set => winData_winTeam = value; }
			ChessGame.Team whoDoChess;
			public ChessGame.Team WhoDoChess { get=>whoDoChess; set=>whoDoChess=value; }
			bool isYouDoChess;
			public bool IsYouDoChess { get => isYouDoChess; set => isYouDoChess = value; }
		}
		public class ChessUpdatePostValueModel {
			public required bool IsForce {  get; set; }
		}
	}
}
