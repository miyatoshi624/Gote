using Gote.Models;

namespace Gote.Services
{
    /// <summary>
    /// Supabaseサービス用インターフェース
    /// </summary>
    internal interface ISupabaseService
    {
        /// <summary>
        /// サインイン
        /// </summary>
        /// <param name="password">パスワード</param>
        /// <returns>ユーザ名</returns>
        Task<string?> SignInAsync(string password);
        /// <summary>
        /// サインアウト
        /// </summary>
        /// <returns></returns>
        Task SignOutAsync();
        /// <summary>
        /// セッションが有効かどうか
        /// </summary>
        /// <returns></returns>
        bool IsSessionValid();
        /// <summary>
        /// セッションの有効期限を取得
        /// </summary>
        /// <returns></returns>
        DateTime? GetSessionExpiry();
        /// <summary>
        /// カテゴリ一覧を取得
        /// </summary>
        /// <returns></returns>
        Task<List<Category>> GetCategoriesAsync();
        /// <summary>
        /// カテゴリを取得
        /// </summary>
        /// <param name="categoryId">カテゴリID</param>
        /// <returns></returns>
        Task<Category?> GetCategoryAsync(int categoryId);
        /// <summary>
        /// カテゴリを作成
        /// </summary>
        /// <param name="category">カテゴリ</param>
        /// <returns></returns>
        Task CreateCategoryAsync(Category category);
        /// <summary>
        /// カテゴリを更新
        /// </summary>
        /// <param name="category">カテゴリ</param>
        /// <returns></returns>
        Task UpdateCategoryAsync(Category category);
        /// <summary>
        /// カテゴリを削除
        /// </summary>
        /// <param name="categoryId">カテゴリID</param>
        /// <returns></returns>
        Task DeleteCategoryAsync(int categoryId);
        /// <summary>
        /// メモ一覧を取得
        /// </summary>
        /// <param name="categoryId">カテゴリID</param>
        /// <returns></returns>
        Task<List<Memo>> GetMemosAsync(int categoryId);
        /// <summary>
        /// 最新のメモ一覧を取得
        /// </summary>
        /// <param name="count">取得する件数</param>
        /// <returns></returns>
        Task<List<Memo>> GetLatestMemosAsync(int count);
        /// <summary>
        /// メモを取得
        /// </summary>
        /// <param name="memoId">メモID</param>
        /// <returns></returns>
        Task<Memo?> GetMemoAsync(int memoId);
        /// <summary>
        /// メモを作成
        /// </summary>
        /// <param name="memo">メモ</param>
        /// <returns></returns>
        Task CreateMemoAsync(Memo memo);
        /// <summary>
        /// メモを更新
        /// </summary>
        /// <param name="memo"></param>
        /// <returns></returns>
        Task UpdateMemoAsync(Memo memo);
        /// <summary>
        /// メモを削除
        /// </summary>
        /// <param name="memoId">メモID</param>
        /// <returns></returns>
        Task DeleteMemoAsync(int memoId);
    }
}
