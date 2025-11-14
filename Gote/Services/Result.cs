namespace Gote.Services
{
    /// <summary>
    /// 処理結果型（RustのResult型を参考にした成功/失敗ラッパー）
    /// </summary>
    /// <typeparam name="TSuccess">処理成功時に返すオブジェクト型</typeparam>
    /// <typeparam name="TFailure">処理失敗時に返すオブジェクト型</typeparam>
    internal readonly struct Result<TSuccess, TFailure> : IEquatable<Result<TSuccess, TFailure>>
    {
        /// <summary>
        /// 成功時の値
        /// </summary>
        private readonly TSuccess? _success;
        /// <summary>
        /// 失敗時の値
        /// </summary>
        private readonly TFailure? _failure;
        /// <summary>
        /// 成功かどうかのフラグ
        /// </summary>
        private readonly bool _isSuccess;

        /// <summary>
        /// 成功値からResultを生成します。
        /// </summary>
        /// <param name="success">成功時の値</param>
        public Result(TSuccess success) : this(success, default, true) { }
        /// <summary>
        /// 失敗値からResultを生成します。
        /// </summary>
        /// <param name="failure">失敗時の値</param>
        public Result(TFailure failure) : this(default, failure, false) { }
        /// <summary>
        /// 内部用コンストラクタ
        /// </summary>
        private Result(TSuccess? success, TFailure? failure, bool isSuccess)
        {
            _success = success;
            _failure = failure;
            _isSuccess = isSuccess;
        }

        /// <summary>
        /// 成功値からResultを生成するファクトリーメソッド
        /// </summary>
        /// <param name="value">成功時の値</param>
        /// <returns>成功状態のResult</returns>
        public static Result<TSuccess, TFailure> Success(TSuccess value) => new(value, default, true);
        /// <summary>
        /// 失敗値からResultを生成するファクトリーメソッド
        /// </summary>
        /// <param name="error">失敗時の値</param>
        /// <returns>失敗状態のResult</returns>
        public static Result<TSuccess, TFailure> Failure(TFailure error) => new(default, error, false);

        /// <summary>
        /// 成功状態かどうかを取得します。
        /// </summary>
        public bool IsSuccess => _isSuccess;

        /// <summary>
        /// 成功時の値を取得します。失敗時は例外をスローします。
        /// </summary>
        /// <returns>成功値</returns>
        /// <exception cref="InvalidOperationException">失敗状態の場合</exception>
        public TSuccess? GetSuccess()
        {
            if (_isSuccess)
            {
                return _success;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// 失敗時の値を取得します。成功時は例外をスローします。
        /// </summary>
        /// <returns>失敗値</returns>
        /// <exception cref="InvalidOperationException">成功状態の場合</exception>
        public TFailure? GetFailure()
        {
            if (!_isSuccess)
            {
                return _failure;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// 成功時の値を安全に取得します。
        /// </summary>
        /// <param name="value">成功値（失敗時はnull）</param>
        /// <returns>成功状態ならtrue</returns>
        public bool TryGetSuccess(out TSuccess? value)
        {
            value = _success;
            return _isSuccess;
        }

        /// <summary>
        /// 失敗時の値を安全に取得します。
        /// </summary>
        /// <param name="error">失敗値（成功時はnull）</param>
        /// <returns>失敗状態ならtrue</returns>
        public bool TryGetFailure(out TFailure? error)
        {
            error = _failure;
            return !_isSuccess;
        }

        /// <summary>
        /// 分解構文用（isSuccess, success, failure）
        /// </summary>
        public void Deconstruct(out bool isSuccess, out TSuccess? success, out TFailure? failure)
        {
            isSuccess = _isSuccess;
            success = _success;
            failure = _failure;
        }

        /// <summary>
        /// 成功値を変換します（失敗時はそのまま）
        /// </summary>
        /// <typeparam name="TNewSuccess">変換後の成功型</typeparam>
        /// <param name="map">変換関数</param>
        /// <returns>変換後のResult</returns>
        public Result<TNewSuccess, TFailure> Map<TNewSuccess>(Func<TSuccess?, TNewSuccess> map)
            => _isSuccess ? new Result<TNewSuccess, TFailure>(map(_success)) : new Result<TNewSuccess, TFailure>(_failure!);

        /// <summary>
        /// 失敗値を変換します（成功時はそのまま）
        /// </summary>
        /// <typeparam name="TNewFailure">変換後の失敗型</typeparam>
        /// <param name="map">変換関数</param>
        /// <returns>変換後のResult</returns>
        public Result<TSuccess, TNewFailure> MapError<TNewFailure>(Func<TFailure?, TNewFailure> map)
            => !_isSuccess ? new Result<TSuccess, TNewFailure>(map(_failure)) : new Result<TSuccess, TNewFailure>(_success!);

        /// <summary>
        /// 成功/失敗で処理を分岐して実行します（戻り値なし）
        /// </summary>
        /// <param name="success">成功時の処理</param>
        /// <param name="failure">失敗時の処理</param>
        public void Match(Action<TSuccess?> success, Action<TFailure?> failure)
        {
            if (_isSuccess)
            {
                success.Invoke(_success);
            }
            else
            {
                failure.Invoke(_failure);
            }
        }

        /// <summary>
        /// 成功/失敗で処理を分岐して実行します（戻り値あり）
        /// </summary>
        /// <typeparam name="TOut">戻り値型</typeparam>
        /// <param name="success">成功時の処理</param>
        /// <param name="failure">失敗時の処理</param>
        /// <returns>分岐後の戻り値</returns>
        public TOut Match<TOut>(Func<TSuccess?, TOut> success, Func<TFailure?, TOut> failure)
        {
            if (_isSuccess)
            {
                return success.Invoke(_success);
            }
            else
            {
                return failure.Invoke(_failure);
            }
        }

        /// <summary>
        /// Result同士の等価性比較
        /// </summary>
        public bool Equals(Result<TSuccess, TFailure> other)
        {
            return EqualityComparer<TFailure?>.Default.Equals(_failure, other._failure)
                && EqualityComparer<TSuccess?>.Default.Equals(_success, other._success)
                && this._isSuccess == other._isSuccess;
        }

        /// <summary>
        /// オブジェクトとしての等価性比較
        /// </summary>
        public override bool Equals(object? obj)
        {
            return obj is Result<TSuccess, TFailure> other && Equals(other);
        }

        /// <summary>
        /// ハッシュコード生成
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(_isSuccess, _success, _failure);
        }

        /// <summary>
        /// 暗黙的な成功値からResultへの変換
        /// </summary>
        public static implicit operator Result<TSuccess, TFailure>(TSuccess success)
        {
            return new Result<TSuccess, TFailure>(success);
        }

        /// <summary>
        /// 暗黙的な失敗値からResultへの変換
        /// </summary>
        public static implicit operator Result<TSuccess, TFailure>(TFailure failure)
        {
            return new Result<TSuccess, TFailure>(failure);
        }

        /// <summary>
        /// デバッグ用文字列表現
        /// </summary>
        public override string ToString()
        {
            return _isSuccess ? $"Success({_success})" : $"Failure({_failure})";
        }
    }
}
