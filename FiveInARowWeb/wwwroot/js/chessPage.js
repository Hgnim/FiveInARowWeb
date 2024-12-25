var canDoChess = true;
function SendChess(id) {
    if (canDoChess) {
        $.ajax({
            url: theUrlRoot + '/Chess/SendDoChess',
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ v: id }),
            success: function (response) {
                //console.log(response);
                if (response.b == false) {
                    if (response.canDoChess == false) {
                        canDoChess = false;
                    }
                }
            },
            error: function () {
                alert("请求错误，疑似网络故障");
            }
        });
    }
}
function GetLocalTeam() {
    var lti = document.getElementById('localTeam_icon');
    var ltt = document.getElementById('localTeam_text');
    $.get(theUrlRoot + '/Chess/GetLocalTeam', function (response) {
        if (response.canDoChess == false) {
            canDoChess = false;
            ltt.innerText = "旁观者模式";
        } else {
            switch (response.id) {
                case "white":
                    ltt.innerText = "你执白子"; break;
                case "black":
                    ltt.innerText = "你执黑子"; break;
            }
        }
        lti.src = response.icon;
    });
}
function UpdateChess(force = false) {
     $.ajax({
         url: theUrlRoot+ '/Chess/GetChessUpdate',
        type: 'POST',
        contentType: "application/json",
        data: JSON.stringify({ isForce: force}),
        success: function (res) {
            //console.log(res);
            for (i = 0; i < 15; i++) {
                for (j = 0; j < 15; j++) {
                    document.getElementById('chessObj_' + i + '-' + j).src = res.chessImgUrl[res.chessData[i][j]];
                    document.getElementById('chessBg_' + i + '-' + j).style.filter = "none";
                }
            }
            for (i = 0; i < 2; i++) {
                if (res.playerData[i] != null) {
                    WhiteOrBlackTeamUserName_TextChange(true, i, res.playerData[i].userName);
                }
                else {
                    WhiteOrBlackTeamUserName_TextChange(false, i);
                }
            } 
            { 
                var rm = document.getElementById('restartMessage');
                if (res.whoClickRestartButton[0] == true && res.whoClickRestartButton[1] == false)
                    rm.innerText = "白方已点击重新开始按钮";
                else if (res.whoClickRestartButton[0] == false && res.whoClickRestartButton[1] == true)
                    rm.innerText = "黑方已点击重新开始按钮";
                else
                    rm.innerText = "";
            }
            {
                var st = document.getElementById('stateText');
                if (canDoChess) {
                    if (res.isYouDoChess) {
                        st.innerText = "你的回合";
                    }
                    else {
                        st.innerText = "等待对手";
                    }
                } else {
                    if (st.innerText != "")
                        st.innerText = "";
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
            else if (res.winData_IsTie == true) {
                 document.getElementById('stateText').innerText="平局";
            }    
            if (res.lastDo != null) {
                document.getElementById('chessBg_' + res.lastDo[0] + '-' + res.lastDo[1]).style.filter = "invert(60%)";
            }
            if (force == false) UpdateChess();
        }
     });
}
var isRestartButtonClick = false;
function RestartGame() {
    if (!isRestartButtonClick) {
        isRestartButtonClick = true;
        document.getElementById("restartButton").className = "disButton";
        if (canDoChess) {
            $.get(theUrlRoot + '/Chess/RestartGame', function (response) {
                if (response.b == true) {
                    window.location.href = response.url;
                }
                else if (response.b == false) {
                    if (response.canDoChess == false) {
                        canDoChess = false;
                    }
                }
            });
        }
    }
}
async function SendHeartBeat() {
    while (canDoChess) {
        $.get(theUrlRoot + '/Chess/HeartBeat', function (response) {
            if (response.b == false) {
                if (response.canDoChess == false) {
                    canDoChess = false;
                }
            }
        });
        await sleep(3000);
    }
}
window.onload = function () {
    GetLocalTeam();
    UpdateChess(true);
    UpdateChess();  
}
document.addEventListener('DOMContentLoaded', function () {
    SendHeartBeat();
});

function WhiteOrBlackTeamUserName_TextChange(online, team, text = "") {
    var tun;
    switch (team) { 
        case 0:
            tun = document.getElementById('whiteTeamUserName'); break;
        case 1:
            tun = document.getElementById('blackTeamUserName'); break;
}
    if (!online) {
        tun.style.color = 'red';
        tun.innerText = "离线";
    }
    else {
        tun.style.color = 'aqua';
        tun.innerText = text;
    }    
}

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}
