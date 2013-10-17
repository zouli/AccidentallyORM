using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using AccidentallyORM.DBHelper;
using AccidentallyORM.Entity;
using AccidentallyORM.Entity.Attribute;

namespace AccidentallyORM.SqlFactory
{
    public class SqlFieldFactory<T> where T : EntityBase
    {
        public static Dictionary<string, DataFieldAttribute> SqlFields = EntityHelper.GetFieldAttributes<T>();

        private SqlFieldFactory()
        {
            if (null == Parameters)
                Parameters = new List<DbParameter>();

            if (null == CompareString)
                CompareString = new StringBuilder();
        }

        private string LastField { get; set; }

        public List<DbParameter> Parameters { get; protected set; }
        public StringBuilder CompareString { get; protected set; }

        public override string ToString()
        {
            return "(" + CompareString + ")";
        }

        public static SqlFieldFactory<T> Field(string fieldName)
        {
            var field = SqlFields[fieldName];
            var operatorHelper = new SqlFieldFactory<T> { LastField = fieldName };

            operatorHelper.CompareString.Append(field.FieldName);

            return operatorHelper;
        }

        public SqlFieldFactory<T> IsNull()
        {
            CompareString.Append(" IS NULL");
            return this;
        }

        public SqlFieldFactory<T> IsNotNull()
        {
            CompareString.Append(" IS NOT NULL");
            return this;
        }

        private static SqlFieldFactory<T> BuildOperator(SqlFieldFactory<T> compare, object value, string operatorString)
        {
            var field = SqlFields[compare.LastField];

            string paraName = "@" + field.FieldName;

            compare.CompareString.Append(operatorString + paraName);
            compare.Parameters.Add(DbService.CreateParameter(paraName, field.ColumnType, value));

            return compare;
        }

        private static SqlFieldFactory<T> BuildLogicOperator(SqlFieldFactory<T> compare, object value,
                                                            string operatorString)
        {
            var newCompare = new SqlFieldFactory<T>();

            newCompare.Parameters.AddRange(compare.Parameters);
            newCompare.CompareString.Append(compare);
            newCompare.CompareString.Append(operatorString);

            if (value is SqlFieldFactory<T>)
                newCompare.Parameters.AddRange(((SqlFieldFactory<T>)value).Parameters);

            newCompare.CompareString.Append(value);

            return newCompare;
        }

        public static SqlFieldFactory<T> operator &(SqlFieldFactory<T> compare, object value)
        {
            return BuildLogicOperator(compare, value, " AND ");
        }

        public static SqlFieldFactory<T> operator |(SqlFieldFactory<T> compare, object value)
        {
            return BuildLogicOperator(compare, value, " OR ");
        }

        public static SqlFieldFactory<T> operator !(SqlFieldFactory<T> compare)
        {
            compare.CompareString.Insert(0, "NOT(");
            compare.CompareString.Append(")");
            return compare;
        }

        public static SqlFieldFactory<T> operator ==(SqlFieldFactory<T> compare, object value)
        {
            return BuildOperator(compare, value, " = ");
        }

        public static SqlFieldFactory<T> operator !=(SqlFieldFactory<T> compare, object value)
        {
            return BuildOperator(compare, value, " <> ");
        }

        public static SqlFieldFactory<T> operator <(SqlFieldFactory<T> compare, object value)
        {
            return BuildOperator(compare, value, " < ");
        }

        public static SqlFieldFactory<T> operator >(SqlFieldFactory<T> compare, object value)
        {
            return BuildOperator(compare, value, " > ");
        }

        public static SqlFieldFactory<T> operator <=(SqlFieldFactory<T> compare, object value)
        {
            return BuildOperator(compare, value, " <= ");
        }

        public static SqlFieldFactory<T> operator >=(SqlFieldFactory<T> compare, object value)
        {
            return BuildOperator(compare, value, " >= ");
        }
    }
}