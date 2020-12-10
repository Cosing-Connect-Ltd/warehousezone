﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain.Feedback
{
    [Serializable]
    public partial class Feedback
    {
        [Key]
        public int Id { get; set; }
        public int AccountID { get; set; }
        public int OrderID { get; set; }
        public string ServiceRate { get; set; }
        public string CustomerName{ get; set; }
        public string FoodRate { get; set; }
        public string AppRate { get; set; }
        public string FeedbackMessge { get; set; }
        public int TenantId { get; set; }
        public bool? IsDeleted { get; set; }
        public virtual Account Customer { get; set; }
        public virtual Order Order { get; set; }
    }
}