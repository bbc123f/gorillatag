using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DynamicSceneManagerHelper
{
	internal class SceneSnapshot
	{
		public Dictionary<OVRAnchor, SceneSnapshot.Data> Anchors
		{
			[CompilerGenerated]
			get
			{
				return this.<Anchors>k__BackingField;
			}
		} = new Dictionary<OVRAnchor, SceneSnapshot.Data>();

		public bool Contains(OVRAnchor anchor)
		{
			return this.Anchors.ContainsKey(anchor);
		}

		public SceneSnapshot()
		{
		}

		[CompilerGenerated]
		private readonly Dictionary<OVRAnchor, SceneSnapshot.Data> <Anchors>k__BackingField;

		public class Data
		{
			public Data()
			{
			}

			public List<OVRAnchor> Children;

			public Rect? Rect;

			public Bounds? Bounds;
		}
	}
}
