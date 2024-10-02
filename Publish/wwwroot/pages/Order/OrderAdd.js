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

    let template = ` <td class="text-center">${count}</td>#
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
    if (Confirm()) {
        table.row($(this).parents('tr')).remove().draw();
    }
})

function Confirm() {
    let text = "Are you sure for delete this data ?";
    return confirm(text);
}

function validate(tr) {
    $.each(tr, function (key, val) {
        let value = $(val).val();
        if (!value || value == 0) {
            $(val).val(1);
        }

        if (parseFloat(value).toFixed(2) > 99999999999999999999999999999999999999) {
            alert("Maximum Price is 99999999999999999999999999999999999999");
            return false;
        }
    })
}


function CalculateRow(tr) {
    validate(tr.find(".sum"))

    const qty = parseInt(tr.find(".qty").val());
    const price = parseFloat(tr.find(".price").val()).toFixed(2);

    tr.find(".total-row").val(FormatNumber(qty * price));
    tr.find(".price").val(price)
    tr.find(".total-row").attr("total", qty * price);
    CalculateFooter();
}

function CalculateFooter() {
    let tr = $("#tbl-item tbody tr");
    let qty = 0;
    let total = 0.00;

    // console.log(tr)

    $.each(tr, function (key, val) {
        if ($(val).find(".qty").val()) {
            qty += parseInt($(val).find(".qty").val());
        }

        if ($(val).find(".total-row")) {
            // console.log($(val).find(".total-row")[0])

            total += parseFloat($(val).find(".total-row").attr("total")).toFixed(2);
        }

    })

    $("#tbl-item").find(".tf-qty").html(qty)
    $("#tbl-item").find(".tf-total").html(FormatNumber(total));
}

function FormatNumber(Number) {
    return new Intl.NumberFormat('en-DE', { minimumFractionDigits: 2 }).format(parseFloat(Number).toFixed(2));
}


$(".submit-data").click(function () {
    let soData = objectifyForm($("#frm-head").serializeArray())
    let item = [];

    $.each($("#tbl-item tbody tr"), function (key, val) {
        let dtItem = {
            // SoItemId: $(val).find(".item-id").val(),
            // SoOrderId: $("#SoOrderId").val(),
            ItemName: $(val).find("#item-id").val(),
            ItemName: $(val).find(".name").val(),
            Quantity: $(val).find(".qty").val(),
            Price: parseFloat($(val).find(".price").val()).toFixed(2)
        };

        item.push(dtItem);
    })

    soData["ListItem"] = item;

    $.ajax({
        url: baseURI + "/order-submit",
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

function objectifyForm(formArray) {
    var returnArray = {};
    for (var i = 0; i < formArray.length; i++) {
        returnArray[formArray[i]['name']] = formArray[i]['value'];
    }
    return returnArray;
}

$('#ComCutomerId').select2({
    width: '100%',
    placeholder: 'Select One',
    minimumInputLength: 0
})