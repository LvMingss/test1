//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace urban_archive.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class VideoCassetteList
    {
        public int ID { get; set; }
        public string ProjectIDS { get; set; }
        public string videoMinutesCount { get; set; }
        public string opticalDiskNo { get; set; }
        public Nullable<int> diskMinutesCount { get; set; }
        public Nullable<System.DateTime> startShootingTime { get; set; }
        public Nullable<System.DateTime> endShootingTime { get; set; }
        public string shootingPerson { get; set; }
        public Nullable<System.DateTime> shootingDate { get; set; }
        public Nullable<System.DateTime> filingTime { get; set; }
        public string filingDesc { get; set; }
        public string filingPeople { get; set; }
        public string checker { get; set; }
        public Nullable<System.DateTime> checkTime { get; set; }
        public string bianzhiUnit { get; set; }
        public Nullable<System.DateTime> bianzhiDateStart { get; set; }
        public Nullable<System.DateTime> bianzhiTime { get; set; }
        public Nullable<System.DateTime> bianzhiDateEnd { get; set; }
        public string videoContent { get; set; }
    }
}
