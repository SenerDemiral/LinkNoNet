namespace TntMud;

public static class Constants
{
    public const string PublishVersion = "0.6";
    public const string BrowserUsrIdKey = "uid";
    public const string Height = "calc(80vh - 120px)";

    public enum dStus { Açık = 0, Kabul = 1, Red = -2 };
    public enum kStus { Katılmadın = 0, Katılırım = 1, Vazgeçtim = -1, Seçildin = 2, Seçilmedin = -2 };
    public enum tStus { İptal = -1, KatılımBaşlamadı = 0, KatılımSürüyor = 1, KatılımBitti = 2, Kapandı = 3, Arşiv = 9 };

}
