$(function () {
    //submit request
    $(document).on('click', '#saveBtn_Groups', function () {
        $(this).prop("disabled", true).text('Working...');

        var form = $('#groupsDetailsForm');

        if (form.valid()) {
            form.submit();
        }
        else {
            $(this).prop("disabled", false).text('Save');
        }
    });
});