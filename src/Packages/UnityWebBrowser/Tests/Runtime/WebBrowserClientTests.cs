using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VoltstroStudios.UnityWebBrowser.Core;

namespace VoltstroStudios.UnityWebBrowser.Tests
{
    public class WebBrowserClientTests
    {
        [UnityTest]
        public IEnumerator WaitForReadyFailTest()
        {
            return UniTask.ToCoroutine(async () =>
            {
                LogAssert.Expect(LogType.Error,
                    "[Web Browser]: The engine did not get ready within engine startup timeout!");
                LogAssert.Expect(LogType.Log, "[Web Browser]: UWB shutdown...");

                WebBrowserClient client = new();
                await client.WaitForEngineReadyTask();

                Assert.That(client.HasDisposed);
            });
        }
    }
}