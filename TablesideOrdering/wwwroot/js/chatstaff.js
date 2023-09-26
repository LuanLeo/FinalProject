"use strict";

var connection1 = new signalR.HubConnectionBuilder().withUrl("/chatlistHub").build();

$(function () {
    connection1.start().then(function () {
        InvokeChats();
    }).catch(function (err) {
        return console.error(err.toString());
    });
});

function InvokeChats() {
    connection1.invoke("SendChats").catch(function (err) {
        return consol.error(err.toString());
    });
}

connection1.on("ReceivedChats", function (chats) {
    BindChatsToGrid(chats)
});
function BindChatsToGrid(chats) {
    $('#chat').empty();
    var tr;
    $.each(chats, function (index, chat) {
        tr = $(`<a onclick="location.href='/Staff/Chat/ChatRoom/${(chat.chatRoomID)}'" class="d-flex align-items-center" />`);
        tr.append(`<div class="flex-shrink-0">`
            + `<img class="img-fluid" src="/Logo/Avatar/defaultavatar.png" alt="user img" height="45px" width="45px">`
            + `</div>`
            + `<div class="flex-grow-1 ms-3">`
            + `<h3>Table - ${(chat.chatRoomID)}</h3>`
            + `</div>`);
        $('#chat').append(tr);
    });
}

var connection2 = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection2.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var receiver = document.getElementById("receiverInput").value;
    var message = document.getElementById("messageInput").value;

    connection2.invoke("SendMessageToGroup", user, receiver, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

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