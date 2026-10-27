using Dapper;
using System;
using System.Data;

namespace Producao.Utils;

public sealed class TimeOnlyToTimeSpanHandler : SqlMapper.TypeHandler<TimeSpan>
{
    public override TimeSpan Parse(object value) => value switch
    {
        TimeOnly time => time.ToTimeSpan(),
        TimeSpan span => span,
        _ => throw new InvalidCastException($"Tipo de horario nao suportado: {value?.GetType().FullName}")
    };

    public override void SetValue(IDbDataParameter parameter, TimeSpan value)
    {
        parameter.Value = value;
    }

    public static void Register()
    {
        SqlMapper.RemoveTypeMap(typeof(TimeSpan));
        SqlMapper.RemoveTypeMap(typeof(TimeSpan?));
        // Dapper also registers the handler for Nullable<TimeSpan>.
        SqlMapper.AddTypeHandler(new TimeOnlyToTimeSpanHandler());
    }
}
