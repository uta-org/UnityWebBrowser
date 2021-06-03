using System.Threading.Tasks;

namespace UnityWebBrowser.Shared
{
    public interface IBrowser
    {
        public Task<byte[]> GetPixels();

        public Task SendKeyDownEvent(int keyCode);
        public Task SendKeyUpEvent(int keyCode);
        public Task SendKeyChars(string chars);

        public Task SendMouseMoveEvent(int x, int y);
        public Task SendSendScrollEvent(int x, int y, int scroll);
        public Task SendMouseClickDownEvent(int x, int y, int clickCount, MouseClickType clickType);
        public Task SendMouseClickUpEvent(int x, int y, int clickCount, MouseClickType clickType);

        public Task LoadUrl(string url);
        public Task LoadHtml(string html);
        public Task ExecuteJs(string js);
    }
}