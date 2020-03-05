function onFileUploadComplete(s, e) {

    debugger;
    if ($("#ProofOfDeliveryImageFilenames").length > 0) {
        $("#ProofOfDeliveryImageFilenames").val(e.callbackData);
        frmDispatchPallets.submit();
        //LoadingPanel.Hide();
        return;
    }

    if (e.callbackData) {

        $("#dvfiles").show();
        var fileData = e.callbackData.split('|');
        var Fileobj = { FileName: fileData };
        if (!$('#hdPFilesEdit').val())
            var result = $("#uploaderTemplate").tmpl(Fileobj);
        else
            var result = $("#uploaderDownloadTemplate").tmpl(Fileobj);

        $("#dvfiles").append(result);
        $('.se-pre-con').hide();
    }

}
     
 

function onFileUploadStart(s, e)
{   debugger;
    $('.se-pre-con').show();
}

function removeFile(filename) {
    $('#dvbusy').show();

    $.ajax({
        type: "POST",
        url: "/Products/_RemoveFile",
        data: 'filename=' + filename,
        success: function (files) {
            $("#dvbusy").hide();
            if (files.files == null) {
                $('#dvfiles').empty();
                $("#dvfiles").hide();
                $("#dvfiles").append(' <strong>Uploaded Files</strong>  ');
            }

            else {
                $('#dvfiles').empty();
                $("#dvfiles").append(' <strong>Uploaded Files</strong>  ');
                $.each(files.files, function (index, value) {
                    var Fileobj = { FileName: value };
                    if (!$('#hdPFilesEdit').val())
                        var result = $("#uploaderTemplate").tmpl(Fileobj);
                    else
                        var result = $("#uploaderDownloadTemplate").tmpl(Fileobj);
                    $("#dvfiles").append(result);
                });

            }
        }
    });

}

function downloadFile(filename) {

    window.open('/Products/Download?filename=' + filename);

}

var onFileImportComplete = function (s, e) {

    $('#dvbusy').hide();
    $("#data-import-results").html(e.callbackData);



};

var onFileImportStart = function (s, e) {

};

function _RemoveProofOfDeliveryFile(filename) {
    var department = $("#TenantDepartment").val();
    var productGroup = $("#productGroup").val();
    $('#dvbusy').show();
    $.ajax({
        type: "POST",
        url: "/Pallets/_RemoveProofOfDeliveryFile",
        data: {
            "filename": filename, "tenantDepartment": department, "TenantGroup": productGroup
        },
        success: function (files) {
            $("#dvbusy").hide();
            if (files.files == null) {
                $('#dvfiles').empty();
                $("#dvfiles").hide();
                $("#dvfiles").append('<strong>Uploaded Files</strong>  ');
                $("#FileLength").val(false);
            }

            else {
                $('#dvfiles').empty();
                $("#dvfiles").append(' <strong>Uploaded Files</strong>  ');
                $.each(files.files, function (index, value) {

                    var Fileobj = { FileName: value };
                    var result = $("#uploaderPalletTemplate").tmpl(Fileobj);
                    $("#dvfiles").append(result);
                });

            }
        }
    });

}


