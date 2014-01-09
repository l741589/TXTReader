﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlib.Utility {
    public class ItemsProxy : INotifyCollectionChanged, IEnumerable {
        public delegate object ProxyGetter(object obj);

        private ProxyGetter Getter;

        class E : IEnumerator {

            private ProxyGetter Getter;
            private IEnumerator TargetEnum;

            public E(IEnumerable target, ProxyGetter GetCurrent) {
                TargetEnum = target.GetEnumerator();
                Getter = GetCurrent;
            }

            public object Current {
                get { return Getter(TargetEnum.Current); }
            }

            public bool MoveNext() {
                return TargetEnum.MoveNext();
            }

            public void Reset() {
                TargetEnum.Reset();
            }
        }

        private IEnumerable Target;
        public ItemsProxy(IEnumerable target, ProxyGetter Getter) {
            Target = target;
            this.Getter = Getter;
            if (target is INotifyCollectionChanged) {
                (target as INotifyCollectionChanged).CollectionChanged += FloatControls_CollectionChanged;
            }
        }

        void FloatControls_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            List<object> NewItems = new List<object>();
            List<object> OldItems = new List<object>();
            if (e.NewItems != null) for (int i = 0; i < e.NewItems.Count; ++i) NewItems.Add(Getter(e.NewItems[i]));
            if (e.OldItems != null) for (int i = 0; i < e.OldItems.Count; ++i) OldItems.Add(Getter(e.OldItems[i]));
            NotifyCollectionChangedEventArgs ee = null;
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add: ee = new NotifyCollectionChangedEventArgs(e.Action, NewItems, e.NewStartingIndex); break;
                case NotifyCollectionChangedAction.Remove: ee = new NotifyCollectionChangedEventArgs(e.Action, OldItems, e.OldStartingIndex); break;
                case NotifyCollectionChangedAction.Replace: ee = new NotifyCollectionChangedEventArgs(e.Action, NewItems, OldItems, e.OldStartingIndex); break;
                case NotifyCollectionChangedAction.Reset: ee = new NotifyCollectionChangedEventArgs(e.Action); break;
                case NotifyCollectionChangedAction.Move: ee = new NotifyCollectionChangedEventArgs(e.Action, NewItems, e.OldStartingIndex, e.NewStartingIndex); break;
            }
            CollectionChanged(sender, ee);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public IEnumerator GetEnumerator() {
            return new E(Target, Getter);
        }
    }
}
