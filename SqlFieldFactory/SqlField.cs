using System.Text;
using AccidentallyORM.Entity;
using AccidentallyORM.Entity.Attribute;
using AccidentallyORM.SqlFactory;

namespace AccidentallyORM.SqlFieldFactory
{
    public partial class SqlField<T> where T : EntityBase, new()
    {
        internal SqlField()
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

        private SqlField<T> IsNull()
        {
            CompareString.Append(" IS NULL");
            return this;
        }

        private SqlField<T> IsNotNull()
        {
            CompareString.Append(" IS NOT NULL");
            return this;
        }

        public SqlField<T> In(string inString)
        {
            CompareString.Append(" IN (");
            CompareString.Append(inString);
            CompareString.Append(")");

            return this;
        }

        public SqlField<T> In<TSub>(SqlQueryFactory<TSub> subQuery) where TSub : EntityBase, new()
        {
            In(subQuery.ToSql());

            Parameters.AddRange(subQuery.Parameters);
            return this;
        }

        public SqlField<T> Like(string likeString)
        {
            CompareString.Append(" LIKE (");
            CompareString.Append(likeString);
            CompareString.Append(")");

            return this;
        }

        public SqlField<T> Like<TSub>(SqlQueryFactory<TSub> subQuery) where TSub : EntityBase, new()
        {
            Like(subQuery.ToSql());

            Parameters.AddRange(subQuery.Parameters);
            return this;
        }

        private static SqlField<T> BuildOperator(SqlField<T> compare, object value, string operatorString)
        {
            if (value.GetType().Name == typeof(SqlField<T>).Name)
            {
                compare.CompareString.Append(operatorString + value);
            }
            else
            {

                if (null == compare.LastField)
                {
                    compare.CompareString.Append(operatorString + value);
                }
                else
                {
                    var paraName = SqlParameter.GetParameterName(compare.LastField.FieldName.Replace(".", "_"));

                    compare.CompareString.Append(operatorString + paraName);
                    compare.Parameters.Add(paraName, compare.LastField.ColumnType, value);
                }
            }
            return compare;
        }

        private static SqlField<T> BuildLogicOperator(SqlField<T> compare, object value,
                                                            string operatorString)
        {
            var newCompare = new SqlField<T>();

            newCompare.Parameters.AddRange(compare.Parameters);
            newCompare.CompareString.Append(compare.ToString());
            newCompare.CompareString.Append(operatorString);

            if (value is SqlField<T>)
                newCompare.Parameters.AddRange(((SqlField<T>)value).Parameters);

            newCompare.CompareString.Append(value);

            return newCompare;
        }

        public static SqlField<T> operator &(SqlField<T> compare, object value)
        {
            return BuildLogicOperator(compare, value, " AND ");
        }

        public static SqlField<T> operator |(SqlField<T> compare, object value)
        {
            return BuildLogicOperator(compare, value, " OR ");
        }

        public static SqlField<T> operator !(SqlField<T> compare)
        {
            compare.CompareString.Insert(0, "NOT(");
            compare.CompareString.Append(")");
            return compare;
        }

        public static SqlField<T> operator ==(SqlField<T> compare, object value)
        {
            return value == null ? compare.IsNull() : BuildOperator(compare, value, " = ");
        }

        public static SqlField<T> operator !=(SqlField<T> compare, object value)
        {
            return value == null ? compare.IsNotNull() : BuildOperator(compare, value, " <> ");
        }

        public static SqlField<T> operator <(SqlField<T> compare, object value)
        {
            return BuildOperator(compare, value, " < ");
        }

        public static SqlField<T> operator >(SqlField<T> compare, object value)
        {
            return BuildOperator(compare, value, " > ");
        }

        public static SqlField<T> operator <=(SqlField<T> compare, object value)
        {
            return BuildOperator(compare, value, " <= ");
        }

        public static SqlField<T> operator >=(SqlField<T> compare, object value)
        {
            return BuildOperator(compare, value, " >= ");
        }

        public static SqlField<T> operator +(SqlField<T> compare, object value)
        {
            return BuildOperator(compare, value, " + ");
        }

        public static SqlField<T> operator -(SqlField<T> compare, object value)
        {
            return BuildOperator(compare, value, " - ");
        }

        public static SqlField<T> operator *(SqlField<T> compare, object value)
        {
            return BuildOperator(compare, value, " * ");
        }

        public static SqlField<T> operator /(SqlField<T> compare, object value)
        {
            return BuildOperator(compare, value, " / ");
        }

        public static implicit operator SqlField<T>(DataFieldAttribute field)
        {
            var operatorHelper = new SqlField<T> { LastField = field };

            operatorHelper.CompareString.Append(field.FieldName);
            return operatorHelper;
        }

        public static implicit operator string(SqlField<T> field)
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