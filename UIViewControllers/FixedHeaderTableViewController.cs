using System;
using UIKit;
using System.Drawing;

namespace Fluxmatix.Mobile.iOS.UIViewControllers
{
	public class FixedHeaderTableViewController : UIViewController
	{
		public UIView FixedHeaderView { get; set; }
		public UITableView TableView { get; private set; }
		public int TopMargin { get; private set; }

		public FixedHeaderTableViewController ()
		{
			TableView = new UITableView ();
			TopMargin = 0;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if(NavigationController != null) {
				TopMargin += (int)NavigationController.NavigationBar.Frame.Height + 20; //add 20 for statusbar
			}

			//setup fixed header view
			FixedHeaderView = new UIView(new RectangleF(0, TopMargin, (int)View.Frame.Width, 100));
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
			newTableViewFrame.Y = FixedHeaderView.Frame.Height + TopMargin;
			newTableViewFrame.Height = newTableViewFrame.Height - (FixedHeaderView.Frame.Height + TopMargin);
			TableView.Frame = newTableViewFrame;
		}
	}
}

