using AutoMapper;
using Ganedata.Core.Services.Feedbacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;

using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.Feedback;

namespace WMS.Controllers.WebAPI
{
    public class ApiFeedbackController : BaseApiController
    {
        readonly IFeedbackService _feedbackServices;
        public ApiFeedbackController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService,
            IProductServices productServices, IUserService userService,IFeedbackService feedbackService, IMapper mapper)
              : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _feedbackServices = feedbackService;
        }


        public IHttpActionResult GetFeedback(int tenantId)
        {
            var feedback = _feedbackServices.GetAll(tenantId);
            if(feedback.Count() > 0)
            {
                return Ok(feedback);
            }
            else
            {
                return Ok("Not Data found");
            }
        }

        [HttpPost]
        public IHttpActionResult PostFeedback(Feedback feedback)
        {
            var res = _feedbackServices.Create(feedback);

            if (!res)
            {
                return Ok("Added Feedback");
            }
            else
            {
                return BadRequest("Unable to save records");
            }
        }
    }
}
