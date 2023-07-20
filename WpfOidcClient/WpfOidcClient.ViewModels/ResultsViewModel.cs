﻿using ReactiveUI;
using System.Globalization;
using System.Reactive.Linq;
using System.Security.Claims;

namespace WpfOidcClient.ViewModels;

public class ResultsViewModel : ViewModel, IResultsViewModel
{
    public ResultsViewModel(IMessageBus messageBus) : base(messageBus)
    {
        _accessToken = messageBus
            .Changes(x => x.AccessToken, x => x.AccessToken)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.AccessToken);

        _identityToken = messageBus
            .Changes(x => x.IdentityToken, x => x.IdentityToken)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.IdentityToken);

        _refreshToken = messageBus
            .Changes(x => x.RefreshToken, x => x.RefreshToken)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.RefreshToken);

        _accessTokenExpiration = messageBus
            .Changes(x => x.AccessTokenExpiration, x => x.AccessTokenExpiration)
            .Select(x => x.ToString("F", CultureInfo.CurrentCulture))
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.AccessTokenExpiration);

        _claims = messageBus
            .Changes(x => x.User.Claims, _ => Array.Empty<Claim>())
            .Select(MapClaims)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.Claims);
    }

    private IReadOnlyList<ClaimViewModel> MapClaims(IEnumerable<Claim> claims)
    {
        return claims
            .Select(x => new ClaimViewModel(x.Type, x.Value))
            .OrderBy(x => x.Type, StringComparer.Ordinal)
            .ThenBy(x => x.Value, StringComparer.Ordinal)
            .ToArray();
    }

    #region Access Token property

    private readonly ObservableAsPropertyHelper<string> _accessToken;

    public string AccessToken => _accessToken.Value;

    #endregion Access Token property

    #region Identity Token property

    private readonly ObservableAsPropertyHelper<string> _identityToken;

    public string IdentityToken => _identityToken.Value;

    #endregion Identity Token property

    #region Refresh Token property

    private readonly ObservableAsPropertyHelper<string> _refreshToken;

    public string RefreshToken => _refreshToken.Value;

    #endregion Refresh Token property

    #region Access Token Expiration property

    private readonly ObservableAsPropertyHelper<string> _accessTokenExpiration;

    public string AccessTokenExpiration => _accessTokenExpiration.Value;

    #endregion Access Token Expiration property

    #region Claims property

    private readonly ObservableAsPropertyHelper<IReadOnlyList<ClaimViewModel>> _claims;

    public IReadOnlyList<ClaimViewModel> Claims => _claims.Value;

    #endregion Claims property
}