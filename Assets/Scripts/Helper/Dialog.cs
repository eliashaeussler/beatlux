using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

/// <summary>
/// Provides methods to handle UI dialogs.
/// </summary>
/// <remarks>
/// == Usage: Basic message. ==
/// Dialog dialog;
/// dialog.ShowDialog("This is the headline.", "Here you can type content.");
/// 
/// == Usage: Input field. ==
/// Dialog dialog;
/// dialog.SetHeading("This is the headline.");
/// dialog.SetInputField("Input string", "Placeholder string.");
/// 
/// == Usage: Headline and message (equals basic message) ==
/// Dialog dialog;
/// dialog.SetHeadline("This is the headline.");
/// dialog.SetText("Here you can type content.");
/// 
/// == Usage: Additional info above input field. ==
/// Dialog dialog;
/// dialog.SetHeading("This is the heading.");
/// dialog.SetInputField("Input string", "Placeholder string.");
/// dialog.SetInfo("Here you can type an error message.");
/// 
/// </remarks>
public class Dialog : MonoBehaviour {

	/// <summary>
	/// The dialog GameObject.
	/// </summary>
	public GameObject dialog;

	/// <summary>
	/// The dialog wrapper <see cref="UnityEngine.GameObject" /> object.
	/// </summary>
	public GameObject wrapper;

	/// <summary>
	/// The headline <see cref="UnityEngine.UI.Text" /> object.
	/// </summary>
	Text heading;

	/// <summary>
	/// The dialog <see cref="UnityEngine.UI.InputField" /> object.
	/// </summary>
	InputField inputField;

	/// <summary>
	/// The dialog info <see cref="UnityEngine.UI.Text" /> object.
	/// </summary>
	Text info;

	/// <summary>
	/// The dialog text <see cref="UnityEngine.UI.Text" /> object.
	/// </summary>
	Text text;

	/// <summary>
	/// Gets the dialog confirm button.
	/// </summary>
	/// <value>The dialog confirm button.</value>
	public Button ButtonOK { get; private set; }

	/// <summary>
	/// The dialog cancel button.
	/// </summary>
	Button buttonCancel;


	/// <summary>
	/// Start this instance and set all UI elements of this dialog.
	/// </summary>
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

	/// <summary>
	/// Sets the headline of this dialog.
	/// </summary>
	/// <param name="text">The headline of this dialog to be set.</param>
	public void SetHeading (string text) {
		heading.text = text;
	}

	/// <summary>
	/// Sets the input field of this dialog. This activated the input field in the UI.
	/// </summary>
	/// <param name="input">The value of the input field.</param>
	/// <param name="placeholder">The placeholder value of the input field.</param>
	public void SetInputField (string input, string placeholder)
	{
		if (dialog != null && wrapper != null && inputField != null)
		{
			// Get placeholder text element
			Text inputPlaceholder = inputField.transform.Find ("Placeholder").GetComponent<Text> ();

			// Set values
			inputField.text = input;
			inputPlaceholder.text = placeholder;

			// Activate input field
			inputField.gameObject.SetActive (true);

			// Focus input field
			FocusInputField ();
		}
	}

	/// <summary>
	/// Gets the input text of the input field.
	/// </summary>
	/// <returns>The input text of the input field.</returns>
	public string GetInputText ()
	{
		if (dialog != null && inputField != null) {
			return inputField.text;
		}

		return null;
	}

	/// <summary>
	/// Sets the info text above the input field.
	/// </summary>
	/// <param name="text">The info text above the input field.</param>
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

	/// <summary>
	/// Sets the main message of this dialog.
	/// </summary>
	/// <param name="content">The main message of this dialog.</param>
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

	/// <summary>
	/// Gets the confirm button text of this dialog.
	/// </summary>
	/// <returns>The confirm button text of this dialog.</returns>
	public Text GetButtonText ()
	{
		if (dialog != null && wrapper != null && ButtonOK != null) {
			return ButtonOK.transform.Find ("Text").GetComponent<Text> ();
		}

		return null;
	}

	/// <summary>
	/// NOTE: METHOD IS NOT WORKING.
	/// Focuses the input field inside this dialog.
	/// </summary>
	public void FocusInputField ()
	{
		if (inputField != null)
		{
			inputField.ActivateInputField ();
			inputField.Select ();
		}
	}

	/// <summary>
	/// Submit this dialog while pressing the confirm button.
	/// </summary>
	public void Submit ()
	{
		if (ButtonOK != null) {
			ButtonOK.onClick.Invoke ();
		}
	}

	/// <summary>
	/// Shows the dialog.
	/// </summary>
	/// <param name="heading">The headline of this dialog.</param>
	/// <param name="text">The main message of this dialog.</param>
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

	/// <summary>
	/// Hides the dialog.
	/// </summary>
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
