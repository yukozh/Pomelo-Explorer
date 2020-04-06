using System;

namespace Pomelo.Explorer.MySQL
{
    public class MySqlType
    { 
        public string Type { get; set; }

        public int Length { get; set; }
    }

    public static class MySqlDbTypeParser
    {
        public static MySqlType Parse(string type)
        {
            var index = type.IndexOf('(');
            var length = 0;
            if (type.IndexOf(')') > index)
            {
                length = Convert.ToInt32(type.Substring(index + 1, type.IndexOf(')') - index - 1));
            }
            var unsigned = type.IndexOf("unsigned") >= 0;
            var plainType = type.Split(' ')[0];
            if (index > 0)
            {
                plainType = plainType.Substring(0, index);
            }
            return new MySqlType 
            {
                Type = unsigned ? $"unsigned {plainType}" : plainType,
                Length = length
            };
        }
    }
}
