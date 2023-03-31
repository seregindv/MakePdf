using System.Collections.Generic;

namespace MakePdf.Markup
{
    public interface IHasTags
    {
        List<Tag> Tags { set; get; }
    }
}
