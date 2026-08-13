using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Producao.Utils
{
    internal static class ExcelExportHelper
    {
        private const string DateFormat = "dd/MM/yyyy";
        private const string DateTimeFormat = "dd/MM/yyyy HH:mm:ss";
        private const string TimeFormat = "HH:mm";
        private const string NumberFormat = "#,##0.00";

        public static void InsertTypedTable<T>(IXLWorksheet worksheet, IEnumerable<T> data, string tableName)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (var column = 0; column < properties.Length; column++)
                worksheet.Cell(1, column + 1).Value = properties[column].Name;

            var row = 2;
            foreach (var item in data)
            {
                for (var column = 0; column < properties.Length; column++)
                    SetTypedValue(worksheet.Cell(row, column + 1), properties[column].GetValue(item));

                row++;
            }

            FormatAsTable(worksheet, tableName, row - 1, properties.Length);
        }

        public static void InsertTypedTable(IXLWorksheet worksheet, DataTable dataTable, string tableName)
        {
            for (var column = 0; column < dataTable.Columns.Count; column++)
                worksheet.Cell(1, column + 1).Value = dataTable.Columns[column].ColumnName;

            for (var row = 0; row < dataTable.Rows.Count; row++)
            {
                for (var column = 0; column < dataTable.Columns.Count; column++)
                    SetTypedValue(worksheet.Cell(row + 2, column + 1), dataTable.Rows[row][column]);
            }

            FormatAsTable(worksheet, tableName, dataTable.Rows.Count + 1, dataTable.Columns.Count);
        }

        public static void SetTypedValue(IXLCell cell, object? value)
        {
            if (value is null || value == DBNull.Value)
            {
                cell.Value = string.Empty;
                return;
            }

            var type = Nullable.GetUnderlyingType(value.GetType()) ?? value.GetType();

            if (type == typeof(DateOnly))
            {
                cell.Value = ((DateOnly)value).ToDateTime(TimeOnly.MinValue);
                cell.Style.DateFormat.Format = DateFormat;
                return;
            }

            if (type == typeof(DateTime))
            {
                var dateTime = (DateTime)value;
                cell.Value = dateTime;
                cell.Style.DateFormat.Format = dateTime.TimeOfDay == TimeSpan.Zero ? DateFormat : DateTimeFormat;
                return;
            }

            if (type == typeof(TimeOnly))
            {
                cell.Value = ((TimeOnly)value).ToTimeSpan();
                cell.Style.DateFormat.Format = TimeFormat;
                return;
            }

            if (type == typeof(TimeSpan))
            {
                cell.Value = (TimeSpan)value;
                cell.Style.DateFormat.Format = TimeFormat;
                return;
            }

            if (type == typeof(bool))
            {
                cell.Value = (bool)value ? "SIM" : "NÃO";
                return;
            }

            if (IsNumericType(type))
            {
                cell.Value = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                cell.Style.NumberFormat.Format = NumberFormat;
                return;
            }

            cell.Value = Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty;
        }

        private static void FormatAsTable(IXLWorksheet worksheet, string tableName, int lastRow, int columnCount)
        {
            if (columnCount == 0)
                return;

            var tableRange = worksheet.Range(1, 1, Math.Max(lastRow, 1), columnCount);
            tableRange.CreateTable(tableName);
            worksheet.Row(1).Style.Font.Bold = true;
        }

        private static bool IsNumericType(Type type)
        {
            return type == typeof(byte) ||
                   type == typeof(sbyte) ||
                   type == typeof(short) ||
                   type == typeof(ushort) ||
                   type == typeof(int) ||
                   type == typeof(uint) ||
                   type == typeof(long) ||
                   type == typeof(ulong) ||
                   type == typeof(float) ||
                   type == typeof(double) ||
                   type == typeof(decimal);
        }
    }
}
