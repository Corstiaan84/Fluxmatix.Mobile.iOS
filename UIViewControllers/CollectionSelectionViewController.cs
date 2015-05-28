using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Fluxmatix.Mobile.iOS.UIViews;

namespace Fluxmatix.Mobile.iOS.UIViewControllers
{
	public class CollectionSelectionViewController<T> : FixedHeaderTableViewController
	{
		public UISearchBar SearchBar { get; private set; }

		public CollectionSelectionViewController (CollectionSelectionTableViewSource<T> source)
		{
			TableView.Source = source;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.White;

			//setup search bar
			SearchBar = new UISearchBar (new RectangleF (0, 0, (int)View.Frame.Width, 50));
			SearchBar.Placeholder = "Enter Search Text";
			SearchBar.AutocorrectionType = UITextAutocorrectionType.No;
			SearchBar.AutocapitalizationType = UITextAutocapitalizationType.None;
			FixedHeaderView.AddSubview (SearchBar);

			Add (FixedHeaderView);

			var g = new UITapGestureRecognizer (() => View.EndEditing (true));
			g.CancelsTouchesInView = false; //for iOS5
			View.AddGestureRecognizer (g);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			//scroll to first selected row
			//TableView.ScrollToRow(new Foundation.NSIndexPath(), UITableViewScrollPosition.Middle, true);
		}
	}

	public class CollectionSelectionTableViewSource<T> : UITableViewSource
	{
		public List<T> CollectionBase { get; set; }

		public List<T> CollectionDisplay { get; set; }

		public List<T> Selection { get; set; }

		public virtual string CellIdentifier { get; set; }

		public event EventHandler<List<T>> SelectionChanged;

		public enum Modes
		{
			SingleSelect,
			MultiSelect
		}

		public Modes SelectionMode { get; set; }

		public CollectionSelectionTableViewSource (List<T> collection)
		{
			CellIdentifier = "collectionSelectionCell";
			CollectionBase = collection;
			CollectionDisplay = collection;
			Selection = new List<T> ();
			SelectionMode = Modes.SingleSelect;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return CollectionDisplay.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (CellIdentifier);
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Default, CellIdentifier);
			var item = CollectionDisplay [indexPath.Row];
			CheckCellSelection (cell, item);
			return cell;
		}

		public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			var item = CollectionDisplay [indexPath.Row];
			tableView.DeselectRow (indexPath, true);
			var cell = tableView.CellAt (indexPath);
			if (cell.Accessory == UITableViewCellAccessory.Checkmark) {
				Selection.Remove (item);
				cell.Accessory = UITableViewCellAccessory.None;
			} else {
				if (SelectionMode == Modes.SingleSelect) {
					Selection.Clear ();
					Selection.Add (item);
				} else {
					if (Selection.Contains (item) == false) {
						Selection.Add (item);
					}
				}
				cell.Accessory = UITableViewCellAccessory.Checkmark;
			}
			tableView.ReloadData ();
			RaiseSelectionChangedEvent ();
		}

		public void DeselectItem (T item, UITableView tableView)
		{
			if (Selection.Contains (item)) {
				Selection.Remove (item);
				tableView.ReloadData ();
				RaiseSelectionChangedEvent ();
			}
		}

		public bool CheckCellSelection (UITableViewCell cell, T item)
		{
			var selected = false;
			if (Selection.Contains (item)) {
				cell.Accessory = UITableViewCellAccessory.Checkmark;
				selected = true;
			} else {
				cell.Accessory = UITableViewCellAccessory.None;
			}
			return selected;
		}

		private void RaiseSelectionChangedEvent ()
		{
			if (SelectionChanged != null)
				SelectionChanged (this, Selection);
		}
	}
}

