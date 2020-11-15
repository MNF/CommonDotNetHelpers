using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CommonDotNetHelpers.Diagnostics
{
    //From https://gist.github.com/StevePy/1237602
    public static class PropertyNameExtensions
    {
        /// <summary>
        /// Extension method for exposing the name of properties without resorting to
        /// magic strings.
        /// Use: objectReference.PropertyName( x => x.{property} ) to retrieve the name.
        /// I.e. objectReference.PropertyName( x => x.IsActive ); //will return "IsActive".
        /// </summary>
        /// <returns>
        /// Property name of the property expression provided.
        /// </returns>
        public static string PropertyName<T, TReturn>(this T obj, Expression<Func<T, TReturn>> property) where T : class
        {
            MemberExpression body = (MemberExpression)property.Body;
            if (body == null)
                throw new ArgumentException("The provided expression did not point to a property.");

            return body.Member.Name;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        /// <example>Assert.IsTrue(resp.OutboundFlightInfo.FlightGroups.Length>0,resp.PropertyNameAndValue(x=> x.OutboundFlightInfo));</example>
        public static string PropertyNameAndValue<T, TReturn>(this T obj, Expression<Func<T, TReturn>> property, string separator = ":") where T : class
        {
            MemberExpression body = (MemberExpression)property.Body;

            if (body == null)

                throw new ArgumentException("The provided expression did not point to a property.");


            String str = String.Format("{0}{2}{1}", body.Member.Name, property.Compile()(obj), separator);
            return str;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        /// <example>resp.NameAndValueAsKeyValuePair(x=> x.OutboundFlightInfo);</example>
        public static KeyValuePair<string, TReturn> NameAndValueAsKeyValuePair<T, TReturn>(this T obj, Expression<Func<T, TReturn>> property) where T : class
        {
            MemberExpression body = (MemberExpression)property.Body;

            if (body == null)

                throw new ArgumentException("The provided expression did not point to a property.");

            var kvp = new KeyValuePair<string, TReturn>(body.Member.Name, property.Compile()(obj));
            return kvp;
        }
    }
}
