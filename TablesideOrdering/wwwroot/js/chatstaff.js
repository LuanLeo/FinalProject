"use strict";
connection2.on("ReceiveMessage", function (user, message) {    
    if (user != "Admin") {
        var currentdate = new Date();
        var datetime = currentdate.getDate() + "/" + (currentdate.getMonth()+1) + "/" + currentdate.getFullYear() + " "
            + currentdate.getHours() + ":" + currentdate.getMinutes();
        var doc = $('<ul/>');
        doc.append(`<li class="repaly">`
            + `<p>${message}</p>`
            + `<span class="time">${datetime}</span>`
            + `</li>`)
        $('#chatbox').append(doc);
    }
    else{
        var currentdate = new Date();
        var datetime = currentdate.getDate() + "/" + (currentdate.getMonth() + 1) + "/" + currentdate.getFullYear() + " "
            + currentdate.getHours() + ":" + currentdate.getMinutes();

        var doc = $('<ul/>');
        doc.append(`<li class="sender">`
            + `<p> ${message}</p>`
            + `<span class="time">${datetime}</span>`
            + `</li>`)
        $('#chatbox').append(doc);
    }
    location.reload();

    SoundChat();        
    UpdateScroll();
});

function UpdateScroll() {
    var element = document.getElementById("messageBody");
    element.scrollTop = element.scrollHeight;
}
function SoundChat() {
    var audio = new Audio("/Sound/SoundChat.mp3");
    audio.addEventListener('ended', function () {
        vol.innerText = "";
    });
    audio.play();
}