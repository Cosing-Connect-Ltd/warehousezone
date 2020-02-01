using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Web;
using DevExpress.Web.ASPxHtmlEditor;
using DevExpress.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WMS.Helpers.DevexHelpers
{
    public class FeaturesOptions : BaseOptions
        {
            [Display(Name = "AllowScripts")]
            public bool AllowScripts { get; set; }
            [Display(Name = "AllowIFrames")]
            public bool AllowIFrames { get; set; }
            [Display(Name = "AllowFormElements")]
            public bool AllowFormElements { get; set; }
            [Display(Name = "AllowIdAttributes")]
            public bool AllowIdAttributes { get; set; }
            [Display(Name = "AllowStyleAttributes")]
            public bool AllowStyleAttributes { get; set; }
            [Display(Name = "AllowCustomColorsInColorPickers")]
            public bool AllowCustomColorsInColorPickers { get; set; }
            [Display(Name = "UpdateDeprecatedElements")]
            public bool UpdateDeprecatedElements { get; set; }
            [Display(Name = "UpdateBoldItalic")]
            public bool UpdateBoldItalic { get; set; }
            [Display(Name = "ResourcePathMode")]
            public ResourcePathMode ResourcePathMode { get; set; }
            [Display(Name = "AllowEditFullDocument")]
            public bool AllowEditFullDocument { get; set; }
            [Display(Name = "EnterMode")]
            public HtmlEditorEnterMode EnterMode { get; set; }
            [Display(Name = "AllowContextMenu")]
            public DefaultBoolean AllowContextMenu { get; set; }
            [Display(Name = "AllowedDocumentType")]
            public AllowedDocumentType AllowedDocumentType { get; set; }
            [Display(Name = "AllowDesignView")]
            public bool AllowDesignView { get; set; }
            [Display(Name = "AllowHtmlView")]
            public bool AllowHtmlView { get; set; }
            [Display(Name = "AllowPreview")]
            public bool AllowPreview { get; set; }

            public string Html { get; set; }

            public static FeaturesOptions CreateDefault()
            {
                FeaturesOptions result = new FeaturesOptions();
                result.UpdateDeprecatedElements = true;
                result.UpdateBoldItalic = true;
                result.EnterMode = HtmlEditorEnterMode.P;
                result.AllowContextMenu = DefaultBoolean.True;
                result.AllowDesignView = true;
                result.AllowHtmlView = true;
                result.AllowPreview = true;
                result.AllowIdAttributes = true;
                result.AllowStyleAttributes = true;
                result.ResourcePathMode = ResourcePathMode.NotSet;
                result.AllowCustomColorsInColorPickers = true;
                result.AllowEditFullDocument = false;
                result.AllowedDocumentType = AllowedDocumentType.XHTML;
                //result.Html = HtmlEditorFeaturesDemosHelper.GeHtmlContentByFileName("General.html");
                return result;
            }
        }

    public class HtmlEditorFeaturesDemosHelper
        {
            public const string ImagesDirectory = "~/Content/HtmlEditor/Images/";
            public const string ThumbnailsDirectory = "~/Content/HtmlEditor/Thumbnails/";
            public const string UploadDirectory = "~/Content/HtmlEditor/UploadFiles/";
            public const string HtmlLocation = "~/Content/HtmlEditor/DemoHtml/";

            public static readonly UploadControlValidationSettings ImageUploadValidationSettings = new UploadControlValidationSettings
            {
                AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".jpe", ".gif", ".png" },
                MaxFileSize = 4000000
            };

            static HtmlEditorFileSaveSettings fileSaveSettings;
            public static HtmlEditorFileSaveSettings FileSaveSettings
            {
                get
                {
                    if (fileSaveSettings == null)
                    {
                        fileSaveSettings = new HtmlEditorFileSaveSettings();
                        fileSaveSettings.FileSystemSettings.UploadFolder = ImagesDirectory + "Upload/";
                    }
                    return fileSaveSettings;
                }
            }

            public static MVCxHtmlEditorImageSelectorSettings SetHtmlEditorImageSelectorSettings(MVCxHtmlEditorImageSelectorSettings settingsImageSelector)
            {
                settingsImageSelector.UploadCallbackRouteValues = new { Controller = "Features", Action = "FeaturesImageSelectorUpload" };

                settingsImageSelector.Enabled = true;
                settingsImageSelector.CommonSettings.RootFolder = ImagesDirectory;
                settingsImageSelector.CommonSettings.ThumbnailFolder = ThumbnailsDirectory;
                settingsImageSelector.CommonSettings.AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".jpe", ".gif", ".png" };
                settingsImageSelector.EditingSettings.AllowCreate = true;
                settingsImageSelector.EditingSettings.AllowDelete = true;
                settingsImageSelector.EditingSettings.AllowMove = true;
                settingsImageSelector.EditingSettings.AllowRename = true;
                settingsImageSelector.UploadSettings.Enabled = true;
                settingsImageSelector.FoldersSettings.ShowLockedFolderIcons = true;

                settingsImageSelector.PermissionSettings.AccessRules.Add(
                    new FileManagerFolderAccessRule
                    {
                        Path = "",
                        Upload = Rights.Deny
                    });
                return settingsImageSelector;
            }

            public static string GeHtmlContentByFileName(string fileName)
            {
                return System.IO.File.ReadAllText(System.Web.HttpContext.Current.Request.MapPath(string.Format("{0}{1}", HtmlLocation, fileName)));
            }
            public static string GeHtmlContentByFileName(string fileName, bool demoPageIsInRoot)
            {
                string result = GeHtmlContentByFileName(fileName);
                return demoPageIsInRoot ? result : result.Replace("Content/", "../Content/");
            }

            public static void SetupGlobalUploadBehaviour(HtmlEditorSettings settings)
            {
                settings.SettingsDialogs.InsertImageDialog.SettingsImageUpload.FileSystemSettings.Assign(HtmlEditorFeaturesDemosHelper.FileSaveSettings.FileSystemSettings);
                settings.SettingsDialogs.InsertImageDialog.SettingsImageUpload.ValidationSettings.Assign(HtmlEditorFeaturesDemosHelper.ImageUploadValidationSettings);

                long maxFileSize = 5 * 1024 * 1024;
                settings.SettingsDialogs.InsertImageDialog.SettingsImageUpload.ValidationSettings.MaxFileSize = maxFileSize;
                settings.SettingsDialogs.InsertImageDialog.SettingsImageSelector.UploadSettings.ValidationSettings.MaxFileSize = maxFileSize;
                settings.SettingsDialogs.InsertAudioDialog.SettingsAudioUpload.ValidationSettings.MaxFileSize = maxFileSize;
                settings.SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.UploadSettings.ValidationSettings.MaxFileSize = maxFileSize;
                settings.SettingsDialogs.InsertFlashDialog.SettingsFlashUpload.ValidationSettings.MaxFileSize = maxFileSize;
                settings.SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.UploadSettings.ValidationSettings.MaxFileSize = maxFileSize;
                settings.SettingsDialogs.InsertVideoDialog.SettingsVideoUpload.ValidationSettings.MaxFileSize = maxFileSize;
                settings.SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.UploadSettings.ValidationSettings.MaxFileSize = maxFileSize;
                settings.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.UploadSettings.ValidationSettings.MaxFileSize = maxFileSize;
                
            }
            
        }
    }
