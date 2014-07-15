using System.Threading.Tasks;
namespace MakePdf.Galleries.Loaders
{
    public class ParallelGalleryLoader : GalleryLoader
    {
        protected override void Load(Gallery gallery)
        {
            Parallel.ForEach(gallery.Items, LoadItem);
        }
    }
}
