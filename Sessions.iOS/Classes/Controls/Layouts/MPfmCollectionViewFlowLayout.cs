// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Linq;
using System.Collections.Generic;
using System;
using Sessions.Core;

namespace Sessions.iOS.Classes.Controls.Layouts
{
    [Register("SessionsCollectionViewFlowLayout")]
    public class SessionsCollectionViewFlowLayout : UICollectionViewFlowLayout
    {
		public SessionsCollectionViewFlowLayout() : base()
        {
			// Remove spacing between items on iPhone to stack two album arts in width
			float spacing = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone ? 0 : 8;
            ItemSize = new SizeF(160, 160);
			SectionInset = new UIEdgeInsets(spacing, spacing, spacing, spacing);
			HeaderReferenceSize = new SizeF(0, 52);
			MinimumInteritemSpacing = spacing;
			MinimumLineSpacing = spacing;
        }

//		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(RectangleF rect)
//		{
//			//var attributes = base.LayoutAttributesForElementsInRect(rect).ToList();
//			var attributes = base.LayoutAttributesForElementsInRect(rect);
//			//Tracing.Log("CollectionViewFlowLayout - LayoutAttributesForElementsInRect - rect: {0} attributes.Count: {1}", rect, attributes.Count);
//			var missingSections = new List<int>();
//			for (int a = 0; a < attributes.Length; a++)
//			{
//				var layoutAttributes = attributes[a];
//				//Tracing.Log("CollectionViewFlowLayout - LayoutAttributesForElementsInRect - a: {0} row: {1} section: {2} RepresentedElementCategory: {3}", a, layoutAttributes.IndexPath.Row, layoutAttributes.IndexPath.Section, layoutAttributes.RepresentedElementCategory);
//				if (layoutAttributes.RepresentedElementCategory == UICollectionElementCategory.Cell)
//					missingSections.Add(layoutAttributes.IndexPath.Section);
//
////				if (layoutAttributes.RepresentedElementKind == "UICollectionElementKindSectionHeader")
////				{
////					attributes.RemoveAt(a);
////					a--;
////				}
//			}
//
//			for (int a = 0; a < attributes.Length; a++)
//			{
//				var layoutAttributes = attributes[a];
//				if (layoutAttributes.RepresentedElementKind == "UICollectionElementKindSectionHeader")
//					missingSections.Remove(layoutAttributes.IndexPath.Section);
//			}
//
////			foreach (int index in missingSections)
////			{
////				var indexPath = NSIndexPath.FromItemSection(0, index);
////				Tracing.Log("CollectionViewFlowLayout - LayoutAttributesForElementsInRect - index: {0} missingSections.Count: {1} row: {2} section: {3}", index, missingSections.Count, indexPath.Row, indexPath.Section);
////				var layoutAttributes = this.LayoutAttributesForSupplementaryView(UICollectionElementKindSection.Header, indexPath);
////				if (layoutAttributes != null)
////					attributes.Add(layoutAttributes);
////			}
//
//			foreach (var layoutAttributes in attributes)
//			{
//				if (layoutAttributes.RepresentedElementKind == "UICollectionElementKindSectionHeader")
//				{
//					int section = layoutAttributes.IndexPath.Section;
//					int numberOfItemsInSection = CollectionView.NumberOfItemsInSection(section);
//
//					var firstCellIndexPath = NSIndexPath.FromItemSection(0, section);
//					var lastCellIndexPath = NSIndexPath.FromItemSection(Math.Max(0, (numberOfItemsInSection - 1)), section);
//
//					var firstCellAttrs = LayoutAttributesForItem(firstCellIndexPath);
//					var lastCellAttrs = LayoutAttributesForItem(lastCellIndexPath);
//
//					PointF origin = layoutAttributes.Frame.Location;
//					origin.Y = Math.Min(Math.Max(CollectionView.ContentOffset.Y, firstCellAttrs.Frame.Top - layoutAttributes.Frame.Height), lastCellAttrs.Frame.Bottom - layoutAttributes.Frame.Height);
//					layoutAttributes.ZIndex = 1024;
//					layoutAttributes.Frame = new RectangleF(origin, layoutAttributes.Frame.Size); 
//				}
//			}
//
//			return attributes.ToArray();
//		}
//
////		public override UICollectionViewLayoutAttributes LayoutAttributesForSupplementaryView(NSString kind, NSIndexPath indexPath)
////		{
////			//return base.LayoutAttributesForSupplementaryView(kind, indexPath);
////			var attributes = base.LayoutAttributesForSupplementaryView(kind, indexPath);
////			if (kind != new NSString("UICollectionElementKindSectionHeader"))
////				return attributes;
////
////			PointF contentOffset = CollectionView.ContentOffset;
////			PointF nextHeaderOrigin = new PointF(0, 0);
////			if (indexPath.Section + 1 < CollectionView.NumberOfSections())
////			{
////				var nextHeaderAttributes = base.LayoutAttributesForSupplementaryView(kind, NSIndexPath.FromItemSection(0, indexPath.Section + 1));
////				nextHeaderOrigin = nextHeaderAttributes.Frame.Location;
////			}
////
////			RectangleF frame = attributes.Frame;
////			if (ScrollDirection == UICollectionViewScrollDirection.Vertical)
////				frame.Location = new PointF(frame.Location.X, Math.Min(Math.Max(contentOffset.Y, frame.Location.Y), nextHeaderOrigin.Y - frame.Height));
////			else
////				frame.Location = new PointF(Math.Min(Math.Max(contentOffset.X, frame.Location.X), nextHeaderOrigin.X - frame.Width), frame.Location.Y);
////			attributes.ZIndex = 1024;
////			attributes.Frame = frame;
////			return attributes;
////		}
//
//		public override bool ShouldInvalidateLayoutForBoundsChange(RectangleF newBounds)
//		{
//			//return base.ShouldInvalidateLayoutForBoundsChange(newBounds);
//			return true;
//		}

    }
}
