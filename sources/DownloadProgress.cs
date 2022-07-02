using System.Diagnostics;
using UTubePatcher.sources;

public class DownloadProgress {
    private bool printOnConsole = false;
    private bool stopProgression = false;
    private int previousConsoleCursorTopPosition = 0;
    private Stopwatch stopWatch = new Stopwatch();
    private int progressPercentage = 0;
    private string bytesReceived = "";
    private string totalBytesToReceive = "";
    private string downloadSpeed = "";
    private string progressBar = "";

    public void Progression(int pP, long bR, long tBR)
    {
        progressPercentage = pP;
        bytesReceived = ((double)((bR / 1024f) / 1024f)).ToString("0.0");
        totalBytesToReceive = ((int)((tBR / 1024f) / 1024f)).ToString("0.0");
        downloadSpeed = (bR / 1024.0 / 1024.0 / stopWatch.Elapsed.TotalSeconds).ToString("0.0");

        printOnConsole = true;
    }
    public void Dispose()
    {
        stopProgression = true;
    }

    public DownloadProgress()
    {
        _ = PrintProgression();
    }
    private async Task<bool> PrintProgression()
    {
        while (!printOnConsole) {
            await Task.Delay(1);
        }

        previousConsoleCursorTopPosition = Console.CursorTop;

        stopProgression = false;
        int currentProgressValue = 0;
        int endProgressValue = 20;
        stopWatch.Start();
        while (!stopProgression)
        {
            currentProgressValue = progressPercentage / 5;

            int subIndex = 0;
            progressBar = "";
            for (int i = 1; i <= endProgressValue; i++)
            {
                subIndex = i - 1;

                if (subIndex <= currentProgressValue)
                {
                    progressBar += "█";
                }
                if (subIndex > currentProgressValue)
                {
                    progressBar += "░";
                }
            }

            ClearConsoleFrom(previousConsoleCursorTopPosition, false);

            ConsoleLog.Normal(progressPercentage == 0
                ?
                "Starting..."
                :
                $"|{progressBar}| {progressPercentage}% \n" +
                $"Download Size: {bytesReceived}MB/{totalBytesToReceive}MB \n" +
                $"Speed: {downloadSpeed}MB/s"
            , true);

            await Task.Delay(30);
        }
        stopWatch.Stop();

        ClearConsoleFrom(previousConsoleCursorTopPosition, true);

        printOnConsole = false;

        return true;
    }
    private void ClearConsoleFrom(int value, bool clearProgressLines)
	{
        Console.SetCursorPosition(0, value);
        if (clearProgressLines)
        {
            for (int i = 0; i < Main.initConsoleSize.Item2 / 2; i++)
            {
                ConsoleLog.Normal(new string(' ', Main.initConsoleSize.Item1), true);
            }
            Console.SetCursorPosition(0, value);
        }
    }
}