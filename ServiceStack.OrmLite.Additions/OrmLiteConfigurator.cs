using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace ServiceStack.OrmLite.Additions
{
    public static class OrmLiteConfigurator
    {
        public class ModelDefinition<T> : ModelDefinition { }

        private static Dictionary<Type, ModelDefinition> _typeModelDefinitionMap = null;

        private static Dictionary<Type, ModelDefinition> GetConfigMap()
        {
            var fieldInfo = (FieldInfo)typeof(OrmLiteConfig).Assembly.GetType("ServiceStack.OrmLite.OrmLiteConfigExtensions")
                .GetMember("typeModelDefinitionMap", MemberTypes.Field, BindingFlags.Static | BindingFlags.NonPublic).First();

            return (Dictionary<Type, ModelDefinition>)fieldInfo.GetValue(null);
        }

        /// <summary>
        /// Gets the ModelDefinition of a type if it already exists in the typemap, otherwise creates it through OrmLite's normal method of attribute inspection.
        /// </summary>
        /// <typeparam name="T">They model type.</typeparam>
        /// <returns>The ModelDefinition object.</returns>
        public static ModelDefinition Init<T>()
        {
            var method = (MethodInfo)typeof(OrmLiteConfig).Assembly.GetType("ServiceStack.OrmLite.OrmLiteConfigExtensions")
                .GetMember("Init", MemberTypes.Method, BindingFlags.Static | BindingFlags.Public).First();

            return (ModelDefinition)method.Invoke(null, new[] { typeof(T) });
        }

        public static Dictionary<Type, ModelDefinition> TypeModelDefinitionMap 
        {
            get { return _typeModelDefinitionMap ?? (_typeModelDefinitionMap = GetConfigMap()); }
        }

        public static ModelDefinition GetModelDefinition<T>()
        {
            ModelDefinition map;
            TypeModelDefinitionMap.TryGetValue(typeof(T), out map);
            return map;
        }

        public static ModelDefinition<T> AddModelDefinition<T>(this Dictionary<Type, ModelDefinition> dict, string alias = null)
        {
            var modelType = typeof(T);
            var def = new ModelDefinition<T>
            {
                ModelType = modelType,
                Name = modelType.Name,
                Alias = alias ?? modelType.Name,
            };
            dict.Add(modelType, def);
            return def;
        }

        public static ModelDefinition<T> AddField<T>(this ModelDefinition<T> modelDef, Expression<Func<T, object>> exp, bool isNullable = false, bool isPrimaryKey = false, bool isAutoIncrement = false,
            string alias = null, string defaultValue = null, Type references = null, string onDelete = null, string onUpdate = null)
        {
            var propInfo = modelDef.ModelType.GetPropertyInfo(exp.Name());

            modelDef.FieldDefinitions.Add(new FieldDefinition
            {
                Name = alias ?? propInfo.Name,
                FieldType = propInfo.PropertyType,
                PropertyInfo = propInfo,
                GetValueFn = propInfo.GetPropertyGetterFn(),
                SetValueFn = propInfo.GetPropertySetterFn(),
                IsNullable = isNullable,
                IsPrimaryKey = isPrimaryKey,
                AutoIncrement = isAutoIncrement,
                ForeignKey = references == null ? null : new ForeignKeyConstraint(references, onDelete, onUpdate),
                DefaultValue = defaultValue,
            });

            return modelDef;
        }

        public static ModelDefinition<T> AddIgnoredField<T>(this ModelDefinition<T> modelDef, Expression<Func<T, object>> exp)
        {
            var propInfo = modelDef.ModelType.GetPropertyInfo(exp.Name());

            modelDef.IgnoredFieldDefinitions.Add(new FieldDefinition
            {
                Name = propInfo.Name,
                FieldType = propInfo.PropertyType,
                PropertyInfo = propInfo,
                GetValueFn = propInfo.GetPropertyGetterFn(),
                SetValueFn = propInfo.GetPropertySetterFn(),
            });

            return modelDef;
        }

        public static string Name<T>(this Expression<Func<T, object>> exp)
        {
            MemberExpression body = exp.Body as MemberExpression;
            if (body == null)
            {
                UnaryExpression ubody = (UnaryExpression)exp.Body;
                body = ubody.Operand as MemberExpression;
            }

            return body.Member.Name;
        }
    }
}
