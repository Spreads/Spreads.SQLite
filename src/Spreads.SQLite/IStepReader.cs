using System.Diagnostics.Contracts;

namespace Spreads.SQLite
{
    public interface IStepReader<TState, TResult>
    {
        [Pure]
        void Invoke(bool hasRow, QueryReader reader, TState state, out TResult result);
    }
}