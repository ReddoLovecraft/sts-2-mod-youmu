using BaseLib.Abstracts;

namespace TH_Youmu.Scripts.Main
{
	public abstract class YoumuPowerModel : CustomPowerModel
	{
		public virtual void Trigger()
		{
			Flash();
		}
	}
	
}
