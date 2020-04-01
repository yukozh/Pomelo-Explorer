using System;
using System.Collections.Generic;

namespace Pomelo.Explorer.MySQL
{
    public static class MySqlTypeMapper
    {
        public static Dictionary<string, Func<string, object>> Map = new Dictionary<string, Func<string, object>>
        {
            // Numeric
            { "bit", (val) => Convert.ToInt16(val) },
            { "tinyint", (val) => Convert.ToInt32(val) },
            { "unsigned tinyint", (val) => Convert.ToInt32(val) },
            { "smallint", (val) => Convert.ToUInt32(val) },
            { "unsigned smallint", (val) => Convert.ToUInt32(val) },
            { "mediumint",  (val) => Convert.ToInt32(val) },
            { "unsigned mediumint",  (val) => Convert.ToUInt32(val) },
            { "integer",  (val) => Convert.ToInt32(val) },
            { "unsigned integer",  (val) => Convert.ToUInt32(val) },
            { "int",  (val) => Convert.ToInt32(val)},
            { "unsigned int",  (val) => Convert.ToUInt32(val) },
            { "bigint",  (val) => Convert.ToInt64(val) },
            { "unsigned bigint", (val) => Convert.ToUInt64(val) },
            { "decimal", (val) => Convert.ToDecimal(val) },
            { "unsigned decimal", (val) => Convert.ToDecimal(val) },
            { "dec", (val) => Convert.ToDecimal(val) },
            { "unsigned dec", (val) => Convert.ToDecimal(val) },
            { "float", (val) => Convert.ToDouble(val) },
            { "unsigned float", (val) => Convert.ToDouble(val) },
            { "double", (val) => Convert.ToDouble(val) },
            { "unsigned double", (val) => Convert.ToDouble(val) },

            // Date & Time
            { "date", (val) => Convert.ToDateTime(val) },
            { "time", (val) => TimeSpan.Parse(val) },
            { "datetime", (val) => Convert.ToDateTime(val) },
            { "timestamp", (val) => Convert.ToDateTime(val) },
            { "year", (val) => Convert.ToInt32(val) },

            // String
            { "char", (val) => val },
            { "varchar", (val) => val },
            { "binary", (val) => Convert.FromBase64String(val) },
            { "varbinary", (val) =>  Convert.FromBase64String(val) },
            { "tinyblob", (val) =>  Convert.FromBase64String(val) },
            { "blob", (val) =>  Convert.FromBase64String(val) },
            { "mediumblob", (val) =>  Convert.FromBase64String(val) },
            { "longblob", (val) =>  Convert.FromBase64String(val) },
            { "tinytext", (val) => val },
            { "text", (val) => val },
            { "mediumtext", (val) => val },
            { "longtext", (val) => val },
            { "enum", (val) => val },
            { "set", (val) => val },
            { "json", (val) => val }

            // TODO: Spatial
        };
    }
}
