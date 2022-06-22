using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Furky
{
	
	//Copy below from here
	
	// Token: 0x0200000F RID: 15
	internal class Injector
	{
		// Token: 0x0200001D RID: 29
		public enum DllInjectionResult
		{
			// Token: 0x040000BA RID: 186
			DllNotFound,
			// Token: 0x040000BB RID: 187
			GameProcessNotFound,
			// Token: 0x040000BC RID: 188
			InjectionFailed,
			// Token: 0x040000BD RID: 189
			Success
		}

		// Token: 0x0200001E RID: 30
		public sealed class DllInjector
		{
			// Token: 0x060000DF RID: 223
			[DllImport("kernel32.dll", SetLastError = true)]
			private static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

			// Token: 0x060000E0 RID: 224
			[DllImport("kernel32.dll", SetLastError = true)]
			private static extern int CloseHandle(IntPtr hObject);

			// Token: 0x060000E1 RID: 225
			[DllImport("kernel32.dll", SetLastError = true)]
			private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

			// Token: 0x060000E2 RID: 226
			[DllImport("kernel32.dll", SetLastError = true)]
			private static extern IntPtr GetModuleHandle(string lpModuleName);

			// Token: 0x060000E3 RID: 227
			[DllImport("kernel32.dll", SetLastError = true)]
			private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

			// Token: 0x060000E4 RID: 228
			[DllImport("kernel32.dll", SetLastError = true)]
			private static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, int lpNumberOfBytesWritten);

			// Token: 0x060000E5 RID: 229
			[DllImport("kernel32.dll", SetLastError = true)]
			private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

			// Token: 0x17000018 RID: 24
			// (get) Token: 0x060000E6 RID: 230 RVA: 0x00009C94 File Offset: 0x00007E94
			public static Injector.DllInjector GetInstance
			{
				get
				{
					bool flag = Injector.DllInjector._instance == null;
					if (flag)
					{
						Injector.DllInjector._instance = new Injector.DllInjector();
					}
					return Injector.DllInjector._instance;
				}
			}

			// Token: 0x060000E7 RID: 231 RVA: 0x00009CC3 File Offset: 0x00007EC3
			private DllInjector()
			{
			}

			// Token: 0x060000E8 RID: 232 RVA: 0x00009CD0 File Offset: 0x00007ED0
			public Injector.DllInjectionResult Inject(string sProcName, string sDllPath)
			{
				bool flag = !File.Exists(sDllPath);
				Injector.DllInjectionResult result;
				if (flag)
				{
					result = Injector.DllInjectionResult.DllNotFound;
				}
				else
				{
					uint num = 0U;
					Process[] processes = Process.GetProcesses();
					for (int i = 0; i < processes.Length; i++)
					{
						bool flag2 = processes[i].ProcessName == sProcName;
						if (flag2)
						{
							num = (uint)processes[i].Id;
							break;
						}
					}
					bool flag3 = num == 0U;
					if (flag3)
					{
						result = Injector.DllInjectionResult.GameProcessNotFound;
					}
					else
					{
						bool flag4 = !this.bInject(num, sDllPath);
						if (flag4)
						{
							result = Injector.DllInjectionResult.InjectionFailed;
						}
						else
						{
							result = Injector.DllInjectionResult.Success;
						}
					}
				}
				return result;
			}

			// Token: 0x060000E9 RID: 233 RVA: 0x00009D60 File Offset: 0x00007F60
			private bool bInject(uint pToBeInjected, string sDllPath)
			{
				IntPtr intPtr = Injector.DllInjector.OpenProcess(1082U, 1, pToBeInjected);
				bool flag = intPtr == Injector.DllInjector.INTPTR_ZERO;
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					IntPtr procAddress = Injector.DllInjector.GetProcAddress(Injector.DllInjector.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
					bool flag2 = procAddress == Injector.DllInjector.INTPTR_ZERO;
					if (flag2)
					{
						result = false;
					}
					else
					{
						IntPtr intPtr2 = Injector.DllInjector.VirtualAllocEx(intPtr, (IntPtr)null, (IntPtr)sDllPath.Length, 12288U, 64U);
						bool flag3 = intPtr2 == Injector.DllInjector.INTPTR_ZERO;
						if (flag3)
						{
							result = false;
						}
						else
						{
							byte[] bytes = Encoding.ASCII.GetBytes(sDllPath);
							bool flag4 = Injector.DllInjector.WriteProcessMemory(intPtr, intPtr2, bytes, (uint)bytes.Length, 0) == 0;
							if (flag4)
							{
								result = false;
							}
							else
							{
								bool flag5 = Injector.DllInjector.CreateRemoteThread(intPtr, (IntPtr)null, Injector.DllInjector.INTPTR_ZERO, procAddress, intPtr2, 0U, (IntPtr)null) == Injector.DllInjector.INTPTR_ZERO;
								if (flag5)
								{
									result = false;
								}
								else
								{
									Injector.DllInjector.CloseHandle(intPtr);
									result = true;
								}
							}
						}
					}
				}
				return result;
			}

			// Token: 0x040000BE RID: 190
			private static readonly IntPtr INTPTR_ZERO = (IntPtr)0;

			// Token: 0x040000BF RID: 191
			private static Injector.DllInjector _instance;
		}
	}
}
