"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var doc = $('<div/>');
    //var datetime = currentdate.getHours() + ":"+ currentdate.getMinutes();
    doc.append(`<div class="chat-message clearfix">`);
    doc.append('<img src="/Logo/Avatar/defaultavatar.png" alt="" width="32" height="32">');
    doc.append('<div class="chat-message-content clearfix">');
    doc.append(`<span class="chat-time"></span>`);
    doc.append(`<h5 id="${user}"></h5>`);
    doc.append(`<p >${message}</p>`);
    doc.append('</div> <!-- end chat-message-content -->');
    doc.append('<hr>');
    $('#chatbox').append(doc);
   
    
    /*tr = $('<tr/>');
    tr.append(`<td>${(index + 1)}</td>`);
    tr.append(`<td>${(order.orderDate)}</td>`);
    tr.append(`<td>${(order.orderPrice)}</td>`);
    tr.append(`<td>${(order.productQuantity)}</td>`);
    tr.append(`<td>${(order.phoneNumber)}</td>`);
    tr.append(`<td>${(order.cusName)}</td>`);
    tr.append(`<td>${(order.tableNo)}</td>`);
    tr.append(`<td><button type="button" class="btn btn-warning m-1" onclick="location.href='../Staff/Order/Details?id=${(order.orderId)}'">Details</button></td>`);
    $('#EatInOrder').append(tr);*/
    

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

    if (receiver != null) {
        connection.invoke("SendMessageToGroup", user, receiver, message).catch(function (err) {
            return console.error(err.toString());
        });
    }
    event.preventDefault();
});

