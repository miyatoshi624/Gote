using Gote.Models;
using System.Net.Http.Json;

namespace Gote.Services
{
    /// <summary>
    /// Supabaseサービスのモック実装
    /// </summary>
    internal sealed class MockSupabaseService : ISupabaseService
    {
        /// <summary>
        /// 認証状態が変更されたときに発火するイベント
        /// </summary>
        public event Action? OnAuthStateChanged;
        /// <summary>
        /// Httpクライアント
        /// </summary>
        private readonly HttpClient _httpClient;
        /// <summary>
        /// 現在のセッション
        /// </summary>
        private Supabase.Gotrue.Session? _currentSession = null;
        /// <summary>
        /// ログイン用メールアドレス
        /// </summary>
        private string? _email;
        /// <summary>
        /// モック用ユーザID
        /// </summary>
        private readonly Guid _mockUserId = Guid.NewGuid();
        /// <summary>
        /// モック用パスワード
        /// </summary>
        private readonly string _mockPassword = "1234";
        /// <summary>
        /// モック用カテゴリデータ
        /// </summary>
        private readonly List<Category> _categories;
        /// <summary>
        /// モック用メモデータ
        /// </summary>
        private readonly List<Memo> _memos;
        /// <summary>
        /// 初期化済みフラグ
        /// </summary>
        private bool _isInitialized = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="httpClient">Httpクライアント</param>
        public MockSupabaseService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _categories =
                [
                    new Category { CategoryId = Guid.NewGuid(), UserId = _mockUserId, Name = "仕事", Description = "仕事に関するメモ", IsReferenced = false },
                    new Category { CategoryId = Guid.NewGuid(), UserId = _mockUserId, Name = "個人", Description = "個人的なメモ", IsReferenced = false },
                    new Category { CategoryId = Guid.NewGuid(), UserId = _mockUserId, Name = "学習", Description = "学習に関するメモ", IsReferenced = false },
                    new Category { CategoryId = Guid.NewGuid(), UserId = _mockUserId, Name = "その他", Description = "その他のメモ", IsReferenced = false }
                ];
            _memos =
                [
                    new Memo { MemoId = Guid.NewGuid(), CategoryId = _categories[0].CategoryId, UserId = _mockUserId, Title = "会議の議事録", Content = "本日の会議では以下の事項が話し合われました...", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                    new Memo { MemoId = Guid.NewGuid(), CategoryId = _categories[1].CategoryId, UserId = _mockUserId, Title = "買い物リスト", Content = "牛乳、卵、パン、野菜、果物...", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                    new Memo { MemoId = Guid.NewGuid(), CategoryId = _categories[2].CategoryId, UserId = _mockUserId, Title = "C#の学習計画", Content = "1. 基本文法の理解\n2. オブジェクト指向プログラミング\n3. 非同期プログラミング...", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
                ];
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <returns></returns>
        private async Task InitializeAsync()
        {
            if (_isInitialized) { return; }

            // appsettings.jsonからSupabase情報を取得
            var config = await _httpClient.GetFromJsonAsync<SupabaseConfig>("appsettings.json");
            _email = config?.Supabase?.Email ?? "";

            _isInitialized = true;
        }

        /// <summary>
        /// サインアップ
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<Result<bool, Error>> SignUpAsync(string password)
        {
            try
            {
                await InitializeAsync();

                return true;
            }
            catch (Exception ex)
            {
                return new Error("", ex.Message);
            }
        }

        /// <summary>
        /// サインイン
        /// </summary>
        /// <param name="password">パスワード</param>
        /// <returns>ユーザ名</returns>
        public async Task<Result<Guid, Error>> SignInAsync(string password)
        {
            try
            {
                await InitializeAsync();

                if (string.IsNullOrEmpty(_email) || _mockPassword != password)
                {
                    return new Error("AUTH_ERROR", "Invalid email or password.");
                }

                _currentSession = new Supabase.Gotrue.Session
                {
                    ExpiresIn = 3600,
                    CreatedAt = DateTime.UtcNow,
                };

                OnAuthStateChanged?.Invoke();

                return _mockUserId;
            }
            catch (Exception ex)
            {
                return new Error("", ex.Message);
            }
        }

        /// <summary>
        /// サインアウト
        /// </summary>
        /// <returns></returns>
        public async Task<Result<bool, Error>> SignOutAsync()
        {
            try
            {
                await InitializeAsync();

                _currentSession = null;

                OnAuthStateChanged?.Invoke();

                return true;
            }
            catch (Exception ex)
            {
                return new Error("", ex.Message);

            }
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
            return !_currentSession.Expired();
        }

        /// <summary>
        /// セッションの有効期限を取得
        /// </summary>
        /// <returns></returns>
        public Result<DateTime, Error> GetSessionExpiry()
        {
            try
            {
                if (_currentSession is null)
                {
                    return new Error("", "セッションが無効です");
                }
                return TimeZoneInfo.ConvertTimeFromUtc(_currentSession.ExpiresAt(), TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));
            }
            catch (Exception ex)
            {
                return new Error("", ex.Message);
            }
        }

        /// <summary>
        /// ログイン中のユーザIDを取得
        /// </summary>
        /// <returns></returns>
        public Result<Guid, Error> GetUserId()
        {
            if (_currentSession is null)
            {
                return new Error("", "セッションが無効です");
            }

            return _mockUserId;
        }

        /// <summary>
        /// カテゴリ一覧を取得
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<Category>, Error>> GetCategoriesAsync()
        {
            try
            {
                await InitializeAsync();

                foreach (var category in _categories)
                {
                    category.IsReferenced = _memos.Any(m => m.CategoryId == category.CategoryId);
                }
                return _categories;
            }
            catch (Exception ex)
            {
                return new Error("", ex.Message);
            }
        }

        /// <summary>
        /// カテゴリを取得
        /// </summary>
        /// <param name="categoryId">カテゴリID</param>
        /// <returns></returns>
        public async Task<Result<Category?, Error>> GetCategoryAsync(Guid categoryId)
        {
            try
            {
                await InitializeAsync();

                return _categories.FirstOrDefault(c => c.CategoryId == categoryId);
            }
            catch (Exception ex)
            {
                return new Error("", ex.Message);
            }
        }

        /// <summary>
        /// カテゴリを作成
        /// </summary>
        /// <param name="category">カテゴリ</param>
        /// <returns></returns>
        public async Task<Result<Category, Error>> CreateCategoryAsync(Category category)
        {
            try
            {
                await InitializeAsync();

                category.CategoryId = Guid.NewGuid();
                _categories.Add(category);
                return category;
            }
            catch (Exception ex)
            {
                return new Error("", ex.Message);
            }
        }
        /// <summary>
        /// カテゴリを更新
        /// </summary>
        /// <param name="category">カテゴリ</param>
        /// <returns></returns>
        public async Task<Result<Category?, Error>> UpdateCategoryAsync(Category category)
        {
            try
            {
                await InitializeAsync();

                var existingCategory = _categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
                if (existingCategory is not null)
                {
                    existingCategory.Name = category.Name;
                    existingCategory.Description = category.Description;
                    existingCategory.IsReferenced = category.IsReferenced;
                }
                return existingCategory;
            }
            catch (Exception ex)
            {
                return new Error("", ex.Message);
            } 
        }
        /// <summary>
        /// カテゴリを削除
        /// </summary>
        /// <param name="categoryId">カテゴリID</param>
        /// <returns></returns>
        public async Task<Result<bool, Error>> DeleteCategoryAsync(Guid categoryId)
        {
            try
            {
                await InitializeAsync();

                var category = _categories.FirstOrDefault(c => c.CategoryId == categoryId);
                if (category is not null)
                {
                    _categories.Remove(category);
                }
                return true;
            }
            catch (Exception ex)
            {
                return new Error("", ex.Message);
            }
        }
        /// <summary>
        /// メモ一覧を取得
        /// </summary>
        /// <param name="categoryId">カテゴリID</param>
        /// <returns></returns>
        public async Task<Result<List<Memo>, Error>> GetMemosAsync(Guid categoryId)
        {
            try
            {
                await InitializeAsync();

                return _memos.Where(m => m.CategoryId == categoryId).ToList();
            }
            catch (Exception ex)
            {
                return new Error("", ex.Message);
            }
        }
        /// <summary>
        /// 最新のメモ一覧を取得
        /// </summary>
        /// <param name="count">取得する件数</param>
        /// <returns></returns>
        public async Task<Result<List<Memo>, Error>> GetLatestMemosAsync(int count)
        {
            try
            {
                await InitializeAsync();

                return _memos.OrderByDescending(m => m.CreatedAt).Take(count).ToList();
            }
            catch (Exception ex)
            {
                return new Error("", ex.Message);
            }
        }
        /// <summary>
        /// メモを取得
        /// </summary>
        /// <param name="memoId">メモID</param>
        /// <returns></returns>
        public async Task<Result<Memo?, Error>> GetMemoAsync(Guid memoId)
        {
            try
            {
                await InitializeAsync();

                return _memos.FirstOrDefault(m => m.MemoId == memoId);
            }
            catch (Exception ex)
            {
                return new Error("", ex.Message);
            }
        }
        /// <summary>
        /// メモを作成
        /// </summary>
        /// <param name="memo">メモ</param>
        /// <returns></returns>
        public async Task<Result<Memo, Error>> CreateMemoAsync(Memo memo)
        {
            try
            {
                await InitializeAsync();

                memo.MemoId = Guid.NewGuid();
                memo.CreatedAt = DateTime.Now;
                memo.UpdatedAt = DateTime.Now;
                _memos.Add(memo);
                return memo;
            }
            catch (Exception ex)
            {
                return new Error("", ex.Message);
            }
        }
        /// <summary>
        /// メモを更新
        /// </summary>
        /// <param name="memo"></param>
        /// <returns></returns>
        public async Task<Result<Memo?, Error>> UpdateMemoAsync(Memo memo)
        {
            try
            {
                await InitializeAsync();

                var existingMemo = _memos.FirstOrDefault(m => m.MemoId == memo.MemoId);
                if (existingMemo is not null)
                {
                    existingMemo.Title = memo.Title;
                    existingMemo.Content = memo.Content;
                    existingMemo.CategoryId = memo.CategoryId;
                    existingMemo.UpdatedAt = DateTime.Now;
                }
                return existingMemo;
            }
            catch (Exception ex)
            {
                return new Error("", ex.Message);
            }
        }
        /// <summary>
        /// メモを削除
        /// </summary>
        /// <param name="memoId">メモID</param>
        /// <returns></returns>
        public async Task<Result<bool, Error>> DeleteMemoAsync(Guid memoId)
        {
            try
            {
                await InitializeAsync();

                var memo = _memos.FirstOrDefault(m => m.MemoId == memoId);
                if (memo is not null)
                {
                    _memos.Remove(memo);
                }
                return true;
            }
            catch (Exception ex)
            {
                return new Error("", ex.Message);
            }
        }
    }
}
