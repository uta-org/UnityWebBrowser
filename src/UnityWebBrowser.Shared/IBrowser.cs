using System.Threading.Tasks;

namespace UnityWebBrowser.Shared
{
    public interface IBrowser
    {
        public Task<byte[]> GetPixels();
    }
}