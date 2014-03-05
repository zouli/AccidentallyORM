using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AccidentallyORM.Entity;
using AccidentallyORM.Entity.Attribute;
using AccidentallyORM.SqlFieldFactory;

namespace AccidentallyORM.SqlFactory
{
    public class SqlInsertFactory<T> : SqlFactoryBase<T> where T : EntityBase, new()
    {
        private Dictionary<string, DataFieldAttribute> _sqlUsingFields = new Dictionary<string, DataFieldAttribute>();

        private ValueSet<T> _value;
        public ValueSet<T> Value
        {
            get { return _value ?? (_value = new ValueSet<T>()); }
        }

        public SqlInsertFactory<T> Insert()
        {
            Sql.Append("INSERT INTO ");
            Sql.Append(TableName);
            return this;
        }

        public SqlInsertFactory<T> Fields()
        {
            Fields(new string[] { });
            return this;
        }

        public SqlInsertFactory<T> Fields(params Expression<Func<T, object>>[] usingFields)
        {
            var usingField = usingFields.Select(EntityHelper.GetPropertyName).ToArray();
            return Fields(usingField);
        }

        public SqlInsertFactory<T> Fields(params string[] usingFields)
        {
            if (usingFields.Length > 0)
            {
                foreach (var field in usingFields)
                {
                    _sqlUsingFields.Add(field, SqlFields[field]);
                }
            }
            else
            {
                _sqlUsingFields = SqlFields;
            }

            Sql.Append(" (");
            Sql.Append(string.Join(",", EntityHelper.GetFieldNames(_sqlUsingFields).ToArray()));
            Sql.Append(")");
            return this;
        }

        public SqlInsertFactory<T> Values(T entity)
        {
            return Values(entity, new string[] { });
        }

        public SqlInsertFactory<T> Values(T entity, params KeyValuePair<Expression<Func<T, object>>, object>[] setFields)
        {
            var defaultField = new Dictionary<string, object>();
            foreach (var keyValuePair in setFields)
            {
                var fieldName = EntityHelper.GetPropertyName(keyValuePair.Key);
                var fieldValue = keyValuePair.Value;

                if (fieldValue is SqlFactoryBase<T>)
                {
                    var sqlFactoryBase = fieldValue as SqlFactoryBase<T>;
                    defaultField.Add(fieldName, "(" + sqlFactoryBase.ToSql() + ")");
                    Parameters.AddRange(sqlFactoryBase.Parameters);
                }
                else
                {
                    defaultField.Add(fieldName, fieldValue);
                }

            }
            Values(entity, defaultField);

            return this;
        }

        private void Values(T entity, Dictionary<string, object> setFields)
        {
            Sql.Append(" VALUES (");
            foreach (var field in _sqlUsingFields)
            {
                if (setFields.ContainsKey(field.Key))
                {
                    Sql.Append(setFields[field.Key] + ",");
                }
                else
                {
                    var paraName = SqlParameter.GetParameterName(field.Value.FieldName.Replace(".", "_"));
                    Sql.Append(paraName + ",");
                    Parameters.Add(paraName, field.Value.ColumnType, entity.GetValue(field.Key));
                }
            }
            Sql.Remove(Sql.Length - 1, 1);
            Sql.Append(")");
        }

        public SqlInsertFactory<T> Values(T entity, params Expression<Func<T, object>>[] defaultFields)
        {
            string[] defaultField = defaultFields.Select(EntityHelper.GetPropertyName).ToArray();
            return Values(entity, defaultField);
        }

        public SqlInsertFactory<T> Values(T entity, params string[] defaultFields)
        {
            var defaultFieldsList = new List<string>(defaultFields);

            Sql.Append(" VALUES (");
            foreach (var field in _sqlUsingFields)
            {
                if (defaultFieldsList.Contains(field.Key))
                {
                    Sql.Append(field.Value.Default + ",");
                }
                else
                {
                    var paraName = SqlParameter.GetParameterName(field.Value.FieldName.Replace(".", "_"));
                    Sql.Append(paraName + ",");
                    Parameters.Add(paraName, field.Value.ColumnType, entity.GetValue(field.Key));
                }
            }
            Sql.Remove(Sql.Length - 1, 1);
            Sql.Append(")");
            return this;
        }

        public int Raw(string sql)
        {
            return Raw(sql, new SqlParameter());
        }

        public int Raw(string sql, SqlParameter parameters)
        {
            Parameters = parameters;
            if (Parameters.Count > 0)
            {
                return SqlHelper.ExecuteNonQuery(sql, Parameters.ToArray());
            }
            return SqlHelper.ExecuteNonQuery(sql);
        }

        public int Go()
        {
            return Parameters.Count > 0
                ? SqlHelper.ExecuteNonQuery(ToSql(), Parameters.ToArray())
                : SqlHelper.ExecuteNonQuery(ToSql());
        }

        public class ValueSet<TS>
        {
            public KeyValuePair<Expression<Func<TS, object>>, object> Set(Expression<Func<TS, object>> key, object value)
            {
                return new KeyValuePair<Expression<Func<TS, object>>, object>(key, value);
            }
        }

    }
}