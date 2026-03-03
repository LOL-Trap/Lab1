namespace Core.Models
{
    public class RefactoringParameters
    {
        public Dictionary<string, object> Parameters { get; set; } = new();
        public T Get<T>(string key)
        {
            return Parameters.ContainsKey(key) ? (T)Parameters(key) : default(T);
        }
    }
}
