using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi
{
    public enum LengthType
    {
        /// <summary>
        /// Fixed length field of six digits
        /// </summary>
        FIXED = 0,
        /// <summary>
        /// LVAR numeric field of up to 6 digits in length
        /// </summary>
        LVAR = 1,
        /// <summary>
        /// LLVAR alpha field of up to 11 characters in length
        /// </summary>
        LLVAR = 2,
        /// <summary>
        /// LLLVAR binary field of up to 999 bits in length
        /// </summary>
        LLLVAR = 3,
        /// <summary>
        /// LLLLVAR binary field of up to 9999 bits in length
        /// </summary>
        LLLLVAR = 4
    }

    public enum DataType
    {
        /// <summary>
        /// BCD Formatted Values
        /// </summary>
        BCD = 1,
        /// <summary>
        /// ASCII Values
        /// </summary>
        ASCII = 2,
        /// <summary>
        /// Binary Formatted Values, currently supports Conversion for Base 16 Numbers
        /// </summary>
        BIN = 3,
        /// <summary>
        /// Hexadecimal Values
        /// </summary>
        HEX = 4
    }

    public enum EncodingType
    {
        /// <summary>
        /// Common ASCII
        /// </summary>
        None = 0,
        /// <summary>
        /// Western European (ISO)
        /// </summary>
        Western_European = 28591,
        /// <summary>
        /// Central European (ISO)
        /// </summary>
        Central_European = 28592,
        /// <summary>
        /// Latin 3 (ISO)
        /// </summary>
        Latin = 28593,
        /// <summary>
        /// Baltic (ISO)
        /// </summary>
        Baltic = 28594,
        /// <summary>
        /// Cyrillic (ISO)  
        /// </summary>
        Cyrillic = 28595,
        /// <summary>
        /// Arabic (ISO)
        /// </summary>
        Arabic = 28596,
        /// <summary>
        /// Greek (ISO)
        /// </summary>
        Greek = 28597,
    }
}
