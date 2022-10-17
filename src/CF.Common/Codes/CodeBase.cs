using CF.Common.Extensions;
using CF.Common.Utility;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CF.Common.Codes
{
    public abstract class CodeBase<TCode, TId> : IValueCode, IIdCode<TId>
        where TCode : CodeBase<TCode, TId>
        where TId : struct, Enum
    {
        private static readonly ConcurrentDictionary<Type, CodeCollection<TCode, TId>> _codeCollectionByCodeType = new ConcurrentDictionary<Type, CodeCollection<TCode, TId>>();

        public static ICodeCollection<TCode, TId> Codes => _codeCollectionByCodeType[typeof(TCode)];

        static CodeBase()
        {
            // To debug the static constructor, uncomment:
            // Debugger.Break();

            // This static constructor is required to ensure that derived code types are initialized
            // in advance of accessing the static codes collection and potentially other static members
            // on the base type.
            RuntimeHelpers.RunClassConstructor(typeof(TCode).TypeHandle);
        }

        public TId Id { get; }

        public string Value { get; }

        protected CodeBase(TId id, string value)
        {
            this.Id = id;
            this.Value = value.EnsureArgumentNotNullOrWhitespace(nameof(value));

            var codeCollection = _codeCollectionByCodeType.GetOrAdd(typeof(TCode), new CodeCollection<TCode, TId>());
            var added = codeCollection.TryAdd((TCode)this);
            if (!added)
            {
                throw new ArgumentException($"A code already exists with either ID [{Enum.GetName(typeof(TId), id)}] or value [{value}].");
            }
        }

        public override bool Equals(object obj)
        {
            // Avoid using operators which cause operator overload issues.
            return obj is TCode otherCode && Enum.Equals(this.Id, otherCode.Id);
        }

        public override int GetHashCode() => HashCodeCalculator.Calculate(this.Id);

        public override string ToString() => $"ID [{Enum.GetName(typeof(TId), this.Id)}]; Value [{this.Value}]";

        public static implicit operator TId(CodeBase<TCode, TId> code) =>  code?.Id ?? default;

        public static implicit operator string(CodeBase<TCode, TId> code) => code?.Value;

        public static implicit operator CodeBase<TCode, TId>(string value) => _codeCollectionByCodeType[typeof(TCode)][value];

        public static implicit operator CodeBase<TCode, TId>(TId id) => _codeCollectionByCodeType[typeof(TCode)][id];

        public static bool operator ==(CodeBase<TCode, TId> code1, CodeBase<TCode, TId> code2)
        {
            // Avoid using operators which cause operator overload issues.
            if (code1 is null)
            {
                return code2 is null;
            }

            return code1.Equals(code2);
        }

        public static bool operator !=(CodeBase<TCode, TId> code1, CodeBase<TCode, TId> code2) => !(code1 == code2);
    }
}
