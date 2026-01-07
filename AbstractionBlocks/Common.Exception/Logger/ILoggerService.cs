using System;
namespace AbstractionBlocks.Common.Exception.Logger
{
    public interface ILoggerService<TLogCategory>
        where TLogCategory : class
    {
        void Information(
            string message,
            object? context = null);
        void Warning(
            string message,
            object? context = null);
        void Warning(
            System.Exception exception,
            string message,
            object? context = null);
        void Error(
            System.Exception exception,
            string message,
            object? context = null);
    }
}
