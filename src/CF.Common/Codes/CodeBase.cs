using CF.Common.Extensions;
using CF.Common.Utility;
using System;
using System.Collections.Concurrent;

namespace CF.Common.Codes
{
    public abstract class CodeBase<TCode, TId> : IValueCode, IIdCode<TId>
        where TCode : CodeBase<TCode, TId>
        where TId : struct, IComparable
    {
        private static readonly ConcurrentDictionary<Type, CodeCollection<TCode, TId>> _codesByType = new ConcurrentDictionary<Type, CodeCollection<TCode, TId>>();

        public static ICodeCollection<TCode, TId> Codes => _codesByType[typeof(TCode)];

        public TId Id { get; }

        public string Value { get; }

        protected CodeBase(TId id, string value)
        {
            // The only way to positively ensure that ID is an enum is by this check.
            // There is no generic constraint check to fully enforce this rule.
            if (!typeof(TId).IsEnum)
            {
                throw new ArgumentException($"The ID is of type [{typeof(TId).FullName}] - an enum type is expected.");
            }

            this.Id = id;
            this.Value = value.EnsureArgumentNotNullOrWhitespace(nameof(value));

            var codeCollection = _codesByType.GetOrAdd(typeof(TCode), new CodeCollection<TCode, TId>());
            var added = codeCollection.TryAdd((TCode)this);
            if (!added)
            {
                throw new ArgumentException($"A code already exists with either ID [{Enum.GetName(typeof(TId), id)}] or value [{value}].");
            }
        }

        public override bool Equals(object obj)
        {
            var otherCode = obj as TCode;

            // Avoid using operators which cause operator overload issues.
            return !object.ReferenceEquals(otherCode, null) && Enum.Equals(this.Id, otherCode.Id);
        }

        public override int GetHashCode() => HashCodeCalculator.Calculate(this.Id);

        public override string ToString() => $"ID [{Enum.GetName(typeof(TId), this.Id)}]; Value [{this.Value}]";

        public static implicit operator TId(CodeBase<TCode, TId> code) => code.Id;

        public static implicit operator string(CodeBase<TCode, TId> code) => code?.Value;

        public static implicit operator CodeBase<TCode, TId>(string value) => _codesByType[typeof(TCode)][value];

        public static implicit operator CodeBase<TCode, TId>(TId id) => _codesByType[typeof(TCode)][id];

        public static bool operator ==(CodeBase<TCode, TId> code1, CodeBase<TCode, TId> code2)
        {
            // Avoid using operators which cause operator overload issues.
            if (object.ReferenceEquals(code1, null))
            {
                return object.ReferenceEquals(code2, null);
            }

            return code1.Equals(code2);
        }

        public static bool operator !=(CodeBase<TCode, TId> code1, CodeBase<TCode, TId> code2) => !(code1 == code2);
    }
}
