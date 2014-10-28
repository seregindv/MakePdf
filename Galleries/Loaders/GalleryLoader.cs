using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using MakePdf.Stuff;
using MakePdf.Sizing;
using System.Threading;

namespace MakePdf.Galleries.Loaders
{

    public class GalleryItemLoadedEventArgs : EventArgs
    {
        public GalleryItemLoadedEventArgs(int loadedCount, int totalCount)
        {
            LoadedCount = loadedCount;
            TotalCount = totalCount;
        }

        public int LoadedCount { private set; get; }
        public int TotalCount { private set; get; }
    }

    public abstract class GalleryLoader
    {
        Gallery _galleryBeingLoaded;
        int _loadedCount;
        protected CancellationToken _ct;

        protected GalleryLoader(CancellationToken ct)
        {
            _ct = ct;
        }

        public event EventHandler<GalleryItemLoadedEventArgs> GalleryItemLoaded;

        protected abstract void Load(Gallery gallery);

        public void LoadGallery(Gallery gallery)
        {
            try
            {
                _galleryBeingLoaded = gallery;
                _loadedCount = 0;
                Load(gallery);
            }
            finally
            {
                _galleryBeingLoaded = null;
            }
        }

        protected void LoadItem(GalleryItem item)
        {
            _galleryBeingLoaded.LoadItem(item);
            OnGalleryItemLoaded();
        }

        protected void OnGalleryItemLoaded()
        {
            Interlocked.Increment(ref _loadedCount);
            if (GalleryItemLoaded != null)
                GalleryItemLoaded(this, new GalleryItemLoadedEventArgs(_loadedCount, _galleryBeingLoaded.Items.Length));
        }
    }
}
