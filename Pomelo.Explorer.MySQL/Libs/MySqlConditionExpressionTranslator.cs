using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pomelo.Explorer.Definitions;

namespace Pomelo.Explorer.MySQL
{
    public static class MySqlConditionExpressionTranslator
    {
        public static string GenerateSql(ConditionExpression expression)
        {
            switch (expression.Operation)
            {
                case Condition.Equal:
                    return $"(`{expression.Parameters[0]}` = {expression.Parameters[1]})";
                case Condition.NotEqual:
                    return $"(`{expression.Parameters[0]}` <> {expression.Parameters[1]})";
                case Condition.In:
                    return $"(`{expression.Parameters[0]}` IN ({string.Join(',', expression.Parameters.Skip(1).ToArray())}))";
                case Condition.NotIn:
                    return $"(`{expression.Parameters[0]}` NOT IN ({string.Join(',', expression.Parameters.Skip(1).ToArray())}))";
                case Condition.IsNull:
                    return $"(`{expression.Parameters[0]}` IS NULL)";
                case Condition.IsNotNull:
                    return $"(`{expression.Parameters[0]}` IS NOT NULL)";
                case Condition.LowerThan:
                    return $"(`{expression.Parameters[0]}` < {expression.Parameters[1]})";
                case Condition.GreaterThan:
                    return $"(`{expression.Parameters[0]}` > {expression.Parameters[1]})";
                case Condition.LowerThanOrEqual:
                    return $"(`{expression.Parameters[0]}` <= {expression.Parameters[1]})";
                case Condition.GreaterThanOrEqual:
                    return $"(`{expression.Parameters[0]}` >= {expression.Parameters[1]})";
                case Condition.Like:
                    return $"(`{expression.Parameters[0]}` LIKE '%{expression.Parameters[1]}%')";
                case Condition.And:
                    return $"({string.Join(" AND ", expression.Parameters.Select(x => GenerateSql((ConditionExpression)x).ToArray()))})";
                case Condition.Or:
                    return $"({string.Join(" OR ", expression.Parameters.Select(x => GenerateSql((ConditionExpression)x).ToArray()))})";
            }
            return null;
        }
    }
}
