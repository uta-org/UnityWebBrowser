// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
//
// This project is under the MIT license. See the LICENSE.md file for more details.

using System.IO;
using VoltstroStudios.UnityWebBrowser.Shared;
using VoltstroStudios.UnityWebBrowser.Shared.Popups;

namespace VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;

/// <summary>
///     Launch arguments for the app
/// </summary>
public class LaunchArguments
{
    /// <summary>
    ///     The initial URL for the browser
    /// </summary>
    public string InitialUrl { get; set; }

    /// <summary>
    ///     The initial width of the browser
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    ///     The initial height of the browser
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    ///     Is JavaScript enabled
    /// </summary>
    public bool JavaScript { get; set; }

    /// <summary>
    ///     Is Web RTC enabled
    /// </summary>
    public bool WebRtc { get; set; }

    /// <summary>
    ///     Control local storage
    /// </summary>
    public bool LocalStorage { get; set; }

    /// <summary>
    ///     How to handle popups
    /// </summary>
    public PopupAction PopupAction { get; set; }

    /// <summary>
    ///     The port to use for remote debugging
    /// </summary>
    public int RemoteDebugging { get; set; }

    /// <summary>
    ///     The <see cref="Color" /> to use for the background
    /// </summary>
    public Color BackgroundColor { get; set; }

    /// <summary>
    ///     The path you should use for your cache
    /// </summary>
    public FileInfo CachePath { get; set; }

    /// <summary>
    ///     Should we use a proxy or direct
    /// </summary>
    public bool ProxyEnabled { get; set; }

    /// <summary>
    ///     Username of the proxy
    /// </summary>
    public string ProxyUsername { get; set; }

    /// <summary>
    ///     Password of the proxy
    /// </summary>
    public string ProxyPassword { get; set; }

    /// <summary>
    ///     The path you should log browser events to
    /// </summary>
    public FileInfo LogPath { get; set; }

    /// <summary>
    ///     What is the log severity
    /// </summary>
    public LogSeverity LogSeverity { get; set; }

    /// <summary>
    ///     Path for the assembly containing the communication layer
    /// </summary>
    internal FileInfo CommunicationLayerPath { get; set; }

    /// <summary>
    ///     In location (Either the pipe name or port)
    /// </summary>
    internal string InLocation { get; set; }

    /// <summary>
    ///     Out location (Either the pipe name or port)
    /// </summary>
    internal string OutLocation { get; set; }

    /// <summary>
    ///     Start delay. Used for testing reasons.
    /// </summary>
    internal uint StartDelay { get; set; }
}