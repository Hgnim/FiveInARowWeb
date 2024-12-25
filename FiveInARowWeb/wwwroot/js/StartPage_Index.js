function StartGameClick() {
    var passc = document.getElementById("passcode").value;
    var tese = document.getElementById("teamSel").value;
    var uname = document.getElementById("userName").value;
    var eo = document.getElementById("errorOutput");
    eo.innerText = "";
    if (passc == "") {
        eo.innerText = "许可代码不能为空";
        return;
    }
    if (uname == "") {
        eo.innerText = "用户昵称不能为空";
        return;
    } else if (uname.length > 34) {
        eo.innerText = "用户昵称长度不得大于34";
        return;
    }
    $.ajax({
        url: theUrlRoot + '/StartPage/StartGame',
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ passCode: passc,team: tese,uname: uname}), 
        success: function (response) {
            switch (response.value) {
                case 0:
                    window.location.href = response.url;
                    break;
                case -1:
                    eo.innerText = "许可代码错误";
                    break;
            }
        },
    });
    setCookie('userName', uname);
    setCookie('teamSel', tese);
}
document.addEventListener('DOMContentLoaded', function () {
    document.getElementById("passcode").addEventListener('keydown', function (event) {
        if (event.key === 'Enter') {
            StartGameClick();
        }
    });
    document.getElementById("userName").value = getCookie('userName');
    {
        var teamValue = getCookie('teamSel');
        if(teamValue!=null)
            document.getElementById("teamSel").value = teamValue;
    }
});

function setCookie(name, value, days = 36500) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}
function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}