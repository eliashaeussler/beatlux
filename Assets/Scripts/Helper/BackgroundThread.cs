using UnityEngine;

using System.Collections;
using System.ComponentModel;
using System.Threading;

/// <summary>
/// Provides methods background threading.
/// </summary>
public class BackgroundThread : BackgroundWorker {

	/// <summary>
	/// Thread which is being processed in the background.
	/// </summary>
	private Thread thread;

	/// <summary>
	/// Raises the do work event.
	/// </summary>
	/// <param name="e">
	/// Event arguments to be called in the background.
	/// </param>
	protected override void OnDoWork (DoWorkEventArgs e)
	{
		thread = Thread.CurrentThread;

		try {
			base.OnDoWork (e);
		} catch (ThreadAbortException) {
			e.Cancel = true;
			Thread.ResetAbort ();
		}
	}

	/// <summary>
	/// Abort this instance.
	/// </summary>
	public void Abort ()
	{
		if (thread != null) {
			thread.Abort ();
			thread = null;
		}
	}
}
