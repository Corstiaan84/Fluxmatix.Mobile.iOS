using UIKit;

namespace Fluxmatix.Mobile.iOS
{
	public static class Utilities
	{
		private static int _numberOfCallsToSetNetworkIndicatorVisible = 0;

		public static void SetNetworkActivityIndicatorVisibility(bool setVisible) {
			if (setVisible) {
				_numberOfCallsToSetNetworkIndicatorVisible++;
			}
			else {
				if(_numberOfCallsToSetNetworkIndicatorVisible != 0) {
					_numberOfCallsToSetNetworkIndicatorVisible--;
				}
			}
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = _numberOfCallsToSetNetworkIndicatorVisible > 0;
		}
	}
}

