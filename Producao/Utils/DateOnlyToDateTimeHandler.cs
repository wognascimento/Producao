using Dapper;
using System;

namespace Producao.Utils;

public class DateOnlyToDateTimeHandler : SqlMapper.TypeHandler<DateTime>
{
    public override DateTime Parse(object value)
    {
        return value switch
        {
            DateOnly dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
            DateTime dateTime => dateTime,
            _ => Convert.ToDateTime(value)
        };
    }

    public override void SetValue(System.Data.IDbDataParameter parameter, DateTime value)
    {
        parameter.Value = value;
    }
}

public class DateOnlyToNullableDateTimeHandler : SqlMapper.TypeHandler<DateTime?>
{
    public override DateTime? Parse(object value)
    {
        if (value is null || value is DBNull)
            return null;

        return value switch
        {
            DateOnly dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
            DateTime dateTime => dateTime,
            _ => Convert.ToDateTime(value)
        };
    }

    public override void SetValue(System.Data.IDbDataParameter parameter, DateTime? value)
    {
        parameter.Value = value ?? (object)DBNull.Value;
    }
}
