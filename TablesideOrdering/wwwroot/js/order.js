﻿"use strict";

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

    $('#EatInOrder tbody').empty();
    var tr;
    $.each(orders, function (index, order) {
        if (order.orderType === "Eat in") {
            tr = $('<tr/>');
            tr.append(`<td>${(order.orderId)}</td>`);
            tr.append(`<td>${(order.orderDate)}</td>`);
            tr.append(`<td>${(order.orderPrice)}</td>`);
            tr.append(`<td>${(order.productQuantity)}</td>`);
            tr.append(`<td>${(order.phoneNumber)}</td>`);
            tr.append(`<td>${(order.cusName)}</td>`);
            tr.append(`<td>${(order.tableNo)}</td>`);
            tr.append(`<td><button type="button" class="btn btn-warning m-1" onclick="location.href='../Staff/Order/Details?id=${(order.orderId)}'">Details</button></td>`);
            $('#EatInOrder').append(tr);

            SoundOrder();

            let x;
            let toast = document.getElementById("toast");
            clearTimeout(x);
            toast.style.transform = "translateX(0)";
            x = setTimeout(() => {
                toast.style.transform = "translateX(400px)"
            }, 5000);
        }
    });
}

function closeToast() {
    toast.style.transform = "translateX(400px)";
}

function SoundOrder() {
    var audio = new Audio("/Sound/SoundOrder.mp3");
    audio.addEventListener('ended', function () {
        vol.innerText = "";
    });
    audio.play();
}