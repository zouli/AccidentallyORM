﻿using System.Text;
using AccidentallyORM.Entity;

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

        public SqlDeleteFactory<T> Where(SqlFieldFactory<T> sqlFieldFactory)
        {
            _sqlWhere.Append(" WHERE ");
            _sqlWhere.Append(sqlFieldFactory);

            Parameters.AddRange(sqlFieldFactory.Parameters);

            return this;
        }
    }
}