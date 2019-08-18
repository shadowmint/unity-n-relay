using System.Threading.Tasks;

namespace N.Package.Network
{
    public class NetworkConnection
    {
        public async Task<TResponse> Execute<TRequest, TResponse>(TRequest request, string clientId = null)
        {
            throw new System.NotImplementedException();
        }
    }
}