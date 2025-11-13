namespace Gote.Services
{
    /// <summary>
    /// 処理結果型
    /// </summary>
    /// <remarks>関数などの処理結果を返す用のラッパー</remarks>
    /// <typeparam name="TSuccess">処理成功時に返すオブジェクト型</typeparam>
    /// <typeparam name="TFailure">処理失敗時に返すオブジェクト型</typeparam>
    internal readonly struct Result<TSuccess, TFailure> : IEquatable<Result<TSuccess, TFailure>>
    {
        private readonly TSuccess _success;
        private readonly TFailure _failure;
        private readonly bool _isSuccess;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="success">処理成功時に返すオブジェクト</param>
        /// <param name="failure">処理失敗時に返すオブジェクト</param>
        /// <param name="isSuccess">処理結果</param>
        private Result(TSuccess success, TFailure failure, bool isSuccess)
        {
            _success = success;
            _failure = failure;
            _isSuccess = isSuccess;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="success">処理成功時に返すオブジェクト</param>
#nullable disable
        public Result(TSuccess success) : this(success, default, true) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="failure">処理失敗時に返すオブジェクト</param>
        public Result(TFailure failure) : this(default, failure, false) { }
#nullable enable

        /// <summary>
        /// 処理結果
        /// </summary>
        /// <remarks>True：成功、false：失敗</remarks>
        public bool IsSuccess => _isSuccess;

        /// <summary>
        /// 処理成功時に返すオブジェクトを取得
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public TSuccess GetSuccess()
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
        /// 処理失敗時に返すオブジェクトを取得
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public TFailure GetFailure()
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
        /// 処理結果別関数実行（戻り値無し）
        /// </summary>
        /// <param name="success">処理成功時処理</param>
        /// <param name="failure">処理失敗時処理</param>
        public void Match(Action<TSuccess> success, Action<TFailure> failure)
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
        /// 処理結果別関数実行（戻り値有り）
        /// </summary>
        /// <typeparam name="TOut">戻り値型</typeparam>
        /// <param name="success">処理成功時処理</param>
        /// <param name="failure">処理失敗時処理</param>
        /// <returns></returns>
        public TOut Match<TOut>(Func<TSuccess, TOut> success, Func<TFailure, TOut> failure)
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

        public bool Equals(Result<TSuccess, TFailure> other)
        {
            return EqualityComparer<TFailure>.Default.Equals(_failure, other._failure)
                && EqualityComparer<TSuccess>.Default.Equals(_success, other._success)
                && this._isSuccess == other._isSuccess;
        }

        public static implicit operator Result<TSuccess, TFailure>(TSuccess success)
        {
            return new Result<TSuccess, TFailure>(success);
        }

        public static implicit operator Result<TSuccess, TFailure>(TFailure failure)
        {
            return new Result<TSuccess, TFailure>(failure);
        }
    }
}
