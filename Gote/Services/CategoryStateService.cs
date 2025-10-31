using Gote.Models;

namespace Gote.Services
{
    /// <summary>
    /// カテゴリの状態を管理するサービス
    /// </summary>
    /// <param name="_supabaseService">Supabaseサービス</param>
    internal sealed class CategoryStateService(ISupabaseService _supabaseService)
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
        public async Task LoadCategoriesAsync()
        {
            Categories = await _supabaseService.GetCategoriesAsync();
            OnCategoryStateChanged?.Invoke();
        }

        /// <summary>
        /// カテゴリを非同期で追加する
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task AddCategoryAsync(Category category)
        {
            await _supabaseService.CreateCategoryAsync(category);
            await LoadCategoriesAsync();
        }

        /// <summary>
        /// カテゴリを非同期で削除する
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task DeleteCategoryAsync(int categoryId)
        {
            await _supabaseService.DeleteCategoryAsync(categoryId);
            await LoadCategoriesAsync();
        }
    }
}
