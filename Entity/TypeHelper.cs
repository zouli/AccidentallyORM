using System;
using System.Data;
using System.Text.RegularExpressions;

namespace AccidentallyORM.Entity
{
    public class TypeHelper
    {
        public static DbType CSharpTypeNameToDbType(Type type)
        {
            switch (type.FullName)
            {
                case "System.Byte[]":
                    return DbType.Binary;

                case "System.String":
                    return DbType.String;

                case "System.Int16":
                    return DbType.Int16;

                case "System.Int32":
                    return DbType.Int32;

                case "System.Int64":
                    return DbType.Int64;

                case "System.UInt16":
                    return DbType.UInt16;

                case "System.UInt32":
                    return DbType.UInt32;

                case "System.UInt64":
                    return DbType.UInt64;

                case "System.Decimal":
                    return DbType.Decimal;

                case "System.DateTime":
                    return DbType.DateTime;

                default:
                    return DbType.Object;
            }
        }

        public static DbType DataTypeNameToDbType(string msSqlTypeName)
        {
            switch (msSqlTypeName)
            {
                case "bit":
                    return DbType.Boolean;
                case "tinyint":
                    return DbType.Byte;
                case "smallint":
                    return DbType.Int16;
                case "int":
                    return DbType.Int32;
                case "bigint":
                    return DbType.Int64;

                case "real":
                    return DbType.Single;
                case "float":
                    return DbType.Double;
                case "decimal":
                case "numeric":
                    return DbType.Decimal;

                case "money":
                case "smallmoney":
                    return DbType.Currency;

                case "smalldatetime":
                case "datetime":
                    return DbType.DateTime;
                case "timestamp":
                    return DbType.DateTimeOffset;

                case "binary":
                case "varbinary":
                case "image":
                    return DbType.Binary;

                case "char":
                    return DbType.AnsiStringFixedLength;
                case "varchar":
                case "text":
                    return DbType.AnsiString;
                case "nchar":
                    return DbType.StringFixedLength;
                case "nvarchar":
                case "ntext":
                    return DbType.String;

                case "uniqueidentifier":
                    return DbType.Guid;
                case "variant":
                    return DbType.Object;
                default:
                    return DbType.String;
            }
        }

        public static string DbTypeToCSharpTypeName(DbType dbType)
        {
            string type = string.Empty;

            switch (dbType)
            {
                case DbType.UInt64:
                    type = "ulong";
                    break;
                case DbType.Int64:
                    type = "long";
                    break;
                case DbType.Int32:
                    type = "int";
                    break;
                case DbType.UInt32:
                    type = "uint";
                    break;
                case DbType.UInt16:
                    type = "UInt16";
                    break;
                case DbType.Int16:
                    type = "Int16";
                    break;
                case DbType.Single:
                    type = "float";
                    break;
                case DbType.Double:
                    type = "double";
                    break;
                case DbType.VarNumeric:
                case DbType.Decimal:
                case DbType.Currency:
                    type = "decimal";
                    break;
                case DbType.Date:
                case DbType.DateTime:
                case DbType.Time:
                case DbType.DateTime2:
                    type = "DateTime";
                    break;

                case DbType.String:
                case DbType.StringFixedLength:
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                    type = "string";
                    break;

                case DbType.SByte:
                    type = "sbyte";
                    break;
                case DbType.Byte:
                    type = "byte";
                    break;

                case DbType.Binary:
                    type = "byte[]";
                    break;

                case DbType.Boolean:
                    type = "bool";
                    break;
                case DbType.Xml:
                    type = "string";
                    break;
                case DbType.Object:
                    type = "object";
                    break;
                case DbType.Guid:
                    type = "Guid";
                    break;
            }

            return type;
        }

        public static Type DbTypeToType(DbType dbType)
        {
            Type type = typeof (DBNull);

            switch (dbType)
            {
                case DbType.UInt64:
                    type = typeof (UInt64);
                    break;
                case DbType.Int64:
                    type = typeof (Int64);
                    break;
                case DbType.Int32:
                    type = typeof (Int32);
                    break;
                case DbType.UInt32:
                    type = typeof (UInt32);
                    break;
                case DbType.UInt16:
                    type = typeof (UInt16);
                    break;
                case DbType.Int16:
                    type = typeof (Int16);
                    break;
                case DbType.Single:
                    type = typeof (Single);
                    break;
                case DbType.Double:
                    type = typeof (Double);
                    break;
                case DbType.VarNumeric:
                case DbType.Decimal:
                case DbType.Currency:
                    type = typeof (Decimal);
                    break;

                case DbType.Date:
                case DbType.DateTime:
                case DbType.Time:
                case DbType.DateTime2:
                    type = typeof (DateTime);
                    break;

                case DbType.String:
                case DbType.StringFixedLength:
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                    type = typeof (String);
                    break;

                case DbType.SByte:
                    type = typeof (SByte);
                    break;
                case DbType.Byte:
                    type = typeof (Byte);
                    break;
                case DbType.Boolean:
                    type = typeof (Boolean);
                    break;
                case DbType.Binary:
                    type = typeof (Byte[]);
                    break;

                case DbType.Object:
                    type = typeof (Object);
                    break;

                case DbType.Guid:
                    type = typeof (Guid);
                    break;
            }

            return type;
        }

        public static int GetColumnSize(string msSqlTypeName)
        {
            if (msSqlTypeName.StartsWith("decimal")
                || msSqlTypeName.StartsWith("numeric"))
            {
                return 17;
            }
            if (msSqlTypeName.StartsWith("varbinary")
                || msSqlTypeName.StartsWith("char")
                || msSqlTypeName.StartsWith("varchar")
                || msSqlTypeName.StartsWith("nchar")
                || msSqlTypeName.StartsWith("nvarchar"))
            {
                Match m = Regex.Match(msSqlTypeName, @"\((.+?)\)");
                if (m.Success && !m.Groups[1].Value.ToLower().Equals("max"))
                {
                    return int.Parse(m.Groups[1].Value);
                }
            }
            int size = 0;
            switch (msSqlTypeName.ToLower())
            {
                case "bit":
                case "tinyint":
                    size = 1;
                    break;
                case "smallint":
                    size = 2;
                    break;
                case "date":
                    size = 3;
                    break;
                case "int":
                case "real":
                case "smallmoney":
                case "smalldatetime":
                    size = 4;
                    break;
                case "time":
                    size = 5;
                    break;
                case "bigint":
                case "datetime":
                case "money":
                case "float":
                case "timestamp":
                    size = 8;
                    break;
                case "uniqueidentifier":
                    size = 16;
                    break;
                case "decimal":
                case "numeric":
                    size = 17;
                    break;
                case "binary":
                    size = 50;
                    break;
                case "ntext":
                    size = 1073741823;
                    break;
                case "varbinary(max)":
                case "nvarchar(max)":
                case "varchar(max)":
                case "xml":
                case "image":
                case "text":

                case "geography":
                case "geometry":
                    size = 2147483647;
                    break;
                case "hierarchyid":
                    size = 892;
                    break;
                case "variant":
                    size = 8009;
                    break;
            }
            return size;
        }
    }
}