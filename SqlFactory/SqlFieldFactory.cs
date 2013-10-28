using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using AccidentallyORM.DBHelper;
using AccidentallyORM.Entity;
using AccidentallyORM.Entity.Attribute;

namespace AccidentallyORM.SqlFactory
{
#pragma warning disable 660,661
    public class SqlFieldFactory<T> where T : EntityBase, new()
#pragma warning restore 660,661
    {
        private SqlFieldFactory()
        {
            if (null == Parameters)
                Parameters = new List<DbParameter>();

            if (null == CompareString)
                CompareString = new StringBuilder();
        }

        private DataFieldAttribute LastField { get; set; }

        public List<DbParameter> Parameters { get; protected set; }
        public StringBuilder CompareString { get; protected set; }

        public override string ToString()
        {
            return "(" + CompareString + ")";
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

        public SqlFieldFactory<T> In<TSub>(SqlQueryFactory<TSub> subQuery) where TSub : EntityBase, new()
        {
            CompareString.Append(" IN (");
            CompareString.Append(subQuery.ToSql());
            CompareString.Append(")");

            Parameters.AddRange(subQuery.Parameters);
            return this;
        }

        private static SqlFieldFactory<T> BuildOperator(SqlFieldFactory<T> compare, object value, string operatorString)
        {
            string paraName = "@" + compare.LastField.FieldName;

            compare.CompareString.Append(operatorString + paraName);
            compare.Parameters.Add(DbService.CreateParameter(paraName, compare.LastField.ColumnType, value));

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

        public static SqlFieldFactory<T> operator +(SqlFieldFactory<T> compare, object value)
        {
            return BuildOperator(compare, value, " + ");
        }

        public static SqlFieldFactory<T> operator -(SqlFieldFactory<T> compare, object value)
        {
            return BuildOperator(compare, value, " - ");
        }

        public static SqlFieldFactory<T> operator *(SqlFieldFactory<T> compare, object value)
        {
            return BuildOperator(compare, value, " * ");
        }

        public static SqlFieldFactory<T> operator /(SqlFieldFactory<T> compare, object value)
        {
            return BuildOperator(compare, value, " / ");
        }

        public static implicit operator SqlFieldFactory<T>(DataFieldAttribute field)
        {
            var operatorHelper = new SqlFieldFactory<T> { LastField = field };

            operatorHelper.CompareString.Append(field.FieldName);
            return operatorHelper;
        }

        public static implicit operator string(SqlFieldFactory<T> field)
        {
            return field.LastField.FieldName;
        }
    }
}