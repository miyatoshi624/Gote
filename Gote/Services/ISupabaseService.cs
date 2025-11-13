using Gote.Models;

namespace Gote.Services
{
    /// <summary>
    /// Supabaseサービス用インターフェース
    /// </summary>
    internal interface ISupabaseService
    {
        /// <summary>
        /// 認証状態が変更されたときに発火するイベント
        /// </summary>
        event Action? OnAuthStateChanged;
        /// <summary>
        /// サインアップ
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<Result<bool, Error>> SignUpAsync(string password);
        /// <summary>
        /// サインイン
        /// </summary>
        /// <param name="password">パスワード</param>
        /// <returns></returns>
        Task<Result<Guid, Error>> SignInAsync(string password);
        /// <summary>
        /// サインアウト
        /// </summary>
        /// <returns></returns>
        Task<Result<bool, Error>> SignOutAsync();
        /// <summary>
        /// セッションが有効かどうか
        /// </summary>
        /// <returns></returns>
        bool IsSessionValid();
        /// <summary>
        /// セッションの有効期限を取得
        /// </summary>
        /// <returns></returns>
        Result<DateTime, Error> GetSessionExpiry();
        /// <summary>
        /// ログイン中のユーザIDを取得
        /// </summary>
        /// <returns></returns>
        Result<Guid, Error> GetUserId();
        /// <summary>
        /// カテゴリ一覧を取得
        /// </summary>
        /// <returns></returns>
        Task<Result<List<Category>, Error>> GetCategoriesAsync();
        /// <summary>
        /// カテゴリを取得
        /// </summary>
        /// <param name="categoryId">カテゴリID</param>
        /// <returns></returns>
        Task<Result<Category?, Error>> GetCategoryAsync(Guid categoryId);
        /// <summary>
        /// カテゴリを作成
        /// </summary>
        /// <param name="category">カテゴリ</param>
        /// <returns></returns>
        Task<Result<Category, Error>> CreateCategoryAsync(Category category);
        /// <summary>
        /// カテゴリを更新
        /// </summary>
        /// <param name="category">カテゴリ</param>
        /// <returns></returns>
        Task<Result<Category?, Error>> UpdateCategoryAsync(Category category);
        /// <summary>
        /// カテゴリを削除
        /// </summary>
        /// <param name="categoryId">カテゴリID</param>
        /// <returns></returns>
        Task<Result<bool, Error>> DeleteCategoryAsync(Guid categoryId);
        /// <summary>
        /// メモ一覧を取得
        /// </summary>
        /// <param name="categoryId">カテゴリID</param>
        /// <returns></returns>
        Task<Result<List<Memo>, Error>> GetMemosAsync(Guid categoryId);
        /// <summary>
        /// 最新のメモ一覧を取得
        /// </summary>
        /// <param name="count">取得する件数</param>
        /// <returns></returns>
        Task<Result<List<Memo>, Error>> GetLatestMemosAsync(int count);
        /// <summary>
        /// メモを取得
        /// </summary>
        /// <param name="memoId">メモID</param>
        /// <returns></returns>
        Task<Result<Memo?, Error>> GetMemoAsync(Guid memoId);
        /// <summary>
        /// メモを作成
        /// </summary>
        /// <param name="memo">メモ</param>
        /// <returns></returns>
        Task<Result<Memo, Error>> CreateMemoAsync(Memo memo);
        /// <summary>
        /// メモを更新
        /// </summary>
        /// <param name="memo"></param>
        /// <returns></returns>
        Task<Result<Memo?, Error>> UpdateMemoAsync(Memo memo);
        /// <summary>
        /// メモを削除
        /// </summary>
        /// <param name="memoId">メモID</param>
        /// <returns></returns>
        Task<Result<bool, Error>> DeleteMemoAsync(Guid memoId);
    }
}
