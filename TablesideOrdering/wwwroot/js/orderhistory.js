"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/orderHub").build();

$(function () {
    connection.start().then(function () {
        InvokeOrders();
    }).catch(function (err) {
        return console.error(err.toString());
    });
});

function InvokeOrders() {
    connection.invoke("SendOrders").catch(function (err) {
        return consol.error(err.toString());
    });
}

connection.on("ReceivedOrders", function (orders) {
    BindOrdersToGrid(orders)
});
function BindOrdersToGrid(orders) {
    $('#status').empty();
    var tr;
    $.each(orders, function (index, order) {        
        tr.append(`<td>${(order.status)}</td>`);
        $('#status').append(tr);

        let x;
        let toast = document.getElementById("toast");
        clearTimeout(x);
        toast.style.transform = "translateX(0)";
        x = setTimeout(() => {
            toast.style.transform = "translateX(400px)"
        }, 5000);
    });
}

function closeToast() {
    toast.style.transform = "translateX(400px)";
}
