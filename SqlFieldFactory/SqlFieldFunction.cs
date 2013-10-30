
namespace AccidentallyORM.SqlFieldFactory
{
    public partial class SqlField<T>
    {
        private SqlField<T> BuildFunction(string function, params object[] param)
        {
            var sqlField = new SqlField<T>();

            sqlField.CompareString.Append(function);
            sqlField.CompareString.Append("(");
            sqlField.CompareString.Append(CompareString);

            foreach (var s in param)
            {
                sqlField.CompareString.Append(",");
                sqlField.CompareString.Append(s);
            }

            sqlField.CompareString.Append(")");
            return sqlField;
        }

        public SqlField<T> SQL_(string function, params object[] param)
        {
            return BuildFunction(function, param);
        }

        public SqlField<T> SQL_Min()
        {
            return BuildFunction("MIN");
        }

        public SqlField<T> SQL_Max()
        {
            return BuildFunction("MAX");
        }

        public SqlField<T> SQL_IsNull(params object[] param)
        {
            return BuildFunction("ISNULL", param);
        }
    }
}
