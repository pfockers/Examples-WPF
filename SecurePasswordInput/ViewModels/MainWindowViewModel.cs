// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace SecurePasswordInput.ViewModels;

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    #region Properties

    public SecureString? Password
    {
        get;
        set
        {
            field = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Password)));
            this.PlainPassword = ToPlainText(value);
        }
    }

    public string? PlainPassword
    {
        get;
        set
        {
            field = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.PlainPassword)));
        }
    }

    #endregion

    #region Events

    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    #region Methods

    private static string ToPlainText(SecureString secureString)
    {
        nint buffer = Marshal.SecureStringToBSTR(secureString);

        try
        {
            return Marshal.PtrToStringBSTR(buffer) ?? string.Empty;
        }
        finally
        {
            Marshal.ZeroFreeBSTR(buffer);
        }
    }

    #endregion
}