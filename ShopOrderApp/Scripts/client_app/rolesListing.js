$(function () {
    //submit request
    $(document).on('click', '#saveBtn_Roles', function () {
        $(this).prop("disabled", true).text('Working...');

        var form = $('#rolesDetailsForm');

        if (form.valid()) {
            form.submit();
        }
        else {
            $(this).prop("disabled", false).text('Save');
        }
    });
});