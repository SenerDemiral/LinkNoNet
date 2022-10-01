using static LinkNoNetApp.Constants;
using HashidsNet;

namespace LinkNoNetApp;

public static class Constants
{
    public const string PublishVersion = "Link#Net© v0.7 Şener Demiral®";
    public const string BrowserUsrIdKey = "uid";
    public const string Height = "calc(100vh - 140px)";
    public enum ttStus { LoginBekliyor = -2, OnayBekliyor = -1, Aktif = 0, InAktif = 1, Durduruldu = 2 };

    public static Hashids hashIds0 = new Hashids("this is my salt", 0);   // Nekadar gerekiyorsa
    public static Hashids hashIds5 = new Hashids("this is my salt", 5);
    public static Hashids hashIds8 = new Hashids("this is my salt", 8);
    public static Hashids hashIds11 = new Hashids("this is my salt", 11);
}
