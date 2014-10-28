using System.Threading;
using MakePdf.Stuff;

namespace MakePdf.Galleries.Loaders
{
    public class SingleThreadedGalleryLoader : GalleryLoader
    {
        public SingleThreadedGalleryLoader(CancellationToken ct) : base(ct) { }

        protected override void Load(Gallery gallery)
        {
            foreach (var galleryItem in gallery.Items)
            {
                if (_ct.IsCancellationRequested)
                    break;
                LoadItem(galleryItem);
            }
        }
    }
}
