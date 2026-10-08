// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace CustomWpfControl.ViewModels;

using System.ComponentModel;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    #region Properties

    public string Text
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Text)));
        }
    } = "Text aus dem ViewModel";

    #endregion

    #region Events

    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion
}