using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
                (DataTableAttribute) System.Attribute.GetCustomAttribute(typeof (T), typeof (DataTableAttribute));
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
            var fields = new Dictionary<string, DataFieldAttribute>();
            var dataTableAttribute =
                (DataTableAttribute) System.Attribute.GetCustomAttribute(typeof (T), typeof (DataTableAttribute));
            var fieldPrefix = string.Empty;
            if (null != dataTableAttribute)
                fieldPrefix = dataTableAttribute.FieldPrefix;

            foreach (var propertyInfo in typeof (T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var dataFieldAttribute =
                    (DataFieldAttribute) System.Attribute.GetCustomAttribute(propertyInfo, typeof (DataFieldAttribute));

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
                dataFieldAttribute.FieldName = fieldPrefix + fieldName;

                if (dataFieldAttribute.ColumnType == DbType.Object)
                {
                    dataFieldAttribute.ColumnType = TypeHelper.CSharpTypeNameToDbType(propertyInfo.PropertyType);
                }

                fields.Add(propertyInfo.Name, dataFieldAttribute);
            }

            return fields;
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
                        System.Attribute.GetCustomAttribute(typeof (T), typeof (DataTableAttribute));
                    var fieldPrefix = string.Empty;
                    if (null != dataTableAttribute)
                        fieldPrefix = dataTableAttribute.FieldPrefix;

                    foreach (var propertyInfo in typeof (T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        var dataFieldAttribute = (
                                                 DataFieldAttribute)
                                                 System.Attribute.GetCustomAttribute(propertyInfo,
                                                                                     typeof (DataFieldAttribute));

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
                            obj = reader[fieldPrefix + fieldName];
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