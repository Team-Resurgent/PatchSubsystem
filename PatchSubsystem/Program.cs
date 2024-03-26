using Mono.Options;

internal class Program
{
    private static bool shouldShowHelp = false;
    private static string input = string.Empty;
    private static string subsystemString = string.Empty;

    private static void Process(string inputFile, short subsystem)
    {
        var fileData = File.ReadAllBytes(inputFile);
        var subsystemOffset = BitConverter.ToUInt32(fileData, 0x3C) + 0x5c;
        fileData[subsystemOffset] = (byte)(subsystem & 0xff);
        fileData[subsystemOffset + 1] = (byte)((subsystem >> 8) & 0xff);
        File.WriteAllBytes(inputFile, fileData);
    }

    private static void Main(string[] args)
    {
        var options = new OptionSet {
            { "i|input=", "Input file to patch.", i => input = i },
            { "s|subsystem=", "Subsystem hex to use e.g. '0x000E' for xbox.", s => subsystemString = s },
            { "h|help", "show this message and exit", h => shouldShowHelp = h != null },
        };

        try
        {
            List<string> extra = options.Parse(args);

            if (shouldShowHelp || args.Length == 0)
            {
                Console.Write("PatchSubsystem:");
                Console.WriteLine("Patahes a exe's subsystem.");
                options.WriteOptionDescriptions(System.Console.Out);
                return;
            }

            var fullPath = Path.GetFullPath(input);
            if (File.Exists(input) == false)
            {
                throw new OptionException($"Input '{input}' not found", "input");
            }

            short subsystem;
            if (subsystemString.StartsWith("0x", StringComparison.OrdinalIgnoreCase) == false || short.TryParse(subsystemString.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out subsystem) == false)
            {
                throw new OptionException("Subsystem is invalid", "subsystem");
            }

            Process(fullPath, subsystem);
        }
        catch (OptionException e)
        {
            Console.Write("PatchSubsystem:");
            Console.WriteLine("Patahes a exe's subsystem.");
            Console.WriteLine(e.Message);
            Console.WriteLine("Try `PatchSubsystem --help' for more information.");
            return;
        }
    }
}