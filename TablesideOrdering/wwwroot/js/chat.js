"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var receiver = document.getElementById("receiverInput").value;
    var message = document.getElementById("messageInput").value;

    connection.invoke("SendMessageToGroup", user, receiver, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();

});

connection.on("ReceiveMessage", function (user, message, receiver) {
    var table = document.getElementById("receiverInput").value;

    if (user === "Admin" && receiver === table) {
        var currentdate = new Date();
        var datetime = currentdate.getDate() + "/" + (currentdate.getMonth() + 1) + "/" + currentdate.getFullYear() + " "
            + currentdate.getHours() + ":" + currentdate.getMinutes();
        var doc = $('<div/>');
        doc.append('    <img src="/Logo/Avatar/user.png" alt="" width="32" height="32">');
        doc.append('    <div class="chat-message-content clearfix">'
            + `<span class="chat-time">${datetime}</span>`
            + `<h5>Table - ${user}</h5>`
            + `<p>${message}</p>`
            + `</div>`
            + `</div>`)
        doc.append('<hr>');

        SoundChat();
        $('#chatbox').append(doc);
    }
    else if (user === table) {
        var currentdate = new Date();
        var datetime = currentdate.getDate() + "/" + (currentdate.getMonth() + 1) + "/" + currentdate.getFullYear() + " "
            + currentdate.getHours() + ":" + currentdate.getMinutes();
        var doc = $('<div/>');
        doc.append('    <img src="/Logo/Avatar/defaultavatar.png" alt="" width="32" height="32">');
        doc.append('    <div class="chat-message-content clearfix">'
            + `<span class="chat-time">${datetime}</span>`
            + `<h5>Table - ${user}</h5>`
            + `<p>${message}</p>`
            + `</div>`
            + `</div>`)
        doc.append('<hr>');
        SoundChat();
        $('#chatbox').append(doc);
    }
    updateScroll();

});
function updateScroll() {
    var element = document.getElementById("messages");
    element.scrollTop = element.scrollHeight;
}
function SoundChat() {
    var audio = new Audio("/Sound/SoundChat.mp3");
    audio.addEventListener('ended', function () {
        vol.innerText = "";
    });
    audio.play();
}