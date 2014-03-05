using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using AccidentallyORM.Entity;
using AccidentallyORM.SqlFieldFactory;

namespace AccidentallyORM.SqlFactory
{
    public partial class SqlQueryFactory<T> : SqlFactoryBase<T> where T : EntityBase, new()
    {
        private readonly StringBuilder _sqlFields = new StringBuilder();
        private readonly StringBuilder _sqlGroupBy = new StringBuilder();
        private readonly StringBuilder _sqlOrderBy = new StringBuilder();
        private readonly StringBuilder _sqlJoin = new StringBuilder();
        private readonly StringBuilder _sqlWhere = new StringBuilder();

        private void From()
        {
            Sql.Append(" FROM ");
            Sql.Append(TableName);
        }

        public SqlQueryFactory<T> Where<TSub>(SqlField<TSub> sqlField) where TSub : EntityBase, new()
        {
            _sqlWhere.Append(" WHERE ");
            _sqlWhere.Append(sqlField.ToString());

            Parameters.AddRange(sqlField.Parameters);

            return this;
        }

        public SqlQueryFactory<T> OrderBy(Expression<Func<T, object>> predicate, string sort = "ASC")
        {
            return OrderBy(EntityHelper.GetPropertyName(predicate), sort);
        }

        public SqlQueryFactory<T> OrderBy(string fieldName, string sort = "ASC")
        {
            var field = SqlFields[fieldName];

            _sqlOrderBy.Append(_sqlOrderBy.Length > 0 ? "," : " ORDER BY ");
            _sqlOrderBy.Append(field.FieldName);
            _sqlOrderBy.Append(" ");
            _sqlOrderBy.Append(sort);
            return this;
        }

        public SqlQueryFactory<T> GroupBy(Expression<Func<T, object>> predicate)
        {
            return GroupBy(EntityHelper.GetPropertyName(predicate));
        }

        public SqlQueryFactory<T> GroupBy(string fieldName)
        {
            var field = SqlFields[fieldName];

            _sqlGroupBy.Append(_sqlGroupBy.Length > 0 ? "," : " GROUP BY ");
            _sqlGroupBy.Append(field.FieldName);
            return this;
        }

        public SqlQueryFactory<T> Having(string sqlHaving)
        {
            _sqlGroupBy.Append(" HAVING ");
            _sqlGroupBy.Append(sqlHaving);
            return this;
        }

        public SqlQueryFactory<T> Having(SqlField<T> sqlHaving)
        {
            return Having(sqlHaving.ToString());
        }

        public List<T> Raw(string sql)
        {
            return Raw(sql, new SqlParameter());
        }

        public List<T> Raw(string sql, SqlParameter parameters)
        {
            Parameters = parameters;
            if (Parameters.Count > 0)
            {
                return SqlHelper.ExecuteReader<T>(sql, Parameters.ToArray());
            }
            return SqlHelper.ExecuteReader<T>(sql);
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
            Sql.Append(_sqlFields);
            From();

            if (_sqlJoin.Length > 0)
            {
                Sql.Append(_sqlJoin);
                Sql.Append(_sqlJoinOn);
            }
            if (_sqlWhere.Length > 0) Sql.Append(_sqlWhere);
            if (_sqlGroupBy.Length > 0) Sql.Append(_sqlGroupBy);
            if (_sqlOrderBy.Length > 0) Sql.Append(_sqlOrderBy);

            return Sql.ToString();
        }
    }
}