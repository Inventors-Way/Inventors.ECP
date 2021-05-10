using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
    public enum ErrorCode
    {
        NO_ERROR = 0x00,
        UNKNOWN_FUNCTION_ERR = 0x01,
        INVALID_CONTENT_ERR  = 0x02,
        DISPATCH_ERR = 0xFF
    }
}
