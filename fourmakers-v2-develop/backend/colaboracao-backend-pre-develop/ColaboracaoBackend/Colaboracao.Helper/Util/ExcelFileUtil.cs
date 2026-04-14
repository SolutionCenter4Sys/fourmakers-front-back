using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Colaboracao.Helper.Util
{
    public static class ExcelFileUtil
    {
        public static byte[] CreateExcelFile<T>(List<T> source, string tableName = "", bool mustFormatWithTable = true, bool mustAddHeaderWhenNotFormated = true)
        {
            if (IsDynamicList(source))
                return CreateDynamicExcelFile(source, tableName, mustFormatWithTable, mustAddHeaderWhenNotFormated);
            else
                return CreateTypedExcelFile(source, tableName);
        }

        private static byte[] CreateDynamicExcelFile<T>(List<T> source, string tableName, bool mustFormatWithTable, bool mustAddHeaderWhenNotFormated)
        {
            if (source.Count == 0)
                return Array.Empty<byte>();

            if (string.IsNullOrEmpty(tableName))
            {
                tableName = "Tab 1";
            }

            // Obtém as propriedades do primeiro item na lista
            var properties = ((IDictionary<string, object>)source.First()).Keys
                .Where(prop => !prop.EndsWith("__ND"))  // Filtra propriedades terminando com __ND
                .ToArray();

            if (mustFormatWithTable)
            {
                DataTable data = new DataTable
                {
                    TableName = tableName
                };

                // Adiciona as colunas filtradas ao DataTable
                foreach (var propertie in properties)
                {
                    data.Columns.Add(propertie, typeof(string));
                }

                // Adiciona as linhas ao DataTable
                source.ForEach(x =>
                {
                    var rowValue = (IDictionary<string, object>)x;
                    var valueList = new List<string>();

                    foreach (var propertie in properties)
                    {
                        rowValue.TryGetValue(propertie, out var value);
                        valueList.Add(value?.ToString() ?? "");
                    }

                    data.Rows.Add(valueList.ToArray());
                });

                // Cria o arquivo Excel com o DataTable filtrado
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var sheet = wb.AddWorksheet(data, tableName);
                    sheet.Columns().AdjustToContents();

                    using (MemoryStream ms = new MemoryStream())
                    {
                        wb.SaveAs(ms);
                        return ms.ToArray();
                    }
                }
            }
            else
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var sheet = wb.Worksheets.Add(tableName);

                    // Se mustAddHeader for true, adiciona os cabeçalhos
                    if (mustAddHeaderWhenNotFormated)
                    {
                        for (int colIndex = 0; colIndex < properties.Length; colIndex++)
                        {
                            sheet.Cell(1, colIndex + 1).Value = properties[colIndex];
                        }
                    }

                    // Adiciona os dados, começando na linha 2 se mustAddHeader for true
                    for (int rowIndex = 0; rowIndex < source.Count; rowIndex++)
                    {
                        var rowValue = (IDictionary<string, object>)source[rowIndex];
                        for (int colIndex = 0; colIndex < properties.Length; colIndex++)
                        {
                            rowValue.TryGetValue(properties[colIndex], out var value);

                            // Define a linha de destino com base no cabeçalho
                            int targetRow = mustAddHeaderWhenNotFormated ? rowIndex + 2 : rowIndex + 1;
                            sheet.Cell(targetRow, colIndex + 1).Value = value?.ToString() ?? "";
                        }
                    }
                    sheet.Columns().AdjustToContents();

                    using (MemoryStream ms = new MemoryStream())
                    {
                        wb.SaveAs(ms);
                        return ms.ToArray();
                    }
                }
            }
        }

        private static byte[] CreateTypedExcelFile<T>(List<T> source, string tableName)
        {
            var properties = typeof(T).GetProperties()
                                      .Where(p => !p.Name.EndsWith("__ND"))
                                      .ToArray();
            ;

            if (string.IsNullOrEmpty(tableName))
            {
                tableName = "Tab 1";
            }

            DataTable data = new DataTable
            {
                TableName = tableName
            };

            foreach (var propertie in properties)
            {
                data.Columns.Add(propertie.Name, typeof(string));
            }
            if (source.Count > 0)
            {
                source.ForEach(x =>
                {
                    var valueList = new List<string>();
                    foreach (var propertie in properties)
                    {
                        valueList.Add(propertie.GetValue(x)?.ToString() ?? "");
                    }
                    data.Rows.Add(values: valueList.ToArray());
                });
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                var sheet = wb.AddWorksheet(data, tableName);
                sheet.Columns().AdjustToContents();
                using (MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    var content = ms.ToArray();
                    return content;
                }
            }
        }

        private static bool IsDynamicList<T>(List<T> source)
        {
            return source.GetType() == typeof(List<dynamic>);
        }
    }
}