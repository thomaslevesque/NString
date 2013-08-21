using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using NString.Internal;
using NString.Properties;

namespace NString
{
    /// <summary>
    /// Represents a string template with named placeholders.
    /// </summary>
    /// <remarks>The template syntax is similar to the one used in String.Format, except that indexes are replaced by names.</remarks>
    public class StringTemplate
    {
        private static readonly Regex _regex = new Regex(@"(?<open>{+)(?<key>\w+)(?<format>:[^}]+)?(?<close>}+)");

        private readonly string _template;
        private readonly string _templateWithIndexes;
        private readonly IList<string> _placeholders;

        private StringTemplate(string template)
        {
            template.CheckArgumentNull("template");
            _template = template;
            ParseTemplate(out _templateWithIndexes, out _placeholders);
        }

        /// <summary>
        /// Parses the provided string into a StringTemplate instance. Parsed templates are cached, so calling this method twice
        /// with the same argument will return the same StringTemplate instance.
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static StringTemplate Parse(string template)
        {
            return GetTemplate(template);
        }

        private void ParseTemplate(out string templateWithIndexes, out IList<string> placeholders)
        {
            var tmp = new List<string>();
            MatchEvaluator evaluator = m =>
            {
                string open = m.Groups["open"].Value;
                string close = m.Groups["close"].Value;
                string key = m.Groups["key"].Value;
                string format = m.Groups["format"].Value;

                if (open.Length % 2 == 0)
                    return m.Value;

                open = RemoveLastChar(open);
                close = RemoveLastChar(close);

                if (!tmp.Contains(key))
                {
                    tmp.Add(key);
                }

                int index = tmp.IndexOf(key);
                return string.Format("{0}{{{1}{2}}}{3}", open, index, format, close);
            };
            templateWithIndexes = _regex.Replace(_template, evaluator);
            placeholders = tmp.AsReadOnly();
        }

        private static string RemoveLastChar(string str)
        {
            if (str.Length > 1)
                return str.Substring(0, str.Length - 1);
            return string.Empty;
        }

        /// <summary>
        /// Convertit une chaine en StringTemplate
        /// </summary>
        /// <param name="s">La chaine à convertir</param>
        /// <returns>Un StringTemplate utilisant la chaine convertie comme template</returns>
        public static implicit operator StringTemplate(string s)
        {
            return GetTemplate(s);
        }

        /// <summary>
        /// Renvoie une chaine représentant cette instance de StringTemplate.
        /// </summary>
        /// <returns>Le template utilisé par ce StringTemplate</returns>
        public override string ToString()
        {
            return _template;
        }

        /// <summary>
        /// Remplace les placeholders du template par les valeurs fournies dans le dictionnaire spécifié
        /// </summary>
        /// <param name="values">Le dictionnaire contenant les valeurs pour les placeholders</param>
        /// <returns>La chaine formatée</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Aucune valeur n'a été trouvée pour
        /// un placeholder du template</exception>
        public string Format(IDictionary<string, object> values)
        {
            return Format(values, true);
        }

        /// <summary>
        /// Remplace les placeholders du template par les valeurs fournies dans le dictionnaire spécifié
        /// </summary>
        /// <param name="values">Le dictionnaire contenant les valeurs pour les placeholders</param>
        /// <param name="throwOnMissingValue">Indique si une exception doit être levée quand aucune valeur
        /// n'est trouvée pour un placeholder du template. Si ce paramètre vaut false, le placeholder est laissé
        /// tel quel dans la chaine formatée.</param>
        /// <returns>La chaine formatée</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Le paramètre <c>throwOnMissingValue</c>
        /// vaut true et aucune valeur n'a été trouvée pour un placeholder du template</exception>
        public string Format(IDictionary<string, object> values, bool throwOnMissingValue)
        {
            values.CheckArgumentNull("values");

            object[] array = new object[_placeholders.Count];
            for (int i = 0; i < _placeholders.Count; i++)
            {
                string key = _placeholders[i];
                object value;
                if (!values.TryGetValue(key, out value))
                {
                    if (throwOnMissingValue)
                        throw new KeyNotFoundException(string.Format(Resources.TemplateKeyNotFound, key));
                    value = string.Format("{{{0}}}", key);
                }
                array[i] = value;
            }
            return string.Format(_templateWithIndexes, array);
        }

        /// <summary>
        /// Remplace les placeholders du template par les valeurs fournies dans l'objet spécifié
        /// </summary>
        /// <param name="values">L'objet contenant les valeurs pour les placeholders. Chaque propriété de
        /// l'objet correspond à un placeholder du template</param>
        /// <returns>La chaine formatée</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Aucune valeur n'a été trouvée pour
        /// un placeholder du template</exception>
        /// <remarks>Cette méthode s'utilise typiquement avec un objet de type anonyme, créé avec la syntaxe
        /// <c>new { nom1 = valeur1, nom2 = valeur2 }</c></remarks>
        public string Format(object values)
        {
            return Format(values, true);
        }

        /// <summary>
        /// Remplace les placeholders du template par les valeurs fournies dans l'objet spécifié
        /// </summary>
        /// <param name="values">L'objet contenant les valeurs pour les placeholders. Chaque propriété de
        /// l'objet correspond à un placeholder du template</param>
        /// <param name="throwOnMissingValue">Indique si une exception doit être levée quand aucune valeur
        /// n'est trouvée pour un placeholder du template. Si ce paramètre vaut false, le placeholder est laissé
        /// tel quel dans la chaine formatée.</param>
        /// <returns>La chaine formatée</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Le paramètre <c>throwOnMissingValue</c>
        /// vaut true et aucune valeur n'a été trouvée pour un placeholder du template</exception>
        /// <remarks>Cette méthode s'utilise typiquement avec un objet de type anonyme, créé avec la syntaxe
        /// <c>new { nom1 = valeur1, nom2 = valeur2 }</c></remarks>
        public string Format(object values, bool throwOnMissingValue)
        {
            values.CheckArgumentNull("values");
            return Format(MakeDictionary(values), throwOnMissingValue);
        }

        private IDictionary<string, object> MakeDictionary(object obj)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (string name in _placeholders)
            {
                object value;
                if (TryGetMemberValue(obj, name, out value))
                    dict.Add(name, value);
            }
            return dict;
        }

        private static bool TryGetMemberValue(object obj, string memberName, out object value)
        {
            Type type = obj.GetType();
            Func<object, object> getter = GetGetterFromCache(type, memberName);
            if (getter != null)
            {
                value = getter(obj);
                return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Remplace les placeholders du template spécifié par les valeurs fournies dans le dictionnaire spécifié
        /// </summary>
        /// <param name="template">Le template à utiliser</param>
        /// <param name="values">Le dictionnaire contenant les valeurs pour les placeholders</param>
        /// <returns>La chaine formatée</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Aucune valeur n'a été trouvée pour
        /// un placeholder du template</exception>
        public static string Format(string template, IDictionary<string, object> values)
        {
            return GetTemplate(template).Format(values);
        }

        /// <summary>
        /// Remplace les placeholders du template spécifié par les valeurs fournies dans le dictionnaire spécifié
        /// </summary>
        /// <param name="template">Le template à utiliser</param>
        /// <param name="values">Le dictionnaire contenant les valeurs pour les placeholders</param>
        /// <param name="throwOnMissingValue">Indique si une exception doit être levée quand aucune valeur
        /// n'est trouvée pour un placeholder du template. Si ce paramètre vaut false, le placeholder est laissé
        /// tel quel dans la chaine formatée.</param>
        /// <returns>La chaine formatée</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Le paramètre <c>throwOnMissingValue</c>
        /// vaut true et aucune valeur n'a été trouvée pour un placeholder du template</exception>
        public static string Format(string template, IDictionary<string, object> values, bool throwOnMissingValue)
        {
            return GetTemplate(template).Format(values, throwOnMissingValue);
        }

        /// <summary>
        /// Remplace les placeholders du template par les valeurs fournies dans l'objet spécifié
        /// </summary>
        /// <param name="template">Le template à utiliser</param>
        /// <param name="values">L'objet contenant les valeurs pour les placeholders. Chaque placeholder du
        /// template est remplacé par la valeur de la propriété ou du champ de même nom</param>
        /// <returns>La chaine formatée</returns>
        /// <remarks>Cette méthode s'utilise typiquement avec un objet de type anonyme, créé avec la syntaxe
        /// <c>new { nom1 = valeur1, nom2 = valeur2 }</c></remarks>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Aucune valeur n'a été trouvée pour
        /// un placeholder du template</exception>
        public static string Format(string template, object values)
        {
            return GetTemplate(template).Format(values);
        }

        /// <summary>
        /// Remplace les placeholders du template par les valeurs fournies dans l'objet spécifié
        /// </summary>
        /// <param name="template">Le template à utiliser</param>
        /// <param name="values">L'objet contenant les valeurs pour les placeholders. Chaque placeholder du
        /// template est remplacé par la valeur de la propriété ou du champ de même nom</param>
        /// <param name="throwOnMissingValue">Indique si une exception doit être levée quand aucune valeur
        /// n'est trouvée pour un placeholder du template. Si ce paramètre vaut false, le placeholder est laissé
        /// tel quel dans la chaine formatée.</param>
        /// <returns>La chaine formatée</returns>
        /// <remarks>Cette méthode s'utilise typiquement avec un objet de type anonyme, créé avec la syntaxe
        /// <c>new { nom1 = valeur1, nom2 = valeur2 }</c></remarks>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Le paramètre <c>throwOnMissingValue</c>
        /// vaut true et aucune valeur n'a été trouvée pour un placeholder du template</exception>
        public static string Format(string template, object values, bool throwOnMissingValue)
        {
            return GetTemplate(template).Format(values, throwOnMissingValue);
        }

        #region Cache

        private static readonly Dictionary<string, StringTemplate> _templateCache =
            new Dictionary<string, StringTemplate>();

        private static StringTemplate GetTemplate(string template)
        {
            StringTemplate stringTemplate;
            if (!_templateCache.TryGetValue(template, out stringTemplate))
            {
                stringTemplate = new StringTemplate(template);
                _templateCache[template] = stringTemplate;
            }
            return stringTemplate;
        }

        private static readonly Dictionary<Type, Dictionary<string, Func<object, object>>> _gettersCache =
            new Dictionary<Type, Dictionary<string, Func<object, object>>>();

        private static Func<object, object> GetGetterFromCache(Type type, string memberName)
        {
            Dictionary<string, Func<object, object>> typeGetters;
            if (!_gettersCache.TryGetValue(type, out typeGetters))
            {
                typeGetters = new Dictionary<string, Func<object, object>>();
                _gettersCache[type] = typeGetters;
            }
            Func<object, object> getter;
            if (!typeGetters.TryGetValue(memberName, out getter))
            {
                getter = CreateGetter(type, memberName);
                typeGetters[memberName] = getter;
            }
            return getter;
        }

        private static Func<object, object> CreateGetter(Type type, string memberName)
        {
            MemberInfo member = null;
            while (type != null)
            {
                var prop = type.GetProperty(memberName, BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
                if (prop != null && prop.CanRead)
                {
                    member = prop;
                    break;
                }
                var field = type.GetField(memberName, BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    member = field;
                    break;
                }
                type = type.BaseType;
            }
            if (member == null)
                return null;
            var param = Expression.Parameter(typeof(object), "x");
            var memberAccess = Expression.MakeMemberAccess(Expression.Convert(param, type), member);
            Expression body = memberAccess;
            if (memberAccess.Type.IsValueType)
                body = Expression.Convert(memberAccess, typeof(object));

            var expr = Expression.Lambda<Func<object, object>>(body, param);
            return expr.Compile();
        }

        #endregion
    }
}
