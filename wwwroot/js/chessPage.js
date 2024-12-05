function SendChess(id) {
    $.ajax({
        url: '/Chess/SendDoChess',
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ v: id }),
        success: function (response) {
            console.log(response);
            if (response.b == true) {
                UpdateChess(true);
            }
        },
        error: function () {
            alert("请求错误，疑似网络故障");
        }
    });
}
function GetLocalTeam() {
    var lti = document.getElementById('localTeam_icon');
    var ltt = document.getElementById('localTeam_text');
    $.get('/Chess/GetLocalTeam', function (response) {
        switch (response.id) {
            case "white":
                ltt.innerText = "你执白子"; break;
            case "black":
                ltt.innerText = "你执黑子"; break;
        }
        lti.src = response.icon;
    });
}
function UpdateChess(force=false) {
     $.ajax({
        url: '/Chess/GetChessUpdate',
        type: 'POST',
        contentType: "application/json",
        data: JSON.stringify({ isForce: force}),
        success: function (res) {
            console.log(res);
            for (i = 0; i < 15; i++) {
                for (j = 0; j < 15; j++) {
                    document.getElementById('chessObj_' + i + '-' + j).src = res.chessImgUrl[res.chessData[i][j]];
                    document.getElementById('chessBg_' + i + '-' + j).style.filter = "none";
                }
            }
            if (res.lastDo != null) {
                document.getElementById('chessBg_' + res.lastDo[0] + '-' + res.lastDo[1]).style.filter = "invert(40%)";
            }
            {
                var st = document.getElementById('stateText');
                if (res.isYouDoChess) {
                    st.innerText = "你的回合";
                }
                else {
                    st.innerText = "等待对手";
                }
            }
            if (res.winData_WinChessPos != null && res.winData_WinTeam != null) {
                for (i = 0; i < 5; i++) {
                    document.getElementById('chessBg_' + res.winData_WinChessPos[i][0] + '-' + res.winData_WinChessPos[i][1]).style.filter = "invert(40%)";
                }
                var st = document.getElementById('stateText');
                switch (res.winData_WinTeam) {
                    case 0:
                        st.innerText = "白方获胜";
                        break;
                    case 1:
                        st.innerText = "黑方获胜";
                        break;
                }
            }
            if (force == true) {
                UpdateChess();
            }
        }
    });
}
var isRestartButtonClick = false;
function RestartGame() {
    if (!isRestartButtonClick) {
        isRestartButtonClick = true;
        document.getElementById("restartButton").className = "disButton";
        $.get('/Chess/RestartGame', function (response) {
            if (response.b == true) {
                window.location.href = response.url;
            }
        });
    }
    }
window.onload = function () {
    GetLocalTeam();
    UpdateChess(true);
}
/*document.addEventListener('DOMContentLoaded', function () {
});*/