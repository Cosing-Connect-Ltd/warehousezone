$(function () {
    //search timelog
    $(document).on('click', '#searchBtn_TimeLog', function () {
        var storeId = $('#StoresList').val();
        var weekNumber = $('#WeekDaysList').val();
        var yearNumber = $('#YearsList').val();

        //redirect
        window.location.replace("/timelog/tindex/?id=" + storeId + "&weekNumber=" + weekNumber + "&YearsList=" + yearNumber);
    });

    $(document).on('click', '#YearsList', function () {
                
            $.ajax({
                type: "GET",
                url: "/TimeLog/GetWeeksforYear",
                data: { year: $('#YearsList').val() },
                dataType: 'json',
                success: function (data) {
                    $('#WeekDaysList').empty();

                    if (data.length > 0) {
                        $.each(data, function (i, item) {
                            $('#WeekDaysList').append($('<option></option>').val(item.Value).html(item.Text));
                        });
                    }
                }
            });
        
    });





});