using System;
using System.Threading;
using System.Windows.Forms;

namespace Furky
{
	// Token: 0x02000011 RID: 17
	internal class Functions
	{
		// Token: 0x060000BA RID: 186 RVA: 0x00006BA0 File Offset: 0x00004DA0
		public static void Inject()
		{
			bool flag = NamedPipes.NamedPipeExist(NamedPipes.luapipename);
			if (flag)
			{
				MessageBox.Show("Already injected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
			else
			{
				bool flag2 = !NamedPipes.NamedPipeExist(NamedPipes.luapipename);
				if (flag2)
				{
					switch (Injector.DllInjector.GetInstance.Inject("RobloxPlayerBeta", AppDomain.CurrentDomain.BaseDirectory + Functions.exploitdllname))
					{
					case Injector.DllInjectionResult.DllNotFound:
						MessageBox.Show("Couldn't find " + Functions.exploitdllname, "Dll was not found!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						break;
					case Injector.DllInjectionResult.GameProcessNotFound:
						MessageBox.Show("Couldn't find RobloxPlayerBeta.exe!", "Target process was not found!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						break;
					case Injector.DllInjectionResult.InjectionFailed:
						MessageBox.Show("Injection Failed!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						break;
					default:
					{
						Thread.Sleep(3000);
						bool flag3 = !NamedPipes.NamedPipeExist(NamedPipes.luapipename);
						if (flag3)
						{
						}
						break;
					}
					}
				}
			}
		}

		// Token: 0x04000070 RID: 112
		public static string exploitdllname = "FurkByteCode.dll";
	}
}
