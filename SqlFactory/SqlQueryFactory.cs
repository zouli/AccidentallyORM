using System.Collections.Generic;
using System.Text;
using AccidentallyORM.Entity;

namespace AccidentallyORM.SqlFactory
{
    public class SqlQueryFactory<T> : SqlFactoryBase<T> where T : EntityBase, new()
    {
        private readonly StringBuilder _sqlGroupBy = new StringBuilder();
        private readonly StringBuilder _sqlOrderBy = new StringBuilder();
        private readonly StringBuilder _sqlWhere = new StringBuilder();

        private void From()
        {
            Sql.Append(" FROM ");
            Sql.Append(SqlTableName);
        }

        public SqlQueryFactory<T> SelectAll(int top = -1)
        {
            Sql.Append("SELECT ");
            if (top > 0)
            {
                Sql.Append("TOP ");
                Sql.Append(top);
                Sql.Append(" ");
            }
            Sql.Append(string.Join(",", EntityHelper.GetFieldNames(SqlFieldFactory<T>.SqlFields).ToArray()));

            From();

            return this;
        }

        public SqlQueryFactory<T> Select(string sqlString)
        {
            Sql = new StringBuilder();
            Sql.Append("SELECT ");
            Sql.Append(sqlString);

            From();

            return this;
        }

        public SqlQueryFactory<T> Where(SqlFieldFactory<T> sqlFieldFactory)
        {
            _sqlWhere.Append(" WHERE ");
            _sqlWhere.Append(sqlFieldFactory);

            Parameters.AddRange(sqlFieldFactory.Parameters);

            return this;
        }

        public SqlQueryFactory<T> OrderBy(string fieldName, string sort = "ASC")
        {
            var field = SqlFieldFactory<T>.SqlFields[fieldName];

            _sqlOrderBy.Append(_sqlOrderBy.Length > 0 ? "," : " ORDER BY ");
            _sqlOrderBy.Append(field.FieldName);
            _sqlOrderBy.Append(" ");
            _sqlOrderBy.Append(sort);
            return this;
        }

        public SqlQueryFactory<T> GroupBy(string fieldName)
        {
            var field = SqlFieldFactory<T>.SqlFields[fieldName];

            _sqlGroupBy.Append(_sqlGroupBy.Length > 0 ? "," : " GROUP BY ");
            _sqlGroupBy.Append(field.FieldName);
            return this;
        }

        public List<T> Go()
        {
            if (Parameters.Count > 0)
            {
                return SqlHelper.ExecuteReader<T>(ToSql(), Parameters.ToArray());
            }
            return SqlHelper.ExecuteReader<T>(ToSql());
        }

        public override string ToSql()
        {
            if (_sqlWhere.Length > 0) Sql.Append(_sqlWhere);
            if (_sqlGroupBy.Length > 0) Sql.Append(_sqlGroupBy);
            if (_sqlOrderBy.Length > 0) Sql.Append(_sqlOrderBy);

            return Sql.ToString();
        }
    }
}