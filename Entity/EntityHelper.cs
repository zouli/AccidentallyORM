using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AccidentallyORM.Entity.Attribute;
using AccidentallyORM.Extensions;

namespace AccidentallyORM.Entity
{
    public class EntityHelper
    {
        public static string GetTableName<T>()
        {
            var dataTableAttribute =
                (DataTableAttribute)System.Attribute.GetCustomAttribute(typeof(T), typeof(DataTableAttribute));
            var tableName = string.Empty;
            if (null != dataTableAttribute)
                tableName = dataTableAttribute.TableName;

            return tableName;
        }

        public static List<string> GetFieldNames(Dictionary<string, DataFieldAttribute> dataFieldAttributes,
                                                 string prefix = "")
        {
            return dataFieldAttributes.Values.Select(value => prefix + value.FieldName).ToList();
        }

        public static Dictionary<string, DataFieldAttribute> GetFieldAttributes<T>()
        {
            return GetFieldAttributes<T>("");
        }

        public static Dictionary<string, DataFieldAttribute> GetFieldAttributes<T>(string tableName)
        {
            var fields = new Dictionary<string, DataFieldAttribute>();
            var dataTableAttribute =
                (DataTableAttribute)System.Attribute.GetCustomAttribute(typeof(T), typeof(DataTableAttribute));
            var fieldPrefix = string.Empty;
            if (null != dataTableAttribute)
                fieldPrefix = dataTableAttribute.FieldPrefix;

            foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var dataFieldAttribute =
                    (DataFieldAttribute)System.Attribute.GetCustomAttribute(propertyInfo, typeof(DataFieldAttribute));

                string fieldName;
                if (null == dataFieldAttribute || dataFieldAttribute.FieldName.Equals(string.Empty))
                {
                    fieldName = propertyInfo.Name;
                }
                else
                {
                    fieldName = dataFieldAttribute.FieldName;
                }

                if (null == dataFieldAttribute) dataFieldAttribute = new DataFieldAttribute();

                dataFieldAttribute.FieldName = string.IsNullOrEmpty(tableName)
                                                   ? fieldPrefix + fieldName
                                                   : tableName + "." + fieldPrefix + fieldName;

                if (dataFieldAttribute.ColumnType == DbType.Object)
                {
                    dataFieldAttribute.ColumnType = TypeHelper.CSharpTypeNameToDbType(propertyInfo.PropertyType);
                }

                fields.Add(propertyInfo.Name, dataFieldAttribute);
            }

            return fields;
        }

        private static readonly Dictionary<string, string> properties = new Dictionary<string, string>();
        public static string GetPropertyName<T>(Expression<Func<T, object>> predicate)
        {
            var fieldName = "";

            if (properties.ContainsKey(predicate.ToString()))
            {
                return properties[predicate.ToString()];
            }

            if (predicate.Body is UnaryExpression)
            {
                fieldName = ((MemberExpression)((UnaryExpression)predicate.Body).Operand).Member.Name;
            }
            else if (predicate.Body is MemberExpression)
            {
                fieldName = ((MemberExpression)predicate.Body).Member.Name;
            }
            else if (predicate.Body is ParameterExpression)
            {
                fieldName = predicate.Body.Type.Name;
            }

            properties.Add(predicate.ToString(), fieldName);

            return fieldName;
        }

        //public static string GetSelectList<T>()
        //{
        //    var selectList = new StringBuilder();
        //    var dataTableAttribute =
        //        (DataTableAttribute) System.Attribute.GetCustomAttribute(typeof (T), typeof (DataTableAttribute));
        //    var fieldPrefix = string.Empty;
        //    if (null != dataTableAttribute)
        //        fieldPrefix = dataTableAttribute.FieldPrefix;

        //    foreach (var propertyInfo in typeof (T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        //    {
        //        var dataFieldAttribute =
        //            (DataFieldAttribute) System.Attribute.GetCustomAttribute(propertyInfo, typeof (DataFieldAttribute));

        //        string fieldName;
        //        if (null == dataFieldAttribute || dataFieldAttribute.FieldName.Equals(string.Empty))
        //        {
        //            fieldName = propertyInfo.Name;
        //        }
        //        else
        //        {
        //            fieldName = dataFieldAttribute.FieldName;
        //        }

        //        selectList.Append(fieldPrefix + fieldName);
        //        selectList.Append(",");
        //    }

        //    return selectList.ToString(0, selectList.Length - 1);
        //}

        public static List<T> EntityParse<T>(IDataReader reader) where T : new()
        {
            var listT = new List<T>();
            using (reader)
            {
                while (reader.Read())
                {
                    var inst = new T();

                    var dataTableAttribute =
                        (DataTableAttribute)
                        System.Attribute.GetCustomAttribute(typeof(T), typeof(DataTableAttribute));
                    var fieldPrefix = string.Empty;
                    if (null != dataTableAttribute)
                        fieldPrefix = dataTableAttribute.FieldPrefix;

                    foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        var dataFieldAttribute = (
                                                 DataFieldAttribute)
                                                 System.Attribute.GetCustomAttribute(propertyInfo,
                                                                                     typeof(DataFieldAttribute));

                        object obj;
                        try
                        {
                            string fieldName;
                            if (null == dataFieldAttribute || dataFieldAttribute.FieldName.Equals(string.Empty))
                            {
                                fieldName = propertyInfo.Name;
                            }
                            else
                            {
                                fieldName = dataFieldAttribute.FieldName;
                            }

                            if (null == dataFieldAttribute || dataFieldAttribute.UsePrefix)
                            {
                                obj = reader[fieldPrefix + fieldName];
                            }
                            else
                            {
                                obj = reader[fieldName];
                            }
                            
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        if (obj == DBNull.Value || obj == null)
                            continue;
                        var si = propertyInfo.GetSetMethod();
                        if (si == null)
                            continue;

                        if (propertyInfo.PropertyType.FullName.Equals("System.Boolean"))
                        {
                            if (null != dataFieldAttribute && !dataFieldAttribute.ConvertFormat.Equals(string.Empty))
                            {
                                obj = obj.ToString().ToBooleanByFormat(dataFieldAttribute.ConvertFormat);
                            }
                        }

                        propertyInfo.SetValue(inst, obj, null);
                    }
                    listT.Add(inst);
                }
            }
            return listT;
        }
    }
}