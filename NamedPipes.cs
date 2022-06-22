﻿using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Furky
{
	// Token: 0x02000010 RID: 16
	internal class NamedPipes
	{
		// Token: 0x060000B5 RID: 181
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool WaitNamedPipe(string name, int timeout);

		// Token: 0x060000B6 RID: 182 RVA: 0x00006AB8 File Offset: 0x00004CB8
		public static bool NamedPipeExist(string pipeName)
		{
			bool result;
			try
			{
				bool flag = !NamedPipes.WaitNamedPipe("\\\\.\\pipe\\" + pipeName, 0);
				if (flag)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					bool flag2 = lastWin32Error == 0;
					if (flag2)
					{
						return false;
					}
					bool flag3 = lastWin32Error == 2;
					if (flag3)
					{
						return false;
					}
				}
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00006B20 File Offset: 0x00004D20
		public static void LuaPipe(string script)
		{
			bool flag = NamedPipes.NamedPipeExist(NamedPipes.luapipename);
			if (flag)
			{
				new Thread(delegate()
				{
					try
					{
						using (NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", NamedPipes.luapipename, PipeDirection.Out))
						{
							namedPipeClientStream.Connect();
							using (StreamWriter streamWriter = new StreamWriter(namedPipeClientStream, Encoding.Default, 999999))
							{
								streamWriter.Write(script);
								streamWriter.Dispose();
							}
							namedPipeClientStream.Dispose();
						}
					}
					catch (IOException)
					{
						MessageBox.Show("Error occured connecting to the pipe.", "Connection Failed!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message.ToString());
					}
				}).Start();
			}
			else
			{
				MessageBox.Show("Inject " + Functions.exploitdllname + " before Using this!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		// Token: 0x0400006F RID: 111
		public static string luapipename = "Furkus";
	}
}
