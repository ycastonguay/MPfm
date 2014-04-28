$(document).ready(function() {
    var hostUrl = document.location.protocol + '//' + document.location.host;

    $('#btnDownloadIndex').click(function (){
        $.getJSON(hostUrl + '/api/index/json', function(data) {
            console.log('Received json from /api/index/json');
            var body = "";
            var artist = "";
            var album = "";
            var counterArtist = 0;
            var counterAlbum = 0;
            $.each(data, function(key, item) {

                var isNewArtist = false;
                if(artist !== item.ArtistName) {
                    isNewArtist = true;
                    counterArtist++;
                    if(counterArtist > 1) {
                        body += "</table></div></div>";
                    }
                    body += "<div class='sectionArtistName'><img src='/images/icon_artist.png' class='icon' /><div class='sectionArtistNameTitle'>" + item.ArtistName + "</div>";
                    artist = item.ArtistName;
                }

                if(album !== item.AlbumTitle) {
                    counterAlbum++;
                    if(counterAlbum > 1 && !isNewArtist) {
                        body += "</table></div>";
                    }
                    body += "<div class='sectionAlbumTitle' style='display: none;'><img src='/images/icon_album.png' class='icon' /><div class='sectionAlbumTitleTitle'>" + item.AlbumTitle + "</div><table class='songs' style='display: none;'>";
                    album = item.AlbumTitle;
                }

                body += "<tr class='song'><td style='width: 20px;'>" + item.TrackNumber + "</td><td>" + item.Title + "</td><td style='width: 80px;'>" + item.Length + "</td>" +
                    "<td style='width: 84px; padding-right: 6px;'><a href='/api/audiofile/" + item.Id + "'>Download</a></td></tr>";
            });
            body += "</div></div>";
            $("#tabDownloadContents").html(body);
        });
    });

    $('#btnUpload').click(function (){
        $('input[type=file]').upload(hostUrl + '/upload', function(res) {
            alert('File uploaded');
        });
    });

    $('#tabDownload').click(function (){
        $('#tabDownload').toggleClass('tabSelected');
        $('#tabUpload').toggleClass('tabSelected');
        $('#tabDownloadContents').toggle();
        $('#tabUploadContents').toggle();
    });

    $('#tabUpload').click(function (){
        $('#tabDownload').toggleClass('tabSelected');
        $('#tabUpload').toggleClass('tabSelected');
        $('#tabDownloadContents').toggle();
        $('#tabUploadContents').toggle();
    });

    $('#tabDownloadContents').on('click', 'div.sectionArtistName div.sectionArtistNameTitle', function() {
        console.log('hello artist');
        jQuery(this).siblings('.sectionAlbumTitle').toggle();  //.css('display', 'block');
    });

    $('#tabDownloadContents').on('click', 'div.sectionAlbumTitle div.sectionAlbumTitleTitle', function() {
        console.log('hello album');
        jQuery(this).siblings('.songs').toggle();
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

function pad(num, size) {
    var s = "000000000" + num;
    return s.substr(s.length-size);
}