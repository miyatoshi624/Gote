using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Gote.Models
{
    /// <summary>
    /// カテゴリモデル
    /// </summary>
    [Table("categories")]
    internal sealed class Category : BaseModel
    {
        /// <summary>
        /// カテゴリID
        /// </summary>
        [PrimaryKey("category_id")]
        public Guid CategoryId { get; set; }
        /// <summary>
        /// ユーザID
        /// </summary>
        [Column("user_id")]
        public Guid UserId { get; set; }
        /// <summary>
        /// カテゴリ名
        /// </summary>
        [Column("name")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 説明
        /// </summary>
        [Column("description")]
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// 外部参照されているかどうか
        /// </summary>
        [JsonIgnore]
        public bool IsReferenced { get; set; } = false;
    }
}
