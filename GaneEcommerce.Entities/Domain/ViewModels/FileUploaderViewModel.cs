using System.Collections.Generic;
using System.Web;

namespace Ganedata.Core.Entities.Domain.ViewModels
{
    public class FileUploaderViewModel
    {
        public string BindingName { get; set; }
        public string ControllerName => HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
        public string DisplayName { get; set; }
        public string[] AllowedFileExtensions { get; set; }
        public List<string> UploadedFiles { get; set; }
        public bool IsMultiselect { get; set; }
        public bool? ShowUploadButton { get; set; }
        public bool? HasUploadedFile { get; set; }
    }
}