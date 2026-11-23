namespace INKED.Application.Common.Exceptions;

/// <summary>Record does not exist -> HTTP 404.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

/// <summary>Duplicate / FK-in-use / unique-constraint problem -> HTTP 409.</summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}

/// <summary>Invalid value, missing parent record, or rule violation -> HTTP 400.</summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}
