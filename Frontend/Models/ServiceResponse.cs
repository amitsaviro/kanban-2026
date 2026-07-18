namespace Frontend.Models
{
    /// <summary>
    /// Mirrors the JSON envelope returned by every method in the Backend's service layer:
    /// <c>{ "ErrorMessage": &lt;string&gt;, "ReturnValue": &lt;T&gt; }</c>.
    /// </summary>
    /// <typeparam name="T">The type of the ReturnValue field for a specific service call.</typeparam>
    // A- every UserService/BoardService/TaskService method returns a JSON string in this exact shape.
    // A- deserializing into this one generic class lets every ViewModel unwrap a response the same way:
    // A- JsonSerializer.Deserialize<ServiceResponse<T>>(json), then check ErrorMessage before using ReturnValue.
    public class ServiceResponse<T>
    {
        /// <summary>
        /// The error message if the service call failed; null/empty if it succeeded.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The value returned by the service call on success; default(T) if it failed.
        /// </summary>
        public T ReturnValue { get; set; }
    }
}
