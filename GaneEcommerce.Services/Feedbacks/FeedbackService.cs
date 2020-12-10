using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Compatibility;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.Feedback;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace Ganedata.Core.Services.Feedbacks
{
    public class FeedbackService: IFeedbackService
    {
        private readonly IApplicationContext _currentDbContext;

        //constructor
        public FeedbackService(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public bool Create(Feedback feedback)
        {
            _currentDbContext.Feedbacks.Add(feedback);
            int result = _currentDbContext.SaveChanges();

            return result > 0;
        }

        public IEnumerable<Feedback> GetAll(int tenantId)
        {
            return _currentDbContext.Feedbacks.Where(x => x.TenantId.Equals(tenantId) && x.IsDeleted != true);
        }
    }
}