namespace UTubePatcher.sources
{
    static class ConsoleLog
    {
        public static void Normal(string message, bool newLine)
        {
            DoPrintTask(message, newLine, ConsoleColor.Gray);
        }

        public static void Warning(string message, bool newLine)
        {
            DoPrintTask(message, newLine, ConsoleColor.Yellow);
        }

        public static void Error(string message, bool newLine)
        {
            DoPrintTask(message, newLine, ConsoleColor.Red);
        }

        public static void Found(string message, bool newLine)
        {
            DoPrintTask(message, newLine, ConsoleColor.Cyan);
        }

        public static void Success(string message, bool newLine)
        {
            DoPrintTask(message, newLine, ConsoleColor.Green);
        }

        public static void DoPrintTask(string message, bool newLine, ConsoleColor printColor)
		{
            void PrintType(string message, bool newLine, ConsoleColor printColor) {
                if (newLine)
                {
                    Console.WriteLine(message, Console.ForegroundColor = printColor);
                }
                else
                {
                    Console.Write(message, Console.ForegroundColor = printColor);
                }
            };

			if (!String.IsNullOrWhiteSpace(message)) {
                string line = "";
                List<string> fixedLines = new List<string>();

                bool endOfIndex = false;
                string leftSpaces = "";
                string rightSpaces = "";
                string tempRightSpaces = "";
                for (int i = 0; i <= message.Length; i++)
                {
                    endOfIndex = i < message.Length;

                    if (endOfIndex && message[i] != '\n')
                    {
                        line += message[i];
                    }
                    else
                    {
                        for (int j = 0; j < line.Length; j++)
                        {
                            leftSpaces += line[j];

                            if (line[j] != ' ')
                            {
                                break;
                            }
                        }
                        for (int j = line.Length - 1; j >= 0; j--)
                        {
                            tempRightSpaces += line[j];

                            if (line[j] != ' ')
                            {
                                break;
                            }
                        }
                        rightSpaces = new string(tempRightSpaces.Reverse().ToArray());
                        
                        fixedLines.Add(line.Replace(leftSpaces, leftSpaces.Replace(" ", ""))
                                        .Replace(rightSpaces, rightSpaces.Replace(" ", "")));

                        line = "";
                    }
                }

                int consolePrintPoint = 0;
                string centeredMessage = "";
                for (int i = 0; i < fixedLines.Count; i++)
                {
                    consolePrintPoint = (Main.initConsoleSize.Item1 - fixedLines[i].Length) / 2;

                    if (consolePrintPoint >= 0)
                    {
                        centeredMessage = (Main.printAtConsoleCenter ? new string(' ', consolePrintPoint) : "") + fixedLines[i];

                        PrintType(centeredMessage, newLine, printColor);
                    }
                    else
                    {
                        Console.WriteLine("The content of line cannot exceed the length of console");
                    }
                }
            }
            else
			{
                PrintType(message, newLine, printColor);
            }
        }
    }
}
