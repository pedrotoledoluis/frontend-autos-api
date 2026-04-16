$(function () {
    $('.notification .delete').on('click', function () {
        $(this).parent().fadeOut(200, function () { $(this).remove(); });
    });

    $('form[data-confirm]').on('submit', function (e) {
        var message = $(this).data('confirm') || '¿Está seguro?';
        if (!confirm(message)) {
            e.preventDefault();
        }
    });
});
