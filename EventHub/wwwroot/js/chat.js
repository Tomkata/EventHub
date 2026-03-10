"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (message) {
    const isMine = message.senderId === currentUserId;
    const div = document.getElementById("messages");
    const date = new Date(message.createdAt);

    div.innerHTML += `
    <div class="d-flex flex-column ${isMine ? 'align-items-end' : 'align-items-start'}">
        <div class="message-bubble ${isMine ? 'message-mine' : 'message-theirs'}">
            ${message.content}
            <div class="message-time">${date.toLocaleTimeString('bg-BG', { hour: '2-digit', minute: '2-digit' })}</div>
        </div>
    </div>`;

    div.scrollTop = div.scrollHeight;
});

connection.start().then(async function () {
    document.getElementById("sendButton").disabled = false;
    await connection.invoke("JoinConversation", conversationId);
}).catch(function (err) {
    console.error(err.toString());
});

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.message-time[data-utc]').forEach(el => {
        const date = new Date(el.dataset.utc);
        el.textContent = date.toLocaleTimeString('bg-BG', { hour: '2-digit', minute: '2-digit' });
    });

    document.querySelectorAll('small[data-utc]').forEach(el => {
        const date = new Date(el.dataset.utc);
        el.textContent = date.toLocaleTimeString('bg-BG', { hour: '2-digit', minute: '2-digit' });
    });
});

async function sendMessage() {
    const input = document.getElementById("messageInput");
    const message = input.value.trim();

    if (!message) return;

    try {
        await connection.invoke("SendMessage", conversationId, message);
        input.value = "";
    } catch (err) {
        console.error("SendMessage error:", err);
    }
}

document.getElementById("messageInput").addEventListener("keydown", function (e) {
    if (e.key === "Enter") sendMessage();
});