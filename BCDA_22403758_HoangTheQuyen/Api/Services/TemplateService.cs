using K8sManager.Api.Domain.Repositories;

namespace K8sManager.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly ITemplateRepository _repo;
        public TemplateService(ITemplateRepository repo) => _repo = repo;

        public async Task<IEnumerable<(int Id, string Name)>> ListAsync()
            => (await _repo.GetAllAsync().ConfigureAwait(false)).Select(t => (t.Id, t.Name));
    }
}