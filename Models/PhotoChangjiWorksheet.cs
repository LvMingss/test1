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
    
    public partial class PhotoChangjiWorksheet
    {
        public int ID { get; set; }
        public Nullable<int> workSheetNo { get; set; }
        public string projectName { get; set; }
        public Nullable<int> seqNo { get; set; }
        public string projectContent { get; set; }
        public Nullable<System.DateTime> shootTime { get; set; }
        public string projectLocation { get; set; }
        public string timeCode1 { get; set; }
        public string timeCode2 { get; set; }
        public string shooter { get; set; }
        public Nullable<int> listCnt { get; set; }
        public string jingbie { get; set; }
        public string jianliUnit { get; set; }
        public string jianliPresent { get; set; }
        public string shootLocation { get; set; }
        public string photoArchiveNo { get; set; }
    }
}
