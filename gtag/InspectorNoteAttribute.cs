using UnityEngine;

public class InspectorNoteAttribute : PropertyAttribute
{
	public readonly string header;

	public readonly string message;

	public InspectorNoteAttribute(string header, string message = "")
	{
		this.header = header;
		this.message = message;
	}
}
