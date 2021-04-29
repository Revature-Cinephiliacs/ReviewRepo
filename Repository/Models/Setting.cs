using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class Setting
    {
        public Guid SettingId { get; set; }
        public string Setting1 { get; set; }
        public int? IntValue { get; set; }
        public string StringValue { get; set; }
    }
}
