/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using UnityEngine.UI;

public class Source : MonoBehaviour
{
    private string _oldSource;

    public Transform Text;


    private void Update()
    {
        if (_oldSource == Settings.Source.Current) return;

        // Remove all GameObjects
        for (var i = Text.transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(Text.transform.GetChild(i).gameObject);

        // Get current source
        var dirs = Settings.Source.Current
            .Replace('\\', '/')
            .Trim('/')
            .Split('/');

        for (var i = 0; i < dirs.Length; i++)
        {
            // Create GameObject
            var go = new GameObject(dirs[i]);
            go.transform.SetParent(Text.transform);

            // Add text
            var goText = go.AddComponent<Text>();
            goText.font = Resources.Load<Font>("Fonts/FuturaStd-Book");
            goText.fontSize = 20;
            goText.color = new Color(1, 1, 1, 0.5f);
            goText.text = dirs[i];

            // Set RectTransform
            var trans = go.GetComponent<RectTransform>();
            trans.localScale = Vector3.one;

            if (i >= dirs.Length - 1) continue;

            // Add arrow
            var arrow = new GameObject("Arrow");
            arrow.transform.SetParent(Text.transform);

            var arrowText = arrow.AddComponent<TextUnicode>();
            arrowText.font = IconFont.Font;
            arrowText.fontSize = 15;
            arrowText.color = new Color(1, 1, 1, 0.5f);
            arrowText.alignment = TextAnchor.MiddleCenter;
            arrowText.text = IconFont.DropdownClosed;

            var arrowTrans = arrow.GetComponent<RectTransform>();
            arrowTrans.localScale = Vector3.one;
        }

        // Set old source
        _oldSource = Settings.Source.Current;
    }
}