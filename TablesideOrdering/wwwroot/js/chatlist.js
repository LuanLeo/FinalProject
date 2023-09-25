"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatlistHub").build();

$(function () {
    connection.start().then(function () {
        InvokeChats();
    }).catch(function (err) {
        return console.error(err.toString());
    });
});

function InvokeChats() {
    connection.invoke("SendChats").catch(function (err) {
        return consol.error(err.toString());
    });
}

connection.on("ReceivedChats", function (chats) {
    BindChatsToGrid(chats)
});
function BindChatsToGrid(chats) {
    $('#chat').empty();
    var tr;
    $.each(chats, function (chat) {
            tr = $('<div/>');
            tr.append(`<a asp-area="Staff" asp-controller="Chat" asp-action="ChatRoom" asp-route-id="${(chat.chatRoomID)}" class="d-flex align-items-center">`);
            tr.append(`     <div class="flex-shrink-0">`);
            tr.append(`         <img class="img-fluid" src="~/Logo/Avatar/defaultavatar.png" alt="user img" height="45px" width="45px">`);
            tr.append(`     </div>`);
            tr.append(`     <div class="flex-grow-1 ms-3">`);
            tr.append(`        <h3>Table - ${(chat.chatRoomID)}</h3>`);
            tr.append(`     </div>`);
            $('#chat').append(tr);
    });
}
