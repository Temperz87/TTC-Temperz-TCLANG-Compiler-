using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SymbolTable
{
    public Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();
}

public struct Symbol
{
    public string name;
    public string type;
    public uint size;
    public uint dimension;
    public uint lineOfDeclaration;
    public List<uint> usageLines;
    // Address information ???

    public Symbol(string name, string type, uint size, uint dimension, uint lineOfDeclaration)
    {
        this.name = name;
        this.type = type;
        this.size = size;
        this.dimension = dimension;
        this.lineOfDeclaration = lineOfDeclaration;
        this.usageLines = new List<uint>();
    }
}
