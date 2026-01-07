using System;
namespace AbstractionBlocks.Common.Application.Caching
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CacheAttribute : Attribute
    {
        public string Key { get; }
        public int DurationInMinutes { get; }
        public CacheAttribute(string key, int durationInMinutes = 5)
        {
            Key = key;
            DurationInMinutes = durationInMinutes;
        }
    }
}
