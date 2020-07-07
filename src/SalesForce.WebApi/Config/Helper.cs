using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using SalesForce.WebApi.Models;

namespace SalesForce.WebApi.Config
{
    public static class Helper
    {
        public static T ToObject<T>(this IDictionary<string, object> source)
        where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType
                         .GetProperty(item.Key)
                         .SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        public static IDictionary<string, object> ToDictionary(this object source)
        {
            return source.ToDictionary<object>();
        }

        public static IDictionary<string, T> ToDictionary<T>(this object source)
        {
            if (source == null)
                ThrowExceptionWhenSourceArgumentIsNull();

            var dictionary = new Dictionary<string, T>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
                AddPropertyToDictionary<T>(property, source, dictionary);
            return dictionary;
        }

        private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, Dictionary<string, T> dictionary)
        {
            object value = property.GetValue(source);
            if (IsOfType<T>(value))
                dictionary.Add(property.Name, (T)value);
        }

        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }

        private static void ThrowExceptionWhenSourceArgumentIsNull()
        {
            throw new ArgumentNullException("source", "Unable to convert object to a dictionary. The source object is null.");
        }

        public static ResponseRemoveKey RemoveKeyToObject(object obj, string key)
        {
            var dic = obj.ToDictionary();
            string keyValue = "";

            if (dic.ContainsKey(key))
            {
                keyValue = dic[key]?.ToString();
                dic.Remove(key);
            }

            string json = JsonConvert.SerializeObject(
                dic,
                Formatting.Indented,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            var newObj = JsonConvert.DeserializeObject(json);

            return new ResponseRemoveKey(newObj, keyValue);
        }
    }
}