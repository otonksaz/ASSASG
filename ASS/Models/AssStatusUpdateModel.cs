using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASS.Models
{
    public class AddStatus
    {
        public List<AddStatusDtl> dtl { get; set; }
    }

    public class AddStatusDtl
    {
        public int compDtlId { get; set; }
        public int statusId { get; set; }
        public string remark { get; set; }
    }
}