namespace JobApplicationTracking.Domain.Exceptions;

public class DuplicateApplicationException(string company, string role, DateOnly appliedDate)
    : Exception($"An application for '{role}' at '{company}' on {appliedDate:MMM d, yyyy} already exists.");
