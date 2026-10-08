// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace SecurePasswordInput.ViewModels;

using System.ComponentModel;
using System.Security;

public sealed class MainWindowViewModel : INotifyPropertyChanged, IDisposable
{
    #region Fields

    private SecureString? _password;

    #endregion

    #region Properties

    public SecureString? Password
    {
        get => this._password;
        set
        {
            if (ReferenceEquals(this._password, value))
            {
                return;
            }

            this._password?.Dispose();
            this._password = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Password)));
        }
    }

    #endregion

    #region Events

    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    #region Methods

    public void Dispose()
    {
        this._password?.Dispose();
        this._password = null;
    }

    #endregion
}