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
	// Token: 0x020002B0 RID: 688
	public class CreditsView : MonoBehaviour
	{
		// Token: 0x1700010F RID: 271
		// (get) Token: 0x0600122C RID: 4652 RVA: 0x00068494 File Offset: 0x00066694
		private int TotalPages
		{
			get
			{
				return this.creditsSections.Sum((CreditsSection section) => this.PagesPerSection(section));
			}
		}

		// Token: 0x0600122D RID: 4653 RVA: 0x000684B0 File Offset: 0x000666B0
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

		// Token: 0x0600122E RID: 4654 RVA: 0x000686BD File Offset: 0x000668BD
		private int PagesPerSection(CreditsSection section)
		{
			return (int)Math.Ceiling((double)section.Entries.Count / (double)this.pageSize);
		}

		// Token: 0x0600122F RID: 4655 RVA: 0x000686D9 File Offset: 0x000668D9
		private IEnumerable<string> PageOfSection(CreditsSection section, int page)
		{
			return section.Entries.Skip(this.pageSize * page).Take(this.pageSize);
		}

		// Token: 0x06001230 RID: 4656 RVA: 0x000686FC File Offset: 0x000668FC
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

		// Token: 0x06001231 RID: 4657 RVA: 0x00068758 File Offset: 0x00066958
		public void ProcessButtonPress(GorillaKeyboardButton buttonPressed)
		{
			if (buttonPressed.characterString == "enter")
			{
				this.currentPage++;
				this.currentPage %= this.TotalPages;
			}
		}

		// Token: 0x06001232 RID: 4658 RVA: 0x0006878D File Offset: 0x0006698D
		public string GetScreenText()
		{
			return this.GetPage(this.currentPage);
		}

		// Token: 0x06001233 RID: 4659 RVA: 0x0006879C File Offset: 0x0006699C
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

		// Token: 0x04001517 RID: 5399
		private CreditsSection[] creditsSections;

		// Token: 0x04001518 RID: 5400
		public int pageSize = 7;

		// Token: 0x04001519 RID: 5401
		private int currentPage;

		// Token: 0x0400151A RID: 5402
		private const string PlayFabKey = "CreditsData";
	}
}
