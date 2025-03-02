using System;
using System.Collections;
using System.Collections.Generic;


namespace Modules.Hive
{
    public class CompositeDisposable : ICollection<IDisposable>, IDisposable
    {
        private List<IDisposable> disposables;

        public int Count => disposables.Count;
        public bool IsReadOnly => false;


        #region Instaning

        public CompositeDisposable()
        {
            disposables = new List<IDisposable>();
        }


        public CompositeDisposable(int capacity)
        {
            disposables = new List<IDisposable>(capacity);
        }


        public CompositeDisposable(IEnumerable<IDisposable> collection)
        {
            disposables = new List<IDisposable>(collection);
        }


        public void Dispose()
        {
            if (disposables == null)
            {
                return;
            }

            foreach (var obj in disposables)
            {
                obj?.Dispose();
            }

            disposables = null;
        }

        #endregion



        #region Collection methods

        public bool Contains(IDisposable item)
        {
            if (disposables == null)
            {
                throw new ObjectDisposedException(nameof(CompositeDisposable));
            }

            if (item == null)
            {
                return false;
            }

            return disposables.Contains(item);
        }


        public void Add(IDisposable item)
        {
            if (disposables == null)
            {
                throw new ObjectDisposedException(nameof(CompositeDisposable));
            }

            if (item == null)
            {
                return;
            }

            disposables.Add(item);
        }


        public bool Remove(IDisposable item)
        {
            if (disposables == null)
            {
                throw new ObjectDisposedException(nameof(CompositeDisposable));
            }

            if (item == null)
            {
                return false;
            }

            return disposables.Remove(item);
        }


        public void Clear()
        {
            if (disposables == null)
            {
                throw new ObjectDisposedException(nameof(CompositeDisposable));
            }

            disposables.Clear();
        }


        public void CopyTo(IDisposable[] array, int arrayIndex)
        {
            if (disposables == null)
            {
                throw new ObjectDisposedException(nameof(CompositeDisposable));
            }

            disposables.CopyTo(array, arrayIndex);
        }


        public IEnumerator<IDisposable> GetEnumerator()
        {
            if (disposables == null)
            {
                throw new ObjectDisposedException(nameof(CompositeDisposable));
            }

            return disposables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (disposables == null)
            {
                throw new ObjectDisposedException(nameof(CompositeDisposable));
            }

            return disposables.GetEnumerator();
        }

        #endregion
    }
}
