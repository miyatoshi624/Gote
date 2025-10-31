using Supabase.Postgrest.Attributes;

namespace Gote.Models
{
    /// <summary>
    /// カテゴリモデル
    /// </summary>
    [Table("categories")]
    internal sealed class Category
    {
        /// <summary>
        /// カテゴリID
        /// </summary>
        [PrimaryKey("id")]
        public int Id { get; set; }
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
        public bool IsReferenced { get; set; } = false;
    }
}
