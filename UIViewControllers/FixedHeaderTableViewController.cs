using System;
using UIKit;
using System.Drawing;

namespace Fluxmatix.Mobile.iOS.UIViewControllers
{
	public class FixedHeaderTableViewController : UIViewController
	{
		public UIView FixedHeaderView { get; set; }
		public UITableView TableView { get; private set; }
		private int _topMargin = 0;

		public FixedHeaderTableViewController ()
		{
			TableView = new UITableView ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if(NavigationController != null) {
				_topMargin += (int)NavigationController.NavigationBar.Frame.Height + 20; //add 20 for statusbar
			}

			//setup fixed header view
			FixedHeaderView = new UIView(new RectangleF(0, _topMargin, (int)View.Frame.Width, 100));
			FixedHeaderView.BackgroundColor = UIColor.Blue;
			Add  (FixedHeaderView);

			//init tableview
			Add (TableView);
			ResetTableViewTopMargin ();

			//set this otherwise the top margin of the tableview gets messed up for some reason
			AutomaticallyAdjustsScrollViewInsets = false;
		}

		public void ResetTableViewTopMargin() {

			var newTableViewFrame = View.Frame;
			newTableViewFrame.Y = FixedHeaderView.Frame.Height + _topMargin;
			newTableViewFrame.Height = newTableViewFrame.Height - (FixedHeaderView.Frame.Height + _topMargin);
			TableView.Frame = newTableViewFrame;
		}
	}
}

