using System;
using System.Threading.Tasks;

namespace FilesManager.Models.Infrastructure
{
    public class Response<T>
    {
        #region Props

        /// <summary>
        /// Gets or sets operation statuses:
        /// Exception = -1
        /// Succes = 0
        /// Another error = N.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets message with warning or error.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets data with type T.
        /// </summary>
        public T Data { get; set; }
        #endregion

        #region DoMethod(action)

        /// <summary>
        /// Wrapper delegate methods for cautching exceptions and custom handling for errors.
        /// </summary>
        /// <type name="T">Type of entity.</type>
        /// <param name="action">Wrapper delegate method.</param>
        /// <returns>Returns result or error.</returns>
        public static Response<T> DoMethod(Action<Response<T>> action) =>
            DoMethod(action, null);
        
        #endregion

        #region DoMethod(action, errorHandler)

        /// <summary>
        /// Wrapper delegate methods for cautching exceptions and custom handling for errors.
        /// </summary>
        /// <type name="T">Type of entity.</type>
        /// <param name="action">Wrapper delegate method.</param>
        /// <param name="errorHandler">Wrapper delegate handler for errors.</param>
        /// <returns>Returns result or error.</returns>
        public static Response<T> DoMethod(Action<Response<T>> action, Action<Response<T>> errorHandler)
        {
            Response<T> result = new();
            try
            {
                action(result);
            }
            catch (ResponseException e)
            {
                Console.WriteLine($"ERROR | Date: {DateTime.Now} | Message: {e.Message}");
                errorHandler?.Invoke(result);
                result.Code = -1;
                result.Message = e.Message;
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR | Date: {DateTime.Now} | Message: {e.Message}");
                result.Code = -1;
                result.Message = e.Message;
                errorHandler?.Invoke(result);
            }

            return result;
        }
        #endregion

        #region DoMethodAsync(action)

        /// <summary>
        /// Async wrapper delegate methods for cautching exceptions and custom handling for errors.
        /// </summary>
        /// <type name="T">Type of entity.</type>
        /// <param name="action">Wrapper delegate method.</param>
        /// <returns>Returns result or error.</returns>
        public static async Task<Response<T>> DoMethodAsync(Func<Response<T>, Task<Response<T>>> action) =>
            await DoMethodAsync(action, null);

        #endregion

        #region DoMethodAsync(action, errorHandler)

        /// <summary>
        /// Async wrapper delegate methods for cautching exceptions and custom handling for errors.
        /// </summary>
        /// <type name="T">Type of entity.</type>
        /// <param name="action">Wrapper delegate method.</param>
        /// <param name="errorHandler">Wrapper delegate handler for errors.</param>
        /// <returns>Returns result or error.</returns>
        public static async Task<Response<T>> DoMethodAsync(Func<Response<T>, Task<Response<T>>> action, Func<Response<T>, Task<Response<T>>> errorHandler)
        {
            Response<T> result = new();
            try
            {
                await action(result);
            }
            catch (ResponseException e)
            {
                Console.WriteLine($"ERROR | Date: {DateTime.Now} | Message: {e.Message}");
                errorHandler?.Invoke(result);
                result.Code = -1;
                result.Message = e.Message;
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR | Date: {DateTime.Now} | Message: {e.Message}");
                result.Code = -1;
                result.Message = e.Message;
                errorHandler?.Invoke(result);
            }

            return result;
        }
        #endregion

        #region Throw(code, message)

        /// <summary>
        /// If error with specified code and message has to be thrown.
        /// </summary>
        /// <param name="code">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <exception cref="ResponseException">Special exception</exception>
        public void Throw(int code, string message) =>
            throw new ResponseException(code, message);
        #endregion

        #region Throw(message)

        /// <summary>
        /// If error with specified message has to be thrown (Code = -1).
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <exception cref="ResponseException">Special exception.</exception>
        public void Throw(string message) =>
            throw new ResponseException(-1, message);
        #endregion

        #region GetResultIfNotError()

        /// <summary>
        /// Returns result if Code == 0.
        /// </summary>
        /// <returns>Data.</returns>
        /// <exception cref="ResponseException">Original error.</exception>
        public T GetResultIfNotError() =>
            GetResultIfNotError(string.Empty);
        #endregion

        #region GetResultIfNotError(errorMessage)

        /// <summary>
        /// Returns result if Code == 0.
        /// </summary>
        /// <param name="errorMessage">Error text will be added to start of message.</param>
        /// <returns>Data.</returns>
        /// <exception cref="ResponseException">Original error.</exception>
        public T GetResultIfNotError(string errorMessage)
        {
            if (Code != 0)
            {
                if (string.IsNullOrEmpty(errorMessage))
                {
                    Throw(Code, Message);
                }

                Throw(Code, $"{errorMessage.Trim()} {Message}");
            }

            return Data;
        }
        #endregion

        #region GetResultIfNotError(errorMessage, action)

        /// <summary>
        /// Returns result if Code == 0.
        /// </summary>
        /// <param name="errorMessage">Error text will be added to start of message.</param>
        /// <param name="action">Error handler method that will be called if code != 0.</param>
        /// <returns>Data.</returns>
        /// <exception cref="ResponseException">Original error.</exception>
        public T GetResultIfNotError(string errorMessage, Action<Response<T>> action)
        {
            if (Code != 0)
            {
                if(action != null)
                {
                    action.Invoke(this);
                }

                if (string.IsNullOrEmpty(errorMessage))
                {
                    Throw(Code, Message);
                }

                Throw(Code, $"{errorMessage.Trim()} {Message}");
            }

            return Data;
        }
        #endregion

        #region ResponseException
        public class ResponseException : Exception
        {
            public int Code { get; set; } = -1;

            public ResponseException(string message)
                : base(message)
            {
            }

            public ResponseException(int code, string message)
                : base(message)
            {
                this.Code = code;
            }
        }
        #endregion
    }
}
