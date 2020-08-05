using System;

namespace Policies.Services
{
    public class FlakyStartupException : Exception
    {
        public FlakyStartupException(int attemptCount) 
            : base($"Startup failure number {attemptCount}")
        {
        }
    }
}