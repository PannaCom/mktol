//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MarketingOnline.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class news
    {
        public int id { get; set; }
        public string title { get; set; }
        public string image { get; set; }
        public string des { get; set; }
        public string fullcontent { get; set; }
        public Nullable<System.DateTime> datetime { get; set; }
        public string keywords { get; set; }
        public string cat { get; set; }
    }
}
