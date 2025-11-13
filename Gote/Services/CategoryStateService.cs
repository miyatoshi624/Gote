using Gote.Models;

namespace Gote.Services
{
    /// <summary>
    /// カテゴリの状態を管理するサービス
    /// </summary>
    /// <param name="supabaseService">
    /// Supabaseサービス
    /// </param>
    internal sealed class CategoryStateService(ISupabaseService supabaseService)
    {
        /// <summary>
        /// カテゴリの状態が変更されたときに発火するイベント
        /// </summary>
        public event Action? OnCategoryStateChanged;
        /// <summary>
        /// カテゴリ一覧
        /// </summary>
        public List<Category>? Categories { get; private set; }

        /// <summary>
        /// カテゴリ一覧を非同期で読み込む
        /// </summary>
        /// <returns></returns>
        public async Task<Result<bool, Error>> LoadCategoriesAsync()
        {
            var result = await supabaseService.GetCategoriesAsync();
            if (result.IsSuccess)
            {
                Categories = result.GetSuccess();
                OnCategoryStateChanged?.Invoke();
                return true;
            }
            else
            {
                return result.GetFailure();
            }  
        }

        /// <summary>
        /// カテゴリを非同期で追加する
        /// </summary>
        /// <param name="category">カテゴリ</param>
        /// <returns></returns>
        public async Task<Result<bool, Error>> AddCategoryAsync(Category category)
        {
            var result = await supabaseService.CreateCategoryAsync(category);
            if (result.IsSuccess)
            {
                return await LoadCategoriesAsync();
            }
            else
            {
                return result.GetFailure();
            }
        }

        /// <summary>
        /// カテゴリを非同期で削除する
        /// </summary>
        /// <param name="categoryId">カテゴリID</param>
        /// <returns></returns>
        public async Task<Result<bool, Error>> DeleteCategoryAsync(Guid categoryId)
        {
            var result = await supabaseService.DeleteCategoryAsync(categoryId);
            if (result.IsSuccess)
            {
                return await LoadCategoriesAsync();
            }
            else
            {
                return result.GetFailure();
            }
        }
    }
}
