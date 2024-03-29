﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Xml.Serialization;
using MakePdf.Markup;
using MakePdf.Helpers;
using MakePdf.Sizing;

namespace MakePdf.Galleries
{
    [System.Diagnostics.DebuggerDisplay("{ImageUrl}")]
    public class GalleryItem : Tag
    {
        public GalleryItem()
        {
        }

        public GalleryItem(string imageUrl)
        {
            ImageUrl = imageUrl;
        }

        public GalleryItem(string imageUrl, string description)
            : this(imageUrl)
        {
            if (!String.IsNullOrEmpty(description))
                Tags = TagFactory.GetParagraphTags(description);
        }

        public GalleryItem(string imageUrl, List<Tag> tags)
            : this(imageUrl)
        {
            Tags = tags;
        }

        public GalleryItem(string imageUrl, List<Tag> tags, int width, int height, string thumbnailUrl)
            : this(imageUrl)
        {
            Tags = tags;
            SetSize(width, height);
            ThumbnailImageUrl = thumbnailUrl;
        }

        public GalleryItem(string imageUrl, string description, int width, int height)
            : this(imageUrl, description)
        {
            SetSize(width, height);
        }

        public GalleryItem(string imageUrl, string description, int width, int height, string thumbnailImageUrl)
            : this(imageUrl, description, width, height)
        {
            ThumbnailImageUrl = thumbnailImageUrl;
        }
        public string ImageUrl { set; get; }
        public string Url { set; get; }

        public string ThumbnailImageUrl { set; get; }

        public ImageSize Size { set; get; }
        public string LocalPath { set; get; }
        public int Index { get; set; }

        protected void SetSize(int width, int height)
        {
            if (width != Int32.MinValue && height != Int32.MinValue)
                Size = new ImageSize(width, height);
        }

        public string GetFormattedIndex(string format = "00000000")
        {
            return PathHelper.GetFileName(Index, format);
        }

        public override Tag Wrap(Tag with)
        {
            throw GetWrappingException();
        }

        public override T Wrap<T>()
        {
            throw GetWrappingException();
        }

        private ArgumentException GetWrappingException()
        {
            return new ArgumentException("GalleryItem doesn't support wrapping");
        }

        public bool HasUrl { get { return Url != null; } }
    }
}
