using Gote.Models;
using System.Net.Http.Json;

namespace Gote.Services
{
    /// <summary>
    /// Supabaseサービスの実装
    /// </summary>
    /// <remarks>
    /// コンストラクタ
    /// </remarks>
    /// <param name="httpClient">Httpクライアント</param>
    internal sealed class SupabaseService(HttpClient httpClient) : ISupabaseService
    {
        /// <summary>
        /// 認証状態が変更されたときに発火するイベント
        /// </summary>
        public event Action? OnAuthStateChanged;
        /// <summary>
        /// Httpクライアント
        /// </summary>
        private readonly HttpClient _httpClient = httpClient;
        /// <summary>
        /// Supabaseクライアント
        /// </summary>
        private Supabase.Client? _supabase;
        /// <summary>
        /// 現在のセッション
        /// </summary>
        private Supabase.Gotrue.Session? _currentSession = null;
        /// <summary>
        /// ログイン用メールアドレス
        /// </summary>
        private string? _email;
        /// <summary>
        /// 初期化済みフラグ
        /// </summary>
        private bool _isInitialized = false;

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <returns></returns>
        private async Task InitializeAsync()
        {
            if (_isInitialized) { return; }

            // appsettings.jsonからSupabase情報を取得
            var config = await _httpClient.GetFromJsonAsync<SupabaseConfig>("appsettings.json");
            var url = config?.Supabase?.Url;
            var key = config?.Supabase?.Key;
            var options = new Supabase.SupabaseOptions
            {
                // WASMではリアルタイム接続は使用できないため無効化する
                AutoConnectRealtime = false
            };
            _supabase = new Supabase.Client(url!, key, options);
            _supabase.InitializeAsync().GetAwaiter().GetResult();

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

                await _supabase!.Auth.SignUp(_email!, password);

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
        /// <returns>ユーザID</returns>
        public async Task<Result<Guid, Error>> SignInAsync(string password)
        {
            try
            {
                await InitializeAsync();

                _currentSession = await _supabase!.Auth.SignInWithPassword(_email!, password);

                OnAuthStateChanged?.Invoke();

                return Guid.Parse(_currentSession!.User!.Id!);
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

                await _supabase!.Auth.SignOut();
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
            try
            {
                if (_currentSession is null)
                {
                    return new Error("", "セッションが無効です");
                }

                return Guid.Parse(_currentSession.User!.Id!);
            }
            catch (Exception ex)
            {
                return new Error("", ex.Message);
            }
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

                var categories = (await _supabase!.From<Category>().Get()).Models;
                var memos = (await _supabase.From<Memo>().Get()).Models;

                foreach (var category in categories)
                {
                    category.IsReferenced = memos.Any(m => m.CategoryId == category.CategoryId);
                }
                return categories;
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

                return await _supabase!.From<Category>()
                    .Where(c => c.CategoryId == categoryId)
                    .Single();
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

                var result = await _supabase!.From<Category>().Insert(category);

                return result.Model!;
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

                var result = await _supabase!.From<Category>()
                    .Where(c => c.CategoryId == category.CategoryId)
                    .Set(c => c.Name, category.Name)
                    .Set(c => c.Description, category.Description)
                    .Update();

                return result.Model;
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

                await _supabase!.From<Category>()
                    .Where(c => c.CategoryId == categoryId)
                    .Delete();

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

                var result = await _supabase!.From<Memo>()
                    .Where(m => m.CategoryId == categoryId)
                    .Order(m => m.UpdatedAt, Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                return result.Models;
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

                var result = await _supabase!.From<Memo>()
                    .Order(m => m.UpdatedAt, Supabase.Postgrest.Constants.Ordering.Descending)
                    .Limit(count)
                    .Get();

                return result.Models;
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

                return await _supabase!.From<Memo>()
                    .Where(m => m.MemoId == memoId)
                    .Single();
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

                var result = await _supabase!.From<Memo>().Insert(memo);

                return result.Model!;
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

                var result = await _supabase!.From<Memo>()
                    .Where(m => m.MemoId == memo.MemoId)
                    .Set(m => m.Title, memo.Title)
                    .Set(m => m.Content, memo.Content)
                    .Set(m => m.UpdatedAt, memo.UpdatedAt)
                    .Update();

                return result.Model;
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

                await _supabase!.From<Memo>()
                    .Where(m => m.MemoId == memoId)
                    .Delete();

                return true;
            }
            catch (Exception ex)
            {
                return new Error("", ex.Message);
            }
        }
    }
}
