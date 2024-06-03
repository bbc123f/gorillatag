using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using LitJson;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking
{
	public class CreditsView : MonoBehaviour
	{
		private int TotalPages
		{
			get
			{
				return this.creditsSections.Sum((CreditsSection section) => this.PagesPerSection(section));
			}
		}

		private void Start()
		{
			this.creditsSections = new CreditsSection[]
			{
				new CreditsSection
				{
					Title = "DEV TEAM",
					Entries = new List<string>
					{
						"Anton \"NtsFranz\" Franzluebbers",
						"Carlo Grossi Jr",
						"Cody O'Quinn",
						"David Neubelt",
						"David \"AA_DavidY\" Yee",
						"Derek \"DunkTrain\" Arabian",
						"Elie Arabian",
						"John Sleeper",
						"Haunted Army",
						"Kerestell Smith",
						"Keith \"ElectronicWall\" Taylor",
						"Laura \"Poppy\" Lorian",
						"Lilly Tothill",
						"Matt \"Crimity\" Ostgard",
						"Nick Taylor",
						"Ross Furmidge",
						"Sasha \"Kayze\" Sanders"
					}
				},
				new CreditsSection
				{
					Title = "SPECIAL THANKS",
					Entries = new List<string>
					{
						"The \"Sticks\"",
						"Alpha Squad",
						"Meta",
						"Scout House",
						"Mighty PR",
						"Caroline Arabian",
						"Clarissa & Declan",
						"Calum Haigh",
						"EZ ICE",
						"Gwen"
					}
				},
				new CreditsSection
				{
					Title = "MUSIC BY",
					Entries = new List<string>
					{
						"Stunshine",
						"David Anderson Kirk",
						"Jaguar Jen",
						"Audiopfeil",
						"Owlobe"
					}
				}
			};
			PlayFabTitleDataCache.Instance.GetTitleData("CreditsData", delegate(string result)
			{
				this.creditsSections = JsonMapper.ToObject<CreditsSection[]>(result);
			}, delegate(PlayFabError error)
			{
				Debug.Log("Error fetching credits data: " + error.ErrorMessage);
			});
		}

		private int PagesPerSection(CreditsSection section)
		{
			return (int)Math.Ceiling((double)section.Entries.Count / (double)this.pageSize);
		}

		private IEnumerable<string> PageOfSection(CreditsSection section, int page)
		{
			return section.Entries.Skip(this.pageSize * page).Take(this.pageSize);
		}

		[return: TupleElementNames(new string[]
		{
			"creditsSection",
			"subPage"
		})]
		private ValueTuple<CreditsSection, int> GetPageEntries(int page)
		{
			int num = 0;
			foreach (CreditsSection creditsSection in this.creditsSections)
			{
				int num2 = this.PagesPerSection(creditsSection);
				if (num + num2 > page)
				{
					int item = page - num;
					return new ValueTuple<CreditsSection, int>(creditsSection, item);
				}
				num += num2;
			}
			return new ValueTuple<CreditsSection, int>(this.creditsSections.First<CreditsSection>(), 0);
		}

		public void ProcessButtonPress(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.enter)
			{
				this.currentPage++;
				this.currentPage %= this.TotalPages;
			}
		}

		public string GetScreenText()
		{
			return this.GetPage(this.currentPage);
		}

		private string GetPage(int page)
		{
			ValueTuple<CreditsSection, int> pageEntries = this.GetPageEntries(page);
			CreditsSection item = pageEntries.Item1;
			int item2 = pageEntries.Item2;
			IEnumerable<string> enumerable = this.PageOfSection(item, item2);
			string value = "CREDITS - " + ((item2 == 0) ? item.Title : (item.Title + " (CONT)"));
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(value);
			stringBuilder.AppendLine();
			foreach (string value2 in enumerable)
			{
				stringBuilder.AppendLine(value2);
			}
			for (int i = 0; i < this.pageSize - enumerable.Count<string>(); i++)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("PRESS ENTER TO CHANGE PAGES");
			return stringBuilder.ToString();
		}

		public CreditsView()
		{
		}

		[CompilerGenerated]
		private int <get_TotalPages>b__4_0(CreditsSection section)
		{
			return this.PagesPerSection(section);
		}

		[CompilerGenerated]
		private void <Start>b__6_0(string result)
		{
			this.creditsSections = JsonMapper.ToObject<CreditsSection[]>(result);
		}

		private CreditsSection[] creditsSections;

		public int pageSize = 7;

		private int currentPage;

		private const string PlayFabKey = "CreditsData";

		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			public <>c()
			{
			}

			internal void <Start>b__6_1(PlayFabError error)
			{
				Debug.Log("Error fetching credits data: " + error.ErrorMessage);
			}

			public static readonly CreditsView.<>c <>9 = new CreditsView.<>c();

			public static Action<PlayFabError> <>9__6_1;
		}
	}
}
