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
    
    public partial class vw_indivCollStatis
    {
        public long paperProjectSeqNo { get; set; }
        public long projectNo { get; set; }
        public string projectName { get; set; }
        public Nullable<long> originalInchCount { get; set; }
        public Nullable<decimal> originalMoneyAmount { get; set; }
        public Nullable<long> copyInchCount { get; set; }
        public Nullable<decimal> copyMoneyCount { get; set; }
        public string InchCountDetail { get; set; }
        public string collator { get; set; }
        public Nullable<System.DateTime> dateArchive { get; set; }
    }
}