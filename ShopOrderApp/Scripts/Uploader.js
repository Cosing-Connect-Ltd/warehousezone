function onFileUploadComplete(s, e) {
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



function onFileUploadStart(s, e) {
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
    var Maufacturer = $("#ProductManufacturer").val();
    var id = $("#Id").val();
    $('#dvbusy').show();
    $.ajax({
        type: "POST",
        url: "/Pallets/_RemoveProofOfDeliveryFile",
        data: {
            "filename": filename, "tenantDepartment": department, "TenantGroup": productGroup, "Maufacturer": Maufacturer, "Id": id
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
                $("#FileLength").val(false);
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
function _RemoveDefaultFile(filename) {
    _RemoveLogoFile(filename,"Default")
}
function _RemoveHoverFile(filename) {

    _RemoveLogoFile(filename, "Hover")
}

function _RemoveLogoFile(filename,loaderType)
{
    if (loaderType !== null && loaderType !== "" && loaderType !== undefined) {
        var navigationWebsite = true;
    }
    var websiteSlider = $("#websiteSlider").val();
    var tenantWebsite = $("#tenantWebsite").val();
    var websiteContent = $("#websiteContent").val();
    var productTag = $("#productTag").val();
    $.ajax({
        type: "POST",
        url: "/TenantWebsites/_RemoveLogoFile",
        data: {
            "filename": filename, "websiteSlider": websiteSlider, "tenantWebsite": tenantWebsite, "navigationWebsite": navigationWebsite, "websiteContent": websiteContent,
            "NavType": loaderType, "productTag": productTag
        },
        success: function (files) {
            $("#dvbusy").hide();
            if (navigationWebsite) {
                if (loaderType === "Hover") {
                    $('#dvfilesHover').empty();
                    $("#dvfilesHover").hide();
                    $("#dvfilesHover").append('<strong>Uploaded Files</strong>  ');
                    $("#FileLengthHover").val(false);
                }
                else {
                    $('#dvfiles').empty();
                    $("#dvfiles").hide();
                    $("#dvfiles").append('<strong>Uploaded Files</strong>  ');
                    $("#FileLength").val(false);

                }
                return;
            }
            if (files.files == null) {
                $('#dvfiles').empty();
                $("#dvfiles").hide();
                $("#dvfiles").append('<strong>Uploaded Files</strong>  ');
                $("#FileLength").val(false);
            }
            else {
                $("#FileLength").val(false);
                $('#dvfiles').empty();
                $("#dvfiles").append(' <strong>Uploaded Files</strong>  ');
                $.each(files.files, function (index, value) {

                    var Fileobj = { FileName: value };
                    var result = $("#uploaderTenantDept").tmpl(Fileobj);
                    $("#dvfiles").append(result);
                });

            }
        }
    });
}


