using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using Fluxmatix.Mobile.iOS.UIViewControllers;
using Fluxmatix.Mobile.iOS.UIViews;
using Reflect.Mobile.iOS;
using System.Drawing;

namespace Reflect.Mobile.iOS
{
	public class SelectTagsViewController : CollectionSelectionViewController<KeyValuePair<string, string>>
	{
		private string cellIdentifier = "selectTagCell";

		public SelectTagsViewController (Dictionary<string, string> tags): base(tags.ToList())
		{

		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			SelectTagTableViewCell cell = tableView.DequeueReusableCell (cellIdentifier) as SelectTagTableViewCell;
			if (cell == null)
				cell = new SelectTagTableViewCell (cellIdentifier);
			var tag = CollectionDisplay [indexPath.Row];
			CheckCellSelection (cell, tag);
			cell.TagView.SetText(tag.Value);
			cell.TagView.Center = cell.Center;
			var correctedFrame = cell.TagView.Frame;
			correctedFrame.Y = (cell.Frame.Height - tableView.RowHeight) / 2;
			correctedFrame.X = 15;

			//resize tagview if its wider the the tableview - some padding
			if(correctedFrame.Width > tableView.Frame.Width - 55)
				correctedFrame.Width = tableView.Frame.Width - 55;			
			cell.TagView.Frame = correctedFrame;

			return cell;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "Select tags";
			TableView.RowHeight = 35;

			//setup collection view for the tag selection summary
			var summaryView = new UIScrollView(new RectangleF(0, 50, (int)View.Frame.Width, 50));
			summaryView.ShowsHorizontalScrollIndicator = true;
			summaryView.ContentSize = new SizeF (800, 50);
			summaryView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			summaryView.BackgroundColor = UIColor.Blue;
			TableView.TableHeaderView.AddSubview (summaryView);

			SelectionChanged += (object sender, List<KeyValuePair<string, string>> e) => {
				//update tag selection summary
				//remove all tags
				foreach(var view in summaryView.Subviews) {
					if(view is TagView) {
						view.RemoveFromSuperview();
					}
				}
				//re-add all tags
				TagView tagView;
				var x = 10;
				var margin = 10;
				foreach(var tag in e) {
					tagView = new TagView (tag.Value);
					var newFrame = tagView.Frame;
					newFrame.X = x;
					tagView.Frame = newFrame;
					x += (int)tagView.Frame.Width + margin;
					summaryView.AddSubview (tagView);
				}
			};

			//setup search stuff
			SearchBar.Placeholder = "Search tags...";
			SearchBar.TextChanged += (object sender, UISearchBarTextChangedEventArgs e) => {
				if(e.SearchText != "") {
					CollectionDisplay = CollectionBase.FindAll(q => q.Value.ToLower().Contains(e.SearchText.ToLower()));
				} else {
					CollectionDisplay = CollectionBase;
				}
				TableView.ReloadData();
			};
			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
		}
	}

	public class SelectTagTableViewCell : UITableViewCell 
	{
		public TagView TagView { get; set; }

		public SelectTagTableViewCell(string identifier) : base(UITableViewCellStyle.Default, identifier) {
			TagView = new TagView ("");
			AddSubview (TagView);
		}
	}
}

