//code copied from https://gist.github.com/jfoshee/5223962

using System;
using UIKit;
using System.Collections.Generic;

namespace Fluxmatix.Mobile.iOS.UIViews
{
	public class ListPickerViewModel<TItem> : UIPickerViewModel
	{
		public TItem SelectedItem { get; private set; }

		IList<TItem> _items;

		public IList<TItem> Items {
			get { return _items; }
			set {
				_items = value;
				Selected (null, 0, 0);
			}
		}

		public ListPickerViewModel ()
		{
		}

		public ListPickerViewModel (IList<TItem> items)
		{
			Items = items;
		}

		public override nint GetRowsInComponent (UIPickerView pickerView, nint component)
		{
			if (NoItem (0))
				return 1;
			return Items.Count;
		}

		public override string GetTitle (UIPickerView pickerView, nint row, nint component)
		{
			if (NoItem (row))
				return "";
			var item = Items [(int)row];
			return GetTitleForItem (item);
		}

		public override void Selected (UIPickerView pickerView, nint row, nint component)
		{
			if (NoItem (row))
				SelectedItem = default(TItem);
			else
				SelectedItem = Items [(int)row];
		}

		public override nint GetComponentCount (UIPickerView picker)
		{
			return 1;
		}

		public virtual string GetTitleForItem (TItem item)
		{
			return item.ToString ();
		}

		bool NoItem (nint row)
		{
			return Items == null || row >= Items.Count;
		}
	}
}

