using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DropHandler : MonoBehaviour, IDropHandler {

	public void OnDrop (PointerEventData eventData)
	{
		// Get reference to playlist object
		Playlist pl = GameObject.Find ("PlaylistContent").GetComponent <Playlist> ();

		// Get file path
		string dir = DragHandler.item.name;

		// Get file object if available
		FileObj file = pl.GetFile (dir);

		// Get playlist object
		PlaylistObj playlist = pl.FindPlaylist (gameObject);

		// Add file to selected playlist
		if (playlist != null)
		{
			if (file == null) {
				file = new FileObj (dir);
			}

			// Add file and show playlist
			long added = pl.AddFile (file, playlist);
			pl.ToggleFiles (playlist, true);

			// Show dialog
			Dialog dialog = GameObject.Find ("MenuManager").GetComponent<Dialog> ();

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
				break;

			}
		}
	}
}
