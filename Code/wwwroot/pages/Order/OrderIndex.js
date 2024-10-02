
$(document).ready(function () {
    LoadDatatTable()
})

$(".btn-exp").click(function () {
    $(".buttons-excel").trigger('click')
})


$("#tbl-Order").on("click", ".btn-delete-order", function () {

    let id = $(this).attr("orderid");

    if (Confirm()) {

        $.ajax({
            url: baseURI + "/order-delete?id=" + id,
            type: 'post',
            dataType: 'json',
            data: {}
        }).then(function (response) {
            if (response.isSuccess) {
                $("#div-table").load(baseURI + `?date=${$("#Date").val()}&keyword=${$("#Keyword").val()}` + " #div-reload", function () {
                    LoadDatatTable();
                });
            } else {
                $("#message").addClass("alert alert-danger").html(response.message);
            }
        });
    }
})

$(".btn-filter").on("click", function () {

    $("#div-table").load(baseURI + `?date=${$("#Date").val()}&keyword=${$("#Keyword").val()}` + " #div-reload", function () {
        LoadDatatTable();
    });
})

function LoadDatatTable() {
    $("#tbl-Order").DataTable({
        serverSide: false,
        autoWidth: false,
        dataLength: 10,
        destroy: true,
        dom: 'Blfrtip',
        buttons: [
            {
                extend: 'excel',
                className: 'd-none',
                exportOptions: {
                    columns: [0, 2, 3, 4]
                },
            },
        ],
        drawCallback: function (setting) {
            $(".dt-length").find("label").remove();
        }
    })
}