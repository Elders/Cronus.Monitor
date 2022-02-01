using System.Collections.Generic;
using System.Linq;

namespace Cronus.Monitor.Models
{
    public abstract class ResponseFromProjection<TResult> where TResult : class, new()
    {
        public TResult GetResult() => Errors.Any() ? null : Data;

        internal List<string> Errors { get; set; } = new List<string>();

        protected TResult Data { get; set; }
    }
}
