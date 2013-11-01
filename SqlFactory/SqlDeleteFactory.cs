using System.Text;
using AccidentallyORM.Entity;
using AccidentallyORM.SqlFieldFactory;

namespace AccidentallyORM.SqlFactory
{
    internal class SqlDeleteFactory<T> : SqlFactoryBase<T> where T : EntityBase, new()
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
    }
}