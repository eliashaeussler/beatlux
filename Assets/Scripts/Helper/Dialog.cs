using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dialog : MonoBehaviour {

	// Game Objects
	public GameObject dialog;
	public GameObject wrapper;

	// UI elements
	Text heading;
	Text info;
	InputField inputField;
	Text text;
	public Button ButtonOK { get; private set; }
	Button buttonCancel;



	void Start ()
	{
		// Get UI components
		heading = wrapper.transform.Find ("Header/Heading").GetComponent<Text> ();
		info = wrapper.transform.Find ("Main/Info").GetComponent<Text> ();
		inputField = wrapper.transform.Find ("Main/InputField").GetComponent<InputField> ();
		text = wrapper.transform.Find ("Main/Text").GetComponent<Text> ();
		ButtonOK = wrapper.transform.Find ("Footer/Button_OK").GetComponent<Button> ();
		buttonCancel = wrapper.transform.Find ("Footer/Button_Cancel").GetComponent<Button> ();

		// Set input maxlength
		inputField.characterLimit = Settings.Input.MaxLength;
	}



	public void SetHeading (string text) {
		heading.text = text;
	}

	public void SetInputField (string input, string placeholder)
	{
		if (dialog != null && wrapper != null && inputField != null)
		{
			// Get elements
			Text inputPlaceholder = inputField.transform.Find ("Placeholder").GetComponent<Text> ();

			// Set contents
			inputField.text = input;
			inputPlaceholder.text = placeholder;

			// Activate input field
			inputField.gameObject.SetActive (true);

			// Focus input field
			FocusInputField ();
		}
	}

	public string GetInputText ()
	{
		if (dialog != null && inputField != null) {
			return inputField.text;
		}

		return null;
	}

	public void SetInfo (string text)
	{
		if (dialog != null && info != null && text.Length > 0)
		{
			// Set text color
			info.color = Settings.Input.InfoColor;

			// Set text
			info.text = text;

			// Make info text visible
			info.gameObject.SetActive (true);
		}
	}

	public void SetText (string content)
	{
		if (dialog != null && wrapper != null && text != null)
		{
			// Set content
			text.text = content;

			// Activate text
			text.gameObject.SetActive (true);
		}
	}

	public Text GetButtonText ()
	{
		if (dialog != null && wrapper != null && ButtonOK != null) {
			return ButtonOK.transform.Find ("Text").GetComponent<Text> ();
		}

		return null;
	}

	public void FocusInputField ()
	{
		if (inputField != null)
		{
			// TODO Not working.
			inputField.ActivateInputField ();
			inputField.Select ();
		}
	}

	public void Submit ()
	{
		if (ButtonOK != null) {
			ButtonOK.onClick.Invoke ();
		}
	}



	public void ShowDialog (string heading, string text)
	{
		// Set heading
		SetHeading (heading);

		// Set text
		SetText (text);

		// Remove all Listeners
		ButtonOK.onClick.RemoveAllListeners ();

		// Add Action Listener
		ButtonOK.onClick.AddListener (HideDialog);

		// Hide cancel button
		buttonCancel.gameObject.SetActive (false);

		// Set active
		dialog.SetActive (true);
	}

	public void HideDialog ()
	{
		if (dialog != null)
		{
			// Hide dialog
			dialog.SetActive (false);

			// Reset UI elements
			info.gameObject.SetActive (false);
			inputField.gameObject.SetActive (false);
			text.gameObject.SetActive (false);
			buttonCancel.gameObject.SetActive (true);

			// Reset UI contents
			heading.text = "";
			info.text = "";
			inputField.text = "";
			text.text = "";

			// Remove Listener
			wrapper.transform.Find ("Footer/Button_OK").GetComponent<Button> ().onClick.RemoveAllListeners ();
		}
	}
}
