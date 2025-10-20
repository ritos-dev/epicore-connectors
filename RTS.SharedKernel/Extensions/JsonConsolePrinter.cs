using Newtonsoft.Json;

namespace RTS.SharedKernel.Extensions
{
    public class JsonConsolePrinter
    {
        public static void WriteColoredJson(object obj)
        {
            var prettyJson = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);


            foreach (var line in prettyJson.Split('\n'))
            {
                var parts = line.Split(':', 2);
                if (parts.Length == 2 && line.TrimStart().StartsWith("\""))
                {

                    Console.Write(parts[0] + ":");
                    Console.ResetColor();


                    var value = parts[1];
                    var trimmed = value.TrimStart();

                    if (trimmed.StartsWith("\""))
                        Console.ForegroundColor = ConsoleColor.Green;
                    else if (trimmed.StartsWith("{") || trimmed.StartsWith("["))
                        Console.ForegroundColor = ConsoleColor.White;
                    else if (trimmed.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                             trimmed.Equals("false", StringComparison.OrdinalIgnoreCase))
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    else if (trimmed.Equals("null", StringComparison.OrdinalIgnoreCase))
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    else
                        Console.ForegroundColor = ConsoleColor.Cyan;

                    Console.WriteLine(value);
                    Console.ResetColor();
                }
                else
                {

                    Console.WriteLine(line);
                }
            }
        }
    }
}
