using System;
using System.Globalization;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	[CreateAssetMenu(fileName = "SeasonInfo", menuName = "Gorilla Tag/Season Info", order = 0)]
	public class OldPoop_SeasonInfoSO : ScriptableObject
	{
		private string LogPrefix
		{
			get
			{
				return "OldPoop_SeasonInfoSO, \"" + base.name + ".asset\": ";
			}
		}

		protected void Awake()
		{
			if (Application.isPlaying && this.TryParseReleaseDate())
			{
				Debug.LogError(this.LogPrefix + this.GetDateParseErrorMsg(), this);
			}
		}

		private bool TryParseReleaseDate()
		{
			return DateTime.TryParseExact(this._releaseDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out this.releaseDate) || DateTime.TryParse(this._releaseDateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out this.releaseDate);
		}

		private string GetDateParseErrorMsg()
		{
			return "Could not parse _releaseDateString value \"" + this._releaseDateString + "\" expected format is \"yyyy-MM-dd\".";
		}

		private const string kDateFormat = "yyyy-MM-dd";

		public string displayName;

		[Tooltip("Planned release date. Format: \"yyyy-MM-dd\". Example: 2023-08-25")]
		[Delayed]
		[SerializeField]
		private string _releaseDateString;

		[NonSerialized]
		public DateTime releaseDate;
	}
}
