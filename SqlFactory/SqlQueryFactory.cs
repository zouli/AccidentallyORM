using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public SqlQueryFactory<T> SelectByOriginal(string select)
        {
            Sql = new StringBuilder();
            Sql.Append("SELECT ");
            Sql.Append(select);

            From();

            return this;
        }

        public SqlQueryFactory<T> Select()
        {
            return Select(-1, new string[] { });
        }

        public SqlQueryFactory<T> Select(int top)
        {
            return Select(top, new string[] { });
        }

        public SqlQueryFactory<T> Select(params Expression<Func<T, object>>[] predicate)
        {
            return Select(-1, predicate);
        }

        public SqlQueryFactory<T> Select(int top, params Expression<Func<T, object>>[] predicate)
        {
            var fieldNames = predicate.Select(EntityHelper.GetPropertyName).ToArray();
            return Select(top, fieldNames);
        }

        public SqlQueryFactory<T> Select(params string[] fieldNames)
        {
            return Select(-1, fieldNames);
        }

        public SqlQueryFactory<T> Select(int top, params string[] fieldNames)
        {
            var sql = new StringBuilder();

            if (top > 0)
            {
                sql.Append("TOP ");
                sql.Append(top);
                sql.Append(" ");
            }

            string[] fields;
            if (fieldNames != null && fieldNames.Length > 0)
            {
                fields = fieldNames.Select(fieldName => SqlFields[fieldName].FieldName).ToArray();
            }
            else
            {
                fields = EntityHelper.GetFieldNames(SqlFields).ToArray();
            }
            sql.Append(string.Join(",", fields));

            return SelectByOriginal(sql.ToString());
        }

        public SqlQueryFactory<T> Where(SqlFieldFactory<T> sqlFieldFactory)
        {
            _sqlWhere.Append(" WHERE ");
            _sqlWhere.Append(sqlFieldFactory.ToString());

            Parameters.AddRange(sqlFieldFactory.Parameters);

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