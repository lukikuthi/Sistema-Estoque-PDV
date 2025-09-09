using Supabase;
using Supabase.Postgrest.Models;
using SistemaEstoquePDV.Models;

namespace SistemaEstoquePDV.Services
{
    public class SupabaseService
    {
        private Client? _supabase;
        private readonly string _url;
        private readonly string _key;

        public SupabaseService()
        {
            // SUBSTITUA ESTAS VARIÁVEIS PELAS SUAS CREDENCIAIS DO SUPABASE
            _url = "SUA_URL_KEY_AQUI";
            _key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9sInJlZiI6Im9ya3hwdmt5cW1idGZwYXl3em9zIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTcxMjkxMTcsImV4cCI6MjA3MjcwNTExN30.mlwPE-SWsAvsnKze6ntinjxujpM_QcfUFbsdpaUsnYE";

            // Exemplo: 
            // _url = "https://xyzcompany.supabase.co";
            // _key = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InN1YnF...";
        }

        public async Task Initialize()
        {
            if (string.IsNullOrEmpty(_url) || _url.Contains("SUA_SUPABASE_URL_AQUI"))
            {
                throw new InvalidOperationException("Configure a URL do Supabase no arquivo SupabaseService.cs");
            }

            if (string.IsNullOrEmpty(_key) || _key.Contains("SUA_SUPABASE_ANON_KEY_AQUI"))
            {
                throw new InvalidOperationException("Configure a chave do Supabase no arquivo SupabaseService.cs");
            }

            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
            };

            _supabase = new Client(_url, _key, options);
            await _supabase.InitializeAsync();
        }

        public Client GetClient()
        {
            if (_supabase == null)
                throw new InvalidOperationException("Supabase não foi inicializado. Chame Initialize() primeiro.");

            return _supabase;
        }

        public async Task<List<T>> Select<T>() where T : BaseModel, new()
        {
            try
            {
                var result = await _supabase!.From<T>().Select("*").Get();
                return result.Models;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Erro ao buscar dados: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
                return new List<T>();
            }
        }

        public async Task<T?> Insert<T>(T model) where T : BaseModel, new()
        {
            try
            {
                var result = await _supabase!.From<T>().Insert(model);
                return result.Models.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Erro ao inserir dados: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
                return null;
            }
        }

        public async Task<T?> Update<T>(T model) where T : BaseModel, new()
        {
            try
            {
                var result = await _supabase!.From<T>().Update(model);
                return result.Models.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Erro ao atualizar dados: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
                return null;
            }
        }

        public async Task<bool> Delete<T>(int id) where T : BaseModel, new()
        {
            try
            {
                await _supabase!.From<T>().Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id).Delete();
                return true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Erro ao deletar dados: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
                return false;
            }
        }
    }
}
