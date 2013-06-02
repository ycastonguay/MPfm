$(document).ready(function() {
    var hostUrl = document.location.protocol + '//' + document.location.host;

    $('#btnDownloadIndex').click(function (){
        $.getJSON(hostUrl + '/api/index/json', function(data) {
            var tbl_body = "";
            console.log('Received json from /api/index/json');
            var artist = "";
            $.each(data, function(key, item) {

                if(artist !== item.ArtistName) {
                    tbl_body += "<tr><td colspan='4' class='sectionHeader'>" + item.ArtistName + "</td></tr>";
                    artist = item.ArtistName;
                }

                tbl_body += "<tr><td>" + item.ArtistName + "</td><td>" + item.AlbumTitle + "</td><td>" + item.Title + "</td>" +
                            "<td><a href='/api/audiofile/" + item.Id + "'>Download</a></td></tr>";
            });
            $("#tableIndex").html(tbl_body);
        });
    });

    $('#btnUpload').click(function (){
        $('input[type=file]').upload(hostUrl + '/upload', function(res) {
            alert('File uploaded');
        });
    });
});

function submitLoginForm() {
    console.log("Submitting login form...");
    var form = document.createElement("form");
    var myForm = document.formLogin;
    var authenticationCode = document.getElementById("authenticationCode").value;
    createCookie('authenticationCode', authenticationCode);
    form.submit.apply(myForm);
}

function createCookie(name, value, days) {
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    } else var expires = "";
    document.cookie = escape(name) + "=" + escape(value) + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = escape(name) + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return unescape(c.substring(nameEQ.length, c.length));
    }
    return null;
}

function eraseCookie(name) {
    createCookie(name, "", -1);
}