using System;
using UIKit;
using System.Drawing;

namespace Fluxmatix.Mobile.iOS.UIViews
{
	public class EmptyActionSheet : UIView
	{
		private UIViewController _owner;
		private UILabel _label;
		private int _marginBottom = 0;
		private int _height = 100;
		private bool _viewAdded = false;

		public UIToolbar ToolBar { get; private set; }

		public bool IsVisible { get; private set; }

		public enum Modes
		{
			Standard,
			SecondaryToolbar
		}

		public Modes Mode { get; private set; }

		public EmptyActionSheet (UIViewController owner) : base (new RectangleF (0, (float)owner.View.Frame.Height, (float)owner.View.Frame.Width, 100))
		{
			_owner = owner;
			_label = new UILabel (new RectangleF (10, 10, 50, 50)) { Text = "test" };
			//AddSubview (_label);
			IsVisible = false;
			Mode = Modes.Standard;
			CalcBottomMargin ();
		}

		public void SetMode (Modes mode)
		{
			Mode = mode;
			CalcBottomMargin ();
			CalcHeight ();
			if (Mode == Modes.SecondaryToolbar) {
				InitToolBar ();
			}
		}

		private void InitToolBar ()
		{
			ToolBar = new UIToolbar (new RectangleF (0, 0, (float)_owner.View.Frame.Width, 44));
			AddSubview (ToolBar);
		}

		public void SetHeight(int height) {
			_height = height;
		}

		public void ShowInView ()
		{
			CalcHeight ();
			CalcBottomMargin ();

			//InitToolBar ();
			if (_viewAdded == false) {
				_owner.View.AddSubview (this);
				_viewAdded = true;
			}

			Animate (.15, 0, UIViewAnimationOptions.CurveLinear, () => {
				Frame = new RectangleF (0, (float)_owner.View.Frame.Height - ((float)_height + _marginBottom), (float)_owner.View.Frame.Width, _height);
			}, () => {
				IsVisible = true;
			});
		}

		public void Dismis ()
		{
			Animate (.25, 0, UIViewAnimationOptions.CurveLinear, () => {
				Frame = new RectangleF (0, (float)_owner.View.Frame.Height, (float)_owner.View.Frame.Width, (float)Frame.Height);
			}, () => {
				IsVisible = false;
			});
		}

		private void CalcBottomMargin ()
		{
			_marginBottom = 0;
			if (_owner.NavigationController != null) {
				if (_owner.NavigationController.ToolbarHidden == false) {
					_marginBottom = (int)_owner.NavigationController.Toolbar.Frame.Height;
				} else {
					_marginBottom = 44;
				}
			} 
		}

		private void CalcHeight ()
		{
			if (Mode == Modes.SecondaryToolbar) {
				if (_owner.NavigationController != null) {
					_height = (int)_owner.NavigationController.Toolbar.Frame.Height;
				} else {
					_height = 64;
				}
			}
		}

	}
}

