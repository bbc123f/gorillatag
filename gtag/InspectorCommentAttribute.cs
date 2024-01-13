using UnityEngine;

public class InspectorCommentAttribute : PropertyAttribute
{
	public readonly string message;

	public InspectorCommentAttribute(string message = "")
	{
		this.message = message;
	}
}
