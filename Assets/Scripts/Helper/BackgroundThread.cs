/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System.ComponentModel;
using System.Threading;

/// <summary>
///     Provides methods background threading.
/// </summary>
public class BackgroundThread : BackgroundWorker
{
	/// <summary>
	///     Thread which is being processed in the background.
	/// </summary>
	private Thread _thread;

	/// <summary>
	///     Raises the do work event.
	/// </summary>
	/// <param name="e">
	///     Event arguments to be called in the background.
	/// </param>
	protected override void OnDoWork(DoWorkEventArgs e)
    {
        _thread = Thread.CurrentThread;

        try
        {
            base.OnDoWork(e);
        }
        catch (ThreadAbortException)
        {
            e.Cancel = true;
            Thread.ResetAbort();
        }
    }

	/// <summary>
	///     Abort this instance.
	/// </summary>
	public void Abort()
    {
        if (_thread == null) return;
        _thread.Abort();
        _thread = null;
    }
}