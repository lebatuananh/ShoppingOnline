"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveMessage", (message) => {
    var template = $('#announcement-template').html();
var html = Mustache.render(template, {
    Content: message.content,
    Id: message.id,
    Title: message.title,
    FullName: message.fullName,
    Avatar: message.avatar
});
$('#annoncementList').prepend(html);

var totalAnnounce = parseInt($('#totalAnnouncement').text());

if (isNaN(totalAnnounce)) {
    totalAnnounce = 1;
} else {
    totalAnnounce = parseInt($('#totalAnnouncement').text()) + 1;

}
$('#announcementArea').show();

$('#totalAnnouncement').text(totalAnnounce);
});