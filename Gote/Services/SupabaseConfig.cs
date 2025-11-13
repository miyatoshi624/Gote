namespace Gote.Services
{
    internal sealed class SupabaseConfig
    {
        public SupabaseSettings? Supabase { get; set; }
    }

    internal sealed class SupabaseSettings
    {
        public string? Url { get; set; }
        public string? Key { get; set; }
        public string? Email { get; set; }
    }
}
