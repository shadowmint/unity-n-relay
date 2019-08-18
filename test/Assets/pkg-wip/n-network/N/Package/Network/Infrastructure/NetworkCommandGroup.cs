namespace N.Package.Network.Infrastructure
{
    public class NetworkCommandGroup
    {
        public NetworkConnection NetworkConnection { get; set; }

        public NetworkCommandGroup Clone()
        {
            return new NetworkCommandGroup();
        }

        public void Register<TRequest, TResponse>(NetworkCommandType source, NetworkCommandHandler<TRequest, TResponse> handler)
            where TResponse : NetworkCommand
            where TRequest : NetworkCommand
        {
        }
    }
}