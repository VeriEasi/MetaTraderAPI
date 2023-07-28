namespace MTAPIService
{
    internal interface ICommandManager
    {
        MTCommandTask SendCommand(MTCommand task);
    }
}
