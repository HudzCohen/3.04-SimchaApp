$(() => {
    let modal = new bootstrap.Modal($(".new-contrib")[0]);

    $("#new-contributor").on('click', function () {
        modal.show();
    });

    $(".cancel, .btn-close").on('click', function () {
        clearModal();
        modal.hide();
    });

    $(".edit-contrib").on('click', function () {

        $(".modal-title, contrib").text("Edit Contributor");
        const id = $(this).data('id');
        $("form").append(`<input type="hidden" id="edit-id" name="id" value="${id}"/>`)
        $("#contrib-first-name").val($(this).data("first-name"));
        $("#contrib-last-name").val($(this).data("last-name"));
        $("#contrib-cell").val($(this).data("cell"));
        $("#contributor_always_include").prop('checked', $(this).data("always-include") === 'True')
        $("#contrib-date").hide();
        $("#initialDepositDiv").hide();
        $("form").attr('action', "/contributor/editContributor");
        modal.show();
    });

    function clearModal() {
        $(".modal-title, contrib").text("New Contributor");
        $("#contrib-first-name").val("");
        $("#contrib-last-name").val("");
        $("#contrib-cell").val("");
        $("#contrib-date").show();
        $("#initialDepositDiv").show();
        $("form").attr('action', "/contributor/new");
    }

    


});