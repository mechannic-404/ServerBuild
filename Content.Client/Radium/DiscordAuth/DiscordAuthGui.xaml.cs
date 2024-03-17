﻿using Content.Client.Corvax;
using Content.Corvax.Interfaces.Client;
using Robust.Client.Console;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Utility;
using Robust.Client.AutoGenerated;
using Robust.Shared.Timing;

namespace Content.Client.Radium.DiscordAuth;

[GenerateTypedNameReferences]
public sealed partial class DiscordAuthGui : Control
{
    [Dependency] private readonly IClientDiscordAuthManager _discordAuthManager = default!;
    [Dependency] private readonly IClientConsoleHost _consoleHost = default!;

    public static readonly SpriteSpecifier Sprite =
        new SpriteSpecifier.Rsi(new ResPath("/Textures/Radium/Menu/maina.rsi"), "maina");
    public event Action? OnSkipPressed;
    public DiscordAuthGui()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);
        LayoutContainer.SetAnchorPreset(this, LayoutContainer.LayoutPreset.Wide);

        Background.SetFromSpriteSpecifier(Sprite);
        Background.HorizontalAlignment = HAlignment.Stretch;
        Background.VerticalAlignment = VAlignment.Stretch;
        Background.DisplayRect.Stretch = TextureRect.StretchMode.KeepAspectCentered;

        QuitButton.OnPressed += (_) =>
        {
            _consoleHost.ExecuteCommand("quit");
        };

        UrlEdit.TextRope = new Rope.Leaf(_discordAuthManager.AuthUrl);

        OpenUrlButton.OnPressed += (_) =>
        {
            if (_discordAuthManager.AuthUrl != string.Empty)
            {
                IoCManager.Resolve<IUriOpener>().OpenUri(_discordAuthManager.AuthUrl);
            }
        };

        SkipButton.OnPressed += (_) =>
        {
            OnSkipPressed?.Invoke();
        };
    }
}
