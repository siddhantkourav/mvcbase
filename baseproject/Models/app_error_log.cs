//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace baseproject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class app_error_log
    {
        public int error_log_id { get; set; }
        public string user_id { get; set; }
        public string error_message { get; set; }
        public System.DateTime created_date { get; set; }
        public bool is_active { get; set; }
    }
}
