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
    
    public partial class VProjectGeos
    {
        public long id { get; set; }
        public Nullable<long> projectID { get; set; }
        public string projectName { get; set; }
        public System.Data.Entity.Spatial.DbGeography ploygon { get; set; }
        public string model { get; set; }
        public Nullable<long> paperProjectSeqNo { get; set; }
    }
}
