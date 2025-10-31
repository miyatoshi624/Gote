using Gote.Models;

namespace Gote.Services
{
    internal sealed class SupabaseService : ISupabaseService
    {
        public Task<string?> SignInAsync(string password)
        {
            throw new NotImplementedException();
        }

        public Task SignOutAsync()
        {
            throw new NotImplementedException();
        }

        public bool IsSessionValid()
        {
            throw new NotImplementedException();
        }

        public DateTime? GetSessionExpiry()
        {
            throw new NotImplementedException();
        }


        public Task<List<Category>> GetCategoriesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Category?> GetCategoryAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task CreateCategoryAsync(Category category)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCategoryAsync(Category category)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCategoryAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Memo>> GetMemosAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Memo>> GetLatestMemosAsync(int count)
        {
            throw new NotImplementedException();
        }

        public Task<Memo?> GetMemoAsync(int memoId)
        {
            throw new NotImplementedException();
        }




        public Task CreateMemoAsync(Memo memo)
        {
            throw new NotImplementedException();
        }

        public Task UpdateMemoAsync(Memo memo)
        {
            throw new NotImplementedException();
        }


        public Task DeleteMemoAsync(int memoId)
        {
            throw new NotImplementedException();
        }
    }
}
