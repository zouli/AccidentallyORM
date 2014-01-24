using System.Text;
using AccidentallyORM.Entity;
using AccidentallyORM.SqlFieldFactory;

namespace AccidentallyORM.SqlFactory
{
    public class SqlDeleteFactory<T> : SqlFactoryBase<T> where T : EntityBase, new()
    {
        private readonly StringBuilder _sqlWhere = new StringBuilder();

        private void From()
        {
            Sql.Append(" FROM ");
            Sql.Append(SqlTableName);
        }

        public SqlDeleteFactory<T> Delete()
        {
            Sql.Append("DELETE ");

            From();

            return this;
        }

        public SqlDeleteFactory<T> Where<TSub>(SqlField<TSub> sqlField) where TSub : EntityBase, new()
        {
            _sqlWhere.Append(" WHERE ");
            _sqlWhere.Append(sqlField.ToString());

            Parameters.AddRange(sqlField.Parameters);

            return this;
        }

        public int Go()
        {
            if (Parameters.Count > 0)
            {
                return SqlHelper.ExecuteNonQuery(ToSql(), Parameters.ToArray());
            }
            return SqlHelper.ExecuteNonQuery(ToSql());
        }

        public override string ToSql()
        {
            if (_sqlWhere.Length > 0) Sql.Append(_sqlWhere);

            return Sql.ToString();
        }
    }
}