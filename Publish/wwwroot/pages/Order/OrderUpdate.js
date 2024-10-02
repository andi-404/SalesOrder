var baseURI = window.location.origin;
var dataItem = [];
var table = $("#tbl-item").DataTable({
    serverSide: false,
    autoWidth: false,
    ordering: false,
    pageLength: 5,
    drawCallback: function (setting) {
        $(".dataTables_length").addClass("d-none")
        $(".dataTables_filter").addClass("d-none")
    }
})

$(".btn-add-item").click(function () {
    let count = table.rows().count() + 1;

    let template = ` <td>${count}</td>#
                               <td><div class="text-center">
                                <button class="btn btn-primary btn-sm d-none edit-item"><i class="bi bi-pencil-square"></i></button>
                                <button class="btn btn-success btn-sm save-item"><i class="bi bi-check-square"></i></button>
                                <button class="btn btn-danger btn-sm d-none delete-item"><i class="bi bi-trash"></i></button>
                            </div></td>#
                                    <td><input type="hidden" class="item-id"/><input type="text" class="form-control name" value="a" /></td>#
                                    <td><input type="number" min="1" class="form-control calculate-row qty sum" value="1" /></td>#
                                    <td><input type="number" min="1" class="form-control calculate-row price sum" value="1" /></td>#
                                    <td><input type="text" class="form-control calculate-row total-row" readOnly total="1" value="1,00"/></td>`;

    table.row.add(template.split("#")).draw(false);
    CalculateFooter();
});

$("#tbl-item").on("change", ".calculate-row", function () {
    CalculateRow($(this).closest("tr"))
})

$("#tbl-item").on("click", ".save-item", function () {
    let tr = $(this).closest("tr");
    CalculateRow(tr)
    $(this).addClass("d-none");
    tr.find(".delete-item").removeClass("d-none")
    tr.find(".edit-item").removeClass("d-none")

    $.each(tr.children(), function (key, val) {
        if ($(val).find(".form-control").length > 0) {
            let elm = $(val).find(".form-control")[0]
            let value;

            if ($(val).find(".price").length > 0) {
                value = FormatNumber($(elm).val());
            } else {
                value = $(elm).val()
            }

            if ($(val).find("label").length > 0) {
                $(val).find("label").html(value)
                $(val).find("label").removeClass("d-none");
            } else {
                let a = `<label class="text-center">${value}</label>`

                $(val).addClass("text-center").append(a)
            }

            $(val).find(".form-control").addClass("d-none")

        }
    })
})

$("#tbl-item").on("click", ".edit-item", function () {
    let tr = $(this).closest("tr");

    $(this).addClass("d-none");
    tr.find(".delete-item").addClass("d-none")
    tr.find(".save-item").removeClass("d-none")

    $.each(tr.children(), function (key, val) {
        if ($(val).find(".form-control").length > 0) {
            $(val).find(".form-control").removeClass("d-none")
            $(val).find(".form-control").next().addClass("d-none")
        }
    })
})

$("#tbl-item").on("click", ".delete-item", function () {

    let id = $(this).closest("tr").find(".item-id").val();

    if (Confirm()) {

        $.ajax({
            url: baseURI + "/order-delete-item?id=" + id ,
            type: 'post',
            dataType: 'json',
            data: {}
        }).then(function (response) {
            if (response.isSuccess) {
                location.reload();
            } else {
                $("#message").addClass("alert alert-danger").html(response.message);
            }
        });
    }
})



$(".submit-data").click(function () {
    let soData = objectifyForm($("#frm-head").serializeArray())
    let item = [];

    $.each($("#tbl-item tbody tr"), function (key, val) {
        let dtItem = {
             SoItemId: $(val).find(".item-id").val(),
             SoOrderId: $("#SoOrderId").val(),
            ItemName: $(val).find("#item-id").val(),
            ItemName: $(val).find(".name").val(),
            Quantity: $(val).find(".qty").val(),
            Price: parseFloat($(val).find(".price").val()).toFixed(2)
        };

        item.push(dtItem);
    })

    soData["ListItem"] = item;

    $.ajax({
        url: baseURI + "/order-update",
        type: 'post',
        dataType: 'json',
        data: soData
    }).then(function (response) {
        if (response.isSuccess) {
            location.href = baseURI;
        } else {
            $("#message").addClass("alert alert-danger").html(response.message);
        }
    });
});

$('#ComCutomerId').select2({
    width: '100%',
    placeholder: 'Select One',
    minimumInputLength: 0
})