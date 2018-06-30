/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public class Visualization : MonoBehaviour
{
    private static List<VisualizationObj> _tempViz;

    // Color scheme object
    public ColorScheme ColorSchemes;


    // Color schemes
    public List<VisualizationObj> Visualizations = new List<VisualizationObj>();

    private void Start()
    {
        // Select visualizations from database
        Load();

        // Set selected elements
        MenuFunctions.SetSelected();

        // Display visualizations
        Display();

        // Display color schemes
        ColorSchemes.Display();

        // Close database connection
        Database.Close();

        // Show or hide start button
        MenuFunctions.ToggleStart();

        // Set start button
        if (Settings.Active.File != null)
            GameObject.Find("Start/Button/Text").GetComponent<Text>().text =
                Settings.MenuManager.LangManager.getString("continue");
    }


    private void Display()
    {
        if (Visualizations == null) return;

        // Remove all GameObjects
        for (var i = transform.childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);

        foreach (var viz in Visualizations)
        {
            // Create GameObject
            var element = new GameObject("#" + viz.Id);
            element.transform.SetParent(transform);

            var trans = element.AddComponent<RectTransform>();
            trans.pivot = new Vector2(0, 0.5f);

            // Add Layout Element
            var layoutElement = element.AddComponent<LayoutElement>();
            layoutElement.minHeight = 30;
            layoutElement.preferredHeight = 30;

            // Add Horizontal Layout Group
            var hlg = element.AddComponent<HorizontalLayoutGroup>();
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;
            hlg.spacing = 10;
            hlg.childAlignment = TextAnchor.MiddleLeft;


            // Create arrow text GameObject
            var mainArrow = new GameObject("Arrow");
            mainArrow.transform.SetParent(element.transform);

            // Add text
            var mainTextArrow = mainArrow.AddComponent<TextUnicode>();
            mainTextArrow.text = viz.Equals(Settings.Selected.Visualization) ? IconFont.DropdownClosed : "";

            // Set text alignment
            mainTextArrow.alignment = TextAnchor.MiddleLeft;

            // Font settings
            mainTextArrow.font = IconFont.Font;
            mainTextArrow.fontSize = 20;

            // Add Layout Element
            var mainLayoutElementArrow = mainArrow.AddComponent<LayoutElement>();
            mainLayoutElementArrow.minWidth = 22;


            // Create Text GameObject
            var mainText = new GameObject("Text");
            mainText.transform.SetParent(element.transform);

            // Add Text
            var text = mainText.AddComponent<Text>();
            text.color = Color.white;
            text.font = Resources.Load<Font>("Fonts/FuturaStd-Book");
            text.text = viz.Name;
            text.fontSize = 30;

            // Add Button
            var button = mainText.AddComponent<Button>();
            button.transition = Selectable.Transition.Animation;

            // Add OnClick Handler
            var currentViz = viz;
            button.onClick.AddListener(delegate
            {
                if (currentViz.Equals(Settings.Selected.Visualization)) return;

                Settings.Selected.Visualization = currentViz;

                Settings.Selected.ColorScheme = Settings.Selected.Visualization.Equals(Settings.Active.Visualization)
                    ? Settings.Active.ColorScheme
                    : ColorScheme.GetDefault(currentViz);

                ColorSchemes.Display();
                Display();

                // Show or hide start button
                MenuFunctions.ToggleStart();
            });

            // Add Animator
            var animator = mainText.AddComponent<Animator>();
            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/MenuButtons");


            // Create active text GameObject
            var mainActive = new GameObject("Active");
            mainActive.transform.SetParent(element.transform);

            // Add text
            var mainTextActive = mainActive.AddComponent<TextUnicode>();

            if (viz.Equals(Settings.Active.Visualization))
            {
                mainTextActive.text = IconFont.Visualization;
                mainTextActive.fontSize = 30;
                mainTextActive.color = new Color(0.7f, 0.7f, 0.7f);
            }

            // Set text alignment
            mainTextActive.alignment = TextAnchor.MiddleRight;

            // Font settings
            mainTextActive.font = IconFont.Font;

            // Add Layout Element
            var mainLayoutElementActive = mainActive.AddComponent<LayoutElement>();
            mainLayoutElementActive.preferredWidth = 40;
            mainLayoutElementActive.preferredHeight = 30;


            // Set scaling
            element.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }


    //-- DATABASE METHODS

    private void Load()
    {
        if (Database.Connect())
        {
            // Database command
            var cmd = new SqliteCommand(Database.Connection);

            // Query statement
            var sql = "SELECT id,name,colors,buildNumber,skybox FROM visualization ORDER BY name ASC";
            cmd.CommandText = sql;

            // Get sql results
            var reader = cmd.ExecuteReader();

            // Read sql results
            while (reader.Read())
            {
                // Create visualization object
                var obj = new VisualizationObj
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1),
                    Colors = reader.GetInt32(2),
                    BuildNumber = reader.GetInt32(3),
                    Skybox = reader.IsDBNull(4) ? null : reader.GetString(4)
                };

                // Add visualization to visualizations array
                if (Application.CanStreamedLevelBeLoaded(obj.BuildNumber))
                    Visualizations.Add(obj);
            }

            // Close reader
            reader.Close();
            cmd.Dispose();

            // Clone visualizations array
            _tempViz = Visualizations;
        }

        // Close database connection
        Database.Close();
    }

    public static VisualizationObj GetVisualization(long id, bool closeConnection)
    {
        if (Database.Connect())
        {
            // Send database query
            var cmd = new SqliteCommand(Database.Connection)
            {
                CommandText = "SELECT id,name,colors,buildNumber,skybox FROM visualization WHERE id = @ID"
            };

            // Add Parameters to statement
            cmd.Parameters.Add(new SqliteParameter("ID", id));

            var reader = cmd.ExecuteReader();
            VisualizationObj viz = null;

            // Read and add visualization
            while (reader.Read())
                viz = new VisualizationObj
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1),
                    Colors = reader.GetInt32(2),
                    BuildNumber = reader.GetInt32(3),
                    Skybox = reader.IsDBNull(4) ? null : reader.GetString(4)
                };

            // Close reader
            reader.Close();
            cmd.Dispose();
            if (closeConnection) Database.Close();

            return viz;
        }

        // Close database connection
        if (closeConnection) Database.Close();

        return null;
    }


    //-- VISUALIZATION SEARCH

    public void SearchVisualizations(string s)
    {
        // Get Visualization object
        var viz = GameObject.Find("VizContent").GetComponent<Visualization>();

        // Do search or reset
        viz.Visualizations = s.Length > 0
            ? _tempViz.Where(x => x.Name.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0).ToList()
            : _tempViz;

        // Display visualizations
        viz.Display();
    }
}