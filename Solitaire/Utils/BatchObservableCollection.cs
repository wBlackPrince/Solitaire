using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System;

namespace Solitaire.Utils;

public sealed class BatchObservableCollection<T>: Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged
{
    private const string CountString = "Count";
    private const string IndexName = "Item[]";

    private static readonly NotifyCollectionChangedEventHandler EmptyDelegate = delegate { };
    private  readonly Reentry
}