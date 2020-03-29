using System;
using System.Collections;
using System.Collections.Generic;

namespace CF.Common.Codes
{
    internal class CodeCollection<TCode, TId> : ICodeCollection<TCode, TId>
        where TCode : CodeBase<TCode, TId>
        where TId : struct, Enum
    {
        private readonly HashSet<TCode> _codes = new HashSet<TCode>();
        private readonly Dictionary<TId, TCode> _codeById = new Dictionary<TId, TCode>();
        private readonly Dictionary<string, TCode> _codeByValue = new Dictionary<string, TCode>();

        private readonly object _lock = new object();

        public TCode this[TId id] => this._codeById.ContainsKey(id) ? this._codeById[id] : null;

        public TCode this[string value] => value == null ? null : (this._codeByValue.ContainsKey(value) ? this._codeByValue[value] : null);

        public IEnumerator<TCode> GetEnumerator()
        {
            return this._codes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._codes.GetEnumerator();
        }

        public bool ContainsCode(TCode code)
        {
            return this._codes.Contains(code);
        }

        public bool TryAdd(TCode code)
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            // Perform double checked locking to ensure uniqueness of items by both ID and value.
            lock (this._lock)
            {
                if (!this._codeById.ContainsKey(code.Id) && !this._codeByValue.ContainsKey(code.Value))
                {
                    lock (this._lock)
                    {
                        this._codes.Add(code);
                        this._codeById[code.Id] = code;
                        this._codeByValue[code.Value] = code;

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
