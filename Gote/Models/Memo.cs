using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Gote.Models
{
    /// <summary>
    /// メモモデル
    /// </summary>
    [Table("memos")]
    internal sealed class Memo : BaseModel
    {
        /// <summary>
        /// メモID
        /// </summary>
        [PrimaryKey("memo_id")]
        public Guid MemoId { get; set; }
        /// <summary>
        /// ユーザID
        /// </summary>
        [Column("user_id")]
        public Guid UserId { get; set; }
        /// <summary>
        /// カテゴリID
        /// </summary>
        [Column("category_id")]
        public Guid CategoryId { get; set; }
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
