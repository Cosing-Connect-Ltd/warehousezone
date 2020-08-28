using DevExpress.Web.Mvc;
using DevExpress.XtraReports.Security;
using Elmah.Contrib.WebApi;
using Ganedata.Core.Entities.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StackExchange.Profiling;
using StackExchange.Profiling.EntityFramework6;
using StackExchange.Profiling.Mvc;
using System;
using System.Net;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WarehouseZone;
using WMS.App_Start;
using WMS.CustomBindings;

namespace WMS
{
    public class MvcApplication : System.Web.HttpApplication
    {


        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;

            DevExtremeBundleConfig.RegisterBundles(BundleTable.Bundles);
            DevExpress.Data.Helpers.ServerModeCore.DefaultForceCaseInsensitiveForAnySource = true;
            DevExpress.XtraReports.Web.ASPxReportDesigner.StaticInitialize();
            ScriptPermissionManager.GlobalInstance = new ScriptPermissionManager(ExecutionMode.Unrestricted);

            //Elmah for Web API errors logging
            GlobalConfiguration.Configuration.Filters.Add(new ElmahHandleErrorApiAttribute());

            // Custom Filter / Attribute to override action execution methods
            //GlobalFilters.Filters.Add(new LogAttribute());

            if (GaneStaticAppExtensions.MiniProfilerEnabled)
            {
                // mini profiler
                MiniProfilerEF6.Initialize();
            }

            //Force application to use strong ssl protocol (TLS 1.2)
            if (GaneStaticAppExtensions.ForceTls12)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }

            GlobalFilters.Filters.Add(new StackExchange.Profiling.Mvc.ProfilingActionFilter());

            // remove unnecessary view engines
            ViewEngines.Engines.Clear();
            IViewEngine razorEngine = new ThemedRazorViewEngine() { FileExtensions = new string[] { "cshtml" } };
            ViewEngines.Engines.Add(razorEngine);

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //custom Json date formatter
            JsonMediaTypeFormatter jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;

            JsonSerializerSettings jSettings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                //Formatting = Formatting.Indented,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
                //NullValueHandling = NullValueHandling.Ignore
            };

            jSettings.Converters.Add(new MyDateTimeConvertor());
            jsonFormatter.SerializerSettings = jSettings;
            ModelBinders.Binders.DefaultBinder = new DevExpressEditorsBinder();

            // Custom Localizer for Dev Express Scheduler
            CustomASPxSchedulerLocalizer.Activate();

            MiniProfiler.Configure(new MiniProfilerOptions
            {
                // Sets up the route to use for MiniProfiler resources:
                // Here, ~/profiler is used for things like /profiler/mini-profiler-includes.js
                RouteBasePath = "~/profiler",

                // Example of using SQLite storage instead
                //Storage = new SqliteMiniProfilerStorage(ConnectionString),

                // Different RDBMS have different ways of declaring sql parameters - SQLite can understand inline sql parameters just fine.
                // By default, sql parameters will be displayed.
                //SqlFormatter = new StackExchange.Profiling.SqlFormatters.InlineFormatter(),

                // These settings are optional and all have defaults, any matching setting specified in .RenderIncludes() will
                // override the application-wide defaults specified here, for example if you had both:
                //    PopupRenderPosition = RenderPosition.Right;
                //    and in the page:
                //    @MiniProfiler.Current.RenderIncludes(position: RenderPosition.Left)
                // ...then the position would be on the left on that page, and on the right (the application default) for anywhere that doesn't
                // specified position in the .RenderIncludes() call.
                PopupRenderPosition = RenderPosition.BottomRight,  // defaults to left
                PopupMaxTracesToShow = 10,                   // defaults to 15

                // ResultsAuthorize (optional - open to all by default):
                // because profiler results can contain sensitive data (e.g. sql queries with parameter values displayed), we
                // can define a function that will authorize clients to see the JSON or full page results.
                // we use it on http://stackoverflow.com to check that the request cookies belong to a valid developer.
                ResultsAuthorize = request => request.IsLocal,

                // ResultsListAuthorize (optional - open to all by default)
                // the list of all sessions in the store is restricted by default, you must return true to allow it
                ResultsListAuthorize = request =>
                {
                    // you may implement this if you need to restrict visibility of profiling lists on a per request basis
                    return true; // all requests are legit in this example
                },

                // Stack trace settings
                StackMaxLength = 256, // default is 120 characters

                // (Optional) You can disable "Connection Open()", "Connection Close()" (and async variant) tracking.
                // (defaults to true, and connection opening/closing is tracked)
                TrackConnectionOpenClose = true
            }
               // Optional settings to control the stack trace output in the details pane, examples:
               .ExcludeType("SessionFactory")  // Ignore any class with the name of SessionFactory)
               .ExcludeAssembly("NHibernate")  // Ignore any assembly named NHibernate
               .ExcludeMethod("Flush")         // Ignore any method with the name of Flush
               .AddViewProfiling()              // Add MVC view profiling (you want this)
                                                // If using EntityFrameworkCore, here's where it'd go.
                                                // .AddEntityFramework()        // Extension method in the MiniProfiler.EntityFrameworkCore package
               );
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Headers.Remove("Server");

            if (GaneStaticAppExtensions.MiniProfilerEnabled)
            {
                // mini profiler
                MiniProfiler.StartNew();
            }
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            Exception ex = Server.GetLastError().GetBaseException();
            var errorMsg = ex.Message;
        }

        //redirection on access denied for elamh error log default route.
        protected void Application_EndRequest(Object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            if (context.Response.Status.Substring(0, 3).Equals("401"))
            {
                context.Response.ClearContent();
                context.Response.Write("<script language=\"javascript\">" +
                             "self.location='/error';</script>");

                // throw new HttpException(401, "Unauthorized access");
            }
            if (GaneStaticAppExtensions.MiniProfilerEnabled)
            {
                // mini profiler
                MiniProfiler.Current?.Stop();
            }

        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            DevExpressHelper.Theme = GaneStaticAppExtensions.DevexTheme;
        }
    }

    //override json datetime converter methods
    public class MyDateTimeConvertor : DateTimeConverterBase
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return null;
            }

            return DateTime.Parse(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
