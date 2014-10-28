using System.Threading;
using System.Threading.Tasks;
namespace MakePdf.Galleries.Loaders
{
    public class ParallelGalleryLoader : GalleryLoader
    {
        public ParallelGalleryLoader(CancellationToken ct) : base(ct) { }

        protected override void Load(Gallery gallery)
        {
            Parallel.ForEach(gallery.Items, item =>
            {
                if (!_ct.IsCancellationRequested)
                    LoadItem(item);
            });
        }
    }
}
