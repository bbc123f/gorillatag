﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class DevErrorSoundAnnoyer : MonoBehaviour
{
	public DevErrorSoundAnnoyer()
	{
	}

	[SerializeField]
	private AudioClip errorSound;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private Text errorUIText;

	[SerializeField]
	private Font errorFont;

	public string displayedText;
}
