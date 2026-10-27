using System.Data;
using Dapper;
using Producao.Utils;

TimeOnlyToTimeSpanHandler.Register();
foreach (var type in new[] { typeof(TimeOnly), typeof(TimeSpan) })
{
    using var table = new DataTable();
    table.Columns.Add("horario_saida", type);
    object zero = type == typeof(TimeOnly) ? TimeOnly.MinValue : TimeSpan.Zero;
    object time = type == typeof(TimeOnly) ? new TimeOnly(14, 35, 22) : new TimeSpan(14, 35, 22);
    table.Rows.Add(zero);
    table.Rows.Add(time);
    table.Rows.Add(DBNull.Value);
    using var reader = table.CreateDataReader();
    var values = reader.Parse<Horario>().ToArray();
    if (values.Length != 3 || values[0].horario_saida != TimeSpan.Zero ||
        values[1].horario_saida != new TimeSpan(14, 35, 22) || values[2].horario_saida != null)
        throw new Exception("Falha no mapeamento de horarios.");
    Console.WriteLine($"PASS: {type.Name}, meia-noite, horario e nulo.");
}
var parameter = new Npgsql.NpgsqlParameter();
new TimeOnlyToTimeSpanHandler().SetValue(parameter, new TimeSpan(14, 35, 22));
if (parameter.Value is not TimeSpan span || span != new TimeSpan(14, 35, 22))
    throw new Exception("Parametro de gravacao alterado.");
Console.WriteLine("PASS: parametro de gravacao preservado.");

public sealed class Horario
{
    public TimeSpan? horario_saida { get; set; }
}
