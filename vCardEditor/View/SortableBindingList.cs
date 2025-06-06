﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using vCardEditor.Model;


namespace vCardEditor.View
{
    public class SortableBindingList<T> : BindingList<T>
    {
        private readonly Dictionary<Type, PropertyComparer<T>> comparers;
        private bool isSorted;
        private ListSortDirection listSortDirection;
        private PropertyDescriptor propertyDescriptor;

        public SortableBindingList()
            : base(new List<T>())
        {
            this.comparers = new Dictionary<Type, PropertyComparer<T>>();
        }

        public SortableBindingList(IEnumerable<T> enumeration)
            : base(new List<T>(enumeration))
        {
            this.comparers = new Dictionary<Type, PropertyComparer<T>>();
        }

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        protected override bool IsSortedCore
        {
            get { return this.isSorted; }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get { return this.propertyDescriptor; }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get { return this.listSortDirection; }
        }

        protected override bool SupportsSearchingCore
        {
            get { return true; }
        }

        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            List<T> itemsList = (List<T>)this.Items;

            Type propertyType = property.PropertyType;
            PropertyComparer<T> comparer;
            if (!this.comparers.TryGetValue(propertyType, out comparer))
            {
                comparer = new PropertyComparer<T>(property, direction);
                this.comparers.Add(propertyType, comparer);
            }

            comparer.SetPropertyAndDirection(property, direction);
            itemsList.Sort(comparer);

            this.propertyDescriptor = property;
            this.listSortDirection = direction;
            this.isSorted = true;

            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override void RemoveSortCore()
        {
            this.isSorted = false;
            this.propertyDescriptor = base.SortPropertyCore;
            this.listSortDirection = base.SortDirectionCore;

            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override int FindCore(PropertyDescriptor property, object key)
        {
            int count = this.Count;
            for (int i = 0; i < count; ++i)
            {
                T element = this[i];
                if (property.GetValue(element).Equals(key))
                {
                    return i;
                }
            }

            return -1;
        }

        //https://stackoverflow.com/questions/43331145/how-can-i-improve-performance-of-an-addrange-method-on-a-custom-bindinglist
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            // Remember the current setting for RaiseListChangedEvents
            // (if it was already deactivated, we shouldn't activate it after adding!).
            bool oldRaiseEventsValue = RaiseListChangedEvents;

            try
            {
                RaiseListChangedEvents = false;

                foreach (var value in collection)
                    Add(value);
            }
            // Restore the old setting for RaiseListChangedEvents (even if there was an exception),
            // and fire the ListChanged-event once (if RaiseListChangedEvents is activated).
            finally
            {
                RaiseListChangedEvents = oldRaiseEventsValue;

                if (RaiseListChangedEvents)
                    ResetBindings();
            }
        }
    }
}
