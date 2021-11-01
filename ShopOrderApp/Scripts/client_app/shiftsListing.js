$(function () {
    //submit shifts
    $(document).on('click', '#saveBtn', function () {
        $("#errorShifts").addClass("visibility-hidden");
        $("#errorShifts").html("");
        if ($("input[name=RepeatShifts]").is(':checked') && $("#ExpectedHours").val() !== "00:00:00") {

            $(this).prop("disabled", true).text('Working...');

            var form = $('#shiftsDetailsForm');

            if (form.valid()) {
                form.submit();
            }
            else {
                $("#errorShifts").removeClass("visibility-hidden");
                $(this).prop("disabled", false).text('Save');
            }

        }
        else {
            $("#errorShifts").removeClass("visibility-hidden");
            if (!$("input[name=RepeatShifts]").is(':checked')) {
                $("#errorShifts").append("<ul><li>Select at least one Repeat Shifts.</li></ul>");
            }
            if ($("#ExpectedHours").val() === "00:00:00") {
                $("#errorShifts").append("<ul><li>Shift time should be greater than zero</li></ul>");
            }

        }
    });

    $("#TimeBreaks").on("change", function (e) {
        SetExpectedHours(e);
    });
});

function SetExpectedHours(e) {
    //var today = moment().format('MMM DD, YYYY');
    var startTime = StartTime.GetDate();
    var endTime = EndTime.GetDate();

    //var mSecStartTime = (today + ' ' + startTime);
    //var mSecEndTime = (today + ' ' + endTime);

    var mSecParseStartTime = Date.parse(startTime);
    var mSecParseEndTime = Date.parse(endTime);

    var timeHour = "";
    var timeMinutes = "";

    var totalBreak = $("#TimeBreaks option:selected").val();
    timeHour = totalBreak.substring(0, 2);
    timeMinutes = totalBreak.substring(3, 5);

    totalBreakMilliSeconds = (timeHour * 3600 * 1000) + (timeMinutes * 60 * 1000);

    var diff = new Date(mSecParseEndTime) - new Date(mSecParseStartTime);

    totalDiff = diff - totalBreakMilliSeconds;

    if (totalDiff <= 0) {
        alert("Shift time can’t be zero or negative");
        $('#ExpectedHours').val("00:00"); //<<---this input is hidden
        $('#ExpectedHoursValue').text("00:00");
    }
    else {

        var seconds = Math.floor(diff / 1000); //ignore any left over units smaller than a second
        var minutes = Math.floor(seconds / 60);
        seconds = seconds % 60;
        var hours = Math.floor(minutes / 60);
        minutes = minutes % 60;

        var expectedHours = hours + ":" + minutes;

        var sub = moment(expectedHours, 'HH:mm').subtract(timeHour, 'hours').subtract(timeMinutes, 'minutes').format('HH:mm');

        $('#ExpectedHours').val(sub); //<<---this input is hidden
        $('#ExpectedHoursValue').text(sub);
    }

}