function StartGameClick() {
    var passc = document.getElementById("passcode").value;
    var tese = document.getElementById("teamSel").value;
    var eo = document.getElementById("errorOutput");
    eo.innerText = "";
    if (passc == "") {
        eo.innerText = "许可代码不能为空";
        return;
    }
    $.ajax({
        url: theUrlRoot + '/StartPage/StartGame',
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ passCode: passc,team: tese}), 
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
}
document.addEventListener('DOMContentLoaded', function () {
    document.getElementById("passcode").addEventListener('keydown', function (event) {
        if (event.key === 'Enter') {
            StartGameClick();
        }
    });
});