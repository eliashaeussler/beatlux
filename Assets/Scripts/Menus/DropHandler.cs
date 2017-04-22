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

			bool added = pl.AddFile (file, playlist);
			pl.ToggleFiles (playlist, true);
		}
	}
}
