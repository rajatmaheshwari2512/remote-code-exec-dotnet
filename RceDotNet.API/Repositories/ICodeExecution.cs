namespace RceDotNet.API.Repositories
{
    public interface ICodeExecution
    {
        public Task<List<string>> ExecuteCodeAsync(string code, string language,string? input="");
    }
}
