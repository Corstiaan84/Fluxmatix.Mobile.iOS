using System;
using UIKit;
using System.Drawing;
using CoreGraphics;
using Fluxmatix.Mobile.Models;

namespace Fluxmatix.Mobile.iOS.UIViews
{
	public class TagView : UILabel
	{
		private UIEdgeInsets _insets;
		public event EventHandler<TagItem> Touched;
		public TagItem TagItem { get; private set; }

		public TagView (TagItem tag)
		{
			TagItem = tag;
			Text = tag.Text;
			Lines = 1;
			Layer.CornerRadius = 10;
			ClipsToBounds = true;
			BackgroundColor = UIColor.LightGray;
			LineBreakMode = UILineBreakMode.TailTruncation;

			_insets.Bottom = 5;
			_insets.Left = 5;
			_insets.Right = 5;
			_insets.Top = 5;

			UserInteractionEnabled = true;

			SizeToFit ();
		}

		public void SetTagItem(TagItem tag) {
			TagItem = tag;
			Text = tag.Text;
			SizeToFit ();
		}

		public override void DrawText (CGRect rect)
		{
			base.DrawText (_insets.InsetRect (rect));
		}

		public override CGRect TextRectForBounds (CGRect bounds, nint numberOfLines)
		{
			var rect = base.TextRectForBounds (bounds, Lines);

			rect.X -= _insets.Left;
			rect.Y -= _insets.Top;
			rect.Width += (_insets.Left + _insets.Right);
			rect.Height += (_insets.Top + _insets.Bottom);

			return rect;
		}

		public override void TouchesEnded (Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);
			if (Touched != null)
				Touched (this, TagItem);
		}
	}
}

