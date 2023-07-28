namespace MTAPIService
{
    internal interface ITaskExecutor
    {
        void Execute(MTCommandTask task);
        
        int Handle { get; }
    }
}
