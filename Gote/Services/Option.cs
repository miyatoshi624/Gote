namespace Gote.Services
{
    /// <summary>
    /// RustのOption型を参考にした値の有無を表すラッパークラス
    /// </summary>
    internal sealed class Option<T>
    {
        /// <summary>
        /// 保持する値（Someの場合のみ有効）
        /// </summary>
        private readonly T _value;

        /// <summary>
        /// 値が存在するかどうか
        /// </summary>
        public bool IsSome { get; }

        /// <summary>
        /// 値が存在しないかどうか
        /// </summary>
        public bool IsNone => !IsSome;

        /// <summary>
        /// 内部用コンストラクタ
        /// </summary>
        private Option(T value, bool isSome)
        {
            _value = value;
            IsSome = isSome;
        }

        /// <summary>
        /// 値が存在するOptionを生成
        /// </summary>
        public static Option<T> Some(T value) => new Option<T>(value, true);

        /// <summary>
        /// 値が存在しないOptionを生成
        /// </summary>
        public static Option<T> None() => new Option<T>(default!, false);

        /// <summary>
        /// 値を取得（Noneの場合は例外）
        /// </summary>
        public T Unwrap()
        {
            if (IsSome) return _value;
            throw new InvalidOperationException("Called Unwrap on None");
        }

        /// <summary>
        /// 値を取得（Noneの場合はデフォルト値を返す）
        /// </summary>
        public T UnwrapOr(T defaultValue) => IsSome ? _value : defaultValue;

        /// <summary>
        /// 値が存在する場合のみ変換して新しいOptionを返す
        /// </summary>
        public Option<U> Map<U>(Func<T, U> mapper)
        {
            if (IsSome) return Option<U>.Some(mapper(_value));
            return Option<U>.None();
        }

        /// <summary>
        /// 値の有無で処理を分岐（戻り値なし）
        /// </summary>
        public void Match(Action<T> some, Action none)
        {
            if (IsSome) some(_value);
            else none();
        }

        /// <summary>
        /// 値の有無で処理を分岐（戻り値あり）
        /// </summary>
        public TOut Match<TOut>(Func<T, TOut> some, Func<TOut> none)
        {
            return IsSome ? some(_value) : none();
        }
    }
}
