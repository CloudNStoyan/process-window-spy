using System.Runtime.InteropServices;

public static class WinApi
{

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hwnd, ref Position position);

    private delegate void WinEventProc(IntPtr hWinEventHook, int eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    [DllImport("user32.dll")]
    private static extern IntPtr SetWinEventHook(int eventMin, int eventMax, IntPtr hmodWinEventProc, WinEventProc lpfnWinEventProc, int idProcess, int idThread, int dwflags);

    [DllImport("user32.dll")]
    public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

    [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
    public static extern bool GetMessage(ref MSG message, IntPtr hWnd, int wMsgFilterMin, uint wMsgFilterMax);
    [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
    public static extern void PostQuitMessage(int nExitCode);

    public struct Position
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
        public override string ToString()
        {
            return $"{this.Top},{this.Right},{this.Bottom},{this.Left}";
        }
    }
    private struct POINT
    {
        long x;
        long y;
    }


    public struct MSG
    {
        IntPtr hwnd;
        public uint message;
        UIntPtr wParam;
        IntPtr lParam;
        uint time;
        POINT pt;
    }

    public const int WINEVENT_OUTOFCONTEXT = 0;
    public const int WINEVENT_SKIPOWNPROCESS = 2;
    public const int EVENT_OBJECT_LOCATIONCHANGE = 32779;
    public const int EVENT_SYSTEM_MINIMIZESTART = 22;
    public const int EVENT_SYSTEM_MINIMIZEEND = 23;
    public const int WM_QUIT = 18;
    public const int EVENT_SYSTEM_DESTROY = 32769;
    public const int EVENT_MIN = 1;
    public const int EVENT_MAX = 2147483647;

    public const int OBJID_WINDOW = 0;


    public static IntPtr CreateWinEventHook(Action<int, int> onProc, int processId) => SetWinEventHook(EVENT_MIN, EVENT_MAX,
        IntPtr.Zero, (_, eventType, _, objectId, _, _, _) =>
        {
            onProc.Invoke(eventType, objectId);
        }, processId, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
}