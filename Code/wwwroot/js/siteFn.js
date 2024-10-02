function objectifyForm(formArray) {
    var returnArray = {};
    for (var i = 0; i < formArray.length; i++) {
        returnArray[formArray[i]['name']] = formArray[i]['value'];
    }
    return returnArray;
}

function Confirm() {
    let text = "Are you sure you want to delete this data?";
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
    let totalPrice = 0;

    $.each(tr, function (key, val) {
        if ($(val).find(".qty").val()) {
            qty += parseInt($(val).find(".qty").val());
        }

        if ($(val).find(".total-row").attr("total")) {
            // console.log($(val).find(".total-row")[0])

            totalPrice += parseFloat($(val).find(".total-row").attr("total"));
        }

    })

    $("#tbl-item").find(".tf-qty").html(qty)
    $("#tbl-item").find(".tf-total").html(FormatNumber(totalPrice));
}

function FormatNumber(Number) {
    return new Intl.NumberFormat('id-ID', { minimumFractionDigits: 2 }).format(parseFloat(Number).toFixed(2));
}
