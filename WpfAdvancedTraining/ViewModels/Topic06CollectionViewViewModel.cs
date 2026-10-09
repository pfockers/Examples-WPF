using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using WpfAdvancedTraining.Models;

namespace WpfAdvancedTraining.ViewModels;

public sealed class Topic06CollectionViewViewModel : TrainingTopicViewModel
{
    private string _searchText = string.Empty;

    public Topic06CollectionViewViewModel(TrainingSession session) : base(session)
    {
        UsersView = new ListCollectionView(Session.Users);
        UsersView.SortDescriptions.Add(new SortDescription(nameof(UserItemViewModel.Name), ListSortDirection.Ascending));
        UsersView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(UserItemViewModel.Role)));
        UsersView.Filter = FilterUser;
    }

    public ICollectionView UsersView { get; }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                UsersView.Refresh();
            }
        }
    }

    private bool FilterUser(object item) => item is UserItemViewModel user
        && (string.IsNullOrWhiteSpace(SearchText)
            || user.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
            || user.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
            || user.Role.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
}
