#nullable enable
using System;

namespace MixyBoos.Api.Data.Models {
    /// <summary>
    /// Todo: Look into using this for multi-level chat
    /// https://github.com/efcore/EFCore.SqlServer.HierarchyId
    /// </summary>
    public class ShowChat : BaseEntity {
        public MixyBoosUser FromUser { get; set; }
        public MixyBoosUser? ToUser { get; set; }
        public DateTime DateSent { get; set; }
        public string Message { get; set; }
        public virtual LiveShow Show { get; set; }
    }
}
