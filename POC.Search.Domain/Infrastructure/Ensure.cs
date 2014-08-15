using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Infrastructure
{
    public class Ensure
    {
        public static void NotNull(object parameterValue,string parameterName)
        {
            if (parameterValue == null) throw new ArgumentNullException(parameterName);
        }
    }
}
