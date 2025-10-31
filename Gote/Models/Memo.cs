using Supabase.Postgrest.Attributes;

namespace Gote.Models
{
    /// <summary>
    /// メモモデル
    /// </summary>
    [Table("memos")]
    internal sealed class Memo
    {
        /// <summary>
        /// メモID
        /// </summary>
        [PrimaryKey("id")]
        public int Id { get; set; }
        /// <summary>
        /// カテゴリID
        /// </summary>
        [Reference(typeof(Category), ReferenceAttribute.JoinType.Inner, columnName: "category_id")]
        public int CategoryId { get; set; }
        /// <summary>
        /// タイトル
        /// </summary>
        [Column("title")]
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// 内容
        /// </summary>
        [Column("content")]
        public string Content { get; set; } = string.Empty;
        /// <summary>
        /// 作成日時
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 更新日時
        /// </summary>
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
