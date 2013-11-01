using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using AccidentallyORM.Entity;
using AccidentallyORM.SqlFieldFactory;

namespace AccidentallyORM.SqlFactory
{
    public partial class SqlQueryFactory<T>
    {
        #region SelectByOriginal
        public SqlQueryFactory<T> SelectByOriginal(string select, bool distinct = false)
        {
            return SelectByOriginal(-1, select, distinct);
        }

        public SqlQueryFactory<T> SelectByOriginal(int top, string select, bool distinct = false)
        {
            Sql = new StringBuilder();
            Sql.Append("SELECT ");

            if (distinct)
                Sql.Append("DISTINCT ");

            if (top > 0)
            {
                Sql.Append("TOP ");
                Sql.Append(top);
                Sql.Append(" ");
            }

            Sql.Append(select);

            return this;
        }
        #endregion

        #region Select
        public SqlQueryFactory<T> Select()
        {
            return Select(false);
        }

        public SqlQueryFactory<T> Select(int top)
        {
            return Select(false, top, new string[] { });
        }

        public SqlQueryFactory<T> Select(bool distinct)
        {
            return Select(distinct, -1, new string[] { });
        }
        #endregion

        #region string[]
        public SqlQueryFactory<T> Select(params string[] fieldNames)
        {
            return Select(false, fieldNames);
        }

        public SqlQueryFactory<T> Select(int top, params string[] fieldNames)
        {
            return Select(false, top, fieldNames);
        }

        public SqlQueryFactory<T> Select(bool distinct, params string[] fieldNames)
        {
            return Select(distinct, -1, fieldNames);
        }

        public SqlQueryFactory<T> Select(bool distinct, int top, params string[] fieldNames)
        {
            var sql = new StringBuilder();

            string[] fields;
            if (fieldNames != null && fieldNames.Length > 0)
            {
                fields = fieldNames.Select(
                        fieldName => SqlFields.ContainsKey(fieldName) ? SqlFields[fieldName].FieldName : fieldName
                    ).ToArray();
            }
            else
            {
                fields = EntityHelper.GetFieldNames(SqlFields).ToArray();
            }
            sql.Append(string.Join(",", fields));

            return SelectByOriginal(top, sql.ToString(), distinct);
        }
        #endregion

        #region Expression<Func<T, object>>[]
        public SqlQueryFactory<T> Select(params Expression<Func<T, object>>[] predicate)
        {
            return Select(false, predicate);
        }

        public SqlQueryFactory<T> Select(int top, params Expression<Func<T, object>>[] predicate)
        {
            return Select(false, top, predicate);
        }

        public SqlQueryFactory<T> Select(bool distinct, params Expression<Func<T, object>>[] predicate)
        {
            return Select(distinct, -1, predicate);
        }

        public SqlQueryFactory<T> Select(bool distinct, int top, params Expression<Func<T, object>>[] predicate)
        {
            var fieldNames = predicate.Select(EntityHelper.GetPropertyName).ToArray();
            return Select(distinct, top, fieldNames);
        }
        #endregion

        #region SqlField<T>
        public SqlQueryFactory<T> Select(SqlField<T> sqlField)
        {
            return Select(false, sqlField);
        }

        public SqlQueryFactory<T> Select(int top, SqlField<T> sqlField)
        {
            return Select(false, top, sqlField);
        }

        public SqlQueryFactory<T> Select(bool distinct, SqlField<T> sqlField)
        {
            return Select(distinct, -1, sqlField);
        }

        public SqlQueryFactory<T> Select(bool distinct, int top, SqlField<T> sqlField)
        {
            return Select(distinct, top, sqlField.ToString());
        }
        #endregion
    }
}
