namespace INKED.Application.Exceptions;

/// <summary>The request is well-formed but breaks a validation rule or references a missing record (HTTP 400).</summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}

/// <summary>The operation conflicts with existing data, e.g. deleting a parent that still has children (HTTP 409).</summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
