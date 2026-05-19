namespace MechanicShop.Domain.Common.Results
{
    public readonly record struct Error
    {
        public string Code { get; }
        public string Description { get; }
        public ErrorKind Kind { get; }

        private Error(string code, string description, ErrorKind kind)
        {
            Code = code;
            Description = description;
            Kind = kind;
        }

        public static Error Create(string code, string description, int type)
            => new (code, description, (ErrorKind)type);
        
        public static Error Failure(string code = nameof(Failure), string description = "General failure")
            => new Error(code, description, ErrorKind.Failure);
        public static Error Unexpected(string code = nameof(Unexpected), string description = "Unexpected Error")
            => new Error(code, description, ErrorKind.Unexpected);
        public static Error Validation(string code = nameof(Validation), string description = "Validation Error")
            => new Error(code, description, ErrorKind.Validation);
        public static Error NotFound(string code = nameof(NotFound), string description = "Not Found Error")
            => new Error(code, description, ErrorKind.NotFound);
        public static Error Conflict(string code = nameof(Conflict), string description = "Conflict Error")
            => new Error(code, description, ErrorKind.Conflict);
        public static Error Unauthorized(string code = nameof(Unauthorized), string description = "Unauthorized Error")
            => new Error(code, description, ErrorKind.Unauthorized);
        public static Error Forbidden(string code = nameof(Forbidden), string description = "Forbidden Error")
            => new Error(code, description, ErrorKind.Forbidden);
    }
}
