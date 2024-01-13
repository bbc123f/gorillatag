using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking;

public class CreditsView : MonoBehaviour
{
	private CreditsSection[] creditsSections;

	public int pageSize = 7;

	private int currentPage;

	private const string PlayFabKey = "CreditsData";

	private int TotalPages => creditsSections.Sum((CreditsSection section) => PagesPerSection(section));

	private void Start()
	{
		creditsSections = new CreditsSection[3]
		{
			new CreditsSection
			{
				Title = "DEV TEAM",
				Entries = new List<string>
				{
					"Anton \"NtsFranz\" Franzluebbers", "Carlo Grossi Jr", "Cody O'Quinn", "David Neubelt", "David \"AA_DavidY\" Yee", "Derek \"DunkTrain\" Arabian", "Elie Arabian", "John Sleeper", "Haunted Army", "Kerestell Smith",
					"Keith \"ElectronicWall\" Taylor", "Laura \"Poppy\" Lorian", "Lilly Tothill", "Matt \"Crimity\" Ostgard", "Nick Taylor", "Ross Furmidge", "Sasha \"Kayze\" Sanders"
				}
			},
			new CreditsSection
			{
				Title = "SPECIAL THANKS",
				Entries = new List<string> { "The \"Sticks\"", "Alpha Squad", "Meta", "Scout House", "Mighty PR", "Caroline Arabian", "Clarissa & Declan", "Calum Haigh", "EZ ICE", "Gwen" }
			},
			new CreditsSection
			{
				Title = "MUSIC BY",
				Entries = new List<string> { "Stunshine", "David Anderson Kirk", "Jaguar Jen", "Audiopfeil", "Owlobe" }
			}
		};
		PlayFabTitleDataCache.Instance.GetTitleData("CreditsData", delegate(string result)
		{
			creditsSections = JsonMapper.ToObject<CreditsSection[]>(result);
		}, delegate(PlayFabError error)
		{
			Debug.Log("Error fetching credits data: " + error.ErrorMessage);
		});
	}

	private int PagesPerSection(CreditsSection section)
	{
		return (int)Math.Ceiling((double)section.Entries.Count / (double)pageSize);
	}

	private IEnumerable<string> PageOfSection(CreditsSection section, int page)
	{
		return section.Entries.Skip(pageSize * page).Take(pageSize);
	}

	private (CreditsSection creditsSection, int subPage) GetPageEntries(int page)
	{
		int num = 0;
		CreditsSection[] array = creditsSections;
		foreach (CreditsSection creditsSection in array)
		{
			int num2 = PagesPerSection(creditsSection);
			if (num + num2 > page)
			{
				int item = page - num;
				return (creditsSection: creditsSection, subPage: item);
			}
			num += num2;
		}
		return (creditsSection: creditsSections.First(), subPage: 0);
	}

	public void ProcessButtonPress(GorillaKeyboardButton buttonPressed)
	{
		string characterString = buttonPressed.characterString;
		if (characterString == "enter")
		{
			currentPage++;
			currentPage %= TotalPages;
		}
	}

	public string GetScreenText()
	{
		return GetPage(currentPage);
	}

	private string GetPage(int page)
	{
		(CreditsSection creditsSection, int subPage) pageEntries = GetPageEntries(page);
		CreditsSection item = pageEntries.creditsSection;
		int item2 = pageEntries.subPage;
		IEnumerable<string> enumerable = PageOfSection(item, item2);
		string value = "CREDITS - " + ((item2 == 0) ? item.Title : (item.Title + " (CONT)"));
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(value);
		stringBuilder.AppendLine();
		foreach (string item3 in enumerable)
		{
			stringBuilder.AppendLine(item3);
		}
		for (int i = 0; i < pageSize - enumerable.Count(); i++)
		{
			stringBuilder.AppendLine();
		}
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("PRESS ENTER TO CHANGE PAGES");
		return stringBuilder.ToString();
	}
}
