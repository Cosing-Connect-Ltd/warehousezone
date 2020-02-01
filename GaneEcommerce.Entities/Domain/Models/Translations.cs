using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class TextTranslations
    {
        [Key, Column(Order = 0)]
        [StringLength(10)]
        public string Culture { get; set; }
        [Key, Column(Order = 1)]
        public string Name { get; set; }
        public string Value { get; set; }
    }
}