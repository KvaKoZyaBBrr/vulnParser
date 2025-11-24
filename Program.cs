using CommandLine;

CommandLine.Parser.Default.ParseArguments<Configuration>(args)
       .WithParsed<Configuration>(o =>
   {
       var data = Parser.Read(o);
       Parser.Write(data);
   });
