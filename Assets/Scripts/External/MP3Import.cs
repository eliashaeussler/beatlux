using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.IO;


public class MP3Import
{
	private static IntPtr handle_mpg;
	private static IntPtr errPtr;
	private static IntPtr rate;
	private static IntPtr channels;
	private static IntPtr encoding;
	private static IntPtr id3v1;
	private static IntPtr id3v2;
	private static IntPtr done;

	#region Consts: Standard values used in almost all conversions.
	private const float const_1_div_128_ = 1.0f / 128.0f;  // 8 bit multiplier
	private const float const_1_div_32768_ = 1.0f / 32768.0f; // 16 bit multiplier
	private const double const_1_div_2147483648_ = 1.0 / 2147483648.0; // 32 bit
	#endregion

	public static AudioClip StartImport(string path)
	{
		MPGImport.mpg123_init ();
		handle_mpg = MPGImport.mpg123_new (null, errPtr);
		int x = MPGImport.mpg123_open (handle_mpg, path);      
		MPGImport.mpg123_getformat (handle_mpg, out rate, out channels, out encoding);
		int intRate = rate.ToInt32 ();
		int intChannels = channels.ToInt32 ();
		int intEncoding = encoding.ToInt32 ();

		MPGImport.mpg123_id3 (handle_mpg, out id3v1, out id3v2);      
		MPGImport.mpg123_format_none (handle_mpg);
		MPGImport.mpg123_format (handle_mpg, intRate, intChannels, 208);

		int FrameSize = MPGImport.mpg123_outblock (handle_mpg);      
		byte[] Buffer = new byte[FrameSize];      
		int lengthSamples = MPGImport.mpg123_length (handle_mpg);

		AudioClip myClip = AudioClip.Create (Path.GetFileNameWithoutExtension (path), lengthSamples, intChannels, intRate, false, false);

		int importIndex = 0;

		while (0 == MPGImport.mpg123_read(handle_mpg, Buffer, FrameSize, out done)) {


			float[] fArray;
			fArray = ByteToFloat (Buffer);

			myClip.SetData (fArray, (importIndex*fArray.Length)/2);

			importIndex++;                
		}          

		MPGImport.mpg123_close (handle_mpg);

		return myClip;
	}

	public static float[] IntToFloat (Int16[] from)
	{
		float[] to = new float[from.Length];

		for (int i = 0; i < from.Length; i++)
			to [i] = (float)(from [i] * const_1_div_32768_);

		return to;
	}

	public static Int16[] ByteToInt16 (byte[] buffer)
	{
		Int16[] result = new Int16[1];
		int size = buffer.Length;
		if ((size % 2) != 0) {
			/* Error here */
			Console.WriteLine ("error");
			return result;
		} else {
			result = new Int16[size / 2];
			IntPtr ptr_src = Marshal.AllocHGlobal (size);
			Marshal.Copy (buffer, 0, ptr_src, size);
			Marshal.Copy (ptr_src, result, 0, result.Length);
			Marshal.FreeHGlobal (ptr_src);
			return result;
		}
	}

	public static float[] ByteToFloat (byte[] bArray)
	{
		Int16[] iArray;      

		iArray = ByteToInt16 (bArray);

		return IntToFloat (iArray);
	}


}