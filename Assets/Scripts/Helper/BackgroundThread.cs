using UnityEngine;
using System.Collections;
using System.ComponentModel;
using System.Threading;

public class BackgroundThread : BackgroundWorker {

	private Thread thread;

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

	public void Abort ()
	{
		if (thread != null) {
			thread.Abort ();
			thread = null;
		}
	}
}
