using ParkyWeb2.Models;
using ParkyWeb2.Repository.IRepository;
using System.Net.Http;

namespace ParkyWeb2.Repository
{
    public class TrailRepository : Repository<Trail>, ITrailRepository
    {
        private readonly IHttpClientFactory _clientFactory;
        public TrailRepository(IHttpClientFactory clientFactory) : base(clientFactory)
        {
            _clientFactory = clientFactory;
        }

    }
}
