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
    
    public partial class company
    {
        public int id { get; set; }
        public string owner { get; set; }
        public string name { get; set; }
        public string name_en { get; set; }
        public string des { get; set; }
        public string province { get; set; }
        public string district { get; set; }
        public string street { get; set; }
        public string business { get; set; }
        public string phone { get; set; }
        public string tax { get; set; }
        public string address { get; set; }
        public string address_owner { get; set; }
        public Nullable<System.DateTime> date_startup { get; set; }
        public string tax_office { get; set; }
        public Nullable<int> employees { get; set; }
    }
}
