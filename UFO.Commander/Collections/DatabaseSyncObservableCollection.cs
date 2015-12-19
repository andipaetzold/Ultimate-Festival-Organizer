﻿namespace UFO.Commander.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using UFO.Domain;
    using UFO.Server;
    using UFO.Server.Interfaces;

    public sealed class DatabaseSyncObservableCollection<T> : ObservableCollection<T>
        where T : DatabaseObject
    {
        #region Fields

        private readonly IBaseServer<T> server;
        private readonly Func<IEnumerable<T>> updateMethod;

        #endregion

        public DatabaseSyncObservableCollection(IBaseServer<T> server, Func<IEnumerable<T>> updateMethod)
        {
            this.server = server;
            this.updateMethod = updateMethod;

            ItemChanged += SyncChangedItem;
            CollectionChanged += SyncChangedCollection;

            PullUpdates();
        }

        public DatabaseSyncObservableCollection(IBaseServer<T> server)
            : this(server, server.GetAll)
        {
        }

        public event EventHandler<ItemChangedEventArgs> ItemChanged;

        public void PullUpdates()
        {
            // remove old
            UnregisterList(Items);
            Items.Clear();

            // add new
            updateMethod().ToList().ForEach(Add);
            RegisterList(Items);
        }

        private void SyncChangedItem(object sender, ItemChangedEventArgs args)
        {
            if (!args.Value.HasId)
            {
                server.Add(args.Value);
            }

            if (!server.Update(args.Value))
            {
                MessageBox.Show(
                    "Invalid data. The item will be reset",
                    "Invalid data.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                ItemChanged -= SyncChangedItem;
                args.Value.OverwriteProperties(server.GetById(args.Value.Id));
                ItemChanged += SyncChangedItem;
            }
        }

        private void SyncChangedCollection(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    RegisterList(args.NewItems?.Cast<T>());
                    foreach (var newItem in args.NewItems ?? new List<T>())
                    {
                        server.Add(newItem as T);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    UnregisterList(args.OldItems.Cast<T>());
                    foreach (var oldItem in args.OldItems ?? new List<T>())
                    {
                        server.Remove(oldItem as T);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RegisterList(IEnumerable<T> list)
        {
            foreach (var item in list ?? new List<T>())
            {
                item.PropertyChanged += OnItemChanged;
            }
        }

        private void UnregisterList(IEnumerable<T> list)
        {
            foreach (var item in list ?? new List<T>())
            {
                item.PropertyChanged -= OnItemChanged;
            }
        }

        private void OnItemChanged(object sender, PropertyChangedEventArgs e)
        {
            ItemChanged?.Invoke(this, new ItemChangedEventArgs(sender as T));
        }

        #region Nested type: ItemChangedEventArgs

        public class ItemChangedEventArgs : EventArgs
        {
            public ItemChangedEventArgs(T value)
            {
                Value = value;
            }

            #region Properties

            public T Value { get; }

            #endregion
        }

        #endregion
    }
}