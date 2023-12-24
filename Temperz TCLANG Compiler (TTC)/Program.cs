string[] code = File.ReadLines(@"C:\Users\tempe\source\repos\Temperz TCLANG Compiler (TTC)\Temperz TCLANG Compiler (TTC)\helloWorld.tclang").ToArray();

foreach (Token t in Lexer.GetTokens(code))
    Console.WriteLine(t.Name + " " + t.Type);
