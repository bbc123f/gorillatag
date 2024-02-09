using System;
using UnityEngine;

namespace GorillaTag
{
	public interface IGuidedRefMonoBehaviour : IGuidedRefObject
	{
		Transform transform { get; }
	}
}
