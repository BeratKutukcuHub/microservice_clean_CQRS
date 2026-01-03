namespace AbstractionBlocks.Common.Exception.Logger
{
    public interface ILoggerService<TLogCategory> where TLogCategory : class
    {
        void Information(string message, Guid id, Guid? correlationId = null);
        void Error(string message, System.Exception ex, Guid id, Guid? correlationId = null);
        void Warning(string message, Guid id, string reason, Guid? correlationId = null);
        void Warning(string message, string email, string reason, Guid? correlationId = null);
    }
} 
