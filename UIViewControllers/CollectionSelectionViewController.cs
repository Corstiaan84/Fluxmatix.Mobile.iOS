using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Fluxmatix.Mobile.iOS.UIViews;
using Foundation;

namespace Fluxmatix.Mobile.iOS.UIViewControllers
{
	public class CollectionSelectionViewController<T> : FixedHeaderTableViewController
	{
		private CollectionSelectionTableViewSource<T> _source;
		
		public UISearchBar SearchBar { get; private set; }

		public CollectionSelectionViewController (CollectionSelectionTableViewSource<T> source)
		{
			_source = source;
			TableView.Source = _source;
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
			if(_source.SelectionMode == CollectionSelectionTableViewSource<T>.Modes.SingleSelect && _source.Selection.Count != 0) {
				TableView.ScrollToRow (NSIndexPath.FromRowSection (_source.CollectionBase.IndexOf (_source.Selection [0]), 0), UITableViewScrollPosition.Middle, true);
			}
		}
	}

	public class CollectionSelectionTableViewSource<T> : UITableViewSource
	{
		public List<T> CollectionBase { get; set; }

		public List<T> CollectionDisplay { get; set; }

		public List<T> Selection { get; set; }

		public virtual string CellIdentifier { get; set; }

		public event EventHandler<List<T>> SelectionChanged;
		public event EventHandler<T> ItemAdded;
		public event EventHandler<ItemEventArgs<T>> ItemAdding;
		public event EventHandler<T> ItemRemoved;
		public event EventHandler<ItemEventArgs<T>> ItemRemoving;
		public event EventHandler<T> ItemSelected;

		public bool LockItemAddingEvent { get; set; }
		public bool LockItemRemovingEvent { get; set; }

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
			LockItemAddingEvent = false;
			LockItemRemovingEvent = false;
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
			RaiseItemSelectedEvent (item);
			if (cell.Accessory == UITableViewCellAccessory.Checkmark) {
				if (SelectionMode == Modes.MultiSelect) {
					if(RaiseItemRemovingEvent (item).Continue == true) {
						Selection.Remove (item);
						RaiseItemRemovedEvent (item);
						cell.Accessory = UITableViewCellAccessory.None;
					};
				}
			} else {
				if (SelectionMode == Modes.SingleSelect) {
					if(RaiseItemRemovingEvent (Selection[0]).Continue == true) {
						var tempItemToRemove = Selection [0];
						Selection.Remove (tempItemToRemove);
						RaiseItemRemovedEvent (tempItemToRemove);
						if(RaiseItemAddingEvent (item).Continue == true) {
							Selection.Add (item);
							RaiseItemAddedEvent (item);
							cell.Accessory = UITableViewCellAccessory.None;
						};
					};
				} else {
					if (Selection.Contains (item) == false) {
						if (RaiseItemAddingEvent (item).Continue == true) {
							Selection.Add (item);
							RaiseItemAddedEvent (item);
							cell.Accessory = UITableViewCellAccessory.Checkmark;
						}
					}
				}
			}
			tableView.ReloadData ();
			RaiseSelectionChangedEvent ();
		}

		public void DeselectItem (T item, UITableView tableView)
		{
			if (Selection.Contains (item)) {
				if (RaiseItemRemovingEvent (item).Continue == true) {
					Selection.Remove (item);
					tableView.ReloadData ();
					RaiseItemRemovedEvent (item);
					RaiseSelectionChangedEvent ();
				}
			}
		}

		public void SelectItem (T item, UITableView tableView)
		{
			if (Selection.Contains (item) == false) {
				if (RaiseItemAddingEvent (item).Continue == true) {
					Selection.Add (item);
					tableView.ReloadData ();
					RaiseItemAddedEvent (item);
					RaiseSelectionChangedEvent ();
				}
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

		private void RaiseItemAddedEvent (T item)
		{
			if (ItemAdded != null)
				ItemAdded (this, item);
		}

		private ItemEventArgs<T> RaiseItemAddingEvent (T item)
		{
			var args = new ItemEventArgs<T> { Item = item, Continue = true };
			if(LockItemAddingEvent == false) {
				if (ItemAdding != null)
					ItemAdding (this, args);
			}
			return args;
		}

		private void RaiseItemRemovedEvent (T item)
		{
			if (ItemRemoved != null)
				ItemRemoved (this, item);
		}

		private ItemEventArgs<T> RaiseItemRemovingEvent (T item)
		{
			var args = new ItemEventArgs<T> { Item = item, Continue = true };
			if(LockItemRemovingEvent == false) {
				if (ItemRemoving != null)
					ItemRemoving (this, args);
			}
			return args;
		}

		private void RaiseItemSelectedEvent (T item)
		{
			if (ItemSelected != null)
				ItemSelected (this, item);
		}
	}

	public class ItemEventArgs<T> : EventArgs
	{
		public T Item { get; set; }
		public bool Continue { get; set; }
	}
}

