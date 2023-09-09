"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    if (message != null) {
        var currentdate = new Date();
        var datetime = currentdate.getDay() + "/" + currentdate.getMonth() + "/" + currentdate.getFullYear() + " "
            + currentdate.getHours() + ":" + currentdate.getMinutes();

        var doc = $('<div class="chat-message clearfix"/>');
        doc.append('    <img src="/Logo/Avatar/defaultavatar.png" alt="" width="32" height="32">');
        doc.append('    <div class="chat-message-content clearfix">'
            + <span class="chat-time">${datetime}</span>
            + <h5>Table - ${user}</h5>
            + <p>${message}</p>
            +</div >);
        doc.append(<hr>);
            $('#chatbox').append(doc);
    }
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});


document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var receiver = document.getElementById("receiverInput").value;
    var message = document.getElementById("messageInput").value;

    if (message != null) {
        connection.invoke("SendMessageToGroup", user, receiver, message).catch(function (err) {
            return console.error(err.toString());
        });
    }
    event.preventDefault();
});

