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
    
    public partial class BindUserAndImageDown
    {
        public int ID { get; set; }
        public string userID { get; set; }
        public string ImageAddress { get; set; }
        public Nullable<System.DateTime> imageTime { get; set; }
        public string imageSize { get; set; }
        public string isWordOrPic { get; set; }
        public string archivesNo { get; set; }
        public Nullable<long> realuserID { get; set; }
        public Nullable<int> recordID { get; set; }
        public string yeci { get; set; }
    }
}
