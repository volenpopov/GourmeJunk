let orderDetailsURL = '/Order/GetOrderDetails';
let orderStatusURL = '/Order/GetOrderStatus';

$(function () {
    $(".anchorDetail").click(function () {
        let $buttonClicked = $(this);
        let id = $buttonClicked.attr('data-id');

        $.ajax({
            type: "GET",
            url: orderDetailsURL,
            contentType: "application/json; charset=utf-8",
            data: { "Id": id },
            cache: false,
            success: function (data) {
                $('#orderDetailsContent').html(data);
                $('#orderDetailsModal').modal('show');
            },
            error: function () {
                alert("Dynamic content load failed");
            }
        });

    });

    $(".btnOrderStatus").click(function () {
        let $buttonClicked = $(this);
        let id = $buttonClicked.attr('data-id');

        $.ajax({
            type: "GET",
            url: orderStatusURL,
            contentType: "application/json; charset=utf-8",
            data: { "Id": id },
            cache: false,
            success: function (data) {
                $('#orderStatusContent').html(data);
                $('#orderStatusModal').modal('show');

            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    });
});       