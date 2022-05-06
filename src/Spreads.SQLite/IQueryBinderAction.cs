using System.Diagnostics.Contracts;

namespace Spreads.SQLite
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQueryBinderAction<T>
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="state"></param>
        [Pure]
        void Invoke(QueryBinder binder, T state);
    }
}