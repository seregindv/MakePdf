using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using MakePdf.Attributes;
using MakePdf.Galleries;

namespace MakePdf.Helpers
{
    public enum AddressType
    {
        [Gallery(Title = "As text")]
        Other,
        [Gallery(Type = typeof(TextGallery), Regex = @".*lenta\.ru\/news\/.+(?<!#\d{1,2})$")]
        LentaNews,
        [Gallery(Type = typeof(LentaArticleGallery), Regex = @".*lenta\.ru.*\/(articles|columns)\/.+")]
        LentaArticle,
        [Gallery(Type = typeof(LentaGallery), Regex = @".*lenta\.ru.*\/(photo|news)\/.+#\d{1,2}$")]
        LentaGallery,
        [Gallery(Type = typeof(LentaRealtyGallery), Regex = @".*realty\.lenta\.ru\/photo.*")]
        LentaRealtyGallery,
        [Gallery(Type = typeof(ForbesGallery), Regex = @".*forbes\.ru.*\/photogallery\/.*")]
        ForbesGallery,
        [Gallery(Type = typeof(GazetaGallery), Regex = @".*gazeta\.ru.*\/photo\/.*")]
        GazetaGallery,
        [Gallery(Type = typeof(ForbesSlideshow), Regex = @".*forbes\.ru.*\-(slideshow|photogallery)\/.*")]
        ForbesSlideshow,
        [Gallery(Type = typeof(ForbesMultipageSlideshow), Regex = @".*forbes\.ru\/node\/\d+\/slide\/\d+.*")]
        ForbesMultipageSlideshow,
        [Gallery(Type = typeof(NovayaGazetaGallery), Regex = @".*novayagazeta\.ru\/photos.*")]
        NovayaGazeta,
        [Gallery(Type = typeof(MkGallery), Regex = @".*mk.ru\/photo.*")]
        MkGallery,
        [Gallery(Type = typeof(TextGallery))]
        TextGallery,
        [Gallery(Type = typeof(MotorArticleGallery), Regex = @".*motor.ru\/articles\/.*")]
        MotorArticleGallery,
        [Gallery(Type = typeof(LentaInterview), Title = "Lenta interview", Group = "As interview", Regex = @".*lenta\.ru\/.*")]
        LentaInterview,
        [Gallery(Type = typeof(RiaGallery), Regex = @".*ria\.ru\/photolents\/.*")]
        RiaGallery,
        [Gallery(Type = typeof(SlonAuthorGallery), Regex = @".*slon\.ru\/authors\/.*")]
        SlonAuthorGallery,
        [Gallery(Type = typeof(MotorPhotoGallery), Regex = @".*motor.ru\/photo\/.*")]
        MotorPhoto,
        [Gallery(Type = typeof(DofigaGallery), Regex = @".*dofiga\.net\/.*")]
        DofigaGallery,
        [Gallery(Type = typeof(LentaBeelineGallery), Regex = @".*beeline\.lenta\.ru\/.*")]
        LentaBeelineGallery,
        [Gallery(Type = typeof(ItarTassGallery), Regex = @".*itar\-tass\.com\/.*")]
        ItarTassGallery,
        [Gallery(Type = typeof(PhotofileGallery), Regex = @".*photo\.qip\.ru.*")]
        PhotofileGallery,
        [Gallery(Type = typeof(MeduzaGallery), Regex = @".*meduza\.io\/galleries\/.*")]
        MeduzaGallery,
        [Gallery(Type = typeof(MeduzaInterview), Title = "Medusa interview", Group = "As interview", Regex = @".*meduza\.io\/(?!galleries\/).*")]
        MedusaInterview,
        [Gallery(Type = typeof(RbcInterview), Title = "RBC interview", Group = "As interview", Regex = @".*rbc\.ru\/.*")]
        RbcInterview,
        [Gallery(Type = typeof(HrefGallery), Title = "As href gallery")]
        HrefGallery,
        [Gallery(Type = typeof(ImgGallery), Title = "As img gallery")]
        ImgGallery,
        [Gallery(Type = typeof(RbcGallery), Regex = @".*\.rbc\.ru\/photoreport\/.+")]
        RbcGallery,
        [Gallery(Type = typeof(MotorGallery), Regex = @".*motor.ru\/gallery\/.*")]
        MotorGallery,
        [Gallery(Type = typeof(PhotoshareGallery), Regex = @".*photoshare\.ru.+")]
        PhotoshareGallery,
        [Gallery(Type = typeof(TextGallery), Title = "With no title")]
        NoTitleGallery,
        [Gallery(Type = typeof(VillageGallery), Regex = @".*the-village\.ru\/village\/.*")]
        VillageGallery,
        [Gallery(Type = typeof(JoannaGallery), Regex = @"\.joannastingray\.com")]
        JoannaGallery,
        [Gallery(Type = typeof(InterfaxGallery), Regex = @"interfax\.ru\/photo\/.+")]
        InterfaxGallery
    }
}