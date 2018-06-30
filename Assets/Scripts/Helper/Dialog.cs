/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using UnityEngine.UI;

/// <inheritdoc />
/// <summary>
///     Provides methods to handle UI dialogs.
/// </summary>
/// <remarks>
///     == Usage: Basic message. ==
///     Dialog dialog;
///     dialog.ShowDialog("This is the headline.", "Here you can type content.");
///     == Usage: Input field. ==
///     Dialog dialog;
///     dialog.SetHeading("This is the headline.");
///     dialog.SetInputField("Input string", "Placeholder string.");
///     == Usage: Headline and message (equals basic message) ==
///     Dialog dialog;
///     dialog.SetHeadline("This is the headline.");
///     dialog.SetText("Here you can type content.");
///     == Usage: Additional info above input field. ==
///     Dialog dialog;
///     dialog.SetHeading("This is the heading.");
///     dialog.SetInputField("Input string", "Placeholder string.");
///     dialog.SetInfo("Here you can type an error message.");
/// </remarks>
public class Dialog : MonoBehaviour
{
    public enum Type
    {
        ColorSchemeAdd = 0,
        ColorSchemeEdit = 1,
        ColorSchemeDelete = 2,
        PlaylistAdd = 10,
        PlaylistEdit = 11,
        PlaylistDelete = 12
    }

	/// <summary>
	///     The dialog cancel button.
	/// </summary>
	private Button _buttonCancel;

	/// <summary>
	///     The headline <see cref="UnityEngine.UI.Text" /> object.
	/// </summary>
	private Text _heading;

	/// <summary>
	///     The dialog info <see cref="UnityEngine.UI.Text" /> object.
	/// </summary>
	private Text _info;

	/// <summary>
	///     The dialog <see cref="UnityEngine.UI.InputField" /> object.
	/// </summary>
	private InputField _inputField;

	/// <summary>
	///     The dialog text <see cref="UnityEngine.UI.Text" /> object.
	/// </summary>
	private Text _text;

	/// <summary>
	///     The dialog GameObject.
	/// </summary>
	public GameObject Element;

	/// <summary>
	///     The dialog wrapper <see cref="UnityEngine.GameObject" /> object.
	/// </summary>
	public GameObject Wrapper;

	/// <summary>
	///     Gets the dialog confirm button.
	/// </summary>
	/// <value>The dialog confirm button.</value>
	public Button ButtonOk { get; private set; }


	/// <summary>
	///     Start this instance and set all UI elements of this dialog.
	/// </summary>
	private void Start()
    {
        // Get UI components
        _heading = Wrapper.transform.Find("Header/Heading").GetComponent<Text>();
        _info = Wrapper.transform.Find("Main/Info").GetComponent<Text>();
        _inputField = Wrapper.transform.Find("Main/InputField").GetComponent<InputField>();
        _text = Wrapper.transform.Find("Main/Text").GetComponent<Text>();
        ButtonOk = Wrapper.transform.Find("Footer/Button_OK").GetComponent<Button>();
        _buttonCancel = Wrapper.transform.Find("Footer/Button_Cancel").GetComponent<Button>();

        // Set input maxlength
        _inputField.characterLimit = Settings.Input.MaxLength;
    }

	/// <summary>
	///     Sets the headline of this dialog.
	/// </summary>
	/// <param name="text">The headline of this dialog to be set.</param>
	public void SetHeading(string text)
    {
        _heading.text = text;
    }

	/// <summary>
	///     Sets the input field of this dialog. This activated the input field in the UI.
	/// </summary>
	/// <param name="input">The value of the input field.</param>
	/// <param name="placeholder">The placeholder value of the input field.</param>
	public void SetInputField(string input, string placeholder)
    {
        if (Element == null || Wrapper == null || _inputField == null) return;

        // Get placeholder text element
        var inputPlaceholder = _inputField.transform.Find("Placeholder").GetComponent<Text>();

        // Set values
        _inputField.text = input;
        inputPlaceholder.text = placeholder;

        // Activate input field
        _inputField.gameObject.SetActive(true);

        // Focus input field
        FocusInputField();
    }

	/// <summary>
	///     Gets the input text of the input field.
	/// </summary>
	/// <returns>The input text of the input field.</returns>
	public string GetInputText()
    {
        if (Element != null && _inputField != null) return _inputField.text;

        return null;
    }

	/// <summary>
	///     Sets the info text above the input field.
	/// </summary>
	/// <param name="text">The info text above the input field.</param>
	public void SetInfo(string text)
    {
        if (Element == null || _info == null || text.Length <= 0) return;

        // Set text color
        _info.color = Settings.Input.InfoColor;

        // Set text
        _info.text = text;

        // Make info text visible
        _info.gameObject.SetActive(true);
    }

	/// <summary>
	///     Sets the main message of this dialog.
	/// </summary>
	/// <param name="content">The main message of this dialog.</param>
	public void SetText(string content)
    {
        if (Element == null || Wrapper == null || _text == null) return;

        // Set content
        _text.text = content;

        // Activate text
        _text.gameObject.SetActive(true);
    }

	/// <summary>
	///     Gets the confirm button text of this dialog.
	/// </summary>
	/// <returns>The confirm button text of this dialog.</returns>
	public Text GetButtonText()
    {
        if (Element != null && Wrapper != null && ButtonOk != null)
            return ButtonOk.transform.Find("Text").GetComponent<Text>();

        return null;
    }

	/// <summary>
	///     NOTE: METHOD IS NOT WORKING.
	///     Focuses the input field inside this dialog.
	/// </summary>
	private void FocusInputField()
    {
        if (_inputField == null) return;

        _inputField.ActivateInputField();
        _inputField.Select();
    }

	/// <summary>
	///     Submit this dialog while pressing the confirm button.
	/// </summary>
	public void Submit()
    {
        if (ButtonOk != null) ButtonOk.onClick.Invoke();
    }

	/// <summary>
	///     Shows the dialog.
	/// </summary>
	/// <param name="heading">The headline of this dialog.</param>
	/// <param name="text">The main message of this dialog.</param>
	public void ShowDialog(string heading, string text)
    {
        // Set heading
        SetHeading(heading);

        // Set text
        SetText(text);

        // Remove all Listeners
        ButtonOk.onClick.RemoveAllListeners();

        // Add Action Listener
        ButtonOk.onClick.AddListener(HideDialog);

        // Hide cancel button
        _buttonCancel.gameObject.SetActive(false);

        // Set active
        Element.SetActive(true);
    }

	/// <summary>
	///     Hides the dialog.
	/// </summary>
	public void HideDialog()
    {
        if (Element == null) return;

        // Hide dialog
        Element.SetActive(false);

        // Reset UI elements
        _info.gameObject.SetActive(false);
        _inputField.gameObject.SetActive(false);
        _text.gameObject.SetActive(false);
        _buttonCancel.gameObject.SetActive(true);

        // Reset UI contents
        _heading.text = "";
        _info.text = "";
        _inputField.text = "";
        _text.text = "";

        // Remove Listener
        Wrapper.transform.Find("Footer/Button_OK").GetComponent<Button>().onClick.RemoveAllListeners();
    }
}