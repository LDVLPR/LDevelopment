var modal = $('#modal');

$(window).on('load', function () {
    modal.fadeOut(1000);
});

$(document).ajaxStart(function () {
    modal.show();
})
.ajaxStop(function () {
    modal.fadeOut(1000);
});