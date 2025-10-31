using Gote.Models;

namespace Gote.Services
{
    /// <summary>
    /// Supabaseサービスのモック実装
    /// </summary>
    internal sealed class MockSupabaseService() : ISupabaseService
    {
        /// <summary>
        /// 現在のセッション
        /// </summary>
        private Supabase.Gotrue.Session? _currentSession = null;
        /// <summary>
        /// モック用メールアドレス
        /// </summary>
        private readonly string _mockEmail = "test@te.st";
        /// <summary>
        /// モック用パスワード
        /// </summary>
        private readonly string _mockPassword = "1234";
        /// <summary>
        /// モック用カテゴリデータ
        /// </summary>
        private readonly List<Category> _categories = 
        [
            new Category { Id = 1, Name = "仕事", Description = "仕事に関するメモ", IsReferenced = true },
            new Category { Id = 2, Name = "個人", Description = "個人的なメモ", IsReferenced = true },
            new Category { Id = 3, Name = "学習", Description = "学習に関するメモ", IsReferenced = true },
            new Category { Id = 4, Name = "その他", Description = "その他のメモ", IsReferenced = false }
        ];
        /// <summary>
        /// モック用メモデータ
        /// </summary>
        private readonly List<Memo> _memos = 
        [
            new Memo { Id = 1, CategoryId = 1, Title = "会議の議事録", Content = "本日の会議では以下の事項が話し合われました...", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
            new Memo { Id = 2, CategoryId = 2, Title = "買い物リスト", Content = "牛乳、卵、パン、野菜、果物...", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
            new Memo { Id = 3, CategoryId = 3, Title = "C#の学習計画", Content = "1. 基本文法の理解\n2. オブジェクト指向プログラミング\n3. 非同期プログラミング...", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
        ];

        /// <summary>
        /// サインイン
        /// </summary>
        /// <param name="password">パスワード</param>
        /// <returns>ユーザ名</returns>
        public Task<string?> SignInAsync(string password)
        {
            if (_mockPassword != password)
            {
                return Task.FromResult((string?)null);
            }

            _currentSession = new Supabase.Gotrue.Session
            {
                ExpiresIn = 3600,
                CreatedAt = DateTime.UtcNow,
            };
            return Task.FromResult((string?)_mockEmail);
        }
        /// <summary>
        /// サインアウト
        /// </summary>
        /// <returns></returns>
        public Task SignOutAsync()
        {
            _currentSession = null;
            return Task.CompletedTask;
        }
        /// <summary>
        /// セッションが有効かどうか
        /// </summary>
        /// <returns></returns>
        public bool IsSessionValid()
        {
            if (_currentSession is null)
            {
                return false;
            }
            return _currentSession.Expired();
        }
        /// <summary>
        /// セッションの有効期限を取得
        /// </summary>
        /// <returns></returns>
        public DateTime? GetSessionExpiry()
        {
            if (_currentSession is null)
            {
                return null;
            }
            return TimeZoneInfo.ConvertTimeFromUtc(_currentSession.ExpiresAt(), TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));
        }
        /// <summary>
        /// カテゴリ一覧を取得
        /// </summary>
        /// <returns></returns>
        public Task<List<Category>> GetCategoriesAsync()
        {
            return Task.FromResult(_categories);
        }
        /// <summary>
        /// カテゴリを取得
        /// </summary>
        /// <param name="categoryId">カテゴリID</param>
        /// <returns></returns>
        public Task<Category?> GetCategoryAsync(int categoryId)
        {
            var category = _categories.FirstOrDefault(c => c.Id == categoryId);
            return Task.FromResult(category);
        }
        /// <summary>
        /// カテゴリを作成
        /// </summary>
        /// <param name="category">カテゴリ</param>
        /// <returns></returns>
        public Task CreateCategoryAsync(Category category)
        {
            category.Id = _categories.Max(c => c.Id) + 1;
            _categories.Add(category);
            return Task.CompletedTask;
        }
        /// <summary>
        /// カテゴリを更新
        /// </summary>
        /// <param name="category">カテゴリ</param>
        /// <returns></returns>
        public Task UpdateCategoryAsync(Category category)
        {
            var existingCategory = _categories.FirstOrDefault(c => c.Id == category.Id);
            if (existingCategory is not null)
            {
                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;
                existingCategory.IsReferenced = category.IsReferenced;
            }
            return Task.CompletedTask;
        }
        /// <summary>
        /// カテゴリを削除
        /// </summary>
        /// <param name="categoryId">カテゴリID</param>
        /// <returns></returns>
        public Task DeleteCategoryAsync(int categoryId)
        {
            var category = _categories.FirstOrDefault(c => c.Id == categoryId);
            if (category is not null)
            {
                _categories.Remove(category);
            }
            return Task.CompletedTask;
        }
        /// <summary>
        /// メモ一覧を取得
        /// </summary>
        /// <param name="categoryId">カテゴリID</param>
        /// <returns></returns>
        public Task<List<Memo>> GetMemosAsync(int categoryId)
        {
            var memos = _memos.Where(m => m.CategoryId == categoryId).ToList();
            return Task.FromResult(memos);
        }
        /// <summary>
        /// 最新のメモ一覧を取得
        /// </summary>
        /// <param name="count">取得する件数</param>
        /// <returns></returns>
        public Task<List<Memo>> GetLatestMemosAsync(int count)
        {
            var memos = _memos.OrderByDescending(m => m.CreatedAt).Take(count).ToList();
            return Task.FromResult(memos);
        }
        /// <summary>
        /// メモを取得
        /// </summary>
        /// <param name="memoId">メモID</param>
        /// <returns></returns>
        public Task<Memo?> GetMemoAsync(int memoId)
        {
            var memo = _memos.FirstOrDefault(m => m.Id == memoId);
            return Task.FromResult(memo);
        }
        /// <summary>
        /// メモを作成
        /// </summary>
        /// <param name="memo">メモ</param>
        /// <returns></returns>
        public Task CreateMemoAsync(Memo memo)
        {
            memo.Id = _memos.Max(m => m.Id) + 1;
            memo.CreatedAt = DateTime.Now;
            memo.UpdatedAt = DateTime.Now;
            _memos.Add(memo);
            return Task.CompletedTask;
        }
        /// <summary>
        /// メモを更新
        /// </summary>
        /// <param name="memo"></param>
        /// <returns></returns>
        public Task UpdateMemoAsync(Memo memo)
        {
            var existingMemo = _memos.FirstOrDefault(m => m.Id == memo.Id);
            if (existingMemo is not null)
            {
                existingMemo.Title = memo.Title;
                existingMemo.Content = memo.Content;
                existingMemo.CategoryId = memo.CategoryId;
                existingMemo.UpdatedAt = DateTime.Now;
            }
            return Task.CompletedTask;
        }
        /// <summary>
        /// メモを削除
        /// </summary>
        /// <param name="memoId">メモID</param>
        /// <returns></returns>
        public Task DeleteMemoAsync(int memoId)
        {
            var memo = _memos.FirstOrDefault(m => m.Id == memoId);
            if (memo is not null)
            {
                _memos.Remove(memo);
            }
            return Task.CompletedTask;
        }
    }
}
