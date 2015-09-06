# Fluxmatix.Mobile.iOS

Fluxmatix.Mobile.iOS is a library for Xamarin.iOS containing a few common UIViews, UIViewControllers and utilities.

IMPORTANT: the project has a dependency on [Fluxmatix.Mobile](https://github.com/Corstiaan84/Fluxmatix.Mobile)

Some of the stuff that's in here:

  * An JSON implementation of Fluxmatix.Mobile.Caching.IMobileCacheStore which stores complex types and simple things like user settings in a JSON file saved on the device
  * A FixedHeaderTableViewController which implements a UITableView and adds a fixed/sticky header at the top of the screen
  * A CollectionSelectionViewController that inherits from FixedHeaderTableViewController and displays a collection of objects which can be selected. The selected objects are placed in the header view, showing a summary of the selected objects. E.g. great for allowing the user to select tags, message recipients, etc
  * A SelectTagsViewController which is just a CollectionSelectionViewController that is further refined to allow the user to select tags
  * An EmptyActionSheet. This is a empty view which you can fill with your own views and slides in from the bottom of the screen. The native ActionSheet is not meant to be modified in such a way so this provides a clean way of building your own ActionSheet
  * A HeaderWebView. This is a UIWebView with an empty view on top that sticks to the top of the container and does not scroll of screen, with the content of the WebView. Just like the Mail app on iOS.
  * A utility to manage the network activity indicator from multiple processes

