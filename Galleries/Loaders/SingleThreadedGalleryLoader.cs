namespace MakePdf.Galleries.Loaders
{
    public class SingleThreadedGalleryLoader : GalleryLoader
    {
        protected override void Load(Gallery gallery)
        {
            foreach (var galleryItem in gallery.Items)
                LoadItem(galleryItem);
        }
    }
}
