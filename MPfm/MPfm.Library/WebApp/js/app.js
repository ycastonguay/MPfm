$(document).ready(function() {
    $('#btnUpload').click(function (){
        $('input[type=file]').upload('http://192.168.1.101:53551/', function(res) {
            alert('File uploaded');
        });
    });
});

