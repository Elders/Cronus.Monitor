using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Cronus.Monitor.Models;

public class ResponseResult
{
    public static readonly ResponseResult Success = new ResponseResult();

    public ResponseResult() { }

    public ResponseResult(params string[] errors)
    {
        Errors = errors;
    }

    public IEnumerable<string> Errors { get; private set; }

    public bool IsSuccess
    {
        get { return Errors == null || !Errors.Any(); }
    }
}

public class ResponseResult<T> : ResponseResult
{
    public ResponseResult(T result)
    {
        Result = result;
    }

    public ResponseResult(T result, params string[] errors)
        : base(errors)
    {
        Result = result;
    }

    public T Result { get; private set; }
}

public class BulkResponseResult<T> where T : ResponseResult
{
    public BulkResponseResult()
    {
        BulkResult = new List<T>();
    }

    public List<T> BulkResult { get; private set; }
    public IEnumerable<string> Errors { get { return BulkResult.Where(x => x.Errors != null && x.Errors.Any()).SelectMany(x => x.Errors); } }
    public bool IsSuccess
    {
        get { return BulkResult.Where(x => x.Errors != null && x.Errors.Any()).Any() == false; }
    }
}
