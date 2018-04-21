/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class DropHandler : MonoBehaviour, IDropHandler {

	public void OnDrop (PointerEventData eventData)
	{
		// Get selected game object
		GameObject dropObj = eventData.pointerCurrentRaycast.gameObject;
		GameObject obj = FindParentGameObject (dropObj);

		if (obj != null)
		{
			// Get file path
			string dir = DragHandler.dir;

			// Insert file into playlist or create new playlist with file
			InsertFile (dir, dropObj, obj);
		}
	}

	public static void InsertFile (string dir, GameObject dropObj, GameObject reference)
	{
		// Get reference to playlist object
		Playlist pl = GameObject.Find ("PlaylistContent").GetComponent <Playlist> ();

		// Get file object if available
		FileObj file = pl.GetFile (dir);

		// Get playlist object
		PlaylistObj playlist = null;
		if (reference != dropObj)
		{
			playlist = pl.FindPlaylist (reference);
		}
		else if (pl.Playlists.Count == 0 || Settings.Selected.Playlist == null)
		{
			pl.fileToAdd = new FileObj (dir);
		}
		else if (Settings.Selected.Playlist != null)
		{
			playlist = Settings.Selected.Playlist;
		}

		// Add file to selected playlist
		if (playlist != null)
		{
			if (file == null) {
				file = new FileObj (dir);
			}

			// Add file and show playlist
			long added = pl.AddFile (file, playlist);

			// Get dialog
			Dialog dialog = GameObject.Find ("Dialog").GetComponent<Dialog> ();

			switch (added) {

			// Playlist already contains file
			case (long) Database.Constants.DuplicateFound:

				dialog.ShowDialog (
					"Lied bereits vorhanden",
					"Das ausgewählte Lied ist in der Playlist \"" + playlist.Name + "\" bereits vorhanden."
				);
				break;

				// Query failed
			case (long) Database.Constants.QueryFailed:

				dialog.ShowDialog (
					"Fehler",
					"Das ausgewählte Lied konnte nicht zur Playlist \"" + playlist.Name + "\" hinzugefügt werden."
				);
				break;

			default:
				// Toggle files
				pl.togglePlaylist = playlist;
				break;

			}
		}
	}

	private GameObject FindParentGameObject (GameObject child)
	{
		if (child.tag == "PlaylistDrop" || (child.name.StartsWith ("#") && !child.name.Contains (".")))
		{
			return child;
		}
		else
		{
			Transform t = child.transform;
			while (t.parent != null)
			{
				if (t.parent.name.StartsWith ("#") && !t.parent.name.Contains (".")) {
					return t.parent.gameObject;
				}

				t = t.parent.transform;
			}
		}

		return null;
	}
}
