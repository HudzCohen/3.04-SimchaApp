
$(() => {
    let modal = new bootstrap.Modal($(".deposit")[0]);

    $(".table-responsive").on('click', '.deposit-btn', function () {
        $("#contrib-first-name").val($(this).data("first-name"));
        let first = $(this).data("first-name");
        let last = $(this).data("last-name");
        $("#deposit-name").val(`${first} ${last}`);
        modal.show();
    })

    $(".cancel").on('click', function () {
        modal.hide();
    });

    $("#search").on('input', function () {
        search($("#search").val().toLowerCase());
    });

    $("#clear").on('click', function () {
        $("#search").val("");
        $("tr").show();
    });

    //$("#search").on('input', function () {
    //    const searchText = $("#search").val().toLowerCase();
    //    $("tr").each(function () {
    //        if ($(this).text.toLowerCase().contains(searchText)) {
    //            $(this).show();
    //        }
    //        else {
    //            $(this).hide();
    //        }
           
    //    });
    //});
    
   
    
});
