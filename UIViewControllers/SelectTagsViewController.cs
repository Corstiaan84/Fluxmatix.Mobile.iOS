using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using Fluxmatix.Mobile.iOS.UIViews;
using System.Drawing;
using Fluxmatix.Mobile.Models;

namespace Fluxmatix.Mobile.iOS.UIViewControllers
{
	public class SelectTagsViewController : CollectionSelectionViewController<TagItem>
	{
		public UIScrollView SummaryView { get; set; }

		private CollectionSelectionTableViewSource<TagItem> _source;

		public SelectTagsViewController (CollectionSelectionTableViewSource<TagItem> source): base(source)
		{
			_source = source;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "Select tags";
			TableView.RowHeight = 40;

			//setup collection view for the tag selection summary
			SummaryView = new UIScrollView(new RectangleF(0, 50, (int)View.Frame.Width, 50));
			SummaryView.ShowsHorizontalScrollIndicator = true; 
			SummaryView.ContentSize = new SizeF (800, 50);
			SummaryView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			SummaryView.BackgroundColor = UIColor.Blue;
			FixedHeaderView.AddSubview (SummaryView);

			_source.SelectionChanged += (object sender, List<TagItem> e) => {
				GetSummaryTagViews (e);
			};
			TableView.Source = _source;
			GetSummaryTagViews (_source.Selection);

			//setup search stuff
			SearchBar.Placeholder = "Search tags...";
			SearchBar.TextChanged += (object sender, UISearchBarTextChangedEventArgs e) => {
				if(e.SearchText != "") {
					_source.CollectionDisplay = _source.CollectionBase.FindAll(q => q.Text.ToLower().Contains(e.SearchText.ToLower()));
				} else {
					_source.CollectionDisplay = _source.CollectionBase;
				}
				TableView.ReloadData();
				TableView.ScrollRectToVisible(new CoreGraphics.CGRect(0, 0, 1, 1), false);
			};
			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
		}

		public virtual TagView GetTagView(TagItem tag) {
			var tagView = new TagView (tag);
			tagView.Touched += (object sender, TagItem touchedTag) => {
				var sheet = new UIActionSheet ("Deselect tag " + tag.Text + " from selection?", null, "Cancel", "Deselect tag");
				sheet.Clicked += (object s, UIButtonEventArgs e) => {
					if(e.ButtonIndex == 0) {
						(TableView.Source as SelectTagsTableViewSource).DeselectItem(touchedTag, TableView);
					}
				};
				sheet.ShowInView(this.View);
			};
			return tagView;	
		}

		public virtual void GetSummaryTagViews (List<TagItem> tagsToSummerize)
		{
			//update tag selection summary

			//remove all tags
			foreach (var view in SummaryView.Subviews) {
				if (view is TagView) {
					view.RemoveFromSuperview ();
				}
			}
			//re-add all selected tags
			TagView tagView;
			var x = 10;
			var margin = 10;
			var maxTagWidth = 200;
			foreach (var tag in tagsToSummerize) {
				tagView = GetTagView (tag);
				var newFrame = tagView.Frame;
				newFrame.X = x;
				newFrame.Y = (SummaryView.Frame.Height - newFrame.Height) / 2;
				//resize tagview width if its to wide
				if (newFrame.Width > maxTagWidth)
					newFrame.Width = maxTagWidth;
				tagView.Frame = newFrame;
				x += (int)tagView.Frame.Width + margin;
				SummaryView.AddSubview (tagView);
				SummaryView.ContentSize = new SizeF (x, 50);
			}
			//scroll to right
			var w = SummaryView.Frame.Width;
			var h = SummaryView.Frame.Height;
			var newPosition = SummaryView.ContentOffset.X + w;
			var toVisible = new CoreGraphics.CGRect (newPosition, 0, w, h);
			SummaryView.ScrollRectToVisible (toVisible, true);
		}
	}

	public class SelectTagsTableViewSource : CollectionSelectionTableViewSource<TagItem>
	{
		public SelectTagsTableViewSource(List<TagItem> collection) : base(collection){

		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			SelectTagTableViewCell cell = tableView.DequeueReusableCell (CellIdentifier) as SelectTagTableViewCell;
			if (cell == null)
				cell = new SelectTagTableViewCell (CellIdentifier);
			var tag = CollectionDisplay [indexPath.Row];
			CheckCellSelection (cell, tag);
			cell.TagView.SetTagItem(tag);
			cell.TagView.Center = cell.Center;
			var correctedFrame = cell.TagView.Frame;
			correctedFrame.Y = (tableView.RowHeight - cell.TagView.Frame.Height) / 2;
			correctedFrame.X = 15;

			//resize tagview if its wider then the tableview - some padding
			if(correctedFrame.Width > tableView.Frame.Width - 55)
				correctedFrame.Width = tableView.Frame.Width - 55;			
			cell.TagView.Frame = correctedFrame;

			return cell;
		}

	}

	public class SelectTagTableViewCell : UITableViewCell 
	{
		public TagView TagView { get; set; }

		public SelectTagTableViewCell(string identifier) : base(UITableViewCellStyle.Default, identifier) {
			TagView = new TagView (new TagItem("", ""));
			AddSubview (TagView);
		}
	}
}

