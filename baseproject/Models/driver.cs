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
    
    public partial class driver
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public driver()
        {
            this.trips = new HashSet<trip>();
        }
    
        public int driver_id { get; set; }
        public string driver_name { get; set; }
        public string driver_contact_no { get; set; }
        public bool is_active { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<trip> trips { get; set; }
    }
}
