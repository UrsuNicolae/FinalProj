namespace Tekwill.Library.Application.Interfaces
{
    public interface IGoogleService
    {
        string GetRedirectLink();

        Task<string> GetIdToken(string code, CancellationToken ct = default);
    }
}
