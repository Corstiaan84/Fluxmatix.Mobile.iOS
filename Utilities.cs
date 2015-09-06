using UIKit;

namespace Fluxmatix.Mobile.iOS
{
	public static class Utilities
	{
		private static int _calls = 0;

		public static void SetNetworkActivityIndicatorVisibility(bool setVisible) {
			if (setVisible) {
				_calls++;
			}
			else {
				if(_calls != 0) {
					_calls--;
				}
			}
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = _calls > 0;
		}
	}
}

