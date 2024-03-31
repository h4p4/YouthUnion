namespace YouthUnion
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Input;

    internal class MainViewModel : ViewModel
    {
        private ObservableCollection<Assignment> _assignments;
        private bool _canEdit;
        private string _eventNameFilter;
        private ObservableCollection<Event> _events;
        private ObservableCollection<Participant> _participants;
        private string _selectedMonth;
        private Participant? _selectedParticipant;
        private bool _showAll;
        private bool _showCurrent;
        private bool _showFuture;
        private bool _showPast;

        public MainViewModel()
        {
            Months = new List<string>(DateTimeFormatInfo.CurrentInfo.MonthNames);
            const string firstMonth = "Все месяцы";
            Months.Insert(0, firstMonth);
            SelectedMonth = firstMonth;
            //DateTimeFormatInfo.CurrentInfo.GetMonthName(DateTime.Now.Month)
            Participants = new ObservableCollection<Participant>(Context.Participants);
            Events = new ObservableCollection<Event>(Context.Events);
            Assignments = new ObservableCollection<Assignment>(Context.Assignments);
            Participants.CollectionChanged += ParticipantsCollectionChanged;
            Events.CollectionChanged += EventsCollectionChanged;
            Assignments.CollectionChanged += AssignmentsCollectionChanged;
            SaveCommand = new RelayCommand(Execute);
            ShowAll = true;
        }


        public ObservableCollection<Assignment> Assignments
        {
            get => _assignments;
            set => SetProperty(ref _assignments, value);
        }

        public bool CanEdit
        {
            get => _canEdit;
            set => SetProperty(ref _canEdit, value);
        }

        private static YouthUnionContext Context => ContextProvider.YouthUnionContext;

        public string EventNameFilter
        {
            get => _eventNameFilter;
            set
            {
                SetProperty(ref _eventNameFilter, value);
                HandleEventsFiltering();
            }
        }

        public ObservableCollection<Event> Events
        {
            get => _events;
            set => SetProperty(ref _events, value);
        }

        public List<string> Months { get; set; }

        public ObservableCollection<Participant> Participants
        {
            get => _participants;
            set => SetProperty(ref _participants, value);
        }

        public ICommand SaveCommand { get; set; }

        public string SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                SetProperty(ref _selectedMonth, value);
                HandleEventsFiltering();
            }
        }

        public Participant? SelectedParticipant
        {
            get => _selectedParticipant;
            set
            {
                SetProperty(ref _selectedParticipant, value);
                HandleEventsFiltering();
            }
        }

        public bool ShowAll
        {
            get => _showAll;
            set
            {
                SetProperty(ref _showAll, value);
                HandleEventsFiltering();
            }
        }

        public bool ShowCurrent
        {
            get => _showCurrent;
            set
            {
                SetProperty(ref _showCurrent, value);
                HandleEventsFiltering();
            }
        }

        public bool ShowFuture
        {
            get => _showFuture;
            set
            {
                SetProperty(ref _showFuture, value);
                HandleEventsFiltering();
            }
        }

        public bool ShowPast
        {
            get => _showPast;
            set
            {
                SetProperty(ref _showPast, value);
                HandleEventsFiltering();
            }
        }

        private void AssignmentsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Context.Assignments.AddRange(e.NewItems.Cast<Assignment>().ToArray());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Context.Assignments.RemoveRange(e.OldItems.Cast<Assignment>().ToArray());
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                default:
                    return;
            }
        }

        private void EventsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Context.Events.AddRange(e.NewItems.Cast<Event>().ToArray());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Context.Events.RemoveRange(e.OldItems.Cast<Event>().ToArray());
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                default:
                    return;
            }
        }

        private void Execute(object obj)
        {
            Context.SaveChanges(true);
        }

        private void HandleEventsFiltering()
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            ObservableCollection<Event>? events = null;
            IQueryable<Event> eventsQueryable;
            if (SelectedMonth != Months[0])
            {
                var currentMonth = DateTimeFormatInfo.CurrentInfo.MonthNames.ToList().IndexOf(SelectedMonth) + 1;
                eventsQueryable = Context.Events.Where(x => x.EndDate.Month == currentMonth ||
                                                            x.StartDate.Month == currentMonth);
            }
            else
                eventsQueryable = Context.Events;

            if (SelectedParticipant != null && SelectedParticipant.FullName != "По ответственному")
                eventsQueryable = eventsQueryable.Where(x => x.ResponsibleParticipant == SelectedParticipant);

            if (!string.IsNullOrWhiteSpace(EventNameFilter))
                eventsQueryable = eventsQueryable.Where(x => x.EventName.StartsWith(EventNameFilter));

            if (ShowAll)
                events = new ObservableCollection<Event>(eventsQueryable);
            if (ShowCurrent)
            {
                events = new ObservableCollection<Event>(eventsQueryable.Where(x =>
                    x.EndDate >= currentDate && x.StartDate <= currentDate));
            }

            if (ShowFuture)
                events = new ObservableCollection<Event>(eventsQueryable.Where(x => currentDate < x.StartDate));
            if (ShowPast)
                events = new ObservableCollection<Event>(eventsQueryable.Where(x => currentDate > x.EndDate));

            Events = events ?? new ObservableCollection<Event>();
            Events.CollectionChanged += EventsCollectionChanged;
        }

        private void ParticipantsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Context.Participants.AddRange(e.NewItems.Cast<Participant>().ToArray());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Context.Participants.RemoveRange(e.OldItems.Cast<Participant>().ToArray());
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                default:
                    return;
            }
        }
    }
}