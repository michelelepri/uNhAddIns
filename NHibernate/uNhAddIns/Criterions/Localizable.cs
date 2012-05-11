using System.Globalization;
using System.Threading;
using NHibernate.Criterion;
using uNhAddIns.UserTypes;

namespace uNhAddIns.Criterions
{
	public static class Localizable
	{
		public static string ToLocalizableLikeClause(this string template)
		{
			return ToLocalizableLikeClause(template, LocalizablePropertyType.DefaultKeyValueEncloser);
		}

		public static string ToLocalizableLikeClause(this string template, char keyValueEncloser)
		{
			return ConvertToLikeClause(Thread.CurrentThread.CurrentCulture, template, keyValueEncloser);
		}

		public static string ConvertToLikeClause(CultureInfo cultureInfo, string template)
		{
			return ConvertToLikeClause(cultureInfo, template, LocalizablePropertyType.DefaultKeyValueEncloser);
		}

		public static string ConvertToLikeClause(CultureInfo cultureInfo, string template, char keyValueEncloser)
		{
			return ConvertToLikeClause(cultureInfo.Name, template, keyValueEncloser);
		}

		public static string ConvertToLikeClause(string cultureInfoName, string template)
		{
			return ConvertToLikeClause(cultureInfoName, template, LocalizablePropertyType.DefaultKeyValueEncloser);
		}

		public static string ConvertToLikeClause(string cultureInfoName, string template, char keyValueEncloser)
		{
			return string.Format("%{0}{1}{0}{0}{2}{0}%", keyValueEncloser, cultureInfoName, template);
		}

		public static AbstractCriterion Like(string propertyName, string cultureInfoName, string template)
		{
			return new LikeExpression(propertyName, ConvertToLikeClause(cultureInfoName, template));
		}

		public static AbstractCriterion Like(string propertyName, string cultureInfoName, string template, char keyValueEncloser)
		{
			return new LikeExpression(propertyName, ConvertToLikeClause(cultureInfoName, template, keyValueEncloser));
		}

		public static AbstractCriterion Like(string propertyName, CultureInfo cultureInfo, string template)
		{
			return new LikeExpression(propertyName, ConvertToLikeClause(cultureInfo, template));
		}

		public static AbstractCriterion Like(string propertyName, CultureInfo cultureInfo, string template, char keyValueEncloser)
		{
			return new LikeExpression(propertyName, ConvertToLikeClause(cultureInfo, template, keyValueEncloser));
		}

		public static AbstractCriterion Like(string propertyName, string template)
		{
			return new LikeExpression(propertyName, template.ToLocalizableLikeClause());
		}

		public static AbstractCriterion Like(string propertyName, string template, char keyValueEncloser)
		{
			return new LikeExpression(propertyName, template.ToLocalizableLikeClause(keyValueEncloser));
		}

		public static AbstractCriterion Like(string propertyName, string template, MatchMode matchMode)
		{
			return new LikeExpression(propertyName, matchMode.ToMatchString(template).ToLocalizableLikeClause(), MatchMode.Exact, null, false);
		}
	}
}