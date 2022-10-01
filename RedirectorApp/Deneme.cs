using HashidsNet;

namespace RedirectorApp;

public sealed class DenemeA
{
    public Hashids hashIds0  = new Hashids("this is my salt", 0);   // Nekadar gerekiyorsa
    public Hashids hashIds5  = new Hashids("this is my salt", 5);
    public Hashids hashIds8  = new Hashids("this is my salt", 8);
    public Hashids hashIds11 = new Hashids("this is my salt", 11);
}

