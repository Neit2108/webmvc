
$(document).ready(function () {

    $('#mobile-menu-toggle').on('click', function () {
        $('#sidebar').toggleClass('active');
        $(this).toggleClass('active');
    });

    // nut dang xuat
    $('#logout-btn').on('click', function (e) {
        e.preventDefault();
        $('#logoutForm').submit();
    });

    $('.modal').each(function () {
        const modalId = $(this).attr('id');
        console.log("Moving modal to body level: " + modalId);
        $(this).detach().appendTo('body');
    });


    // dong modal
    $(document).on('click', '.close', function () {
        console.log("Close button clicked");
        $(this).closest('.modal').hide();
    });

    // dong modal khi bam ra ngoai
    $(document).on('click', '.modal', function (event) {
        if ($(event.target).hasClass('modal')) {
            console.log("Clicking outside modal area");
            $(this).hide();
        }
    });

    $(document).on('click', '.modal-content', function (event) {
        event.stopPropagation();
    });

    $(document).keydown(function (event) {
        if (event.keyCode === 27) { // ESC key
            $('.modal:visible').hide();
        }
    });
});

// mo modal theo id
function openModal(modalId) {
    console.log("Opening modal: " + modalId);
    $('#' + modalId).css('display', 'block');
}

// dong 
function closeModal(modalId) {
    console.log("Closing modal: " + modalId);
    $('#' + modalId).hide();
}