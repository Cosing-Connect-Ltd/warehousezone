using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ganedata.Core.Entities.Domain.Feedback;

namespace Ganedata.Core.Services.Feedbacks
{
    public interface IFeedbackService
    {
        IEnumerable<Feedback> GetAll(int tenantId);
        bool Create(Feedback feedback);
    }
}