using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Services
{
    public interface IProjection
    {
        void Execute(IEvent e);
        void Execute(IEvent[] events);
    }
}
