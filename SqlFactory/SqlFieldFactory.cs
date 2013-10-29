using System.Text;
using AccidentallyORM.Entity;
using AccidentallyORM.Entity.Attribute;

namespace AccidentallyORM.SqlFactory
{
    public class SqlFieldFactory<T> where T : EntityBase, new()
    {
        private SqlFieldFactory()
        {
            if (null == Parameters)
                Parameters = new SqlParameter();

            if (null == CompareString)
                CompareString = new StringBuilder();
        }

        private DataFieldAttribute LastField { get; set; }

        public SqlParameter Parameters { get; protected set; }
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

        public SqlFieldFactory<T> In(string inString)
        {
            CompareString.Append(" IN (");
            CompareString.Append(inString);
            CompareString.Append(")");

            return this;
        }

        public SqlFieldFactory<T> In<TSub>(SqlQueryFactory<TSub> subQuery) where TSub : EntityBase, new()
        {
            In(subQuery.ToSql());

            Parameters.AddRange(subQuery.Parameters);
            return this;
        }

        public SqlFieldFactory<T> Like(string likeString)
        {
            CompareString.Append(" LIKE (");
            CompareString.Append(likeString);
            CompareString.Append(")");

            return this;
        }

        public SqlFieldFactory<T> Like<TSub>(SqlQueryFactory<TSub> subQuery) where TSub : EntityBase, new()
        {
            Like(subQuery.ToSql());

            Parameters.AddRange(subQuery.Parameters);
            return this;
        }

        private static SqlFieldFactory<T> BuildOperator(SqlFieldFactory<T> compare, object value, string operatorString)
        {
            var paraName = SqlParameter.GetParameterName(compare.LastField.FieldName);

            compare.CompareString.Append(operatorString + paraName);
            compare.Parameters.Add(paraName, compare.LastField.ColumnType, value);

            return compare;
        }

        private static SqlFieldFactory<T> BuildLogicOperator(SqlFieldFactory<T> compare, object value,
                                                            string operatorString)
        {
            var newCompare = new SqlFieldFactory<T>();

            newCompare.Parameters.AddRange(compare.Parameters);
            newCompare.CompareString.Append(compare.ToString());
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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (LastField != null ? LastField.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Parameters != null ? Parameters.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CompareString != null ? CompareString.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}