using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Dialog {

	// Dialog
	public GameObject dialog { get; set; }
	public GameObject wrapper { get; set; }

	// UI elements
	Transform main;
	InputField inputField;
	Text text;



	public Dialog (GameObject dialog)
	{
		// Set wrapper
		this.dialog = dialog;
		this.wrapper = dialog.transform.Find ("DialogWrapper").gameObject;

		// Get UI components
		main = wrapper.transform.Find ("Main");
		inputField = main.Find ("InputField").gameObject.GetComponent<InputField> ();
		text = main.Find ("Text").gameObject.GetComponent<Text> ();
	}



	public InputField AddInputField (string input, string placeholder)
	{
		if (dialog != null && wrapper != null && inputField != null)
		{
			// Get elements
			Text inputText = inputField.gameObject.transform.Find ("Text").GetComponent<Text> ();
			Text inputPlaceholder = inputField.gameObject.transform.Find ("Placeholder").GetComponent<Text> ();

			// Set contents
			inputField.text = input;
			inputText.text = input;
			inputPlaceholder.text = placeholder;

			// Activate input field
			inputField.gameObject.SetActive (true);

			return inputField;
		}

		return null;
	}

	public Text AddText (string content)
	{
		if (dialog != null && wrapper != null && text != null)
		{
			// Set content
			text.text = content;

			// Activate text
			text.gameObject.SetActive (true);

			return text;
		}

		return null;
	}

	public void HideDialog ()
	{
		if (dialog != null)
		{
			// Hide dialog
			dialog.SetActive (false);

			// Reset UI elements
			inputField.gameObject.SetActive (false);
			text.gameObject.SetActive (false);
		}
	}
}
