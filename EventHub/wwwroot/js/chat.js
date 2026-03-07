"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (message) {
    console.log("ReceiveMessage:", message);
    const div = document.getElementById("messages");
    div.innerHTML += `<p>${message.senderId}: ${message.content}</p>`;
});

connection.start().then(async function () {
    document.getElementById("sendButton").disabled = false;
    console.log("Joining conversation:", conversationId);
    await connection.invoke("JoinConversation", conversationId);
    console.log("Joined!");
}).catch(function (err) {
    console.error(err.toString());
});

async function sendMessage() {
    const input = document.getElementById("messageInput");
    const message = input.value;
    console.log("Sending:", conversationId, message);

    try {
        await connection.invoke("SendMessage", conversationId, message);
        input.value = "";
    } catch (err) {
        console.error("SendMessage error:", err);
    }
}