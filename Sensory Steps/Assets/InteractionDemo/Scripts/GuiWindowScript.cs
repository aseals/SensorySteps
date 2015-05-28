using UnityEngine;
using System.Collections;

public class GuiWindowScript : MonoBehaviour 
{
	public Rect guiWindowRect = new Rect(-140, 40, 140, 420);
	public GUISkin guiSkin;

	private bool button1Clicked = false;
	private bool button2Clicked = false;
	private bool button3Clicked = false;
	private bool button4Clicked = false;

	private string label1Text = string.Empty;
	private string label2Text = string.Empty;


	private void ShowGuiWindow(int windowID) 
	{
		GUILayout.BeginVertical();
		GUILayout.Space(30);

		button1Clicked = GUILayout.Button("Button 1");
		if(button1Clicked)
		{
			label1Text = "Button 1 Clicked.";
		}

		GUILayout.Label(label1Text);

		button2Clicked = GUILayout.Button("Button 2");
		if(button2Clicked)
		{
			label2Text = "Button 2 Clicked.";
		}
		
		GUILayout.Label(label2Text);
		GUILayout.FlexibleSpace();

		button3Clicked = GUILayout.Toggle(button3Clicked, "Check Box 1");
		button4Clicked = GUILayout.Toggle(button4Clicked, "Check Box 2");

		GUILayout.Space(30);

		GUILayout.EndVertical();
		
		// Make the window draggable.
		GUI.DragWindow();
	}
	
	
	void OnGUI()
	{
		Rect windowRect = guiWindowRect;
		if(windowRect.x < 0)
			windowRect.x += Screen.width;
		if(windowRect.y < 0)
			windowRect.y += Screen.height;
		
		GUI.skin = guiSkin;
		guiWindowRect = GUI.Window(1, windowRect, ShowGuiWindow, "GUI Window");
	}
	
}
