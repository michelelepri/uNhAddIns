using System;
using System.Collections.Generic;

namespace uNhAddIns.Inflector
{
	/// <summary>
	/// Inflector for pluralize and singularize Spanish nouns.
	/// </summary>
	/// <remarks>
	/// Inspired from Bermi Ferrer Martinez python implementation.
	/// </remarks>
	public class SpanishInflector : AbstractInflector
	{
		private class RuleReplacement
		{
			public string Rule { get; set; }
			public string Replacement { get; set; }
		}

		public SpanishInflector()
		{
			AddPlural("$", "es"); // ELSE +es (v.g. �rbol->�rboles)
			AddPlural("(ng|[mwckgtp])$", "$1s"); // Anglicismos como puenting, frac, crack, show, item 
			AddPlural("([��])$", "$1es"); // ceut�->ceut�es, tab�->tab�es
			AddPlural("z$", "ces"); // luz->luces
			AddPlural("([aeiou])s$", "$1s"); // atlas->atlas, virus->virus, etc.
			AddPlural("([aeiou���])$", "$1s"); // casa->casas, padre->padres, pap�->pap�s
			AddPlural("(^[bcdfghjklmn�pqrstvwxyz]*)([aeiou])([ns])$", "$1$2$3es"); // tren->trenes
			AddPlural("(^[bcdfghjklmn�pqrstvwxyz]*)an$", "$1anes"); // clan->clanes
			AddPluralForEach("�����", "aeiou", '#', "(#)([ns])$", "#$2es");
			AddPlural("([aeiou])x$", "$1x");

			AddSingular("es$", ""); // ELSE remove _es_  monitores->monitor
			AddSingular("([gh�pv]e)s$", "$1");
			AddSingular("([bcdfghjklmn�prstvwxyz]{2,}e)s$", "$1");
			AddSingular("([^e])s$", "$1");
			AddSingular("(�)s$", "$1");
			AddSingular("(sis|tis|xis)+$", "$1");
			AddSingular("(ces)$", "z");
			AddSingular("oides$", "oide");
			AddSingularForEach("aeiou", "�����", '#', "(#)([ns])es$", "#$2");
			AddSingular("^([bcdfghjklmn�pqrstvwxyz]*)([aeiou])([ns])es$", "$1$2$3");

			AddIrregular("pa�s", "pa�ses");
			AddIrregular("jersey", "jers�is");
			AddIrregular("esp�cimen", "espec�menes");
			AddIrregular("car�cter", "caracteres");
			AddIrregular("men�", "men�s");
			AddIrregular("r�gimen", "reg�menes");
			AddIrregular("curriculum", "curr�culos");
			AddIrregular("ultim�tum", "ultimatos");
			AddIrregular("memor�ndum", "memorandos");
			AddIrregular("refer�ndum", "referendos");
			AddIrregular("s�ndwich", "s�ndwiches");

			AddUncountable("paraguas");
			AddUncountable("tijeras");
			AddUncountable("gafas");
			AddUncountable("vacaciones");
			AddUncountable("v�veres");
			AddUncountable("lunes");
			AddUncountable("martes");
			AddUncountable("mi�rcoles");
			AddUncountable("jueves");
			AddUncountable("viernes");
			AddUncountable("cumplea�os");
			AddUncountable("virus");
			AddUncountable("atlas");
			AddUncountable("sms");
			AddUncountable("d�ficit");
		}

		protected override void AddIrregular(string singular, string plural)
		{
			base.AddIrregular(singular, plural);
			base.AddIrregular(Unaccent(singular), Unaccent(plural));
		}

		protected override void AddUncountable(string word)
		{
			base.AddUncountable(word);
			base.AddUncountable(Unaccent(word));
		}

		private void AddPluralForEach(string charectersToMatch, string charectersToReplace, char wildChar, string ruleTemplate,
		                              string replacementTemplate)
		{
			IEnumerable<RuleReplacement> e = RulesReplacements(charectersToMatch, charectersToReplace, wildChar, ruleTemplate,
			                                                   replacementTemplate);
			foreach (var element in e)
			{
				AddPlural(element.Rule, element.Replacement);
			}
		}

		private void AddSingularForEach(string charectersToMatch, string charectersToReplace, char wildChar,
		                                string ruleTemplate, string replacementTemplate)
		{
			IEnumerable<RuleReplacement> e = RulesReplacements(charectersToMatch, charectersToReplace, wildChar, ruleTemplate,
			                                                   replacementTemplate);
			foreach (var element in e)
			{
				AddSingular(element.Rule, element.Replacement);
			}
		}

		private static IEnumerable<RuleReplacement> RulesReplacements(string charectersToMatch, string charectersToReplace,
		                                                              char wildChar, string ruleTemplate,
		                                                              string replacementTemplate)
		{
			if (charectersToMatch.Length != charectersToReplace.Length)
			{
				throw new ArgumentException("charectersToMatch and charectersToReplace must have same length", charectersToReplace);
			}
			for (int i = 0; i < charectersToMatch.Length; i++)
			{
				string rule = ruleTemplate.Replace(wildChar, charectersToMatch[i]);
				string replacement = replacementTemplate.Replace(wildChar, charectersToReplace[i]);
				yield return new RuleReplacement {Rule = rule, Replacement = replacement};
			}
		}

		#region Overrides of AbstractInflector

		public override string Ordinalize(string number)
		{
			return number + "o.";
		}

		#endregion
	}
}