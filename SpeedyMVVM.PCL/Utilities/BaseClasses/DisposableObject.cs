using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Utilities
{
    public abstract class DisposableObject:ObservableObject, IDisposable
    {
        private bool isDisposed;

        /// <summary>
        /// Gets a value indicating whether the object is disposed.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return isDisposed;
            }
        }


        #region IDisposable Implementation
        /// <summary>
        /// Releasing all the unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(false);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose managed resources.
        /// </summary>
        protected virtual void DisposeManaged() { }

        /// <summary>
        /// Dispose unmanaged resources.
        /// </summary>
        protected virtual void DisposeUnmanaged() { }

        /// <summary>
        /// Throw a ObjectDisposedException if disposed.
        /// </summary>
        protected virtual void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        /// <summary>
        /// Dispose the managed and unmanaged resources
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!isDisposed)
                DisposeManaged();
            DisposeUnmanaged();
            isDisposed = true;
        }
        #endregion

        #region Constructors/Distructors        
        /// <summary>
        /// Create a new instance of DisposableObject
        /// </summary>
        public DisposableObject()
        {
            ThrowIfDisposed();
        }

        /// <summary>
        /// Finalizes the DisposableObject
        /// </summary>
        ~DisposableObject()
        {
            this.Dispose(false);
        }
        #endregion
    }
}
