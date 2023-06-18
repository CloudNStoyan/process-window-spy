using System.Diagnostics;

if (args.Length < 1)
{
    Console.WriteLine("Requires first argument to be process name");
    return;
}

while (true)
{
    var processes = Process.GetProcessesByName(args[0]);

    if (processes.Length == 0)
    {
        continue;
    }

    Console.WriteLine("event:started");

    var process = processes[0];

    var msg = new WinApi.MSG();
    var pos = new WinApi.Position();

    string lastPos = string.Empty;

    var winHook = WinApi.CreateWinEventHook((eventType, objectId) =>
    {
        switch (eventType)
        {
            case WinApi.EVENT_SYSTEM_DESTROY when process.HasExited:
                WinApi.PostQuitMessage(WinApi.WM_QUIT);
                return;
            case WinApi.EVENT_SYSTEM_MINIMIZEEND:
                Console.WriteLine("event:maximized");
                return;
            case WinApi.EVENT_SYSTEM_MINIMIZESTART:
                Console.WriteLine("event:minimized");
                return;
            case WinApi.EVENT_OBJECT_LOCATIONCHANGE when objectId == WinApi.OBJID_WINDOW:
                WinApi.GetWindowRect(process.MainWindowHandle, ref pos);

                if (pos is { Right: <= 0, Bottom: <= 0, Top: <= 0, Left: <= 0 })
                {
                    return;
                }

                string position = pos.ToString();

                if (position == lastPos)
                {
                    return;
                }

                lastPos = position;

                Console.WriteLine("data:" + position);
                return;
        }
    }, process.Id);

    while (WinApi.GetMessage(ref msg, IntPtr.Zero, 0, 0))
    {
        if (msg.message == WinApi.WM_QUIT)
        {
            break;
        }
    }

    WinApi.UnhookWinEvent(winHook);

    Console.WriteLine("event:exited");

    await Task.Delay(500);
}