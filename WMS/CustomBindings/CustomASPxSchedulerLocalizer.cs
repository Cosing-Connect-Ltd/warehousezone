using DevExpress.Utils.Localization.Internal;
using DevExpress.Web.ASPxScheduler.Localization;
using System.Collections;

namespace WMS.CustomBindings
{
    public class CustomASPxSchedulerLocalizer : ASPxSchedulerLocalizer
    {
        public static void Activate()
        {
            CustomASPxSchedulerLocalizer localizer = new CustomASPxSchedulerLocalizer();
            DefaultActiveLocalizerProvider<ASPxSchedulerStringId> provider = new DefaultActiveLocalizerProvider<ASPxSchedulerStringId>(localizer);
            CustomASPxSchedulerLocalizer.SetActiveLocalizerProvider(provider);
        }
        public override string GetLocalizedString(ASPxSchedulerStringId id)
        {
            if (id == ASPxSchedulerStringId.Form_ButtonDelete)
                return "Cancel Appointment";
            if (id == ASPxSchedulerStringId.Form_Label)
                return "Priority";
            return base.GetLocalizedString(id);
        }
    }

    public class SchedulerDataObject
    {
        public IEnumerable Resources { get; set; }
        public DevExpress.Web.Mvc.FetchAppointmentsMethod FetchAppointments { get; set; }
    }
}