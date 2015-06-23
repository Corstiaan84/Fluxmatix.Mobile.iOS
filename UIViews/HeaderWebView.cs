using System;
using UIKit;
using System.Drawing;
using Foundation;

namespace Fluxmatix.Mobile.iOS.UIViews
{
	public class UIHeaderWebView : UIWebView
	{
		public UIView HeaderView { get; set; }

		public UIHeaderWebView (Rectangle frame, UIView headerView) : base(frame)
		{
			HeaderView = headerView;
			ScrollView.AddSubview (HeaderView);
			ScrollView.Delegate = new HeaderWebViewScrollViewDelegate (this);
			ScrollView.ContentInset = new UIEdgeInsets(HeaderView.Frame.Height, 20, 0, 20);
			ScrollView.ContentOffset = new CoreGraphics.CGPoint(0, 0 - HeaderView.Frame.Height);
		}
	}

	public class HeaderWebViewScrollViewDelegate : UIScrollViewDelegate
	{
		private UIHeaderWebView _headerWebView;
		public event EventHandler HeaderViewRepositioned;
		public UIEdgeInsets ContentInset { get; set; }

		public HeaderWebViewScrollViewDelegate(UIHeaderWebView headerWebView)
		{
			_headerWebView = headerWebView;
		}

		public override void Scrolled (UIScrollView scrollView)
		{
			//Console.WriteLine("Scrolled " + scrollView.ContentOffset.X.ToString());
			RepositionHeader (scrollView);
		}

		public void RepositionHeader(UIScrollView scrollView)
		{
			var headerFrame = _headerWebView.HeaderView.Frame;
			headerFrame.Y = 0 - headerFrame.Size.Height;
			headerFrame.X = scrollView.ContentOffset.X;
			_headerWebView.HeaderView.Frame = headerFrame;
			scrollView.ContentInset = new UIEdgeInsets (headerFrame.Height, ContentInset.Left, ContentInset.Bottom, ContentInset.Right);
			if(HeaderViewRepositioned != null)
				HeaderViewRepositioned (this, null);
		}
	}
}

