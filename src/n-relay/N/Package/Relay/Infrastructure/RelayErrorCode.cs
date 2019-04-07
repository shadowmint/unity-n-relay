namespace N.Package.Relay.Infrastructure
{
    public enum RelayErrorCode
    {
        AlreadyConnected,
        InnerError,
        NotConnected,
        UnknownObjectCode,
        TransactionTimeout,
        InvalidObjectState,
        InvalidConfiguration,
        AuthFailed,
        InitializationFailed,
        Disconnected,
        InvalidEvent,
        JoinFailed,
        SendFailed,
        ExternalError,
        UnknownEvent,
        SerializationError
    }
}