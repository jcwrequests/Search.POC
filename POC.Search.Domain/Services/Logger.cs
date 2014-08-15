using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Services
{
    public enum SystemEventTypes
    {
        Error,
        Warn,
        Debug
    }

    public interface ILogger
    {
        void Warn(string message);
        void Error(string message);
        void Info(string message);
        void Debug(string message);
        void System(IEvent @event, Type projectionType, string message, SystemEventTypes systemEventType);
    }
}
