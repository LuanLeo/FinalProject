"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/orderHub").build();

$(function () {
    connection.start().then(function () {
        alert('Connected to orderHub');

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
    $('#tblOrder tbody').empty();
    var tr;
    $.each(orders, function (index, order) {
        tr = $('<tr/>');
        tr.append(`<td>${(order.orderId)}</td>`);
        tr.append(`<td>${(order.orderDate)}</td>`);
        tr.append(`<td>${(order.orderPrice)}</td>`);
        tr.append(`<td>${(order.productQuantity)}</td>`);
        tr.append(`<td>${(order.phoneNumber)}</td>`);
        tr.append(`<td>${(order.tableNo)}</td>`);
        $('#tblOrder').append(tr);
    });
}